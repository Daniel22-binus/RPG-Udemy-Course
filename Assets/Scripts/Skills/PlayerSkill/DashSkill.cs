using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : Skills
{
    public override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("Created clone Behind");
    }
}
