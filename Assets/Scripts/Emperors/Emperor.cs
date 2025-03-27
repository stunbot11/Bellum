using UnityEngine;

public class Emperor : MonoBehaviour
{
    public EmperorType[] emperor;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.activeEmperor = emperor[Random.Range(0, emperor.Length + 1)];
    }
}

[CreateAssetMenu(fileName = "New emperor", menuName = "Emperor")]
public class EmperorType : ScriptableObject
{
    public string emperorName;
    [TextArea] public string bossEffectDiscription;
    [TextArea] public string arenaEffectDiscription;
}
