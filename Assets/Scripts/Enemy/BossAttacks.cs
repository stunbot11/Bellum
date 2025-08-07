using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Attacks")]
public class BossAttacks : ScriptableObject
{
    [Header("Attacks")]
    public atk attack;
    public enum atk
    {
        meleeSwing,
        meleeBurst,
        rangedSingle,
        rangedBurst,
        rangedTrip,
        lunge,
        special
    }
    public int mbAmount;
    public int rbAmount;
    public int rtAmount;

    [Header("Stats/properties")]
    public AudioClip sfx;
    public int phase; // used to determin the active weapon
    public int dmg;
    public string animName;
    public float teleTime;
    public float projSpeed;
    public float projLifeTime;
    public float attackRange;
    public float cooldownTime;
}
