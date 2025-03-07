using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public PlayerController playerController;
    public int classType = 1; // 1 sword and shield / 2 bow / 3 tridant
    public int boss = 1;
    private int currentScene;


    [HideInInspector] public int lionReady;
    [HideInInspector] public int lionCheck;
    [HideInInspector] public int totalBosses;
    [HideInInspector] public int bossesDead;

    public Image playerHealthBar;

    private void Start()
    {
        DontDestroyOnLoad(gameObject); 
        currentScene = SceneManager.GetActiveScene().buildIndex;
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
}
