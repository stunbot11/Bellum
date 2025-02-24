using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int classType = 1; // 1 sword and shield / 2 bow / 3 tridant
    public int boss = 1;

    public int currentLevel = 0;

    private void Start()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }
}
