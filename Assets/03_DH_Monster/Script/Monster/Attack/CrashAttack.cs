using System.Collections;
using UnityEngine;

public class CrashAttack : MonoBehaviour
{
    public Collider aoeCollider; // ���� ������ �ݶ��̴�
    public float aoeDamage = 50f; // ���� ������
    public float aoeActiveTime = 0.5f; // ���� ������ Ȱ��ȭ �ð�
    public float clashDuration = 3.0f; // �ݵ� ���� �ð�

    private bool isClashActive = false; // ���� �ݵ� ����

    private void Start()
    {
        // ���� ������ �ݶ��̴� �ʱ�ȭ
        if (aoeCollider == null)
        {
            Debug.LogError("AOE Collider is not assigned!");
            return;
        }

        aoeCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; // �÷��̾ �ƴ� ��� ����

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.IsReadyForClash()) // �÷��̾ �ݵ� �غ� �������� Ȯ��
        {
            Debug.Log("Player is ready for Clash. Starting Clash Event...");
            StartClash(player);
        }
        else
        {
            Debug.Log("Player failed to defend Clash!");
            FailClash(player);
        }
    }

    private void StartClash(PlayerController player)
    {
        if (isClashActive) return; // �ߺ� ���� ����
        isClashActive = true;
        PlayerController.clashSuccess = false; // �ݵ� ���� �÷��� �ʱ�ȭ
        // �÷��̾�� �ݵ� �̺�Ʈ ó��
        player.StartClashEvent(this);


        // ���� �ð� �� �ݵ� ��� Ȯ��
        StartCoroutine(DelayedCheckClashResult(player, clashDuration));
    }
    private IEnumerator DelayedCheckClashResult(PlayerController player, float delay)
    {
        yield return new WaitForSeconds(delay); // ������ �ð���ŭ ���
        CheckClashResult(player); // ��� Ȯ�� �޼��� ȣ��
    }

    private void CheckClashResult(PlayerController player)
    {
        if (!isClashActive) return; // �ݵ��� Ȱ��ȭ���� �ʾҴٸ� ����
        isClashActive = false; // �ݵ� ����

        if (PlayerController.clashSuccess)
        {
            Debug.Log("Player defended the Clash Attack successfully!");
            EndClash(true);
        }
        else
        {
            Debug.Log("Player failed to defend the Clash Attack!");
            FailClash(player); // �̹� ������ �÷��̾ ����
        }
    }

    private void EndClash(bool success)
    {
        if (success)
        {
            Debug.Log("Clash successfully defended! Enemy enters Groggy State.");
            // �� �׷α� ���� ó�� ���� �߰�
        }
    }

    private void FailClash(PlayerController player)
    {
        if (player == null) return;

        Debug.Log("Clash Failed! Applying AOE Damage to Player.");

        if (!aoeCollider.enabled) // �ߺ� Ȱ��ȭ ����
        {
            ToggleAOECollider(true);

            // ���� �ð� �� ���� ������ ��Ȱ��ȭ
            Invoke(nameof(DisableAOEDamage), aoeActiveTime);
        }

        // �÷��̾�� ������ ����
        player.TakeDamage(aoeDamage);
    }

    private void ToggleAOECollider(bool state)
    {
        if (aoeCollider != null)
        {
            aoeCollider.enabled = state;
            Debug.Log($"AOE Collider {(state ? "Activated" : "Deactivated")}!");
        }
    }

    private void DisableAOEDamage()
    {
        ToggleAOECollider(false);
    }
}
