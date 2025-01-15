using UnityEngine;

public class DashState : IState
{
    PlayerController player;


    public DashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Dash;

        player.Animator.SetTrigger("DoDash");
    }

    public void Execute()
    {
        if (player.IsLookRight)
        {
            Vector3 moveVector = new Vector3(0, 0, 1);
            player.CharacterController.Move(moveVector * player.DashSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 moveVector = new Vector3(0, 0, -1);
            player.CharacterController.Move(moveVector * player.DashSpeed * Time.deltaTime);
        }

    }

    public void Exit()
    {

    }
}