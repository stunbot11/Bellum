using System.Collections;
using UnityEngine;
using UnityEngine.WSA;

public class Commodus : MonoBehaviour
{
    private EnemyController enemyController;

    [Header("atk stats")]
    public GameObject arrow;
    [HideInInspector] public int pendingAttack;
    public float attackRange;

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
    }

    private void Update()
    {
        if (enemyController.canAttack)
            attack();
    }

    private void attack()
    {
        pendingAttack = Random.Range(1, 5);
        enemyController.canAttack = false;
        enemyController.canMove = false;
        switch (pendingAttack)
        {
            case 1: // single shot
                GameObject p = Instantiate(arrow, transform.position, Quaternion.identity, null);
                ProjectileHandler projectileData = p.GetComponent<ProjectileHandler>();
                projectileData.creator = this.gameObject;
                projectileData.damage = singleDmg;
                p.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Sin(enemyController.rb.rotation * Mathf.Rad2Deg), Mathf.Cos(enemyController.rb.rotation * Mathf.Rad2Deg)).normalized * arrowSpeed;
                Destroy(p, 5);
                StartCoroutine(enemyController.cooldown(1));
                break;
                
            case 2: // triple shot
                float ang1 = (Mathf.Sin(enemyController.rb.rotation * Mathf.Rad2Deg) / Mathf.Cos(enemyController.rb.rotation * Mathf.Rad2Deg) * Mathf.Rad2Deg);
                for (int i = 0; i < tripNum; i++)
                {
                    float ang = Mathf.LerpAngle(ang1 - 45, ang1 + 45, (i / (float)tripNum));
                    GameObject p1 = Instantiate(arrow, transform.position, Quaternion.identity, null);
                    ProjectileHandler projectileData1 = p1.GetComponent<ProjectileHandler>();
                    projectileData1.creator = this.gameObject;
                    projectileData1.damage = tripDmg;
                    p1.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad)).normalized * arrowSpeed;
                    Destroy(p1, 5);
                    StartCoroutine(enemyController.cooldown(2));
                }
                break;

            case 3: // burst shot
                StartCoroutine(burst());
                break;

            case 4: // volly shot
                volly.transform.position = enemyController.player.transform.position;
                StartCoroutine(vollyStuff(.5f));
                break;
        }
    }

    IEnumerator burst()
    {
        for (int i = 0; i < burstNum; i++)
        {
            GameObject p = Instantiate(arrow, transform.position, Quaternion.identity, null);
            ProjectileHandler projectileData = p.GetComponent<ProjectileHandler>();
            projectileData.creator = this.gameObject;
            projectileData.damage = singleDmg;
            p.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Sin(enemyController.rb.rotation * Mathf.Rad2Deg), Mathf.Cos(enemyController.rb.rotation * Mathf.Rad2Deg)).normalized * arrowSpeed;
            Destroy(p, 5);
            StartCoroutine(enemyController.cooldown(1));
            Destroy(p, 5);
            yield return new WaitForSeconds(burstSpeed);
        }
        StartCoroutine(enemyController.cooldown(2));
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
}
