using UnityEngine;
public class BlackHoleSkill : Skills
{
    [SerializeField] private GameObject blackHolePrefab;
    [Header("Black Hole Info")]
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float blackHoleDuration;

    [Header("Clone Info")]
    [SerializeField] private int amountOfAttack;
    [SerializeField] private float cloneAttackCooldown;
    BlackHoleSkillController currentBlackHole;
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);
        currentBlackHole = newBlackHole.GetComponent<BlackHoleSkillController>();
        currentBlackHole.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttack, cloneAttackCooldown, blackHoleDuration);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if (!currentBlackHole) return false;

        if (currentBlackHole.playerCanExitSet)
        {
            currentBlackHole = null;
            return true;
        }
        return false;
    }

    public float GetBlackHoleRadius()
    {
        return maxSize / 2;
    }
}
