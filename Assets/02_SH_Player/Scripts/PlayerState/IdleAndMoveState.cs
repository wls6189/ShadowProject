using UnityEngine;

public class IdleAndMoveState : IState
{
    PlayerController player;
    float moveBlendValue = 0;
    float moveBlendDeltaValue = 5f;

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
            if (player.IsLookRight) // ���� �����ʿ� �ִ� ���
            {
                if (player.MoveActionValue == 1)
                {
                    moveBlendValue += Time.deltaTime * moveBlendDeltaValue * 2;
                    moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
                }
                else if (player.MoveActionValue == -1)
                {
                    moveBlendValue -= Time.deltaTime * moveBlendDeltaValue * 2;
                    moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
                }
                else
                {
                    if (moveBlendValue > 0)
                    {
                        moveBlendValue -= Time.deltaTime * moveBlendDeltaValue * 2;
                        moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
                    }
                    else if (moveBlendValue < 0)
                    {
                        moveBlendValue += Time.deltaTime * moveBlendDeltaValue * 2;
                        moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
                    }
                }
            }
            else
            {
                if (player.MoveActionValue == 1)
                {
                    moveBlendValue -= Time.deltaTime * moveBlendDeltaValue * 2;
                    moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
                }
                else if (player.MoveActionValue == -1)
                {
                    moveBlendValue += Time.deltaTime * moveBlendDeltaValue * 2;
                    moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
                }
                else
                {
                    if (moveBlendValue > 0)
                    {
                        moveBlendValue -= Time.deltaTime * moveBlendDeltaValue * 2;
                        moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
                    }
                    else if (moveBlendValue < 0)
                    {
                        moveBlendValue += Time.deltaTime * moveBlendDeltaValue * 2;
                        moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
                    }
                }
            }
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
        moveBlendValue = 0;
        player.Animator.SetFloat("MoveBlendValue", moveBlendValue);
    }
}