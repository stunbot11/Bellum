using UnityEngine;

public class ArenaHandler : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject[] classes;
    public GameObject[] lions;
    public GameObject Commodus;
    public GameObject Janus;
    public GameObject door;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        classes[gameManager.classType - 1].SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            door.SetActive(true);
            switch (gameManager.boss)
            {
                case 1:
                    for (int i = 0; i < lions.Length; i++)
                        lions[i].SetActive(true);
                    break;

                case 2:
                    Commodus.SetActive(true);
                    break;

                case 3:
                    Janus.SetActive(true);
                    break;
            }
            
            Destroy(this);
        }
    }
}
