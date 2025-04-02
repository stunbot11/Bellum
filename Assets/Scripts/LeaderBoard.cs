using TMPro;
using UnityEditor;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    private GameManager gameManager;

    public int boss;
    public string playerName;
    private float score;
    private float health;
    private float time;
    public float challengesMod;

    public int place;

    public TextMeshProUGUI[] scores;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        health = gameManager.health;
        time = gameManager.time;
        challengesMod = 1 + (.25f * gameManager.activeChallenges);
        boss = gameManager.boss;
    }

    public void leaderBoardStuff()
    {
        //calc score
        score = (health * 10 + Mathf.Lerp(5000, 0, time / 200)) * challengesMod;

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

    public void deletePlayer()
    {
        PlayerPrefs.DeleteAll();
    }
}
