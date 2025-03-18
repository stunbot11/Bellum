using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public GameObject creator;

    public bool net;

    private void Start()
    {
        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (creator.CompareTag(collision.tag))
        {
            switch (collision.tag)
            {
                case "Player":
                    PlayerController playerController = collision.GetComponent<PlayerController>();
                    playerController.takeDamage(damage);
                    Destroy(gameObject);
                    break;

                case "Enemy":
                    EnemyController enemyController = collision.GetComponent<EnemyController>();
                    if (net)
                        enemyController.imbolized = true;
                    enemyController.takeDamage(damage, net, "norm", creator.GetComponent<PlayerController>().gameManager.classType == 2 ? 5 : 0);
                    Destroy(gameObject);
                    break;

                case "TestDummy":
                    collision.GetComponent<DamageDummy>().takeDamage(damage);
                    break;
            }
        }
    }
}
