using UnityEngine;

public class BasicHorizonSlash1State : IState
{
    PlayerController player;
    float frame = 35;

    public BasicHorizonSlash1State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicHorizonSlash1;
        player.Animator.SetTrigger("DoBasicHorizonSlash1");
        player.IsAttacking = true;
    }

    public void Execute()
    {
        if (!player.StateInfo.IsName("BasicHorizonSlash1"))
        {
            player.Animator.Play("BasicHorizonSlash1");
            return;
        }

        // ���� �� ���� ����
        if (player.StateInfo.normalizedTime >= 6f / frame && player.StateInfo.normalizedTime <= 10f / frame) // ù �� ���
        {
            player.AttackMoving(3f); // �� ��� �� �÷��̾ �����̴� �ӵ��� �Ű������� �Է�.
        }
        else if (player.StateInfo.normalizedTime >= 18f / frame && player.StateInfo.normalizedTime <= 32f / frame) // ���ƿ��� �� ���
        {
            player.AttackMoving(2f);
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (player.StateInfo.normalizedTime >= 7f / frame && player.StateInfo.normalizedTime <= 10f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����) + (�޺� ���� ��� ����)
        if (player.StateInfo.normalizedTime >= 20f / frame)
        {
            player.IsAttacking = false;
            player.CanBasicHorizonSlashCombo = true;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (player.StateInfo.normalizedTime >= 32f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
            player.CanBasicHorizonSlashCombo = false;
        }
    }

    public void Exit()
    {
        // ������ ����� ���� �ʱ�ȭ
        player.IsAttacking = false;
        player.CanBasicHorizonSlashCombo = false;
        player.IsAttackColliderEnabled = false;
    }
}