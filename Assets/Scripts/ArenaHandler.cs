using UnityEngine;

public class ArenaHandler : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject[] classes;
    public GameObject[] lions;
    public GameObject Commodus;
    public GameObject Janus;
    public GameObject door;
    public GameObject obsticles;
    public EmperorInfo emperorInfo;

    private int active;
    private int inactive;
    private int left;
    private int total;
    private float percent = .66f;

    [Header("Music")]
    public AudioSource speaker;
    public AudioClip[] bossMusic;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        classes[gameManager.classType - 1].SetActive(true);
        speaker.resource = bossMusic[gameManager.boss - 1];
        speaker.Play();
        speaker.Stop();

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
            speaker.Play();
            if (gameManager.activeEmperor.ObjectToSpawn != null)
                emperorInfo.fakeStart();
            switch (gameManager.boss)
            {
                case 1:
                    for (int i = 0; i < lions.Length - 1; i++)
                    {
                        lions[i].SetActive(true);
                    }
                    if (gameManager.activeEmperor.emperorName == "Commodus")
                        lions[3].SetActive(true);
                    break;

                case 2:
                    Commodus.SetActive(true);
                    break;

                case 3:
                    Janus.SetActive(true);
                    break;
            }
            gameManager.bossActive = true;
            Destroy(this);
        }
    }
}
