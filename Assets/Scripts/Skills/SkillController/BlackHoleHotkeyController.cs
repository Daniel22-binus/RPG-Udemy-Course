using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackHoleHotkeyController : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotkey;
    private TextMeshProUGUI myText;

    private Transform myEnemy;
    private BlackHoleSkillController blackHole;

    public void SetUpHotKey(KeyCode _myNewHotkey, Transform _myEnemy, BlackHoleSkillController _blackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();
        myEnemy = _myEnemy;
        blackHole = _blackHole;

        myHotkey = _myNewHotkey;
        myText.text = myHotkey.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(myHotkey))
        {
            blackHole.AddEnemyToList(myEnemy);
            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
