using Unity.VisualScripting;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public GameObject parent;
    public int damage;

    private void Start()
    {
        if (parent.tag == "Player")
            damage = parent.GetComponent<PlayerController>().damage;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (parent.tag != collision.tag)
        {
            switch (collision.tag)
            {
                case "Player":
                    collision.GetComponent<PlayerController>().takeDamage(damage);
                    break;

                case "Enemy":
                    PlayerController playerController = parent.GetComponent<PlayerController>();
                    collision.GetComponent<EnemyController>().takeDamage(damage, false, "norm", playerController.gameManager.classType == 1 ? (playerController.upgrades[0] > 0 ? 5 : 0) : 0);
                    break;

                case "TestDummy":
                    collision.GetComponent<DamageDummy>().takeDamage(damage);
                    break;
            }

        }
    }
}
