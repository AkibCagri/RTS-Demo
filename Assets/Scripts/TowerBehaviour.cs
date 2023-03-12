using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : BuildingBehaviour
{
    [SerializeField] SpriteRenderer muzzleVisual;
    public Transform target;
    [SerializeField] float attackDistance;
    Coroutine attackCoroutine;
    [SerializeField] float attackTime = .5f;
    [SerializeField] int damage = 10;

    [SerializeField] GameObject muzzleEffectPrefab;
    [SerializeField] Transform muzzlePoint;
    List<GameObject> muzzleEffectPool = new List<GameObject>();
    [SerializeField] float muzzleEffectTime = .1f;
    [SerializeField] float lookLerpValue;

    protected override void Start()
    {
        base.Start();
        muzzleVisual.color = teamColors[teamIndex];
        myType = BuildingType.Tower;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 dir = target.transform.position - muzzleVisual.transform.position;
            dir = dir.normalized;
            MuzzleDirection(dir);
        }
        CheckEnemy();
    }

    // This function check enemy.
    void CheckEnemy()
    {
        if (target == null)
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
                if (attackCoroutine != null)
                    StopCoroutine(attackCoroutine);
                attackCoroutine = StartCoroutine(AttackEnum());
            }
        }
    }

    // This function find neares object from incoming list.
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

    // This coroutine handle attack and wait for attack time.
    IEnumerator AttackEnum()
    {
        yield return new WaitForSeconds(attackTime);

        if (target == null)
        {
            yield break;
        }

        if (Vector3.Distance(target.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position) > attackDistance)
        {
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
            target = null;
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

    // This coroutine turn off muzzle effect and add muzzle effect pool.
    IEnumerator TurnOffMuzzle(GameObject muzzleEffect)
    {
        yield return new WaitForSeconds(muzzleEffectTime);
        muzzleEffect.SetActive(false);
        muzzleEffectPool.Add(muzzleEffect);
    }

    // This function provides to looking in the direction.
    public void MuzzleDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        muzzleVisual.transform.rotation = Quaternion.Slerp(muzzleVisual.transform.rotation, rotation, lookLerpValue * Time.deltaTime);
    }
}
