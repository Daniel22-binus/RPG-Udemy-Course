using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major Stats")]
    public Stats strength; // 1 point increase damage by 1 and crit power by 1%
    public Stats agility; // 1 point increate evasion by 1% and crit chance by 1%
    public Stats intelligence; // 1 point increate magic damage 1 and magic resistance by 3
    public Stats vitality; // 1 point increase health by 3 or 5 point

    [Header("Offensive Stats")]
    public Stats damage;
    public Stats critChance;
    public Stats critDamage;

    [Header("Defensive Stats")]
    public Stats maxHealth;
    public Stats armor;
    public Stats evasion;
    public Stats magicResistance;

    [Header("Magic Stats")]
    public Stats fireDamage;
    public Stats iceDamage;
    public Stats lightningDamage;

    [Header("Status Effect")]
    public bool isIgnited; // does damage ofer time
    public bool isChilled; // reduce armor 20%
    public bool isShocked; // reduce acuracy 20%

    private float ignitedTimer;
    private float ignitedCooldDown = 0.3f;
    private float ignitedDamageTimer;

    [Space]
    public int currentHealth;


    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
        critDamage.setValue(150);
    }

    void Update()
    {
        ignitedTimer -= Time.deltaTime;
    }

    public virtual void DoDamage(CharacterStats targetStats)
    {
        if (TargetCanAvoidAttack(targetStats)) return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(targetStats, totalDamage);
        //targetStats.TakeDamage(totalDamage);
        DoMagicalDamage(targetStats);
    }

    public virtual void DoMagicalDamage(CharacterStats targetStats)
    {
        int fire = fireDamage.GetValue();
        int ice = iceDamage.GetValue();
        int lightning = lightningDamage.GetValue();
        int totalMagicalDamage = fire + ice + lightning + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(targetStats, totalMagicalDamage);
        targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(fire, ice, lightning) <= 0)
            return;

        bool ignite = fire > ice && fire > lightning;
        bool chill = ice > fire && ice > lightning;
        bool shock = lightning > fire && lightning > ice;

        while(!ignite && !chill && !shock)
        {
            if (Random.value < 0.35f && fire > 0)
            {
                ignite = true;
                break;
            }
            if (Random.value < 0.5f && ice > 0)
            {
                chill = true;
                break;
            }
            if (Random.value < 0.5f && lightning > 0)
            {
                shock = true;
                break;
            }
        }

        targetStats.ApplyAilment(ignite, chill, shock);
    }

    private static int CheckTargetResistance(CharacterStats targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage = totalMagicalDamage - (targetStats.magicResistance.GetValue() + targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyAilment(bool _ignite, bool _chill, bool _shock)
    {
        if (isIgnited || isChilled || isShocked)
            return;
        isIgnited = _ignite;
        isChilled = _chill;
        isShocked = _shock;
    }


    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;
        Debug.Log(_damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " Die");
    }
    private bool TargetCanAvoidAttack(CharacterStats targetStats)
    {
        int totalEvasion = targetStats.evasion.GetValue() + targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Attack Avoided");
            return true;
        }

        return false;
    }
    private int CheckTargetArmor(CharacterStats targetStats, int totalDamage)
    {
        if (targetStats.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(targetStats.armor.GetValue() * 0.8f);
        }
        else
            totalDamage -= targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private bool CanCrit()
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0,100) <= totalCritChance)
        {
            return true;
        }
        return false;
    }

    private int CalculateCriticalDamage(int damage)
    {
        float totalCritPower = (critDamage.GetValue() + strength.GetValue()) * 0.01f;
        float TotalCritDamage = damage * totalCritPower;
        return Mathf.RoundToInt(TotalCritDamage);
    }
}
