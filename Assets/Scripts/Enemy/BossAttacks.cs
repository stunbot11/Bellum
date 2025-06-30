using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Attacks")]
public class BossAttacks : ScriptableObject
{
    public int attackNum;

    [Header("Attacks")]
    public atk attack;
    public enum atk
    {
        meleeSwing,
        meleeBurst,
        rangedSingle,
        rangedBurst,
        rangedTrip,
        lunge
    }
    public int mbAmount;
    public int rbAmount;
    public int rtAmount;

    [Header("Stats/properties")]
    public int phase; // used to determin the active weapon
    public float projSpeed;
    public float projLifeTime;
    public float attackRange;
    public float cooldownTime;
}
