using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Attacks")]
public class BossAttacks : ScriptableObject
{
    public int attackNum;

    [Header("Attacks")]
    public bool meleeSwing;
    public bool meleeBurst;
    public int mbAmount;
    public bool rangedSingle;
    public bool rangedBurst;
    public int rbAmount;
    public bool lunge;

    [Header("Stats/properties")]
    public int phase; // used to determin the active weapon
    public float cooldownTime;
    public GameObject weapon;
    public GameObject dmgSpot; // projectile / hitbox
}
