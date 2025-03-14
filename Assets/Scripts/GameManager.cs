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
    public GameObject[] challengeOn;
    public GameObject[] challengeOff;

    private int currentMainMenu;
    public GameObject[] mainMenus;


     public int lionReady;
     public int lionCheck;
    [HideInInspector] public int totalBosses;
    [HideInInspector] public int bossesDead;

    public Image playerHealthBar;

    private void Start()
    {
        DontDestroyOnLoad(gameObject); 
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    public void start()
    {
        SceneManager.LoadScene(boss);
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
        challengeOn[num].SetActive(challenges[num]);
        challengeOff[num].SetActive(!challenges[num]);
    }

    public void menuSelect(int menu)
    {
        mainMenus[currentMainMenu].SetActive(false);
        mainMenus[menu].SetActive(true);
        currentMainMenu = menu;

        eventSystem.SetSelectedGameObject(GameObject.Find(menu == 0 ? "start" : "Challenges Back"));
    }
}
