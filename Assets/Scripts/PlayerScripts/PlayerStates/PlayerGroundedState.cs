using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
            stateMachine.changeState(player.blackHoleState);

        if (Input.GetKeyDown(KeyCode.R) && HasNoSword())
            stateMachine.changeState(player.aimSword);

        if (Input.GetKey(KeyCode.Mouse0))
            stateMachine.changeState(player.primaryAttack);

        if (!player.isGroundDetected())
            stateMachine.changeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space) && player.isGroundDetected())
        {
            stateMachine.changeState(player.jumpState);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            stateMachine.changeState(player.counterAttack);
        }
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }
        player.sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;

    }
}
