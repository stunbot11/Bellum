using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    public bool targetOveride;
    [HideInInspector] public Vector2 target;
    [HideInInspector] public Animator anim;

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public Rigidbody2D rb;
    public SpriteRenderer[] hitEffect;
    public bool goingToTarget;
    public GameObject rotPoint;

    public GameObject deathReplacement;

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
    //private int pickYourPoison;

    [HideInInspector] public float angle;
    [HideInInspector] public float distance;

    [Header("Attack")]
    public bool usePhase;
    public int phase;
    private float timeBetweenAttacks;
    [HideInInspector] public float dmgMod = 1;
    public BossAttacks[] attacks;
    public GameObject[] attacksHitbox;

    void Start()
    {
        target = Vector2.up * 999999;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        dmgMod += gameManager.round * 1.10f;
        gameManager.totalBosses++;
        health = gameManager.challenges[1] ? health * 2 : health;
        canMove = true;
        canAttack = true;
        StartCoroutine(goToTime());
        speed = gameManager.activeEmperor.increaseSpeed ? speed * gameManager.activeEmperor.bossEffectStrength : speed;
        health = gameManager.activeEmperor.increaseHealth ? Mathf.RoundToInt(health * gameManager.activeEmperor.bossEffectStrength) : health;
        anim = GetComponentInChildren<Animator>();

        gameManager.totEHealth += health;
        gameManager.eHealth += health;
    }

    private void Update()
    {
        //gets angle from enemy to player in 8 directions and moves towards them  max and min x of arena                     lower min and max of arena      how far from center    upper min and max                          width of edge minused
        Vector2 targetPos = ((targetOveride ? new Vector2(Mathf.Clamp(target.x, -36, 36), Mathf.Clamp(target.y , Mathf.Lerp(16f, 25.5f, (Mathf.Abs(target.x) - 27) / 8.5f), Mathf.Lerp(35f, 25.5f, (Mathf.Abs(target.x) - 27) / 8.5f))) : player.transform.position) - transform.position).normalized;
        float tempRot = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg * (gameManager.boss == 2 && !targetOveride && Vector2.Distance(player.transform.position, transform.position) <= distance ? -1 : 1);
        angle = (Mathf.Round((tempRot - 45) / 45) * 45 - 45);
        RaycastHit2D objectDect = Physics2D.Raycast(transform.position, rb.linearVelocity.normalized, 2, LayerMask.NameToLayer("Default"));
        if (objectDect.collider != null)
        {
            print(objectDect.collider.gameObject.name + "               " + this.gameObject.name);
            if (objectDect.collider.CompareTag("Untagged"))
            {
                print("object in way");
                angle += 45;
            }
        }
        rotPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
        this.transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * (rb.linearVelocityX > 0 ? -1 : (rb.linearVelocityX < 0 ? 1 : transform.localScale.x / Mathf.Abs(transform.localScale.x))), transform.localScale.y);

        Vector2 moveDir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * -1, Mathf.Cos(angle * Mathf.Deg2Rad)).normalized;
        if ((targetOveride ? Vector2.Distance(transform.position, target) >= 1 : true) && !imbolized && canMove)
        {
            rb.linearVelocity = moveDir * speed * speedMod;
            anim.SetBool("Move", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Move", false);
        }

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

        timeBetweenAttacks -= Time.deltaTime;
        if (canAttack && timeBetweenAttacks < 0)
            atk();
    }

    [HideInInspector] public ArenaHandler arenaHandler;
    public void takeDamage(int damage, bool net = false, string dmgType = null, int ToDoT = 0)
    {
        PlayerController playerController = gameManager.playerController;
        if (ToDoT >= dotTicks)
            dotTicks = ToDoT;
        if (net)
            StartCoroutine(imbolizedCooldown());
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
        StartCoroutine(gameManager.hitEffect(hitEffect));
        float dmgBoost = 1;
        dmgBoost += imbolized && playerController.upgrades[1] > 2 ? .75f : 0; //if imbolized increase damage if have upgrade
        dmgBoost += gameManager.classType == 1 && playerController.upgrades[0] >= 3 && dmgType != "DoT" && inDoT ? .5f : 0; //if in dot and have upgrade increaase damage
        dmgBoost += playerController.upgrades[1] > 2 && playerController.pBlock > 0 ? .5f : 0;
        gameManager.eHealth -= (int)(damage * dmgBoost >= 0 ? damage * dmgBoost : health);
        health -= (int)(damage * dmgBoost);
        gameManager.updateBar();
        if (health <= 0)
        {
            eVocalCords.Pause();
            gameManager.bossesDead++;
            gameManager.lionCheck--;
            deathReplacement.transform.position = this.transform.position;
            deathReplacement.transform.localScale = new Vector2(this.transform.localScale.x > 0 ? deathReplacement.transform.localScale.x : -deathReplacement.transform.localScale.x, deathReplacement.transform.localScale.y);
            deathReplacement.SetActive(true);
            Destroy(this.gameObject);
        }
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
        timeBetweenAttacks = Random.Range(0.5f, 3f);
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

    void atk()
    {
        canAttack = false;
        int atkNum = Random.Range(0, attacks.Length);
        if (attacks[atkNum].attackRange <= Vector2.Distance(player.transform.position, transform.position) || usePhase ? attacks[atkNum].phase == phase : true)
            StartCoroutine(attack(atkNum)); 
        else
            atk();
    }


    IEnumerator attack(int atkNum)
    {
        canMove = false;
        BossAttacks atk = attacks[atkNum];
        GameObject neededHitbox = attacksHitbox[atkNum];

        Vector2 arrowDirectionTemp = (player.transform.position - transform.position).normalized;
        float ang1 = (Mathf.Round(((Mathf.Atan2(arrowDirectionTemp.y, arrowDirectionTemp.x) * Mathf.Rad2Deg) - 45) / 45) * 45 - 45);
        Vector2 arrowDirection = new Vector2(Mathf.Sin(ang1 * Mathf.Deg2Rad) * -1, Mathf.Cos(ang1 * Mathf.Deg2Rad)).normalized;

        switch (atk.attack)
        {
            case BossAttacks.atk.meleeSwing:
                anim.SetTrigger(atk.animName);
                yield return new WaitForSeconds(atk.teleTime);
                eVocalCords.PlayOneShot(atk.sfx);
                neededHitbox.GetComponentInChildren<AttackHandler>().damage = Mathf.RoundToInt(atk.dmg * dmgMod);
                neededHitbox.SetActive(true);
                StartCoroutine(cooldown(atk.cooldownTime, neededHitbox));
                break;

            case BossAttacks.atk.meleeBurst:
                anim.SetTrigger(atk.animName);
                neededHitbox.GetComponentInChildren<AttackHandler>().damage = Mathf.RoundToInt(atk.dmg * dmgMod);
                for (int i = 0; i < atk.mbAmount; i++)
                {
                    canMove = false;
                    yield return new WaitForSeconds(atk.teleTime);
                    eVocalCords.PlayOneShot(atk.sfx);
                    neededHitbox.SetActive(true);
                    yield return new WaitForSeconds(.1f);
                    neededHitbox.SetActive(false);
                    canMove = true;
                    yield return new WaitForSeconds(.5f);
                }
                StartCoroutine(cooldown(atk.cooldownTime));
                break;

            case BossAttacks.atk.rangedSingle:
                print("base" + atk.dmg);
                print("mod" + dmgMod);
                print("both" + atk.dmg * dmgMod);
                print("rounded" + Mathf.RoundToInt(atk.dmg * dmgMod));
                anim.SetTrigger(atk.animName);
                yield return new WaitForSeconds(atk.teleTime);
                eVocalCords.PlayOneShot(atk.sfx);
                shoot(neededHitbox, ang1, Mathf.RoundToInt(atk.dmg * dmgMod), arrowDirection, atk.projSpeed, atk.projLifeTime);
                StartCoroutine(cooldown(atk.cooldownTime));
                break;

            case BossAttacks.atk.rangedBurst:
                anim.SetTrigger(atk.animName);
                for (int i = 0; i < atk.rbAmount; i++)
                {
                    yield return new WaitForSeconds(atk.teleTime);
                    eVocalCords.PlayOneShot(atk.sfx);
                    shoot(neededHitbox, ang1, Mathf.RoundToInt(atk.dmg * dmgMod), arrowDirection, atk.projSpeed, atk.projLifeTime);
                    yield return new WaitForSeconds(.3f);
                }
                StartCoroutine(cooldown(atk.cooldownTime));
                break;

            case BossAttacks.atk.rangedTrip:
                anim.SetTrigger(atk.animName);
                yield return new WaitForSeconds(atk.teleTime);
                for (int i = 0; i < atk.rtAmount; i++)
                {
                    float ang = Mathf.Lerp(ang1 - 45, ang1 + 45, (i / (float)atk.rtAmount));
                    print(ang);
                    eVocalCords.PlayOneShot(atk.sfx);
                    GameObject p1 = Instantiate(neededHitbox, transform.position, Quaternion.identity, null);
                    p1.GetComponent<Rigidbody2D>().rotation = ang;
                    ProjectileHandler projectileData1 = p1.GetComponent<ProjectileHandler>();
                    projectileData1.creator = this.gameObject;
                    projectileData1.damage = Mathf.RoundToInt(atk.dmg * dmgMod);
                    p1.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Sin(ang * Mathf.Deg2Rad) * -1, Mathf.Cos(ang * Mathf.Deg2Rad)).normalized * atk.projSpeed;
                    Destroy(p1, atk.projLifeTime);
                }
                StartCoroutine(cooldown(atk.cooldownTime));
                break;


            case BossAttacks.atk.lunge:
                anim.SetTrigger(atk.animName);
                yield return new WaitForSeconds(atk.teleTime);
                eVocalCords.PlayOneShot(atk.sfx);
                spearThrown = true;
                targetOveride = true;
                goingToTarget = true;
                target = arrowDirection * 10000;
                speedMod = atk.projSpeed;
                yield return new WaitForSeconds(atk.projLifeTime);
                spearThrown = false;
                spearThrown = false;
                targetOveride = false;
                goingToTarget = false;
                speedMod = 1;
                break;

            case BossAttacks.atk.special:
                yield return new WaitForSeconds(atk.teleTime);
                if (TryGetComponent<Lion>(out Lion lion))
                {
                    lion.attack();
                }
                else if (TryGetComponent<Commodus>(out Commodus commodus))
                {
                    StartCoroutine(commodus.attack());
                } // janus doesnt have special attack, he has phases that does it automatically
                break;
        }
    }

    void shoot(GameObject proj, float dir, int dmg, Vector2 bDir, float speed, float lifeTime)
    {
        GameObject p = Instantiate(proj, transform.position, Quaternion.identity, null);
        ProjectileHandler projectileData = p.GetComponent<ProjectileHandler>();
        p.GetComponent<Rigidbody2D>().rotation = dir;
        projectileData.creator = this.gameObject;
        projectileData.damage = dmg;
        p.GetComponent<Rigidbody2D>().linearVelocity = bDir * speed;
        Destroy(p, lifeTime);
    }
}
