using System.Collections;
using UnityEngine;

public class Commodus : MonoBehaviour
{
    private EnemyController enemyController;

    private float moveTime;
    public float spaceAway;
    [Header("atk stats")]
    public GameObject arrow;
    [HideInInspector] public int pendingAttack;
    public float attackRange;
    public bool meGoShootyShootyShootShoot = true;

    [Header("volly")]
    public GameObject volly;
    public int vollyDmg;
    public float vollyRad;
    public float vollyTime;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        volly.GetComponent<ProjectileHandler>().creator = this.gameObject;
        volly.GetComponent<ProjectileHandler>().damage = Mathf.RoundToInt(enemyController.dmgMod * enemyController.attacks[0].dmg);
        enemyController.distance = spaceAway;
    }

    public IEnumerator attack()
    {
        enemyController.canAttack = false;
        enemyController.canMove = false;
        meGoShootyShootyShootShoot = false;
        enemyController.anim.SetTrigger("ArrowRain");
        volly.transform.position = enemyController.player.transform.position;
        yield return new WaitForSeconds(.5f);
        volly.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        volly.SetActive(false);

        yield return new WaitForSeconds(.5f);
        StartCoroutine(enemyController.cooldown(2));
        StartCoroutine(meGoNameThings());
    }

    IEnumerator meGoNameThings()
    {
        yield return new WaitForSeconds(Random.Range(3, 5));
        meGoShootyShootyShootShoot = true;
    }
}
