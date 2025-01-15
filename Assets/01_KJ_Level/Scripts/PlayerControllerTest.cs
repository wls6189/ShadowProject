using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerControllerTest : MonoBehaviour
{
    public Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // ������ �̸��� ������ ����

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
        float h = Input.GetAxisRaw("Horizontal"); //-1,0,1 ��ȯ

        Vector3 movement = new Vector3(h, 0, 0) * moveSpeed * Time.deltaTime;

        // Transform�� ����� �̵�
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

        Debug.Log("ȹ���� ������" + itemName + "�� ����:" + collectedItems[itemName]);
    }

    void InventoryCheck()
    {
        Debug.Log("���� �κ��丮 ����:");
        foreach (var item in collectedItems)
        {
            Debug.Log($"������: {item.Key}, ����: {item.Value}");
        }

        if (collectedItems.Count == 0)
        {
            Debug.Log("�κ��丮�� ��� �ֽ��ϴ�.");
        }
    }

   public void RemoveItem(string itemName,int count)
    {
        if (collectedItems.ContainsKey(itemName))
        {
            collectedItems[itemName] -= count;
            Debug.Log($"������ '{itemName}'�� ������ {count} ��ŭ ����. ���� ����: {collectedItems[itemName]}");

            if (collectedItems[itemName] <= 0)
            {
                collectedItems.Remove(itemName); // ������ 0 ���ϰ� �Ǹ� ��ųʸ����� ����
                Debug.Log($"������ '{itemName}'�� �κ��丮���� ������.");
            }
        }
        else
        {
            Debug.Log($"������ '{itemName}'�� �κ��丮�� �����ϴ�.");
        }
    }
}
