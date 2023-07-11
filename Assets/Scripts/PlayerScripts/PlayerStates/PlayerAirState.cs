public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (player.isWallDetected())
        {
            stateMachine.changeState(player.wallSlide);
        }
        if (xInput != 0)
        {
            player.setVelocity(player.moveSpeed * 0.8F * xInput, rb.velocity.y);
        }
        if (player.isGroundDetected())
        {
            stateMachine.changeState(player.idleState);
        }
    }
}
