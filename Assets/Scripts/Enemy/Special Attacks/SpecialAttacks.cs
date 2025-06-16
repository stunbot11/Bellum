using System.Collections;
using UnityEngine;

public abstract class SpecialAttacks
{
    public EnemyController enemycontroller;
    public GameObject[] hitbox;

    protected abstract IEnumerator attack();
}
