using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameManager gameManager;
    private GameObject weapon;

    [Header("Controlls")]
    public InputActionReference move;
    public InputActionReference primaryButton;
    public InputActionReference secondaryButton;

    [Header("Player Stats")]
    public int health;
    public float speed;

    private float direction;

    [SerializeField] private bool blocking;
    [SerializeField] private bool canDodge;
    [SerializeField] private bool canAttack;
    private bool dodgeing;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        primaryButton.action.started += primary;
        secondaryButton.action.started += secondary;
        secondaryButton.action.canceled += secondary;
    }

    private void Update()
    {
        if (!dodgeing)
            rb.linearVelocity = move.action.ReadValue<Vector2>() * speed;

        if (rb.linearVelocity != Vector2.zero)
            direction = (Mathf.Atan2(move.action.ReadValue<Vector2>().y, move.action.ReadValue<Vector2>().x) * Mathf.Rad2Deg) - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, direction));
    }

    public void takeDamage(int damage)
    {
        health -= blocking ? damage / 4 : damage;
    }

    public void primary(InputAction.CallbackContext phase)
    {
        print("attack");
    }

    public void secondary(InputAction.CallbackContext phase)
    {
        switch (gameManager.classType)
        {
            case 1:
                //blocking = phase.canceled;
                break;

            case 2:
                if (phase.started)
                {
                    dodgeing = true;
                    canDodge = false;
                    rb.linearVelocity = move.action.ReadValue<Vector2>() * speed * 3;
                }
                break;

            case 3:
                break;

            default:
                break;
        }
    }

    private IEnumerator dodgeCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        dodgeing = false;

        yield return new WaitForSeconds(1);
        canDodge = true;
    }

}
