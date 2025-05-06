using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class LeaderBoard : MonoBehaviour
{
    public List<string> posibleChar = new List<string>();
    public Font font;
    private GameManager gameManager;
    private bool canGoNext;

    public int boss;
    public string playerName;
    public float score;
    public float health;
    public float time;
    public float challengesMod;

    public int place;

    public int[] characters = {0, 0, 0};
    public TextMeshProUGUI[] charTxt;

    public TextMeshProUGUI[] scores;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager != null)
        {
            health = gameManager.health;
            time = gameManager.time;
            challengesMod = 1 + (.25f * gameManager.activeChallenges);
            boss = gameManager.boss;
        }
        else
            Debug.Log("i see you are testing things, have fun, eh");
        
        //adds posible characters for the leaderboard
        foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-=!@#$%^&*()_+|`~;':,.<>?")
        {
            if (font.HasCharacter(c))
            {
                posibleChar.Add(c.ToString());
            }
            else
                print(c + " is not supported");
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown && canGoNext)
            mainMenu();
    }

    public void leaderBoardStuff()
    {
        //calc score
        score = (health * 10 + Mathf.Lerp(5000, 0, Mathf.Clamp((time - 30) / 200, 0, 1))) * challengesMod;

        //get place
        place = 6;
        for (int i = 5; score > PlayerPrefs.GetInt(boss + "Score" + i) && i != 0; i--)
        {
            place--;
        }

        // set player pref scores
        for (int i = 5; i >= place; i--)
        {
            PlayerPrefs.SetInt(boss + "Score" + i, PlayerPrefs.GetInt("Score" + (i - 1)));
            PlayerPrefs.SetString(boss + "Name" + i, PlayerPrefs.GetString("Name" + (i - 1)));
        }
        PlayerPrefs.SetInt(boss + "Score" + place, Mathf.RoundToInt(score));
        PlayerPrefs.SetString(boss + "Name" + place, playerName);

        PlayerPrefs.Save();

        //set leaderboard
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i].text = PlayerPrefs.GetString(boss + "Name" + (i + 1)) + " ----- " + PlayerPrefs.GetInt(boss + "Score" + (i + 1));
        }
    }

    public void increaseVal(int num)
    {
        if (characters[num] < posibleChar.Count - 1)
            characters[num]++;
        else
            characters[num] = 0;
        charTxt[num].text = posibleChar[characters[num]];
    }

    public void decreaseVal(int num)
    {
        if (characters[num] > 0)
            characters[num]--;
        else
            characters[num] = posibleChar.Count - 1;
        charTxt[num].text = posibleChar[characters[num]];
    }

    public void confirm()
    {
        playerName = posibleChar[characters[0]] + posibleChar[characters[1]] + posibleChar[characters[2]];
        leaderBoardStuff();
        StartCoroutine(timtimplaysfordaysinamazeandtheresaminitartar());
    }

    public void mainMenu()
    {
        if (gameManager != null)
            Destroy(gameManager.gameObject);
        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator timtimplaysfordaysinamazeandtheresaminitartar()
    {
        yield return new WaitForSeconds(1);
        canGoNext = true;
    }
}
