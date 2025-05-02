using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    public bool targetOveride;
    [HideInInspector] public Vector2 target;

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public Rigidbody2D rb;
    public GameObject hitEffect;
    public bool goingToTarget;

    [Header("Stats")]
    [HideInInspector] public float effectMod;
    public int health;
    public float speed;
    public float speedMod = 1;

    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool imbolized;
    [HideInInspector] public bool spearThrown;

    private bool inDoT;
    public int dotTicks;

    [Header("SFX")]
    public AudioSource eVocalCords;
    public AudioClip steppy;
    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip hurtAagh;
    public AudioClip hurtHoogh;
    public AudioClip hurtOugh;
    public AudioClip netHit;
    private bool canSteppy = true;
    //private int pickYourPoison;

    [HideInInspector] public float angle;
    [HideInInspector] public float distance;

    void Start()
    {
        target = Vector2.up * 999999;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameManager.totalBosses++;
        health = gameManager.challenges[1] ? health * 2 : health;
        canMove = true;
        canAttack = true;
        StartCoroutine(goToTime());
        speed = gameManager.activeEmperor.increaseSpeed ? speed * gameManager.activeEmperor.bossEffectStrength : speed;
        health = gameManager.activeEmperor.increaseHealth ? Mathf.RoundToInt(health * gameManager.activeEmperor.bossEffectStrength) : health;
    }

    private void Update()
    {
        //gets angle from enemy to player in 8 directions and moves towards them
        Vector2 targetPos = ((targetOveride ? new Vector2(Mathf.Clamp(target.x, -38, 38), Mathf.Clamp(target.y , Mathf.Lerp(12f, 17.5f, (Mathf.Abs(target.x) - 25.5f) / 12.5f), Mathf.Lerp(36f, 30.5f, (Mathf.Abs(target.x) - 25.5f) / 12.5f))) : player.transform.position) - transform.position).normalized;
        float tempRot = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg * (gameManager.boss == 2 && !targetOveride && Vector2.Distance(player.transform.position, transform.position) <= distance ? -1 : 1);
        angle = (Mathf.Round((tempRot - 45) / 45) * 45 - 45);
        RaycastHit2D objectDect = Physics2D.Raycast(transform.position, rb.linearVelocity.normalized, 2, LayerMask.NameToLayer("Default"));
        print(new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * -1, Mathf.Cos(angle * Mathf.Deg2Rad)).normalized);
        if (objectDect.collider != null)
        {
            print(objectDect.collider.gameObject.name + "               " + this.gameObject.name);
            if (objectDect.collider.CompareTag("Untagged"))
            {
                print("object in way");
                angle += 45;
            }
        }
        rb.rotation = angle;

        Vector2 moveDir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * -1, Mathf.Cos(angle * Mathf.Deg2Rad)).normalized;
        if ((targetOveride ? Vector2.Distance(transform.position, target) >= 1 : true) && !imbolized && canMove)
            rb.linearVelocity = moveDir * speed * speedMod;
        else
            rb.linearVelocity = Vector2.zero;

        if (Vector2.Distance(transform.position, target) <= 1 && goingToTarget && !spearThrown)
        {
            goingToTarget = false;
            targetOveride = false;
            StartCoroutine(goToTime());
        }

        if (dotTicks > 0 && !inDoT)
        {
            inDoT = true;
            StartCoroutine(DoT());
        }

        if (gameManager.boss < 3 && goingToTarget && canSteppy)
            StartCoroutine(biggerSteppy());
    }

    public void takeDamage(int damage, bool net = false, string dmgType = null, int ToDoT = 0)
    {
        PlayerController playerController = gameManager.playerController;
        if (ToDoT >= dotTicks)
            dotTicks = ToDoT;
        if (net)
            StartCoroutine(imbolizedCooldown());
        hitEffect.SetActive(true);
        if (!net)
        {
                switch (Random.Range(1, 3))
                {
                    case 1:
                        eVocalCords.PlayOneShot(hurtAagh);
                        break;

                    case 2:
                        eVocalCords.PlayOneShot(hurtHoogh);
                        break;

                    case 3:
                        eVocalCords.PlayOneShot(hurtOugh);
                        break;

                    default:
                        break;
                }
        }
        else eVocalCords.PlayOneShot(netHit);
        StartCoroutine(hitEffectStop());
        float dmgBoost = 1;
        dmgBoost += imbolized && playerController.upgrades[1] > 2 ? .75f : 0; //if imbolized increase damage if have upgrade
        dmgBoost += gameManager.classType == 1 && playerController.upgrades[0] >= 3 && dmgType != "DoT" && inDoT ? .5f : 0; //if in dot and have upgrade increaase damage
        dmgBoost += playerController.upgrades[1] > 2 && playerController.pBlock > 0 ? .5f : 0;
        health -= (int)(damage * dmgBoost);
        if (health <= 0)
        {
            eVocalCords.Pause();
            gameManager.bossesDead++;
            Destroy(this.gameObject);
        }
    }

    
    IEnumerator hitEffectStop()
    {
        yield return new WaitForSeconds(.2f);
        hitEffect.SetActive(false);
    }

    public IEnumerator cooldown(float cooldown, GameObject hitbox = null)
    {
        if (hitbox != null)
        {
            yield return new WaitForSeconds(.1f);
            hitbox.SetActive(false);
        }

        yield return new WaitForSeconds(cooldown);
        canAttack = true;
        canMove = true;
    }

    public IEnumerator imbolizedCooldown()
    {
        yield return new WaitForSeconds(player.GetComponent<PlayerController>().upgrades[1] > 0 ? 6 : 3);
        imbolized = false;
    }

    private void goToRandom()
    {
        Vector2 goTo = new Vector2(Random.Range(transform.position.x - 5, transform.position.x + 5), Random.Range(transform.position.y - 5, transform.position.y + 5));
        target = goTo;
        targetOveride = true;
        goingToTarget = true;
    }

    IEnumerator goToTime()
    {
        yield return new WaitForSeconds(Random.Range(4f, 10f));
        if (!spearThrown)
            goToRandom();
        else
            StartCoroutine(goToTime());
    }

    IEnumerator DoT()
    {
        dotTicks--;
        yield return new WaitForSeconds(1);
        takeDamage(5, false, "DoT");
        inDoT = false;
    }

    IEnumerator biggerSteppy()
    {
        canSteppy = false;
        eVocalCords.PlayOneShot(steppy);
        yield return new WaitForSeconds(0.17f);
        canSteppy = true;
    }
}
