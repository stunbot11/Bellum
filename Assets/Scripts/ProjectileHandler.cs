using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public GameObject creator;

    private void Start()
    {
        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (creator.CompareTag("Player") && collision.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.GetComponent<EnemyController>();
            enemyController.takeDamage(damage);
            Destroy(gameObject);
        }
        else if (creator.CompareTag("Enemy") && collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            playerController.takeDamage(damage);
            Destroy(gameObject);
        }
    }
}
