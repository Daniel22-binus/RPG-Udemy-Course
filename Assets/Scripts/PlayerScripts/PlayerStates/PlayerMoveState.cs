public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        player.setVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if (xInput == 0 || player.isWallDetected())
            stateMachine.changeState(player.idleState);
    }
}
