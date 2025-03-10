using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public GameObject player;
     public bool targetOveride;
    [HideInInspector] public Vector2 target;

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public Rigidbody2D rb;
    public GameObject hitEffect;
    private bool goingToTarget;

    [Header("Stats")]
    public int health;
    public float speed;
    public float speedMod = 1;

    public bool canAttack;
    
    void Start()
    {
        target = Vector2.up * 999999;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameManager.totalBosses++;
        StartCoroutine(goToTime());
    }

    private void Update()
    {
        //gets angle from enemy to player in 8 directions and moves towards them
        Vector2 targetPos = ((targetOveride ? target : player.transform.position) - transform.position).normalized;
        float tempRot = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        float angle = (Mathf.Round((tempRot - 45) / 45) * 45 - 45);
        rb.rotation = angle;

        Vector2 moveDir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * -1, Mathf.Cos(angle * Mathf.Deg2Rad)).normalized;
        if ((targetOveride ? Vector2.Distance(transform.position, target) >= .2 : true))
            rb.linearVelocity = moveDir * speed * speedMod;
        else
            rb.linearVelocity = Vector2.zero;

        if (Vector2.Distance(transform.position, target) <= .3 && goingToTarget)
        {
            goingToTarget = false;
            targetOveride = false;
            StartCoroutine(goToTime());
        }
    }

    public void takeDamage(int damage)
    {
        hitEffect.SetActive(true);
        StartCoroutine(hitEffectStop());
        health -= damage;
        if (health <= 0)
        {
            gameManager.bossesDead++;
            Destroy(this.gameObject);
        }
    }

    
    IEnumerator hitEffectStop()
    {
        yield return new WaitForSeconds(.2f);
        hitEffect.SetActive(false);
    }

    public IEnumerator hitboxCooldown(GameObject hitbox, float cooldown)
    {
        yield return new WaitForSeconds(.1f);
        hitbox.SetActive(false);

        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    private void goToRandom()
    {
        Vector2 goTo = new Vector2(Random.Range(transform.position.x - 5, transform.position.x + 5), Random.Range(transform.position.y - 5, transform.position.y + 5));
        print(goTo + "             goto");
        print(transform.position + "             transform");
        target = goTo;
        targetOveride = true;
        goingToTarget = true;
    }

    IEnumerator goToTime()
    {
        yield return new WaitForSeconds(Random.Range(4f, 10f));
        goToRandom();
    }
}
