using System.Text.RegularExpressions;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField]
    string itemName; 

    private void Start()
    {
        itemName = GetCleanName(this.gameObject.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControllerTest player = other.GetComponent<PlayerControllerTest>();
            player.CollectItem(itemName);
            Destroy(this.gameObject); 
        }
    }

    private string GetCleanName(string originalName)
    {
        // \s* -> ������ ������ 0�� �̻��� ���� ���� ����
        //\(.*\) -> ��ȣ �ȿ� �ִ� ��� ���� ����.

        return Regex.Replace(originalName, @"\s*\(.*\)", "");
    }
}
