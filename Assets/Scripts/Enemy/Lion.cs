using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Lion : MonoBehaviour
{
    private EnemyController enemyController;

    public int thisLionNum;
    [HideInInspector] public int pendingAttack;
    public Lion lion1;
    public Lion lion2;
    public Lion lion3;

    private bool ready;

    public float lungeDisMult;
    public float lungeSpeed;

    public GameObject biteHitBox;
    public GameObject slashHitBox;
    public GameObject lungeHitBox;

    public float attackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyController.canAttack && Vector2.Distance(transform.position, enemyController.player.transform.position) < attackRange && pendingAttack != 4)
            attack();

        if (pendingAttack == 4 && enemyController.gameManager.lionCheck == enemyController.gameManager.lionReady && enemyController.gameManager.lionCheck != 0 && ready)
        {
            lungeATK();
        }

        if ((enemyController.targetOveride ? Vector2.Distance(transform.position, enemyController.target) <= .2 : true) && !ready && pendingAttack == 4)
        {
            ready = true;
            enemyController.gameManager.lionReady++;
        }
    }

    public void attack()
    {
        enemyController.canAttack = false;
        float temp = Random.Range(1f, 100.0f);

        if (temp <= 50)
            pendingAttack = 1;
        else if (temp <= 66.66f)
            pendingAttack = 3;
        else
            pendingAttack = 2;
        switch (pendingAttack)
        {
            case 1: //bite
                biteHitBox.SetActive(true);
                StartCoroutine(enemyController.cooldown(1.5f, biteHitBox));
                break;

            case 2: //slash
                slashHitBox.SetActive(true);
                StartCoroutine(enemyController.cooldown(1.5f, slashHitBox));
                break;

            case 3: //group lunge
                if (lion1 != null)
                {
                    enemyController.gameManager.lionCheck++;
                    lion1.pendingAttack = 4;
                    lion1.enemyController.targetOveride = true;
                    lion1.enemyController.speedMod = 3f;
                    lion1.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.up * lungeDisMult;
                }

                if (lion2 != null)
                {
                    enemyController.gameManager.lionCheck++;
                    lion2.pendingAttack = 4;
                    lion2.enemyController.targetOveride = true;
                    lion2.enemyController.speedMod = 3f;
                    lion2.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.left * lungeDisMult;
                }

                if (lion3 != null)
                {
                    enemyController.gameManager.lionCheck++;
                    lion3.pendingAttack = 4;
                    lion3.enemyController.targetOveride = true;
                    lion3.enemyController.speedMod = 3f;
                    lion3.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.right * lungeDisMult;
                }
                break;
            case 4:
                enemyController.canAttack = false;
                break;
        }
    }
    public void lungeATK()
    {
        enemyController.rb.linearVelocity = (thisLionNum == 1 ? Vector2.down : (thisLionNum == 2 ? Vector2.right : Vector2.left)) * lungeSpeed;
        lungeHitBox.SetActive(true);
        StartCoroutine(lungeTime(2f));
    }    

    IEnumerator lungeTime(float time)
    {
        yield return new WaitForSeconds(time);
        ready = false;
        enemyController.gameManager.lionCheck = 0;
        enemyController.gameManager.lionReady = 0;
        enemyController.targetOveride = false;
        enemyController.speedMod = 1;
        enemyController.target = Vector2.up * 999999;
        StartCoroutine(enemyController.cooldown(1, lungeHitBox));
        pendingAttack = Random.Range(1, 4);
    }
}
