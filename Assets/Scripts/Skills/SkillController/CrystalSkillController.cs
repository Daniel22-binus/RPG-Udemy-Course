using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{
    private Animator anim;
    private CircleCollider2D cd;
    private float crystalExistTimer;

    [Header("Explosive Crystal")]
    private bool canExplode;

    [Header("Moving Crystal")]
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cd = GetComponent<CircleCollider2D>();
    }

    public void Setup(float crystalDuration, bool _canExplode, bool _canMoveToEnemy, float _moveSpeed, float _growSpeed, Transform _closestTarget)
    {
        crystalExistTimer = crystalDuration;
        canExplode = _canExplode;
        canMove = _canMoveToEnemy;
        moveSpeed = _moveSpeed;
        growSpeed = _growSpeed;
        closestTarget = _closestTarget;

        canGrow = false;
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackHole.GetBlackHoleRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if (colliders.Length > 0)
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
    }

    void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3,3), growSpeed * Time.deltaTime);
        }

        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, closestTarget.position) < 1)
        {
            FinishCrystal();
            canMove = false;
        }
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().DamageEffect();
            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
