using System.Collections;
using UnityEngine;

public class Commodus : MonoBehaviour
{
    private EnemyController enemyController;

    private float moveTime;
    public float spaceAway;
    [Header("atk stats")]
    [HideInInspector] public Vector2 arrowDirection;
    public GameObject arrow;
    [HideInInspector] public int pendingAttack;
    public float attackRange;
    public bool meGoShootyShootyShootShoot = true;
    public float ang1;

    public int singleDmg;
    public float arrowSpeed;

    [Header("burst")]
    public int burstNum;
    public float burstSpeed;
    public int burstDmg;

    [Header("triple shot")]
    public int tripNum;
    public int tripDmg;

    [Header("volly")]
    public GameObject volly;
    public int vollyDmg;
    public float vollyRad;
    public float vollyTime;
    




    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        volly.GetComponent<ProjectileHandler>().creator = this.gameObject;
        pendingAttack = Random.Range(1, 3);
        enemyController.distance = spaceAway;
    }

    private void Update()
    {
        if (enemyController.canAttack && meGoShootyShootyShootShoot && !enemyController.imbolized) // add another bool that will happend during cooldown that lets player move
            attack();
    }

    private void attack()
    { // set attack vars to false and get rotation needed for arrow
        pendingAttack = Random.Range(1, 5);
        enemyController.canAttack = false;
        enemyController.canMove = false;
        meGoShootyShootyShootShoot = false;
        Vector2 arrowDirectionTemp = (enemyController.player.transform.position -transform.position).normalized;
        ang1 = (Mathf.Round(((Mathf.Atan2(arrowDirectionTemp.y, arrowDirectionTemp.x) * Mathf.Rad2Deg) - 45) / 45) * 45 - 45);
        arrowDirection = new Vector2(Mathf.Sin(ang1 * Mathf.Deg2Rad) * -1, Mathf.Cos(ang1 * Mathf.Deg2Rad)).normalized;

        switch (pendingAttack)
        {
            case 1: // single shot
                enemyController.eVocalCords.PlayOneShot(enemyController.attack1);
                GameObject p = Instantiate(arrow, transform.position, Quaternion.identity, null);
                ProjectileHandler projectileData = p.GetComponent<ProjectileHandler>();
                p.GetComponent<Rigidbody2D>().rotation = ang1;
                projectileData.creator = this.gameObject;
                projectileData.damage = singleDmg;
                p.GetComponent<Rigidbody2D>().linearVelocity = arrowDirection * arrowSpeed;
                Destroy(p, 5);
                StartCoroutine(enemyController.cooldown(1));
                break;
                
            case 2: // triple shot

                for (int i = 0; i < tripNum; i++)
                {
                    float ang = Mathf.Lerp(ang1 - 45, ang1 + 45, (i / (float)tripNum));
                    print(ang);
                    enemyController.eVocalCords.PlayOneShot(enemyController.attack1);
                    GameObject p1 = Instantiate(arrow, transform.position, Quaternion.identity, null);
                    p1.GetComponent<Rigidbody2D>().rotation = ang;
                    ProjectileHandler projectileData1 = p1.GetComponent<ProjectileHandler>();
                    projectileData1.creator = this.gameObject;
                    projectileData1.damage = tripDmg;
                    p1.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Sin(ang * Mathf.Deg2Rad) * -1, Mathf.Cos(ang * Mathf.Deg2Rad)).normalized * arrowSpeed;
                    Destroy(p1, 5);
                }
                StartCoroutine(enemyController.cooldown(2));
                break;

            case 3: // burst shot
                StartCoroutine(burst());
                break;

            case 4: // volly shot
                volly.transform.position = enemyController.player.transform.position;
                StartCoroutine(vollyStuff(.5f));
                break;
        }
        StartCoroutine(meGoNameThings());
    }

    
    IEnumerator burst()
    {
        for (int i = 0; i < burstNum; i++)
        {
            yield return new WaitForSeconds(burstSpeed);
            enemyController.eVocalCords.PlayOneShot(enemyController.attack1);
            GameObject p = Instantiate(arrow, transform.position, Quaternion.identity, null);
            p.GetComponent<Rigidbody2D>().rotation = ang1;
            ProjectileHandler projectileData = p.GetComponent<ProjectileHandler>();
            projectileData.creator = this.gameObject;
            projectileData.damage = singleDmg;
            p.GetComponent<Rigidbody2D>().linearVelocity = arrowDirection * arrowSpeed;
            Destroy(p, 5);
        }
        StartCoroutine(enemyController.cooldown(1));
    }

    IEnumerator vollyStuff(float telegraph)
    {
        yield return new WaitForSeconds(telegraph);
        volly.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        volly.SetActive(false);

        yield return new WaitForSeconds(.5f);
        StartCoroutine(enemyController.cooldown(2));
    }

    IEnumerator meGoNameThings()
    {
        yield return new WaitForSeconds(Random.Range(3, 5));
        meGoShootyShootyShootShoot = true;
    }
}
