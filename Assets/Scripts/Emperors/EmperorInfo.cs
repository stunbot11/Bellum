using System.Collections;
using TMPro;
using UnityEngine;

public class EmperorInfo : MonoBehaviour
{
    public EmperorType[] emperor;
    private GameManager gameManager;
    public GameObject emperorInfo;
    public TextMeshProUGUI emperorName;
    public TextMeshProUGUI effectDiscription;
    public TextMeshProUGUI arenaEffect;
    public GameObject obsticle;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int empNum = Random.Range(0, emperor.Length);

        gameManager.activeEmperor = emperor[empNum];
        emperorName.text = emperor[empNum].emperorName;
        effectDiscription.text = emperor[empNum].bossEffectDiscription;
        arenaEffect.text = emperor[empNum].arenaEffectDiscription;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            emperorInfo.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            emperorInfo.SetActive(false);
    }

    public void fakeStart()
    {
        StartCoroutine(spawnObject());
    }

    IEnumerator spawnObject()
    { //get absolute lerp based on x that changes where the y can be
        GameObject o =  Instantiate(gameManager.activeEmperor.ObjectToSpawn);
        float xPos = Random.Range(-38f, 38f);
        float yPos = Random.Range(Mathf.Lerp(12f, 17.5f, (Mathf.Abs(xPos) - 25.5f) / 12.5f), Mathf.Lerp(36f, 30.5f, (Mathf.Abs(xPos) - 25.5f) / 12.5f));
        o.transform.position = new Vector2(xPos, yPos);
        //min x for change in y is 25.5
        //Random.Range(12f, 36f) max y (less constrictve)
        //Randmo.Range(17.5f, 30.5f) min y (more constrctive)
        yield return new WaitForSeconds(gameManager.activeEmperor.timeBetweenEffects);
        StartCoroutine(spawnObject());
    }
}
