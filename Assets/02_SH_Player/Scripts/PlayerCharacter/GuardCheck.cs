using System.Collections.Generic;
using UnityEngine;

public class GuardCheck : MonoBehaviour
{
    [SerializeField] PlayerController player;
    BoxCollider guardCollider;
    List<GameObject> monstersAttackingPlayer = new(); // �ߺ� üũ�� ���� ����Ʈ
    void Awake()
    {
        TryGetComponent(out guardCollider);
    }

    void Update()
    {
        OnGuardCollider();
    }

    void OnGuardCollider()
    {
        if (player.IsGuarding)
        {
            guardCollider.enabled = true;
        }
        else
        {
            guardCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) // �����ϴ� ���� ���� ���� ����� 2�� �������� �ʴ� ���� �ʿ�
    {
        if (player.IsGuarding && other.CompareTag("Enemy"))
        {
            if (player.IsParring)
            {
                player.Animator.SetTrigger("DoParry");
                player.IsParring = false; // �и��� �����ϸ� �и� ���� ��� ����
            }
            else
            {
                player.Animator.SetTrigger("DoGuardHit");
            }
        }
    }
}
