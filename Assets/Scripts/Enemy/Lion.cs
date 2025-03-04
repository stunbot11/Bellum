using UnityEngine;

public class Lion : MonoBehaviour
{
    private EnemyController enemyController;

    public int thisLionNum;
    [HideInInspector] public int pendingAttack;
    public Lion lion1;
    public Lion lion2;
    public Lion lion3;

    private bool ready;

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

        if (pendingAttack == 4 && enemyController.gameManager.lionCheck == enemyController.gameManager.lionReady)
        {
            lungeATK();
        }

        if ((enemyController.targetOveride ? Vector2.Distance(transform.position, enemyController.target) >= .2 : true) && !ready)
        {
            ready = true;
            enemyController.gameManager.lionReady++;
        }

    }

    public void attack()
    {
        enemyController.canAttack = false;
        pendingAttack = Random.Range(1, 4);
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
                    enemyController.gameManager.lionCheck++;
                }

                if (lion2 != null)
                {
                    lion2.pendingAttack = 4;
                    lion2.enemyController.targetOveride = true;
                    lion2.enemyController.speedMod = 1.5f;
                    lion2.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.left * lungDisMult;
                    enemyController.gameManager.lionCheck++;
                }

                if (lion3 != null)
                {
                    lion3.pendingAttack = 4;
                    lion3.enemyController.targetOveride = true;
                    lion3.enemyController.speedMod = 1.5f;
                    lion3.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.right * lungDisMult;
                    enemyController.gameManager.lionCheck++;
                }
                break;
            case 4:
                enemyController.canAttack = false;
                break;
        }
    }

    public void lungeATK()
    {
        print("lung");
        ready = false;
        enemyController.gameManager.lionCheck = 0;
        enemyController.gameManager.lionReady = 0;
        lungeHitBox.SetActive(true);
        StartCoroutine(enemyController.hitboxCooldown(lungeHitBox, 1.5f));
    }    
}
