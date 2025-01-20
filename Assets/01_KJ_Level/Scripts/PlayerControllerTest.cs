using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerControllerTest : MonoBehaviour
{
    public Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // ������ �̸��� ������ ����

    [SerializeField]
    float moveSpeed = 3.0f;

    //InputActionAsset inputActionAsset;
    // InputAction gameMenuAction;

    private void Awake()
    {
     
    }
    private void Start()
    {
    
        
        //gameMenuAction = inputActionAsset.FindAction("GameMenu");
    }
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

        OnInteraction();
    }
    void OnInteraction()
    {

        if (Input.GetKeyDown(KeyCode.Escape )) //  if (gameMenuAction.WasPressedThisFrame()) ���߿� �������� �̰ɷ� �� ����.
        {
            if (UIManager.Instance.IsGameMenuOpen) 
            {
                UIManager.Instance.GameMenuClose(); 
            }
            else
            {
                UIManager.Instance.GameMenuOpen(); 
            }
        }
    }
    void MovePlayer()
    {
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 movement = new Vector3(h, 0, 0) * moveSpeed * Time.deltaTime;
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
        if (collectedItems.ContainsKey(itemName)) //�κ��丮�� itemName�̶�� �������� ���� ���. ������ �̸����� �Ǻ�.
        {
            collectedItems[itemName] -= count;
            Debug.Log($"������ '{itemName}'�� ������ {count} ��ŭ ����. ���� ����: {collectedItems[itemName]}");

            if (collectedItems[itemName] <= 0)
            {
                collectedItems.Remove(itemName); // ������ 0 ���ϰ� �Ǹ� ��ųʸ����� ����
                Debug.Log($"������ '{itemName}'�� �κ��丮���� ������.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("FragMent"))
        {
            SavePlayerData();
        }
        if (other.CompareTag("Portal"))
        {
            SceneManager.LoadScene(2);
        }
    }

    private void SavePlayerData()
    {
        // �÷��̾� ��ġ �� ���� �� ����
        DataManager.Instance.nowPlayer.position = transform.position;
        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name;

        // ������ ����
        DataManager.Instance.SaveData();
        Debug.Log("Player data saved." + DataManager.Instance.nowPlayer.position);
    }
}
