using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private EventSystem eventSystem;
    [HideInInspector] public PlayerController playerController;
    public bool arcadeMode;
    public int classType = 1; // 1 sword and shield / 2 bow / 3 tridant
    public int boss = 1;
    public bool[] challenges; //1: half damage / 2: double boss health / 3: no secondary abilities / 4: no upgrades
    public Image[] challengeColors;
    public Image[] classColors;
    public Image[] bossColors;

    private int currentMainMenu;
    public GameObject[] mainMenus;
    public AudioSource loudspeaker;

    [HideInInspector] public EmperorType activeEmperor;

     public int lionReady;
     public int lionCheck;
     public int totalBosses;
    [HideInInspector] public int bossesDead;

    public Image playerHealthBar;
    public bool bossActive;

    public int maxHealth;
    public int health;
    public float time;
    public int activeChallenges;

    public int upgradePoints;
    public int money;
    public int lastboss = 1;
    public int upgradePointLevel;
    public int maxHealthLevel;
    public int healthRegenLevel;
    public int round;

    private void Awake()
    {
        Application.targetFrameRate = -1;
    }
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

    public void stort()
    {
        loudspeaker.Stop();
        StartCoroutine(start());
    }

    public IEnumerator start()
    {
        if (arcadeMode)
        {
            boss = Random.Range(1, totalBosses + 2);
            if (boss == lastboss && boss == totalBosses + 2)
                boss--;
            else
                boss++;
        }
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(boss);
    }

    public void menu()
    {
        SceneManager.LoadScene(0);
        Destroy(this.gameObject);
    }

    public void gameMode(bool arcade)
    {
        arcadeMode = arcade;
    }

    public void setClass(int num) //sets class and changes color of class buttons
    {
        classType = num;

        for (int i = 0; i < 3; i++)
            classColors[i].color = Color.red;
        classColors[num - 1].color = Color.green;
    }

    public void setBoss(int num) //sets boss and changes color of boss buttons
    {
        boss = num;

        for (int i = 0; i < 3; i++)
            bossColors[i].color = Color.red;
        bossColors[num - 1].color = Color.green;
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

    [HideInInspector] public Image bossHealthBar;
     public float tEHealth;
    [HideInInspector] public float tEMHealth;
    public void updateBar()
    {
        bossHealthBar.fillAmount = tEHealth / tEMHealth;
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

    public void win()
    {
        if (arcadeMode)
        {
            round++;
            SceneManager.LoadScene("Shop");
        }
        else
            leaderBoard();
    }

    public void loss()
    {
        if (arcadeMode)
            leaderBoard();
        else
            menu();
    }

    public IEnumerator hitEffect(SpriteRenderer[] limbs)
    {
        for (int i = 0; i < limbs.Length; i++)
            limbs[i].color = Color.red;
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < limbs.Length; i++)
            limbs[i].color = Color.white;
    }

    public void deleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
