using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>(); // ������ ������ ���
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void AddItem(Item item) // ������ �߰�
    {
        items.Add(item);
       
    }
}