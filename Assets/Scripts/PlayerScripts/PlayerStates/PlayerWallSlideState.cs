using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.changeState(player.wallJump);
            return;
        }

        if (yInput < 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y * 0.3f);


        if (xInput != 0 && player.facingDir != xInput)
        {
            stateMachine.changeState(player.idleState);
        }

        if (player.isGroundDetected())
        {
            stateMachine.changeState(player.idleState);
        }
        if (!player.isGroundDetected() && !player.isWallDetected())
        {
            stateMachine.changeState(player.airState);
        }
    }
}
