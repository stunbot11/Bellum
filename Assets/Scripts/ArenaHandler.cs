using UnityEngine;

public class ArenaHandler : MonoBehaviour
{
    public GameObject[] boss;
    public GameObject door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            door.SetActive(true);
            for (int i = 0; i < boss.Length; i++)
            {
                boss[i].SetActive(true);
            }
            Destroy(this);
        }
    }
}
