using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private EventSystem eventSystem;
    [HideInInspector] public PlayerController playerController;
    public int classType = 1; // 1 sword and shield / 2 bow / 3 tridant
    public int boss = 1;
    public bool[] challenges; //1: half damage / 2: double boss health / 3: no secondary abilities / 4: no upgrades
    public Image[] challengeColors;

    private int currentMainMenu;
    public GameObject[] mainMenus;

    [HideInInspector] public EmperorType activeEmperor;

     public int lionReady;
     public int lionCheck;
    [HideInInspector] public int totalBosses;
    [HideInInspector] public int bossesDead;

    public Image playerHealthBar;
    public bool bossActive;

     public int health;
     public float time;
     public int activeChallenges;

    private void Start()
    {
        DontDestroyOnLoad(gameObject); 
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

    }

    private void Update()
    {
        if (bossActive)
            time += Time.deltaTime;
    }

    public void start()
    {
        SceneManager.LoadScene(1);
    }

    public void menu()
    {
        SceneManager.LoadScene(0);
        Destroy(this.gameObject);
    }

    public void setClass(int num)
    {
        classType = num;
    }

    public void setBoss(int num)
    {
        boss = num;
    }

    public void setChallenges(int num)
    {
        num--;
        challenges[num] = challenges[num] == true ? false : true;
        challengeColors[num].color = challenges[num] ? Color.green : Color.red;
    }

    public void menuSelect(int menu)
    {
        mainMenus[currentMainMenu].SetActive(false);
        mainMenus[menu].SetActive(true);
        currentMainMenu = menu;

        eventSystem.SetSelectedGameObject(GameObject.Find(menu == 0 ? "start" : "Challenges Back"));
    }

    public void leaderBoard()
    {
        for (int i = 0; i < challenges.Length; i++)
        {
            if (challenges[i] == true)
                activeChallenges++;
        }
        SceneManager.LoadScene("Leaderboard");
    }

    public void deleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
