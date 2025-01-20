using System.Collections;
using UnityEngine;

public class GuardState : IState
{
    PlayerController player;
    float guardBlendValue = 0;
    float guardBlendDeltaValue = 3f;

    public GuardState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Guard;
        player.Animator.SetTrigger("DoGuard");
        player.IsParring = true;
        player.IsGuarding = true;
    }

    public void Execute()
    {
        // �÷��̾��� ���� �̵�
        Vector3 moveVector = new Vector3(0, 0, player.MoveActionValue);
        player.CharacterController.Move(moveVector * player.MoveSpeed * Time.deltaTime / 2);

        // �̵� �ִϸ��̼� 
        if (player.IsLockOn) // ��� ���� ���� ��
        {
            if (player.IsLookRight) // ���� �����ʿ� �ִ� ���
            {
                if (player.MoveActionValue == 1)
                {
                    guardBlendValue += Time.deltaTime * guardBlendDeltaValue * 2;
                    guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
                }
                else if (player.MoveActionValue == -1)
                {
                    guardBlendValue -= Time.deltaTime * guardBlendDeltaValue * 2;
                    guardBlendValue = Mathf.Clamp(guardBlendValue, -1f, 0f);
                }
                else
                {
                    if (guardBlendValue > 0)
                    {
                        guardBlendValue -= Time.deltaTime * guardBlendDeltaValue * 2;
                        guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
                    }
                    else if (guardBlendValue < 0)
                    {
                        guardBlendValue += Time.deltaTime * guardBlendDeltaValue * 2;
                        guardBlendValue = Mathf.Clamp(guardBlendValue, -1f, 0f);
                    }
                }
            }
            else
            {
                if (player.MoveActionValue == 1)
                {
                    guardBlendValue -= Time.deltaTime * guardBlendDeltaValue * 2;
                    guardBlendValue = Mathf.Clamp(guardBlendValue, -1f, 0f);
                }
                else if (player.MoveActionValue == -1)
                {
                    guardBlendValue += Time.deltaTime * guardBlendDeltaValue * 2;
                    guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
                }
                else
                {
                    if (guardBlendValue > 0)
                    {
                        guardBlendValue -= Time.deltaTime * guardBlendDeltaValue * 2;
                        guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
                    }
                    else if (guardBlendValue < 0)
                    {
                        guardBlendValue += Time.deltaTime * guardBlendDeltaValue * 2;
                        guardBlendValue = Mathf.Clamp(guardBlendValue, -1f, 0f);
                    }
                }
            }
        }
        else // ��� ������ �ƴ� ��
        {
            if (player.MoveActionValue == 1 || player.MoveActionValue == -1)
            {
                guardBlendValue += Time.deltaTime * guardBlendDeltaValue;
            }
            else
            {
                guardBlendValue -= Time.deltaTime * guardBlendDeltaValue;
            }

            guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
        }

        player.Animator.SetFloat("GuardBlendValue", guardBlendValue); // �ִϸ������� moveBlendValue�� ����
    }

    public void Exit()
    {
        player.IsParring = false;
        player.IsGuarding = false;
        guardBlendValue = 0;
        player.Animator.SetFloat("GuardBlendValue", guardBlendValue);
    }
}