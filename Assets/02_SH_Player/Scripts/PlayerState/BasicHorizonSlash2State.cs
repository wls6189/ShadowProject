using UnityEngine;

public class BasicHorizonSlash2State : IState
{
    PlayerController player;
    float frame = 37;

    public BasicHorizonSlash2State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicHorizonSlash2;
        player.Animator.SetTrigger("DoBasicHorizonSlash2");
        player.IsAttacking = true;
        player.CanBasicHorizonSlashCombo = false;
    }

    public void Execute()
    {
        if (!player.StateInfo.IsName("BasicHorizonSlash2"))
        {
            player.Animator.Play("BasicHorizonSlash2");
            return;
        }

        if (player.StateInfo.normalizedTime >= 0f && player.StateInfo.normalizedTime <= 1f / frame) // �ִϸ��̼� ����
        {
            player.IsAttacking = true;
            player.CanBasicHorizonSlashCombo = false;
        }

        // ���� �� ���� ����
        if (player.StateInfo.normalizedTime >= 6f / frame && player.StateInfo.normalizedTime <= 11f / frame) // ù �� ���
        {
            player.AttackMoving(4f); // �� ��� �� �÷��̾ �����̴� �ӵ��� �Ű������� �Է�.
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (player.StateInfo.normalizedTime >= 7f / frame && player.StateInfo.normalizedTime <= 12f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����) + (�޺� ���� ��� ����)
        if (player.StateInfo.normalizedTime >= 22f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (player.StateInfo.normalizedTime >= 34f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsAttacking = false;
        player.IsAttackColliderEnabled = false;
    }
}