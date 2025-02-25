using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameManager gameManager;

    [Header("weapons")]
    private GameObject weapon;
    public GameObject hitbox;
    public GameObject shield;
    public GameObject arrow;
    public float arrowSpeed;
    public GameObject net;
    public float netSpeed;

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
    [SerializeField] private bool canAttack = true;
    private bool dodgeing;
    private void Start()
    {
        primaryButton.action.started += primary;
        secondaryButton.action.started += secondary;
        secondaryButton.action.canceled += secondary;

        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        weapon = GameObject.Find("sword");
    }

    private void Update()
    {
        if (!dodgeing)
            rb.linearVelocity = move.action.ReadValue<Vector2>() * speed * (blocking ? .5f : 1);

        if (rb.linearVelocity != Vector2.zero && !dodgeing)
            direction = (Mathf.Atan2(move.action.ReadValue<Vector2>().y, move.action.ReadValue<Vector2>().x) * Mathf.Rad2Deg) - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, direction));
    }

    public void takeDamage(int damage)
    {
        if (!dodgeing)
            health -= blocking ? damage / 4 : damage;
    }

    public void primary(InputAction.CallbackContext phase)
    {
        if (canAttack)
        {
                switch (gameManager.classType)
            {
                case 1: //sword
                    canAttack = false;
                    hitbox.SetActive(true);
                    StartCoroutine(attackCooldown(.4f, .1f));
                    break;

                case 2: //bow
                    canAttack = false;
                    GameObject a = Instantiate(arrow, transform.position, transform.rotation, null);
                    a.GetComponent<Rigidbody2D>().linearVelocity = rb.linearVelocity * arrowSpeed;
                    StartCoroutine(attackCooldown(.5f));
                    break;

                case 3: //trident

                    break;

                default:
                    Debug.LogError("class type isnt 1-3");
                    break;
            }
        }
    }

    IEnumerator attackCooldown(float time, float hitboxActiveTime = 0)
    {
        yield return new WaitForSeconds(hitboxActiveTime);
        hitbox.SetActive(false);

        yield return new WaitForSeconds(time - hitboxActiveTime);
        canAttack = true;
    }

    public void secondary(InputAction.CallbackContext phase)
    {
        switch (gameManager.classType)
        {
            case 1: //block
                if (phase.started)
                {
                    StopCoroutine(attackCooldown(.25f, .1f));
                    blocking = true;
                    canAttack = false;
                    shield.SetActive(true);
                }
                else if (phase.canceled)
                {
                    StartCoroutine(attackCooldown(.25f));
                    blocking = false;
                    shield.SetActive(false);
                }
                break;

            case 2: //dodge
                if (phase.started && canDodge)
                {
                    dodgeing = true;
                    canDodge = false;
                    rb.linearVelocity = move.action.ReadValue<Vector2>() * speed * 3;
                    StartCoroutine(dodgeCooldown(.15f));
                }
                break;

            case 3: //throw net
                break;

            default:
                break;
        }
    }

    private IEnumerator blockCounter()
    {
        //count up while blocking
        return null;
    }

    private IEnumerator dodgeCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        dodgeing = false;

        yield return new WaitForSeconds(.75f);
        canDodge = true;
    }

}

