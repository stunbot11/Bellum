using UnityEngine;

[CreateAssetMenu(fileName = "New emperor", menuName = "Emperor")]
public class EmperorType : ScriptableObject
{
    public string emperorName;
    [TextArea] public string bossEffectDiscription;
    [TextArea] public string arenaEffectDiscription;

    [Header("Boss Effects")]
    public bool increaseDamage;
    public bool increaseHealth;
    public bool increaseSpeed;
    public float bossEffectStrength = 1;

    [Header("Player Effects")]
    public bool decreaseDamage;
    public bool decreaseHealth;
    public bool decreaseSpeed;
    public float playerEffectStrength = 1;
}

