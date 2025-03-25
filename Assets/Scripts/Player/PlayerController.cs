using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public GameObject sisyphus;
    private Rigidbody2D rb;
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

    [Header("weapons")]
    private GameObject weapon;
    public GameObject swordHitbox;
    public GameObject tridantHitbox;
    public GameObject shield;
    public GameObject arrow;
    public float arrowSpeed;
    public GameObject net;
    public float netSpeed;

    [Header("Controlls")]
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

    [Header("Upgrade stuffs")]
    // Secutor upgrades:
    //// upgrade path 1: DoT / Faster Swings / Enemies take more damage while DoT is in effect (done)
    //// upgrade path 2: blocking blocks more damage / easier perfect blocks (done) / enemies take more damage after perfect block
    // Sagittarius upgrades:
    //// upgrade path 1: faster charges / fire arrows / more arrows shot
    //// upgrade path 2: faster dodge cooldown / 2 dodge charges / dodging through enemies damage them
    // Retiarius upgrades:
    //// upgrade path 1: longer attack range / wider attack range / more damage
    //// upgrade path 2: effect-damge increase / knockback/ knockback damage
    // upgrade path 3: faster move speed / more health / more damage
    public int[] upgrades = { 0, 0, 0 };

    [Header("Misc.")]
    public GameObject hitEffect;
    private float direction;
    private float timeSinceAction;
    private float iframes;

    private bool blocking;
    private bool chargingDodge;
    private int canDodge = 1;
    private bool canAttack = true;
    private bool dodgeing;
    private bool canNet = true;
    private void Start()
    {
        move.action.Disable();
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.playerController = this;
        weapon = GameObject.Find("sword");

        damage = Mathf.RoundToInt((gameManager.challenges[0] ? damage / 2 : damage) * (upgrades[2] > 2 ? 1.25f : 1));

        fadeScreen = GameObject.Find("death screen").GetComponent<Image>();
        fadeText = GameObject.Find("death text").GetComponent<TextMeshProUGUI>();
        winScreen = GameObject.Find("win screen").GetComponent<Image>();
        winText = GameObject.Find("win text").GetComponent<TextMeshProUGUI>();
    }

    public void fakeStart()
    {
        health += (upgrades[2] > 1 ? health / 4 : 0);
        maxHealth += (upgrades[2] > 1 ? maxHealth / 4 : 0);

        move.action.Enable();
        primaryButton.action.started += primary;
        if (gameManager.classType == 2)
            primaryButton.action.canceled += primary;
        if (!gameManager.challenges[2])
        {
            secondaryButton.action.started += secondary;
            secondaryButton.action.canceled += secondary;
        }
    }

    private void Update()
    {
        healthBar.fillAmount = (float)health / (float)maxHealth;
        iframes -= Time.deltaTime;
        timeSinceAction += Time.deltaTime;
        if (!dodgeing && health > 0)
            rb.linearVelocity = move.action.ReadValue<Vector2>() * speed * ((blocking || (gameManager.classType == 2 && primaryButton.action.inProgress)) ? .5f : 1) * (upgrades[0] > 0 ? 1.25f : 1);

        if (rb.linearVelocity != Vector2.zero && !dodgeing)
        {
            lastInput = move.action.ReadValue<Vector2>();
            direction = (Mathf.Atan2(move.action.ReadValue<Vector2>().y, move.action.ReadValue<Vector2>().x) * Mathf.Rad2Deg) - 90;
        }
        rotPoint.transform.rotation = Quaternion.Euler(new Vector3(0, 0, direction));

        if (blocking || gameManager.classType == 2 && primaryButton.action.inProgress && canAttack)
        {
            if (chargeTime < 4)
                chargeTime += Time.deltaTime * (gameManager.classType == 2 && upgrades[0] > 0 ? 1.25f : 1) * (gameManager.classType == 1 && upgrades[1] > 1 ? .75f : 1);
            else
                negCharge -= Time.deltaTime;
        }
        else
        {
            chargeTime = 0;
            negCharge = 0;
        }
        chargeBar.fillAmount = (chargeTime + negCharge) / 2;

        if (health <= 0)
        {
            Color tempScreen = fadeScreen.color;
            tempScreen.a += Time.deltaTime / 3;
            fadeScreen.color = tempScreen;

            Color tempText = fadeText.color;
            tempText.a += Time.deltaTime / 3;
            fadeText.color = tempText;
            if (tempScreen.a - (2 / 3) > 1)
                gameManager.menu();
        }

        if (gameManager.bossesDead >= gameManager.totalBosses && gameManager.totalBosses != 0)
        {
            Color tempScreen = winScreen.color;
            tempScreen.a += Time.deltaTime / 3;
            winScreen.color = tempScreen;

            Color tempText = winText.color;
            tempText.a += Time.deltaTime / 3;
            winText.color = tempText;
            if (tempScreen.a - (2 / 3) > 1)
                gameManager.menu();
        }

        if (canDodge < (upgrades[1] > 1 ? 2 : 1) && !chargingDodge)
        {
            chargingDodge = true;
            StartCoroutine(dodgeCooldown(.15f * (upgrades[1] > 0 ? .75f : 1)));
        }
    }

    public void takeDamage(int damage)
    {
        if (iframes < 0)
        {
            if (blocking && chargeTime <= .3)
            {
                print("perfect block");
                iframes = .15f;
            }
            else if (!dodgeing)
            {
                health -= blocking ? damage / (gameManager.classType == 0 && upgrades[1] > 0 ? 8 : 4) : damage;
                hitEffect.SetActive(true);
                StartCoroutine(hitVXF());
                iframes = .3f;
            }

            if (health <= 0)
            {
                StopAllCoroutines();
                primaryButton.action.started -= primary;
                secondaryButton.action.started -= secondary;
                secondaryButton.action.canceled -= secondary;
            }
        }
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
                    StartCoroutine(attackCooldown(.4f * (upgrades[0] > 0 ? .75f : 1), .1f, swordHitbox));
                    break;

                case 2: //bow
                    if (phase.canceled && canAttack)
                    {
                        canAttack = false;
                        GameObject p = Instantiate(arrow, transform.position, rotPoint.transform.rotation, null);

                        p.GetComponent<Rigidbody2D>().linearVelocity = (rb.linearVelocity / 8 + lastInput) * Mathf.Lerp(arrowSpeed / 4, arrowSpeed, Mathf.Clamp(chargeTime + negCharge, 0, arrowSpeed) / 2);
                        p.GetComponent<ProjectileHandler>().damage = Mathf.RoundToInt(Mathf.Lerp(damage, damage * 4, chargeTime - (chargeTime + negCharge) / 2)); //changes arrow speed and dmg based on charge time
                        p.GetComponent<ProjectileHandler>().creator = this.gameObject;
                        StartCoroutine(attackCooldown(.5f));
                    }
                    break;

                case 3: //trident
                    canAttack = false;
                    tridantHitbox.SetActive(true);
                    StartCoroutine(attackCooldown(.4f, .1f, tridantHitbox));
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
                        dodgeing = true;
                        canDodge--;
                        rb.linearVelocity = move.action.ReadValue<Vector2>() * speed * 3;
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
        yield return new WaitForSeconds(time);
        dodgeing = false;

        yield return new WaitForSeconds(.75f);
        canDodge++;
        chargingDodge = false;
    }

    private IEnumerator netCoolDown()
    {
        yield return new WaitForSeconds(15);
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
}

