using UnityEngine;

public class PiercingAttack : MonoBehaviour
{
public float damage = 10f; // �⺻ ���ݷ�
private bool parrySuccessful = false; // �и� ���� ����


    private void OnTriggerEnter(Collider other)
    {

        PlayerController player = other.GetComponent<PlayerController>();
        //if (player != null && player.IsPenetrating)
        {

            parrySuccessful = true; // �и� ���� ���� ���
          //playerScript.OnParrySuccess(); // �÷��̾�� �и� ���� �˸� 
            return; // ���� ó�� �ߴ�
        }

        // Player �±׿� �浹 ��
        if (other.CompareTag("Player"))
        {
            if (parrySuccessful)
            {
                Debug.Log("�и����� ���� �������� ��ȿȭ�Ǿ����ϴ�.");
                parrySuccessful = false; // ���� �ʱ�ȭ
                return; // ������ ó�� �ߴ�
            }

            Debug.Log("�÷��̾�� ������ " + damage);
            //other.GetComponent<Health>().Damage(damage); // ������ ó��
        }
    }
}