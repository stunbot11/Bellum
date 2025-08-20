using System.Collections;
using UnityEngine;

public class Janus : MonoBehaviour
{
    private EnemyController enemyController;
    private ArenaHandler AH;

    public GameObject swordHitBox;
    public GameObject spearHitBox;
    public GameObject spear;
    public GameObject thrownSpear;
    public bool spearThrown;
    public bool canPickUp;
    public float spearSpeed;

    public float swordAtkRange;
    public float spearAtkRange;
    public int damage;

    public GameObject swordAnim;
    public GameObject spearAnim;

    [Header("Music")]
    public AudioSource janusPhaseMusic;
    public AudioClip[] jPhase;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        StartCoroutine(phaseTime());
    }

    private void Update()
    {
        if (spearThrown && Vector2.Distance(transform.position, thrownSpear.transform.position) < 1f && canPickUp)
        {
            spearThrown = false;
            spearAnim.SetActive(true);
            Destroy(thrownSpear);
            thrownSpear = null;
            enemyController.targetOveride = false;
            enemyController.goingToTarget = false;
            enemyController.spearThrown = false;
            enemyController.canAttack = true;
            canPickUp = false;
        }
    }

    IEnumerator phaseTime()
    {
        yield return new WaitForSeconds(Random.Range(25, 35f));
        StartCoroutine(switchPhase());
    }

    private IEnumerator switchPhase()
    {
        enemyController.canAttack = false;
        print("switched phase");
        //flip coin anim
        bool side = Random.Range(0, 2) == 0; // if rolls 0 goes to sword phase
        yield return new WaitForSeconds(.5f);
        if (side) // switch to sword
        {
            enemyController.phase = 1;
            spearAnim.SetActive(false);
            swordAnim.SetActive(true);
            enemyController.anim.SetBool("Sword", true);
            enemyController.anim.SetTrigger("Phase Sword");
            janusPhaseMusic.Stop();
            janusPhaseMusic.resource = jPhase[0];
            janusPhaseMusic.Play();
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
            enemyController.phase = 0;
            swordAnim.SetActive(false);
            spearAnim.SetActive(true);
            enemyController.anim.SetBool("Sword", false);
            enemyController.anim.SetTrigger("Phase Spear");
            yield return new WaitForSeconds(1);
            spearAnim.SetActive(false);
            janusPhaseMusic.Stop();
            janusPhaseMusic.resource = jPhase[1];
            janusPhaseMusic.Play();
            print("Spear");
            enemyController.canAttack = false;
            enemyController.canMove = false;
            enemyController.spearThrown = true;
            spearThrown = true; //gets angle for launch
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
            canPickUp = true;
            thrownSpear.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            enemyController.canMove = true;
            enemyController.targetOveride = true;
            enemyController.target = thrownSpear.transform.position;
        }
        enemyController.cooldown(2);
        StartCoroutine(phaseTime());
    }
}
