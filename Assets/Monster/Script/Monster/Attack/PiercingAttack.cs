using UnityEngine;

public class PiercingAttack : MonoBehaviour
{
public float damage = 10f; // �⺻ ���ݷ�
private bool parrySuccessful = false; // �и� ���� ����
private Player playerScript; // �÷��̾� ��ũ��Ʈ ����

private void Start()
{
    // �÷��̾� ��ũ��Ʈ ����
    playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
}

private void OnTriggerEnter(Collider other)
{
    // Parry �±׿� �浹 ��
    if (other.CompareTag("detection"))
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