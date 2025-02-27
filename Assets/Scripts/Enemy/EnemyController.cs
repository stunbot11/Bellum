using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    private Rigidbody2D rb;
    public GameObject hitEffect;

    [Header("Stats")]
    public int health;
    public float speed;

    public bool canAttack;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        Vector2 targetPos = (player.transform.position - transform.position).normalized;
        rb.rotation = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
        rb.linearVelocity = targetPos * speed;
    }

    public void takeDamage(int damage)
    {
        hitEffect.SetActive(true);
        StartCoroutine(hitEffectStop());
        health -= damage;
        if (health <= 0)
            Destroy(this.gameObject);
    }

    
    IEnumerator hitEffectStop()
    {
        yield return new WaitForSeconds(.2f);
        hitEffect.SetActive(false);
    }

    public IEnumerator hitboxCooldown(GameObject hitbox, float cooldown)
    {
        yield return new WaitForSeconds(.1f);
        hitbox.SetActive(false);

        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
