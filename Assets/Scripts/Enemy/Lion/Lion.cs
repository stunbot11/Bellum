using System.Collections;
using UnityEngine;

public class Lion : MonoBehaviour
{
    private EnemyController enemyController;

    public int thisLionNum;
    public int pendingAttack;
    public Lion lion1;
    public Lion lion2;
    public Lion lion3;
    public Lion lion4;

    private bool ready;

    public float lungeDisMult;
    public float lungeSpeed;

    public GameObject lungeHitBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        if (enemyController.gameManager.activeEmperor.name == "Commodus")
        {
            if (GameObject.Find("Lion 4"))
            {
                lion4.lion1 = lion1;
                lion4.lion2 = lion2;
                lion4.lion3 = lion3;
                lion4.lion4 = lion4;
            }
            if (thisLionNum == 4)
            {
                enemyController.deathReplacement = GameObject.Find("DRL");
                if (enemyController.gameManager.boss == 1)
                {
                    lion1.lion4 = lion4;
                    lion2.lion4 = lion4;
                    lion3.lion4 = lion4;
                    lion4.lion4 = lion4;
                    enemyController.eVocalCords = lion1.enemyController.eVocalCords;
                }
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pendingAttack == 4 && enemyController.gameManager.lionCheck == enemyController.gameManager.lionReady && enemyController.gameManager.lionCheck != 0 && ready)
        {
            enemyController.anim.SetBool("Lunge", true);
            lungeATK();
            pendingAttack = 0;
        }

        if ((enemyController.targetOveride ? Vector2.Distance(transform.position, new Vector2(Mathf.Clamp(enemyController.target.x, -36, 36), Mathf.Clamp(enemyController.target.y, Mathf.Lerp(16f, 25.5f, (Mathf.Abs(enemyController.target.x) - 27) / 8.5f), Mathf.Lerp(35f, 25.5f, (Mathf.Abs(enemyController.target.x) - 27) / 8.5f)))) <= 1 : true) && !ready && pendingAttack == 4)
        {
            ready = true;
            enemyController.gameManager.lionReady++;
        }
    }

    public void attack()
    {
        enemyController.canAttack = false;

        if (lion1 != null && lion1.enemyController.health > 0)
        {
            enemyController.gameManager.lionCheck++;
            enemyController.eVocalCords.PlayOneShot(enemyController.attack2);
            lion1.pendingAttack = 4;
            lion1.enemyController.canAttack = false;
            lion1.enemyController.targetOveride = true;
            lion1.enemyController.speedMod = 3f;
            lion1.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.up * lungeDisMult;
            lion1.enemyController.spearThrown = true;
        }

        if (lion2 != null && lion2.enemyController.health > 0)
        {
            enemyController.gameManager.lionCheck++;
            enemyController.eVocalCords.PlayOneShot(enemyController.attack2);
            lion2.pendingAttack = 4;
            lion2.enemyController.canAttack = false;
            lion2.enemyController.targetOveride = true;
            lion2.enemyController.speedMod = 3f;
            lion2.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.left * lungeDisMult;
            lion2.enemyController.spearThrown = true;
        }

        if (lion3 != null && lion3.enemyController.health > 0)
        {
            enemyController.gameManager.lionCheck++;
            enemyController.eVocalCords.PlayOneShot(enemyController.attack2);
            lion3.pendingAttack = 4;
            lion3.enemyController.canAttack = false;
            lion3.enemyController.targetOveride = true;
            lion3.enemyController.speedMod = 3f;
            lion3.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.right * lungeDisMult;
            lion3.enemyController.spearThrown = true;
        }

        if (lion4 != null && lion4.enemyController.health > 0 && lion4.isActiveAndEnabled)
        {
            enemyController.gameManager.lionCheck++;
            enemyController.eVocalCords.PlayOneShot(enemyController.attack2);
            lion4.pendingAttack = 4;
            lion4.enemyController.canAttack = false;
            lion4.enemyController.targetOveride = true;
            lion4.enemyController.speedMod = 3f;
            lion4.enemyController.target = (Vector2)enemyController.player.transform.position + Vector2.down * lungeDisMult;
            lion4.enemyController.spearThrown = true;
        }
    }
    public void lungeATK()
    {
        enemyController.rb.linearVelocity = (thisLionNum == 1 ? Vector2.down :  (thisLionNum == 4 ? Vector2.up : (thisLionNum == 2 ? Vector2.right : Vector2.left))) * lungeSpeed;
        lungeHitBox.SetActive(true);
        GetComponent<PolygonCollider2D>().excludeLayers = 8;
        GetComponent<PolygonCollider2D>().excludeLayers += 64;
        StartCoroutine(lungeTime(2f));
    }    

    IEnumerator lungeTime(float time)
    {
        yield return new WaitForSeconds(time);
        enemyController.anim.SetBool("Lunge", false);
        ready = false;
        enemyController.spearThrown = false;
        enemyController.gameManager.lionCheck = 0;
        enemyController.gameManager.lionReady = 0;
        enemyController.targetOveride = false;
        enemyController.speedMod = 1;
        enemyController.target = Vector2.up * 999999;
        GetComponent<PolygonCollider2D>().excludeLayers = 0;
        StartCoroutine(enemyController.cooldown(1, lungeHitBox));
        pendingAttack = Random.Range(1, 4);
    }
}
