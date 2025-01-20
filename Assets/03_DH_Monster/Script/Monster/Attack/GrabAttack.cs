using UnityEngine;

public class GrabAttack : MonoBehaviour
{
    public Transform grabPoint; // �÷��̾ ������ ��ġ
    public float grabDuration = 2.0f; // ��� ���� �ð�
    private Animator animator; // Animator ������Ʈ ����
    public float damage = 20f; // ��� ���� ������

    private void Start()
    {      
        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾����� Ȯ��
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // �ִϸ��̼� Ʈ���� Ȱ��ȭ
                animator.SetTrigger("GrabSuccess");
                // �÷��̾� ��� ����
                player.OnGrabbed(grabPoint, grabDuration);
                other.GetComponent<Health>().Damage(damage); // ������ ó��
            }
        }
    }
}