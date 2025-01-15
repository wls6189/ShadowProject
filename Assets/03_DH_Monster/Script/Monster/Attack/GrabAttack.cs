using System.Collections;
using UnityEngine;

public class GrabAttack : MonoBehaviour
{
    public Transform grabPosition;  // �׷� ��ġ
    public float grabDuration = 3f;  // �׷� ���� �ð� (3��)
    public string grabAnimationTrigger = "GrabSuccess";  // ������ ��� ���� �ִϸ��̼� Ʈ���� �̸�

    private Animator animator;  // ������ Animator (�ڽ� �ݶ��̴��� �ƴ� ���� ��ü�� �ִϸ����� ���)

    private void Start()
    {
        // ������ Animator ������Ʈ�� ã���ϴ�.
        animator = GetComponentInParent<Animator>();  // ������ �ִϸ����͸� �θ𿡼� ã���ϴ�
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && !player.IsGrabbed)
            {
                player.SetGrabbedState(true);
                StartCoroutine(GrabPlayer(player));

                // ��� ���� �� ���� �ִϸ��̼� ���
                if (animator != null)
                {
                    // �ִϸ��̼� Ʈ���� ȣ��
                    animator.SetTrigger(grabAnimationTrigger);
                }
                else
                {
                    Debug.LogError("Animator not found on monster or its parent!");
                }
            }
        }
    }

    private IEnumerator GrabPlayer(Player player)
    {
        // �׷� ��ġ�� �̵�
        player.transform.position = grabPosition.position;
        player.transform.SetParent(grabPosition);  // �׷� ��ġ�� �ڽ����� ����

        // 3�� ���
        yield return new WaitForSeconds(grabDuration);

        // 3�� �ڿ� �׷� ����
        player.transform.SetParent(null);  // �׷� ��ġ���� ���
        player.SetGrabbedState(false);  // �׷� ���� ����

        // ȸ�� �ʱ�ȭ (������ ����)
        player.transform.rotation = Quaternion.identity;  // ȸ�� �� �ʱ�ȭ

        // �ʿ信 ���� Player�� ���� ��ġ�� ���ư��� �ϰų�, �ٸ� ó���� �߰��� �� �ֽ��ϴ�.
    }
}
