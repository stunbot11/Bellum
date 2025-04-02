using UnityEngine;

public class ArenaHandler : MonoBehaviour
{
    private GameManager gameManager;
    private EnemyController EC;
    public GameObject[] classes;
    public GameObject[] lions;
    public GameObject Commodus;
    public GameObject Janus;
    public GameObject door;

    [Header("Music")]
    public AudioClip theLionsDen;
    public AudioClip commotion;
    public AudioClip janusFundamentum;

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
                    EC.eVocalCords.PlayOneShot(theLionsDen);
                    break;

                case 2:
                    Commodus.SetActive(true);
                    EC.eVocalCords.PlayOneShot(commotion);
                    break;

                case 3:
                    Janus.SetActive(true);
                    EC.eVocalCords.PlayOneShot(janusFundamentum);
                    break;
            }
            gameManager.bossActive = true;
            Destroy(this);
        }
    }
}
