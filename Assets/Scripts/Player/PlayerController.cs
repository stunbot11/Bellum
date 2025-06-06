using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public GameObject sisyphus;
    private Rigidbody2D rb;
    private Animator animator;
    [HideInInspector] public GameManager gameManager;

    [Header("UI")]
    public Image healthBar;
    public Image chargeBar;
    private Image fadeScreen;
    private TextMeshProUGUI fadeText;
    private Image winScreen;
    private TextMeshProUGUI winText;

    public float chargeTime;
    public float negCharge;

    public GameObject pauseMenu;

    [Header("weapons")]
    private GameObject weapon;
    public GameObject swordHitbox;
    public GameObject tridantHitbox;
    public GameObject dodgeHitBox;
    public GameObject shield;
    public GameObject arrow;
    public float arrowSpeed;
    public GameObject net;
    public float netSpeed;

    [Header("Controlls")]
    public InputActionReference pauseButton;
    public InputActionReference move;
    public InputActionReference primaryButton;
    public InputActionReference secondaryButton;
    public GameObject rotPoint;
    private Vector2 lastInput;

    [Header("Player Stats")]
    public int maxHealth;
    public int health;
    public float speed;
     public int damage;
    public int[] classDmg;

    [Header("Upgrade stuffs")]
    // Secutor upgrades:
    //// upgrade path 1: DoT / Faster Swings / Enemies take more damage while DoT is in effect (done)
    //// upgrade path 2: blocking blocks more damage / easier perfect blocks / enemies take more damage after perfect block (done)
    // Sagittarius upgrades:
    //// upgrade path 1: faster charges / fire arrows / more arrows shot (done)
    //// upgrade path 2: faster dodge cooldown / 2 dodge charges / dodging through enemies damage them (done)
    // Retiarius upgrades:
    //// upgrade path 1: longer attack range / wider attack range / more damage (done)
    //// upgrade path 2: longer time netted / faster cooldown / enemies take more damage while netted (done)
    // upgrade path 3: faster move speed / more health / more damage (done)
    public int[] upgrades = { 0, 0, 0 };
    public int tripArrowCount = 3;

    private int dotTicks;
    private bool inDoT;

    [Header("Misc.")]
    public GameObject hitEffect;
    private float direction;
    private float timeSinceAction;
    private float iframes;
    [HideInInspector] public float pBlock;

    private bool blocking;
    private bool chargingDodge;
    private int canDodge = 1;
    private bool canAttack = true;
    private bool dodgeing;
    private bool canNet = true;

    public GameObject resumeButton;

    [Header("SFX")]
    public AudioSource pVocalCords;
    public AudioClip step;
    public AudioClip hurtHoogh;
    public AudioClip hurtAagh;
    public AudioClip hurtOugh;
    public AudioClip swingSword;
    public AudioClip shootBow;
    public AudioClip blockAttack;
    public AudioClip victory;
    public AudioClip defeat;
    private bool canSteppy = true;
    private bool canJingle = true;
    private int pickYourPoison;

    private void Start()
    {
        move.action.Disable();
        canJingle = true;
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.playerController = this;
        damage = classDmg[gameManager.classType - 1];
        weapon = GameObject.Find("sword");
        animator = GetComponent<Animator>();

        damage = Mathf.RoundToInt((gameManager.challenges[0] ? damage / 2 : damage) * (upgrades[2] > 2 ? 1.25f : 1));

        fadeScreen = GameObject.Find("death screen").GetComponent<Image>();
        fadeText = GameObject.Find("death text").GetComponent<TextMeshProUGUI>();
        winScreen = GameObject.Find("win screen").GetComponent<Image>();
        winText = GameObject.Find("win text").GetComponent<TextMeshProUGUI>();

        maxHealth = gameManager.maxHealth;
        health = gameManager.health;
    }

    public void fakeStart()
    {
        health += (upgrades[2] > 1 ? health / 4 : 0);
        maxHealth += (upgrades[2] > 1 ? maxHealth / 4 : 0);
        speed = gameManager.activeEmperor.decreaseSpeed ? speed * gameManager.activeEmperor.playerEffectStrength : speed;
        health = gameManager.activeEmperor.decreaseHealth ? Mathf.RoundToInt(health * gameManager.activeEmperor.playerEffectStrength) : health;
        maxHealth = gameManager.activeEmperor.decreaseHealth ? Mathf.RoundToInt(maxHealth * gameManager.activeEmperor.playerEffectStrength) : maxHealth;
        damage = gameManager.activeEmperor.decreaseDamage ? Mathf.RoundToInt(damage * gameManager.activeEmperor.playerEffectStrength) : damage;
        damage = gameManager.classType == 3 && upgrades[0] > 2 ? Mathf.RoundToInt(damage * 1.25f) : damage;
        canDodge = (upgrades[1] > 1 ? 2 : 1);

        pauseButton.action.started += pause;
        move.action.Enable();
        primaryButton.action.started += primary;
        move.action.performed += pAnim;
        move.action.canceled += pAnim;
        if (gameManager.classType == 2)
            primaryButton.action.canceled += primary;

        else if (gameManager.classType == 3 && upgrades[0] > 0) //adjusts the trient based on if it is bigger or not
        {
            tridantHitbox.transform.localScale = new Vector2((upgrades[0] > 1 ? 1.6f : 1), 1.3f);
            tridantHitbox.transform.localPosition = new Vector2((upgrades[0] > 1 ? 1.35f : 0), .075f);
        }
        if (!gameManager.challenges[2])
        {
            secondaryButton.action.started += secondary;
            secondaryButton.action.canceled += secondary;
        }
    }
    private void FixedUpdate()
    {
        if (!dodgeing && health > 0)
            rb.linearVelocity = move.action.ReadValue<Vector2>() * speed * ((blocking || (gameManager.classType == 2 && primaryButton.action.inProgress)) ? .5f : 1) * (upgrades[2] > 0 ? 1.25f : 1);
        else if (health <= 0)
            rb.linearVelocity = Vector2.zero;

        if (rb.linearVelocity != Vector2.zero && !dodgeing)
        {
            lastInput = move.action.ReadValue<Vector2>();
            direction = (Mathf.Atan2(move.action.ReadValue<Vector2>().y, move.action.ReadValue<Vector2>().x) * Mathf.Rad2Deg) - 90;
            if (canSteppy) StartCoroutine(bigSteppy());
        }
        rotPoint.transform.rotation = Quaternion.Euler(new Vector3(0, 0, direction));
    }

    private void Update()
    {
        healthBar.fillAmount = (float)health / (float)maxHealth;
        iframes -= Time.deltaTime;
        pBlock -= Time.deltaTime;
        
        timeSinceAction += Time.deltaTime;


        if (blocking || gameManager.classType == 2 && primaryButton.action.inProgress && canAttack)
        {
            if (chargeTime < 4)
                chargeTime += Time.deltaTime * (gameManager.classType == 2 && upgrades[0] > 0 ? 1.25f : 1) * (gameManager.classType == 1 && upgrades[1] > 1 ? .75f : 1);
            else if (gameManager.classType == 2)
                negCharge -= Time.deltaTime;
        }
        else if (gameManager.classType == 3 && !canNet)
            chargeTime += Time.deltaTime;

        else
        {
            chargeTime = 0;
            negCharge = 0;
        }
        chargeBar.fillAmount = gameManager.classType == 1 ? chargeTime / (.3f * (chargeTime > .3f ? 999999999 : 1)) : (gameManager.classType == 2 ? (chargeTime + negCharge) / 2 : chargeTime / (upgrades[1] > 1 ? 10 : 15));

        if (health <= 0)
        {
            
            Color tempScreen = fadeScreen.color;
            tempScreen.a += Time.deltaTime / 3;
            fadeScreen.color = tempScreen;

            Color tempText = fadeText.color;
            tempText.a += Time.deltaTime / 3;
            fadeText.color = tempText;
            if (tempScreen.a - (2 / 3) > 1)
                gameManager.loss();
        }

        if (gameManager.bossesDead >= gameManager.totalBosses && gameManager.totalBosses != 0)
        {
            unbindControlls();
            gameManager.bossActive = false;
            if (canJingle)
            {
                pVocalCords.PlayOneShot(victory);
                canJingle = false;
            }
            gameManager.health = health;
            Color tempScreen = winScreen.color;
            tempScreen.a += Time.deltaTime / 3;
            winScreen.color = tempScreen;

            Color tempText = winText.color;
            tempText.a += Time.deltaTime / 3;
            winText.color = tempText;
            if (tempScreen.a - (2 / 3) > 1)
                gameManager.win();
        }

        if (dotTicks > 0 && !inDoT)
        {
            inDoT = true;
            StartCoroutine(DoT());
        }
    }

    public IEnumerator bigSteppy()
    {
        canSteppy = false;
        pVocalCords.PlayOneShot(step);
        yield return new WaitForSeconds(0.17f);
        canSteppy = true;
    }

    public void takeDamage(int damage, string damageType = null, int ToDoT = 0)
    {
        if (ToDoT >= dotTicks)
            dotTicks = ToDoT;
        if (damageType == "DoT" && ToDoT > 0)
        {
            health -= damage;
        }
        else if (iframes < 0)
        {
            if (blocking && chargeTime <= .3 && damageType != "Unblockable")
            {
                print("perfect block");
                iframes = .45f;
                pBlock = 2f;
                pVocalCords.PlayOneShot(blockAttack);
            }
            else if (!dodgeing)
            {
                health -= Mathf.RoundToInt((blocking ? damage / (gameManager.classType == 0 && upgrades[1] > 0 ? 8 : 4) : damage) * (gameManager.activeEmperor.increaseDamage ? gameManager.activeEmperor.bossEffectStrength : 1));
                if (blocking)
                {
                    pVocalCords.PlayOneShot(blockAttack);
                }
                pickYourPoison = Random.Range(1, 3);

                switch (pickYourPoison)
                {
                    case 1:
                        pVocalCords.PlayOneShot(hurtAagh);
                        break;

                    case 2:
                        pVocalCords.PlayOneShot(hurtHoogh);
                        break;

                    case 3:
                        pVocalCords.PlayOneShot(hurtOugh);
                        break;


                    default:
                        break;
                }

                hitEffect.SetActive(true);
                StartCoroutine(hitVXF());
                iframes = .3f;
            }
        }
        if (health <= 0)
        {
            pVocalCords.PlayOneShot(defeat);
            StopAllCoroutines();
            unbindControlls();
            primaryButton.action.started -= primary;
            secondaryButton.action.started -= secondary;
            secondaryButton.action.canceled -= secondary;
        }
    }

    IEnumerator DoT()
    {

        dotTicks--;
        yield return new WaitForSeconds(1);
        takeDamage(1, "DoT");
        inDoT = false;
    }

    IEnumerator hitVXF()
    {
        yield return new WaitForSeconds(.3f);
        hitEffect.SetActive(false);
    }

    public void primary(InputAction.CallbackContext phase)
    {
        if (canAttack && !blocking && timeSinceAction >= .2f || gameManager.classType == 2 && chargeTime > 0 && !canAttack)
        {
            timeSinceAction = 0;
                switch (gameManager.classType)
            {
                case 1: //sword
                    canAttack = false;
                    swordHitbox.SetActive(true);
                    pVocalCords.PlayOneShot(swingSword);
                    StartCoroutine(attackCooldown(.4f * (upgrades[0] > 0 ? .75f : 1), .1f, swordHitbox));
                    animator.SetTrigger("SeAttack");
                    break;

                case 2: //bow
                    if (phase.started && canAttack)
                        animator.SetTrigger("SaAttack");
                    if (phase.canceled && canAttack)
                    {
                        animator.SetTrigger("SaDown");
                        if (upgrades[0] > 2) // if player has upgrade 3 for path 1 it will shoot multiple arrows
                        {
                            for (int i = 0; i < tripArrowCount; i++)
                            {
                                float ang = Mathf.Lerp(direction - 30, direction + 30, (i / (float)tripArrowCount));
                                GameObject p1 = Instantiate(arrow, transform.position, Quaternion.identity, null);
                                p1.GetComponent<Rigidbody2D>().rotation = ang;
                                ProjectileHandler projectileData1 = p1.GetComponent<ProjectileHandler>();
                                projectileData1.creator = this.gameObject;
                                projectileData1.damage = Mathf.RoundToInt(Mathf.Lerp(damage, damage * 4, chargeTime - (chargeTime + negCharge) / 2));
                                p1.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Sin(ang * Mathf.Deg2Rad) * -1, Mathf.Cos(ang * Mathf.Deg2Rad)).normalized * Mathf.Lerp(arrowSpeed / 4, arrowSpeed, Mathf.Clamp(chargeTime + negCharge, 0, arrowSpeed) / 2);
                                Destroy(p1, 5);
                            }
                        }
                        else
                        {
                            GameObject p = Instantiate(arrow, transform.position, rotPoint.transform.rotation, null);

                            p.GetComponent<Rigidbody2D>().linearVelocity = (rb.linearVelocity / 8 + lastInput) * Mathf.Lerp(arrowSpeed / 4, arrowSpeed, Mathf.Clamp(chargeTime + negCharge, 0, arrowSpeed) / 2);
                            p.GetComponent<ProjectileHandler>().damage = Mathf.RoundToInt(Mathf.Lerp(damage, damage * 4, chargeTime - (chargeTime + negCharge) / 2)); //changes arrow speed and dmg based on charge time
                            p.GetComponent<ProjectileHandler>().creator = this.gameObject;
                        }
                        StartCoroutine(attackCooldown(.5f));
                    }
                    break;

                case 3: //trident
                    canAttack = false;
                    tridantHitbox.SetActive(true);
                    StartCoroutine(attackCooldown(.4f, .1f, tridantHitbox));
                    animator.SetTrigger("RAttack");
                    break;

                default:
                    Debug.LogError("class type isnt 1-3");
                    break;
            }
        }
    }

    IEnumerator attackCooldown(float time, float hitboxActiveTime = 0, GameObject hitbox = null)
    {
        yield return new WaitForSeconds(hitboxActiveTime);
        if (hitbox != null)
            hitbox.SetActive(false);

        yield return new WaitForSeconds(time - hitboxActiveTime);
        canAttack = true;
    }

    public void secondary(InputAction.CallbackContext phase)
    {
        if (timeSinceAction >= .2f && canNet|| phase.canceled && blocking)
        {
            timeSinceAction = 0;
            switch (gameManager.classType)
            {
                case 1: //block
                    if (phase.started)
                    {
                        StopCoroutine(attackCooldown(.25f, .1f));
                        blocking = true;
                        canAttack = false;
                        shield.SetActive(true);
                    }
                    else if (phase.canceled)
                    {
                        StartCoroutine(attackCooldown(.25f));
                        blocking = false;
                        shield.SetActive(false);
                    }
                    break;

                case 2: //dodge
                    if (phase.started && canDodge > 0)
                    {
                        StartCoroutine(dodgeCooldown(upgrades[1] > 0 ? .75f : 1.5f));
                        dodgeing = true;
                        GetComponent<CircleCollider2D>().excludeLayers = 8;
                        canDodge--;
                        rb.linearVelocity = move.action.ReadValue<Vector2>() * speed * 3;
                        if (upgrades[1] > 2)
                            dodgeHitBox.SetActive(true);
                    }
                    break;

                case 3: //throw net
                    if (phase.started)
                    {
                        GameObject n = Instantiate(net, transform.position, rotPoint.transform.rotation, null);
                        n.GetComponent<Rigidbody2D>().linearVelocity = (rb.linearVelocity / 8 + lastInput) * netSpeed;
                        n.GetComponent<ProjectileHandler>().creator = this.gameObject;
                        canNet = false;
                        StartCoroutine(netCoolDown());
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private IEnumerator dodgeCooldown(float time)
    {
        yield return new WaitForSeconds(.15f);
        dodgeing = false;
        dodgeHitBox.SetActive(false);
        GetComponent<CircleCollider2D>().excludeLayers = 0;

        yield return new WaitForSeconds(time);
        canDodge++;
    }

    private IEnumerator netCoolDown()
    {
        yield return new WaitForSeconds(upgrades[1] > 1 ? 10 : 15);
        canNet = true;
    }

    public float susytime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("rock"))
        {
            susytime += Time.deltaTime;
            if (susytime > 60)
                sisyphus.SetActive(true);
        }
    }

    private void pAnim(InputAction.CallbackContext phase)
    {
        float val = move.action.ReadValue<Vector2>().x;
        animator.SetBool("Walking", val != 0 || move.action.ReadValue<Vector2>().y != 0);
        if (val > 0)
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        else if (val < 0)
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }


    //pause menu stuff
    public void pause(InputAction.CallbackContext phase)
    {
        if (phase.started)
        {
            Time.timeScale = (Time.timeScale == 1 ? 0 : 1);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(resumeButton);
            pauseMenu.SetActive(Time.timeScale == 0);
        }
    }

    public void resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void menu()
    {
        unbindControlls();
        gameManager.menu();
    }

    private void unbindControlls()
    {
        move.action.Disable();
        primaryButton.action.Disable();
        secondaryButton.action.Disable();
        pauseButton.action.Disable();
    }
}

