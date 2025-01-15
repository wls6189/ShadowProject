using UnityEngine;

public class BasicHorizonSlash1State : IState
{
    PlayerController player;


    public BasicHorizonSlash1State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicHorizonSlash1;

        player.AnimationSetter.StartBasicHorizonSlash1();

        player.Animator.SetTrigger("DoBasicHorizonSlash1");

        player.IsAttacking = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}