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

    public int place;

    public TextMeshProUGUI[] scores;

    public void calculateScore()
    {
        score = (health * 10 + Mathf.Lerp(5000, 0, time / 200)) * challengesMod;
    }

    public void getPlace()
    {
        place = 6;
        for (int i = 5; score > PlayerPrefs.GetInt("Score" + i) && i != 0; i--)
        {
            place--;
        }
    }

    public void setScore()
    {
        for (int i = 5; i >= place; i--)
        {
                PlayerPrefs.SetInt("Score" + i, PlayerPrefs.GetInt("Score" + (i - 1)));
                PlayerPrefs.SetString("Name" + i, PlayerPrefs.GetString("Name" + (i - 1)));
        }
        PlayerPrefs.SetInt("Score" + place, Mathf.RoundToInt(score));
        PlayerPrefs.SetString("Name" + place, playerName);

        PlayerPrefs.Save();
    }

    public void setLeaderBoard()
    {
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i].text = PlayerPrefs.GetString("Name" + (i + 1)) + " ----- " + PlayerPrefs.GetInt("Score" + (i + 1));
        }
    }

    public void deletePlayer()
    {
        PlayerPrefs.DeleteAll();
    }

    public void hardSet()
    {
        PlayerPrefs.SetInt("Score1", 10000);
        PlayerPrefs.SetString("Name1", "aaa");
        PlayerPrefs.SetInt("Score2", 7000);
        PlayerPrefs.SetString("Name2", "sss");
        PlayerPrefs.SetInt("Score3", 5000);
        PlayerPrefs.SetString("Name3", "ddd");
        PlayerPrefs.SetInt("Score4", 3000);
        PlayerPrefs.SetString("Name4", "fff");
        PlayerPrefs.SetInt("Score5", 1000);
        PlayerPrefs.SetString("Name5", "ggg");
    }
}
