using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSkill : Skills
{
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;
    [SerializeField] private float crystalDuration;
    [SerializeField] private float growSpeed;

    [Header("Explosive Crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving Crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi Stacking Crystal")]
    [SerializeField] private bool canMultiStack;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    [Header("CrystalM Mirage")]
    [SerializeField] private bool cloneInsteadOfCrystal;


    public override void UseSkill()
    {
        base.UseSkill();

        if (getCanMultiStack()) return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
                return;
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<CrystalSkillController>()?.FinishCrystal();
            }

        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        CrystalSkillController currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();
        currentCrystalScript.Setup(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, growSpeed, FindCloseEnemy(currentCrystal.transform));
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<CrystalSkillController>().ChooseRandomEnemy();

    private bool getCanMultiStack()
    {
        if (canMultiStack)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);

                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);
                newCrystal.GetComponent<CrystalSkillController>().Setup(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, growSpeed, FindCloseEnemy(newCrystal.transform));
            }
            if (crystalLeft.Count <= 0)
            {
                cooldown = multiStackCooldown;
                RefillCrsytal();
            }
        }

        return canMultiStack;
    }

    private void RefillCrsytal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;
        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0) return;

        cooldownTimer = multiStackCooldown;
        RefillCrsytal();
    }
}