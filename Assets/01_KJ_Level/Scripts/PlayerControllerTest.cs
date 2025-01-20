using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerControllerTest : MonoBehaviour
{
    public Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // 아이템 이름과 개수를 관리

    [SerializeField]
    float moveSpeed = 3.0f;
    void Update()
    {
        if(DialogSystem.Instance.isdialogueCanvas == false)
        {
            MovePlayer();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryCheck();
        }
    }

    void MovePlayer()
    {
        float h = Input.GetAxisRaw("Horizontal"); //-1,0,1 반환

        Vector3 movement = new Vector3(h, 0, 0) * moveSpeed * Time.deltaTime;

        // Transform을 사용한 이동
        transform.Translate(movement);
    }

    public void CollectItem(string itemName)
    {
       if(collectedItems.ContainsKey(itemName))
        {
            collectedItems[itemName]++;
        }
       else
        {
            collectedItems[itemName] = 1;
        }

        Debug.Log("획득한 아이템" + itemName + "총 개수:" + collectedItems[itemName]);
    }

    void InventoryCheck()
    {
        Debug.Log("현재 인벤토리 상태:");
        foreach (var item in collectedItems)
        {
            Debug.Log($"아이템: {item.Key}, 개수: {item.Value}");
        }

        if (collectedItems.Count == 0)
        {
            Debug.Log("인벤토리가 비어 있습니다.");
        }
    }

   public void RemoveItem(string itemName,int count)
    {
        if (collectedItems.ContainsKey(itemName))
        {
            collectedItems[itemName] -= count;
            Debug.Log($"아이템 '{itemName}'의 개수가 {count} 만큼 감소. 남은 개수: {collectedItems[itemName]}");

            if (collectedItems[itemName] <= 0)
            {
                collectedItems.Remove(itemName); // 개수가 0 이하가 되면 딕셔너리에서 삭제
                Debug.Log($"아이템 '{itemName}'이 인벤토리에서 없어짐.");
            }
        }
        else
        {
            Debug.Log($"아이템 '{itemName}'이 인벤토리에 없습니다.");
        }
    }
}
