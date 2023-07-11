using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSkillController : MonoBehaviour
{
    [Header("Black Hole Info")]
    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private bool canGrow;
    private bool canShrink;
    private float blackHoleTimer;
    private bool playerCanDisappear;

    [Header("Hotkey Info")]
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;
    private bool canCreateHotkey;
    private List<GameObject> createdHotKey;

    [Header("Clone Info")]
    private List<Transform> targets;
    private bool cloneAttackRelease;
    public int amountOfAttack;
    private float cloneAttackCooldown;
    private float cloneAttackTimer;

    public bool playerCanExitSet {get; private set;}

    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttack, float _cloneAttackCooldown, float _blackHoleDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttack = _amountOfAttack;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackHoleTimer = _blackHoleDuration;

        playerCanExitSet = false;
        playerCanDisappear = true;

        canGrow = true;
        canShrink = false;
        canCreateHotkey = true;
        targets = new List<Transform>();
        createdHotKey = new List<GameObject>();
        cloneAttackRelease = false;

        if (SkillManager.instance.clone.crystalInsteadOfClone)
        {
            playerCanDisappear = false;
        }
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackHoleTimer -= Time.deltaTime;

        if (blackHoleTimer < 0)
        {
            blackHoleTimer = Mathf.Infinity;
            if (targets.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackHoleAbility();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }
        else if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0) Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {

        if (targets.Count <= 0) return;

        DestroyHotKey();
        cloneAttackRelease = true;
        canCreateHotkey = false;
        if (playerCanDisappear)
        {
            PlayerManager.getInstance().getPlayer().MakeTransparant(true);
            playerCanDisappear = false;
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackRelease && amountOfAttack > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;
            int randomIndex = Random.Range(0, targets.Count);
            float xOffset = Random.Range(0, 100) > 50 ? 2 : -2;

            if (SkillManager.instance.clone.crystalInsteadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }


            amountOfAttack -= 1;
            if (amountOfAttack <= 0)
            {
                Invoke("FinishBlackHoleAbility", 1.3f);

            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestroyHotKey();
        playerCanExitSet = true;
        canShrink = true;
        cloneAttackRelease = false;
    }

    private void DestroyHotKey()
    {
        if (createdHotKey.Count <= 0)
            return;

        foreach(GameObject hotkey in createdHotKey)
            Destroy(hotkey);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
            collision.GetComponent<Enemy>().FreezeTime(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHotKey(collision);
        }

    }

    private void CreateHotKey(Collider2D collision)
    {

        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot keys in key code list");
        }

        if (!canCreateHotkey)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        BlackHoleHotkeyController newHotKeyScript = newHotKey.GetComponent<BlackHoleHotkeyController>();
        newHotKeyScript.SetUpHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransfom) => targets.Add(_enemyTransfom); 
}
