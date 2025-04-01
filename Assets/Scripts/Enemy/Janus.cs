using System.Collections;
using UnityEngine;

public class Janus : MonoBehaviour
{
    private EnemyController enemyController;
    private bool phase; //false = sword / true = spear

    public GameObject swordHitBox;
    public GameObject spearHitBox;
    public GameObject spear;
    private GameObject thrownSpear;
    private bool spearThrown;
    public float spearSpeed;

    public float swordAtkRange;
    public float spearAtkRange;
    public int damage;

    [Header("Music")]
    public AudioClip janusSinistro;
    public AudioClip janusDextrum;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (spearThrown && Vector2.Distance(transform.position, thrownSpear.transform.position) < .5f)
        {
            spearThrown = false;
            Destroy(thrownSpear);
            thrownSpear = null;
            enemyController.targetOveride = false;
        }

        if (enemyController.canAttack && !spearThrown && Vector2.Distance(transform.position, enemyController.player.transform.position) <= (!phase ? swordAtkRange : spearAtkRange))
            StartCoroutine(attack());
    }

    private IEnumerator attack()
    {
        enemyController.canAttack = false;
        if (!phase) //sword attacks
        {
            swordHitBox.SetActive(true);
            yield return new WaitForSeconds(.1f);
            swordHitBox.SetActive(false);
        }
        else
        {
            spearHitBox.SetActive(true);
            yield return new WaitForSeconds(.1f);
            spearHitBox.SetActive(false);
        }
        StartCoroutine(enemyController.cooldown(.5f));
    }

    IEnumerator phaseTime()
    {
        yield return new WaitForSeconds(Random.Range(25, 35f));
        StartCoroutine(switchPhase());
    }

    private IEnumerator switchPhase()
    {
        print("switched phase");
        //flip coin anim
        bool side = Random.Range(0, 2) == 0; // if rolls 0 goes to sword phase
        yield return new WaitForSeconds(.5f);
        if (side) // switch to sword
        {
            phase = false;
            enemyController.eVocalCords.PlayOneShot(janusDextrum);
            for (int i = 0; i < 3; i++)// three sword swings after changing to sword
            {
                swordHitBox.SetActive(true);
                yield return new WaitForSeconds(.1f);
                swordHitBox.SetActive(false);
                yield return new WaitForSeconds(.3f);
            }
            enemyController.canAttack = false;
            StartCoroutine(enemyController.cooldown(.7f));
        }
        else // switch to spear
        {
            enemyController.eVocalCords.PlayOneShot(janusSinistro);
            enemyController.canAttack = false;
            enemyController.canMove = false;
            spearThrown = true;
            phase = true; //gets angle for launch
            Vector2 arrowDirectionTemp = (enemyController.player.transform.position - transform.position).normalized;
            float ang1 = (Mathf.Round(((Mathf.Atan2(arrowDirectionTemp.y, arrowDirectionTemp.x) * Mathf.Rad2Deg) - 45) / 45) * 45 - 45);
            Vector2 arrowDirection = new Vector2(Mathf.Sin(ang1 * Mathf.Deg2Rad) * -1, Mathf.Cos(ang1 * Mathf.Deg2Rad)).normalized;
            //spawns and launches spear
            thrownSpear = Instantiate(spear, transform.position, Quaternion.identity, null);
            ProjectileHandler projectileData = thrownSpear.GetComponent<ProjectileHandler>();
            thrownSpear.GetComponent<Rigidbody2D>().rotation = ang1;
            projectileData.creator = this.gameObject;
            projectileData.damage = damage;
            thrownSpear.GetComponent<Rigidbody2D>().linearVelocity = arrowDirection * spearSpeed;

            yield return new WaitForSeconds(1); //waits 1 second then goes to pick up spear
            thrownSpear.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            enemyController.canMove = true;
            enemyController.targetOveride = true;
            enemyController.target = thrownSpear.transform.position;
        }
        StartCoroutine(phaseTime());
    }
}
