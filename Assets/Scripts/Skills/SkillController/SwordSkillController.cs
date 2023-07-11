using System.Collections.Generic;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    #region component
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;
    #endregion

    private float returnSpeed = 12;
    private bool canRotate = true;
    private bool isReturning = false;
    private float freezeTimeDuration;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing = false;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    public int targetIndex;

    [Header("Pierce Info")]
    private int pierceAmount;

    [Header("Spin Info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;
    private float hitTimer;
    private float hitCooldown;
    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
        Invoke("DestroyMe", 7f);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounce, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        enemyTarget = new List<Transform>();
        bounceSpeed = _bounceSpeed;
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 0.5f)
                player.CatchSword();
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (!isSpinning)
            return;

        if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance & !wasStopped)
        {
            StopWhenSpinning();
        }

        if (wasStopped)
        {
            spinTimer -= Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);
            if (spinTimer < 0)
            {
                isReturning = true;
                isSpinning = false;
            }

            hitTimer -= Time.deltaTime;
            if (hitTimer < 0)
            {
                hitTimer = hitCooldown;
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        hit.GetComponent<Enemy>().DamageEffect();
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {
                //?? bukannya sudah ada damage di line 195, ketika terjadinya collision, bukan update
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                targetIndex += 1;

                bounceAmount -= 1;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                    //bounceStuck();
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    private void bounceStuck()
    {
        anim.SetBool("Rotation", false);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                enemyTarget.Add(hit.transform);
                transform.parent = hit.transform;
                break;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;


        SetupTargetForBounce(collision);
        stuckInto(collision);
        
        if (collision.GetComponent<Enemy>() == null)
            return;

        Enemy enemy = collision.GetComponent<Enemy>();
        SwordSkillDamage(enemy);
        
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        enemy.DamageEffect();
        enemy.StartCoroutine("FreezeTimeFor", freezeTimeDuration);
    }

    private void SetupTargetForBounce(Collider2D collision)
    {
        if (isBouncing && enemyTarget.Count <= 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null)
                {
                    enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void stuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount -= 1;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
