using System.Collections;
using UnityEngine;

public class DamageDummy : MonoBehaviour
{
    public GameObject hitEffect;
    public void takeDamage(int damage)
    { 
        print("OOOOOOHHHHH NOOOOOOOOOO... I GOT HIT for " +  damage);
        hitEffect.SetActive(true);
        StartCoroutine(hitEffectStop());
    }

    IEnumerator hitEffectStop()
    {
        yield return new WaitForSeconds(.2f);
        hitEffect.SetActive(false);
    }
}
