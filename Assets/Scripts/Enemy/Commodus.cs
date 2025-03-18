using System.Collections;
using UnityEngine;

public class Commodus : MonoBehaviour
{
    private EnemyController enemyController;

    [Header("atk stats")]
    public int pendingAttack;
    public float attackRange;

    public GameObject volly;
    public int vollyDmg;
    public float vollyRad;
    public float vollyTime;
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        pendingAttack = Random.Range(1, 3);
    }

    private void Update()
    {
        
    }

    private void attack()
    {
        switch (pendingAttack)
        {
            case 1: // single shot
                break;
                
            case 2: // triple shot
                break;

            case 3: // burst shot
                break;

            case 4: // volly shot
                volly.transform.position = enemyController.player.transform.position;
                break;
        }
    }

    IEnumerator vollyStuff(float telegraph)
    {
        yield return new WaitForSeconds(telegraph);
        volly.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        volly.SetActive(false);
    }
}
