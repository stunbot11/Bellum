using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : SpecialAttacks
{
    protected override IEnumerator attack()
    {
        yield return new WaitForSeconds(1);
    }
}
