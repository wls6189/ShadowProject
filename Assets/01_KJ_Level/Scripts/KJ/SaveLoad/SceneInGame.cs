using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInGame : MonoBehaviour
{
    public TextMeshProUGUI name;
    public TextMeshProUGUI level;
    public TextMeshProUGUI coin;

    //public GameObject[] WeaponItem; �׽�Ʈ��



    private void Start() 
    {
        // ���� ���� �Ѿ���� �÷��̾��� ����(�̸�, ����, ����)�� �˸°� ��Ÿ���� ��.
        name.text += DataManager.Instance.nowPlayer.name; // name �� ���� �̸�: �̹Ƿ� name += ���·� ����.
        level.text += DataManager.Instance.nowPlayer.level.ToString(); // text string�̰� level�� int�̹Ƿ� ToString()
        coin.text += DataManager.Instance.nowPlayer.coin.ToString();

        // �÷��̾��� ��ġ ���� (���� ������ �ε�� �� ��ġ ����)
    
       

        // ������ ���� (�ּ�ó���� �κ�, �ʿ�� Ȱ��ȭ)
        // ItemSetting(DataManager.Instance.nowPlayer.item);
    }

    public void SetPlayerPos()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("�÷��̾� ã��");
            Debug.Log("�÷��̾� ��ġ�� " + DataManager.Instance.nowPlayer.position);
            player.transform.position = DataManager.Instance.nowPlayer.position;
        }
    }


    public void LevelUp()
    {
        DataManager.Instance.nowPlayer.level++; //�� �ٲٱ�
        level.text = "���� : " + DataManager.Instance.nowPlayer.level.ToString(); //UI��ȭ�� ����
    }

    public void CoinUp()
    {
        DataManager.Instance.nowPlayer.coin += 100;
        coin.text = "���� : " + DataManager.Instance.nowPlayer.coin.ToString(); //UI��ȭ�� ����
    }

    public void Save()
    {
        DataManager.Instance.SaveData();
    }



    //public void ItemSetting(int number)
    //{
    //    for(int i = 0; i< WeaponItem.Length; i++)
    //    {
    //        if(number == i) //������ ����(0)�� ù��°(0) ��� 
    //        { 
    //            WeaponItem[i].SetActive(true); //ù��° ���⸸ Ȱ��ȭ
    //            DataManager.Instance.nowPlayer.item = number; //Ȱ��ȭ�� ������ ��ȣ�� PlayerData�� item ������ �����. ��, ���� ��.
    //        }
    //        else //�ƴ϶��
    //        {
    //            WeaponItem[i].SetActive(false); //��Ȱ��ȭ
    //        }
    //    }
    //}
}
