using TMPro;
using UnityEditor;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    //score = time, health, challenges
    public string playerName;
    public float score;
    public float health;
    public float time;
    public float challengesMod;

    private int place;

    public TextMeshProUGUI[] scores;

    void calculateScore()
    {
        score = (health * 10 + Mathf.Lerp(5000, 0, time / 200)) * challengesMod;
    }

    void getPlace()
    {
        for (int i = 1; score > PlayerPrefs.GetInt("Score" + i); i++)
        {
            place++;
        }
    }

    void saveScore()
    {
        PlayerPrefs.SetInt("Score1", Mathf.RoundToInt(score));
        PlayerPrefs.SetInt("Score2", Mathf.RoundToInt(score));
        PlayerPrefs.SetInt("Score3", Mathf.RoundToInt(score));
        PlayerPrefs.SetInt("Score4", Mathf.RoundToInt(score));
        PlayerPrefs.SetInt("Score5", Mathf.RoundToInt(score));

        if (place == 5)
        {
            PlayerPrefs.SetInt("Score1", Mathf.RoundToInt(score));
            PlayerPrefs.SetString("Name1", playerName);

            for (int i = 5; i < place; i--)
            {

            }
        }

        PlayerPrefs.Save();
    }

    void getScores()
    {
        PlayerPrefs.GetInt("Score1");
        PlayerPrefs.GetInt("Score2");
        PlayerPrefs.GetInt("Score3");
        PlayerPrefs.GetInt("Score4");
        PlayerPrefs.GetInt("Score5");

        PlayerPrefs.GetString("Name1");
        PlayerPrefs.GetString("Name2");
        PlayerPrefs.GetString("Name3");
        PlayerPrefs.GetString("Name4");
        PlayerPrefs.GetString("Name5");
    }

    void setLeaderBoard()
    {
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i].text = PlayerPrefs.GetString("Name" + (i + 1)) + "-----" + PlayerPrefs.GetInt("Score" + (i + 1));
        }
    }
}
