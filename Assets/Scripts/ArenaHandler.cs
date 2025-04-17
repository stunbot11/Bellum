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
    public GameObject obsticles;

    private int active;
    private int inactive;
    private int left;
    private int total;
    private float percent = .66f;

    [Header("Music")]
    public AudioClip theLionsDen;
    public AudioClip commotion;
    public AudioClip janusFundamentum;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        classes[gameManager.classType - 1].SetActive(true);

        //gets each object in the obsticles empty and randomizes if they will be active or not (33% * (active objects / inactive objects) to be active)
        Transform[] obT = obsticles.GetComponentsInChildren<Transform>();
        total = obT.Length;
        foreach (Transform g in obT)
        {
            percent = .66f * (((float)active + 1) / ((float)inactive + 1));
            float rng = Random.Range(0, 1f);
            g.gameObject.SetActive(rng > percent);
            if (rng > percent)
                active++;
            else
                inactive++;
        }
        obsticles.SetActive(true);   
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
