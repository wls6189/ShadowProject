using UnityEngine;

public class IdleAndMoveState : IState
{
    PlayerController player;
    float moveBlendValue = 0;
    float moveBlendDeltaValue = 7f;

    public IdleAndMoveState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.IdleAndMove;

        player.Animator.SetTrigger("DoIdleAndMove");

        player.IsAttacking = false;
    }

    public void Execute()
    {
        // �÷��̾��� ���� �̵�
        Vector3 moveVector = new Vector3(0, 0, player.MoveActionValue);
        player.CharacterController.Move(moveVector * player.MoveSpeed * Time.deltaTime);

        // �̵� �ִϸ��̼� 
        if (player.IsLockOn) // ��� ���� ���� ��
        {
            //if (player.moveActionValue > 0)
            //{
            //    moveBlendValue += Time.deltaTime * moveBlendDeltaValue;
            //}
            //else if (player.moveActionValue == 0)
            //{
            //    if (player.moveActionValue > 0)
            //    {
            //        moveBlendValue -= Time.deltaTime * moveBlendDeltaValue;
            //        moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
            //    }
            //    else if (player.moveActionValue < 0)
            //    {
            //        moveBlendValue += Time.deltaTime * moveBlendDeltaValue;
            //        moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
            //    }
            //}
            //else if (player.moveActionValue < 0)
            //{

            //}
        }
        else // ��� ������ �ƴ� ��
        {
            if (player.MoveActionValue == 1 || player.MoveActionValue == -1)
            {
                moveBlendValue += Time.deltaTime * moveBlendDeltaValue;
            }
            else
            {
                moveBlendValue -= Time.deltaTime * moveBlendDeltaValue;
            }

            moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
        }

        player.Animator.SetFloat("MoveBlendValue", moveBlendValue); // �ִϸ������� moveBlendValue�� ����
    }

    public void Exit()
    {

    }
}