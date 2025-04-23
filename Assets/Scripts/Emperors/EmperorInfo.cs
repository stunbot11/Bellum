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
}
