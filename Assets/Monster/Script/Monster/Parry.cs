using UnityEngine;

public class Parry : MonoBehaviour
{
    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>(); // �θ� ������Ʈ���� EnemyHealth ��������

    }

    // �и� �ݶ��̴��� �浹 ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            // �÷��̾� ������ �������� ��ȥ ������ �ޱ�
            //float damage = other.GetComponent<PlayerController>().damage;
            //float soulDamage = other.GetComponent<PlayerController>().soulDamage;
            //float attackPower = other.GetComponent<PlayerController>().attackPower;

            // �и� ���¿��� ������ ������ ����
            enemyHealth.TakeDamage(0f, 0f, 0f);
        }
    }
}
