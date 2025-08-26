using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ArenaHandler : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject[] classes;
    public GameObject[] bosses;
    public GameObject door;
    public GameObject pDoor;
    public GameObject obsticles;
    public EmperorInfo emperorInfo;
    public Image bossBar;

    public int totEHealth;
    public int eHealth;

    public GameObject globalLight;

    private int active;
    private int inactive;
    private int left;
    private int total;
    private float percent = .66f;

    [Header("Music")]
    public AudioSource speaker;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        classes[gameManager.classType - 1].SetActive(true);
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
            gameManager.bossBar = bossBar;
            door.SetActive(true);
            pDoor.SetActive(true);
            speaker.Play();
            bossBar.transform.parent.transform.gameObject.SetActive(true);
            if (gameManager.activeEmperor.ObjectToSpawn != null)
                emperorInfo.fakeStart();
            for (int i = 0; i < bosses.Length; i ++)
            {
                bosses[i].gameObject.SetActive(true);
                bosses[i].GetComponentInChildren<Light2D>().enabled = gameManager.activeEmperor.emperorName == "Caligula";
            }
            if (gameManager.activeEmperor.emperorName == "Caligula")
                globalLight.SetActive(true);
            gameManager.bossActive = true;
            Destroy(this);
        }
    }
}
