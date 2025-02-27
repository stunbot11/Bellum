using UnityEngine;

public class Lion : MonoBehaviour
{
    private EnemyController enemyController;

    public int thisTigerNum;
    [HideInInspector] public int pendingAttack;
    private Lion tiger1;
    private Lion tiger2;
    private Lion tiger3;

    public GameObject biteHitBox;
    public GameObject slashHitBox;
    public GameObject lungeHitBox;

    public float attackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        pendingAttack = Random.Range(1, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyController.canAttack && Vector2.Distance(transform.position, enemyController.player.transform.position) < attackRange)
            attack();
    }

    public void attack()
    {
        enemyController.canAttack = false;
        switch (pendingAttack)
        {
            case 1: //bite
                biteHitBox.SetActive(true);
                StartCoroutine(enemyController.hitboxCooldown(biteHitBox, 1.5f));
                break;

            case 2: //slash
                slashHitBox.SetActive(true);
                StartCoroutine(enemyController.hitboxCooldown(slashHitBox, 1.5f));
                break;

            case 3: //group lunge
                lungeHitBox.SetActive(true);
                StartCoroutine(enemyController.hitboxCooldown(lungeHitBox, 1.5f));
                break;
        }

        pendingAttack = Random.Range(1, 3);
    }
}
