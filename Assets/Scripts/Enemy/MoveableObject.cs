using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    //cross spawn time 10
    public int damage;
    public int DoTT;
    public float timeAlive;
    public string damageType;
    public string entityNotToDamage;

    private void Start()
    {
        Destroy(this.gameObject, timeAlive);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (entityNotToDamage != collision.tag)
        {
            switch (collision.tag)
            {
                case "Player":
                    collision.GetComponent<PlayerController>().takeDamage(damage, damageType, DoTT);
                    break;

                case "Enemy":
                    collision.GetComponent<PlayerController>().takeDamage(damage, damageType, DoTT);
                    break;
            }
        }
    }
}
