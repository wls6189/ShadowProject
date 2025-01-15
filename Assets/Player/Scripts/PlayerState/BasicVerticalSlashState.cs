using UnityEngine;

public class BasicVerticalSlashState : IState
{
    PlayerController player;


    public BasicVerticalSlashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicVerticalSlash;

        player.AnimationSetter.StartBasicVerticalSlash();

        player.Animator.SetTrigger("DoBasicVerticalSlash");

        player.IsAttacking = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}