using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    [SerializeField] PlayerController player;
    CapsuleCollider attackCollider;
    List<GameObject> attackedMonstersByPlayer = new(); // �ߺ� üũ�� ���� ����Ʈ

    void Awake()
    {
        TryGetComponent(out attackCollider);
    }

    void Update()
    {
        OnAttackColllider();
    }

    void OnAttackColllider()
    {
        if (player.IsAttackColliderEnabled && player.CurrentPlayerState != PlayerState.Thrust)
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
        }
    }
}
