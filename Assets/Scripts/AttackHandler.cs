using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public GameObject parent;
    public int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.gameObject);
        if (parent.tag != collision.tag)
        {
            switch (collision.tag)
            {
                case "Player":
                    collision.GetComponent<PlayerController>().takeDamage(damage);
                    break;

                case "Enemy":
                    collision.GetComponent<EnemyController>().takeDamage(damage);
                    break;

                case "TestDummy":
                    collision.GetComponent<DamageDummy>().takeDamage(damage);
                    break;
            }

        }
    }
}
