using UnityEngine;

public class Tiger : MonoBehaviour
{
    public int thisTigerNum;
    [HideInInspector] public int pendingAttack;
    private Tiger tiger1;
    private Tiger tiger2;
    private Tiger tiger3;

    public GameObject biteHitBox;
    public GameObject slashHitBox;
    public GameObject lungeHitBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void attack()
    {
        switch (pendingAttack)
        {
            case 1: //bite
                break;

            case 2: //slash
                break;

            case 3: //group lunge
                break;
        }

        pendingAttack = Random.Range(1, 3);
    }
}
