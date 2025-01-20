using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerData
{
    //�̸�, ���� , ����, �������� ���� ���� ����
    public string name;
    public int level = 1;
    public int coin = 100;
    //  public int item = -1; 

    public Vector3 position;
    public string currentScene;

}
public class DataManager : MonoBehaviour
{
    //���� ���� �׻� �����ϸ� �����Ƿ� �̱���
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DataManager"); //EventBus��� �� ��ü�� �����
                instance = go.AddComponent<DataManager>(); //EventBus �� ��ü�� EventBus ��ũ��Ʈ(������Ʈ)�� �߰�
            }
            return instance;
        }
    }

    public PlayerData nowPlayer = new PlayerData();

    public string path;

    public int nowSlot;

    private void Awake()
    {
        #region �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        path = Application.persistentDataPath + "/Save"; 
    }

    public void SaveData()
    {
       
        string data = JsonUtility.ToJson(nowPlayer);

        //�����ϱ� - WriteAllText() ���
        File.WriteAllText(path  + nowSlot.ToString(), data); // ����� �����̸��� ������ �� �����͸� �����ϰ� ���� �̸� �ڿ� ������ ��ȣ���� �߰�. Save0,Save1...
    }

    public void LoadData()
    {
        //�ҷ����� - ReadAllText() ���
        string data = File.ReadAllText(path  + nowSlot.ToString()); 

        nowPlayer = JsonUtility.FromJson<PlayerData>(data); //������ �����ߴ� nowPlayer�� �ҷ����� ���� ���� ����� ��

    }

    public void DataClear()
    {
        nowSlot = -1; //���� ��ȣ�� 
        nowPlayer = new PlayerData(); //�ʱⰪ���� �ٽ� �ʱ�ȭ
    }


}
