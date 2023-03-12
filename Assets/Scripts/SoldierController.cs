using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour, IDestroyable, ITeam
{
    public bool isSelected;
    SoldierMovement movement;
    [SerializeField] GameObject selectionIndicator;
    [SerializeField] int teamIndex;
    [SerializeField] int typeIndex;
    [SerializeField] SpriteRenderer visual;

    public Transform target;
    [SerializeField] float attackDistance;
    Coroutine attackCoroutine;
    [SerializeField] float attackTime = .5f;
    [SerializeField] int damage = 10;

    [SerializeField] GameObject muzzleEffectPrefab;
    [SerializeField] Transform muzzlePoint;
    List<GameObject> muzzleEffectPool = new List<GameObject>();
    [SerializeField] float muzzleEffectTime = .1f;

    public enum State
    {
        Idle,
        Moving,
        Attacking
    }

    public State myState = State.Idle;

    [Header("Colors")]
    [SerializeField] List<Color> index0Colors = new List<Color>();
    [SerializeField] List<Color> index1Colors = new List<Color>();
    List<List<Color>> indexColorList;


    void Awake()
    {
        indexColorList = new List<List<Color>>();
        indexColorList.Add(index0Colors);
        indexColorList.Add(index1Colors);
        movement = GetComponent<SoldierMovement>();
        visual.color = indexColorList[teamIndex][typeIndex];
    }


    void Update()
    {
        if (target != null)
        {
            if (myState != State.Attacking)
            {
                // If target is not null and enemy distance lower than attack distance, soldier stops.
                if (Vector3.Distance(target.position, transform.position) < attackDistance)
                {
                    movement.StopMoving();
                    myState = State.Attacking;

                    if (attackCoroutine != null)
                        StopCoroutine(attackCoroutine);
                    attackCoroutine = StartCoroutine(AttackEnum());
                }
            }

            // If target is not null and my state equal attacking, soldier looks in the direction.
            if (myState == State.Attacking)
            {
                Vector3 dir = target.position - transform.position;
                dir = dir.normalized;
                movement.LookAtDirection(dir);
            }
        }

        CheckEnemy();
    }

    // This function check enemy.
    void CheckEnemy()
    {
        if (myState == State.Idle && target == null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackDistance);
            List<Transform> enemies = new List<Transform>();

            foreach (Collider2D hit in hits)
            {
                ITeam currentITeam = hit.GetComponent<ITeam>();
                if (currentITeam != null)
                {
                    if (currentITeam.GetTeamIndex() != teamIndex)
                    {
                        enemies.Add(hit.transform);
                    }
                }
            }

            if (enemies.Count > 0)
            {
                target = FindNearest(enemies);
                myState = State.Attacking;
                if (attackCoroutine != null)
                    StopCoroutine(attackCoroutine);
                attackCoroutine = StartCoroutine(AttackEnum());
            }
        }
    }

    // This function find nearest object from incoming list.
    Transform FindNearest(List<Transform> transfomList)
    {
        int nearetsIndex = 0;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < transfomList.Count; i++)
        {
            float currentDistance = Vector3.Distance(transform.position, transfomList[i].position);
            if (currentDistance < nearestDistance)
            {
                nearestDistance = currentDistance;
                nearetsIndex = i;
            }
        }

        return transfomList[nearetsIndex];
    }

    // This coroutine attack enemy and wait for attack time.
    IEnumerator AttackEnum()
    {
        yield return new WaitForSeconds(attackTime);

        if (target == null)
        {
            myState = State.Idle;
            yield break;
        }

        if (Vector3.Distance(target.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position) > attackDistance)
        {
            myState = State.Idle;
            target = null;
            yield break;
        }

        bool isDead = target.GetComponent<HealthSystem>().GetDamage(damage);
        if (!isDead)
        {
            attackCoroutine = StartCoroutine(AttackEnum());
        }
        else
        {
            myState = State.Idle;
        }

        GameObject currentMuzzle = null;
        if (muzzleEffectPool.Count > 0)
        {
            currentMuzzle = muzzleEffectPool[0];
            currentMuzzle.SetActive(true);
            muzzleEffectPool.Remove(currentMuzzle);
        }
        else
        {
            currentMuzzle = Instantiate(muzzleEffectPrefab);
            currentMuzzle.transform.parent = transform;
        }

        currentMuzzle.transform.position = muzzlePoint.position;
        currentMuzzle.transform.rotation = muzzlePoint.rotation;
        StartCoroutine(TurnOffMuzzle(currentMuzzle));
    }


    // This coroutine turn off muzzle effect and add in the muzzle effect pool.
    IEnumerator TurnOffMuzzle(GameObject muzzleEffect)
    {
        yield return new WaitForSeconds(muzzleEffectTime);
        muzzleEffect.SetActive(false);
        muzzleEffectPool.Add(muzzleEffect); 
    }

    // this function return team index
    public int GetTeamIndex()
    {
        return teamIndex;
    }

    // This function set start way points.
    public void SetStart(Vector2 doorPoint, Vector2 spawnPoint)
    {
        movement.doorPoint = doorPoint;
        movement.spawnPoint = spawnPoint;
    }

    // This fonction set destination.
    public void SetDestination(Vector2 worldPos)
    {
        movement.SetDestination(worldPos, false);
        myState = State.Moving;

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }


    // This function select soldier.
    public void SelectMe()
    {
        // ReferansHolder.instance.selectionManager.SetSoldierSelection(gameObject);
        isSelected = true;
        selectionIndicator.SetActive(isSelected);
    }

    // This function deselect soldier.
    public void DeselectMe()
    {
        isSelected = false;
        selectionIndicator.SetActive(isSelected);
    }

    // This function destroy soldier.
    public void DestroyMe()
    {
        ReferansHolder.instance.selectionManager.RemoveSoldierFromSelectedList(this);
        movement.DestroyMe();
        Destroy(gameObject);
    }
}
