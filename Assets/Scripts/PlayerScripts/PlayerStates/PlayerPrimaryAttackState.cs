using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{

    private int comboCounter;
    private float lastTimeAttack;
    private float comboWindow = 1;

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        xInput = 0; // we need this to fix bug on attack direction

        if (comboCounter > 2 || Time.time >= lastTimeAttack + comboWindow)
            comboCounter = 0;
        player.anim.SetInteger("ComboCounter", comboCounter);
        stateTimer = 0.1f;

        float attackDir = xInput == 0 ? player.facingDir : xInput;

        player.setVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.15f);

        comboCounter += 1;
        lastTimeAttack = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.changeState(player.idleState);
    }
}
