using UnityEngine;

public class BasicHorizonSlash2State : IState
{
    PlayerController player;


    public BasicHorizonSlash2State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicHorizonSlash2;

        player.AnimationSetter.StartBasicHorizonSlash2();

        player.Animator.SetTrigger("DoBasicHorizonSlash2");

        player.IsAttacking = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}