using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rb;

    [Header("Stats")]
    public int health;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        Vector2 targetPos = new Vector2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x);

        rb.linearVelocity = targetPos * speed;
    }

    public void takeDamage(int damage)
    {
        health -= damage;
    }
}
