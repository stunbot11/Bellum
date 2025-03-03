using UnityEngine;

public class Lion : MonoBehaviour
{
    private EnemyController enemyController;

    public int thisLionNum;
    [HideInInspector] public int pendingAttack;
    private Lion lion1;
    private Lion lion2;
    private Lion lion3;

    [HideInInspector] public bool inPos;

    public float lungDisMult;

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
        if (enemyController.canAttack && Vector2.Distance(transform.position, enemyController.player.transform.position) < attackRange && pendingAttack != 4)
            attack();
        if (pendingAttack == 4 && lion1.inPos && lion2.inPos && lion3.inPos)
            lungeATK();
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
                if (lion1 != null)
                {
                    lion1.pendingAttack = 4;
                    lion1.enemyController.targetOveride = true;
                    lion1.enemyController.speedMod = 1.5f;
                    lion1.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.up * lungDisMult;
                }

                if (lion2 != null)
                {
                    lion2.pendingAttack = 4;
                    lion2.enemyController.targetOveride = true;
                    lion2.enemyController.speedMod = 1.5f;
                    lion2.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.up * lungDisMult;
                }

                if (lion3 != null)
                {
                    lion3.pendingAttack = 4;
                    lion3.enemyController.targetOveride = true;
                    lion3.enemyController.speedMod = 1.5f;
                    lion3.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.up * lungDisMult;
                }
                break;
            case 4:
                print("group lung");
                break;
        }

        pendingAttack = Random.Range(1, 3);
    }

    public void lungeATK()
    {
        lungeHitBox.SetActive(true);
        StartCoroutine(enemyController.hitboxCooldown(lungeHitBox, 1.5f));
    }    
}
