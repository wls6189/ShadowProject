using UnityEngine;
using static UnityEditor.Progress;

public class ItemPickup : MonoBehaviour
{
    public Item item; // 드롭된 아이템 데이터

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 닿았을 때
        {
            Inventory playerInventory = other.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                playerInventory.AddItem(item); // 아이템 추가
                Destroy(gameObject); // 아이템 오브젝트 제거
            }
        }
    }
}
