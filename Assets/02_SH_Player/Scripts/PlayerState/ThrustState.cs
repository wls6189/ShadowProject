using UnityEngine;

public class ThrustState : IState
{
    PlayerController player;
    float frame = 35;

    public ThrustState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Thrust;
        player.Animator.SetTrigger("DoThrust");
        player.IsAttacking = true;
    }

    public void Execute()
    {
        if (player.Animator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����
        if (duration >= 1f / frame && duration <= 5f / frame)
        {
            player.AttackMoving(16f);
        }

        // �и� ���� �� ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 1f / frame && duration <= 5f / frame)
        {
            player.IsParring = true;
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsParring = false;
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����)
        if (duration >= 20f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (duration >= 32f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsAttacking = false;
        player.IsParring = false;
        player.IsAttackColliderEnabled = false;
    }
}