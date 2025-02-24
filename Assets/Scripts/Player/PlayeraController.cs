using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayeraController : MonoBehaviour
{
    private Rigidbody2D rb;

    private GameObject weapon;

    [Header("Player Stats")]
    public int health;
    public float speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //control = GetComponent<PlayerInput>();
        //control.actions.Enable();
    }

    private void Update()
    {
        
    }

    public void primary()
    {

    }

    public void secondary()
    {

    }

}
