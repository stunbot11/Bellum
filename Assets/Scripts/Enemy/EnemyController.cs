using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;

    [Header("Stats")]
    public int health;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
    }

    public void takeDamage(int damage)
    {
        health -= damage;
    }
}
