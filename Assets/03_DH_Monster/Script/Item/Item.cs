using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;

    public virtual void ApplyEffect(Player player)
    {
        Debug.Log($"{itemName} ȿ���� �÷��̾ �����մϴ�.");
    }
}
