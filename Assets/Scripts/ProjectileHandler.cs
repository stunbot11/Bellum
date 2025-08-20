using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public GameObject creator;

    public bool net;
    public bool stay;

    private void Start()
    {
        if (!stay)
        Destroy(gameObject, 5);
        print(damage);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!creator.CompareTag(collision.tag))
        {
            switch (collision.tag)
            {
                case "Player":
                    PlayerController playerController = collision.GetComponent<PlayerController>();
                    playerController.takeDamage(damage);
                    if (!stay)
                        Destroy(gameObject);
                    break;

                case "Enemy":
                    EnemyController enemyController = collision.GetComponent<EnemyController>();
                    if (net)
                        enemyController.imbolized = true;
                    enemyController.takeDamage(damage, net, "norm", creator.GetComponent<PlayerController>().gameManager.classType == 2 && creator.GetComponent<PlayerController>().upgrades[0] > 1 ? 5 : 0);
                    if (!stay)
                        Destroy(gameObject);
                    break;

                case "TestDummy":
                    collision.GetComponent<DamageDummy>().takeDamage(damage);
                    if (!stay)
                        Destroy(gameObject);
                    break;
            }
        }
    }
}
