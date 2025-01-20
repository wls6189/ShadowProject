using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // ��ӵ� ������ ������
    private bool isPlayerNearby = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ��

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // �÷��̾ ���� �ȿ� ����
        }
    
    if (isPlayerNearby && Input.GetKeyDown(KeyCode.F)) // ���� ������ F Ű�� ������ ��
        {
            // �÷��̾��� �κ��丮�� ��������
            GameObject player = GameObject.FindGameObjectWithTag("Player");
    Inventory playerInventory = player.GetComponent<Inventory>();

            if (playerInventory != null)
            {
                playerInventory.AddItem(item); // ������ �߰�
                Destroy(gameObject); // ������ ������Ʈ ����
}
        }
    }
}

