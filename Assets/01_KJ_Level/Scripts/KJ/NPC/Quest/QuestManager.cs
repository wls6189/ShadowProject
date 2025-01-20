using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager instance;
    public static QuestManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DialogSystem"); //EventBus��� �� ��ü�� �����
                instance = go.AddComponent<QuestManager>(); //EventBus �� ��ü�� EventBus ��ũ��Ʈ(������Ʈ)�� �߰�
                Debug.Log("instance");
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Quest> allActiveQuests; //Ȱ��ȭ�� ����Ʈ�� ������ ����Ʈ. ��, ���ο� ����Ʈ�� ���� �� �� �� ����Ʈ�� �߰���
    public List<Quest> allCompletedQuests; //Ȱ��ȭ�� ����Ʈ�� �Ϸ��ϰ� ���� ������ ����Ʈ. ��, ����Ʈ�� �Ϸ����� �� �� ����Ʈ�� �߰���.


    [Header("QuestMenu")]
    public GameObject questMenu; //�� ����Ʈ ��� UI
    public bool isQuestMenuOpen; //�� ����Ʈ UI Ȱ��ȭ ����

    public GameObject activeQuestPrefab;  //���� Ȱ��ȭ�� ����Ʈ�� ǥ���ϱ� ���� UI ������
    public GameObject completeQuestPrefab; //�Ϸ�� ����Ʈ�� ǥ���ϱ� ���� UI ������

    public GameObject questMenuContent; //����Ʈ �޴��� ��ũ�Ѻ� ������ ��ü�� ����, Ȱ��ȭ�� ����Ʈ�� �Ϸ�� ����Ʈ�� UI�� �������� �߰��� ��,
                                        //�� ��ü�� �ڽ����� �������� �����˴ϴ�.

    [Header("QuestTracker")]
    public GameObject questTrackerContent; //���� ���� ���� ����Ʈ ������ ǥ���ϴ� UI�� ������ ��ü


    private void Update()
    {
        
    }

    public void AddActiveQuest(Quest quest)
    {
        allActiveQuests.Add(quest);
        RefreshQuestList();
    }

    public void MarkQuestCompleted(Quest quest)
    {
        allActiveQuests.Remove(quest);
        allCompletedQuests.Add(quest);

        RefreshQuestList();
    }

    public void RefreshQuestList()//Ȱ��ȭ�� ����Ʈ ��� �Ǵ� �Ϸ�� ����Ʈ ��Ͽ� ���� ����Ʈ ���ΰ�ħ ����� �ʿ�.
    {
        foreach(Transform child in questMenuContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(Quest actvieQuest in allActiveQuests)
        {
            GameObject questPrefab = Instantiate(activeQuestPrefab, Vector3.zero, Quaternion.identity); //Ȱ��ȭ�� ����Ʈ ������ ����
            questPrefab.transform.SetParent(questMenuContent.transform, false); //Ȱ��ȭ�� ����Ʈ �������� �θ� Content�� ����

            QuestRow qRow = questPrefab.GetComponent<QuestRow>(); //Ȱ��ȭ�� ����Ʈ �������� QuestRow ������Ʈ ������

            qRow.quesetName.text = actvieQuest.questName;
            qRow.questGiver.text = actvieQuest.questGiver;

            qRow.isActive = true;
            qRow.isTracking = true;

            qRow.coinAmount.text = $"{actvieQuest.info.coinReward}";

           // qRow.firstRewardAmount.text = $"{actvieQuest.info.firstRequirmentAmount}";
            qRow.firstRewardAmount.text = "";

            qRow.secondRewardAmount.text = "";
        }

        foreach (Quest completeQuest in allCompletedQuests)
        {
            GameObject questPrefab = Instantiate(completeQuestPrefab, Vector3.zero, Quaternion.identity); //Ȱ��ȭ�� ����Ʈ ������ ����
            questPrefab.transform.SetParent(questMenuContent.transform, false); //Ȱ��ȭ�� ����Ʈ �������� �θ� Content�� ����

            QuestRow qRow = questPrefab.GetComponent<QuestRow>(); //Ȱ��ȭ�� ����Ʈ �������� QuestRow ������Ʈ ������

            qRow.quesetName.text = completeQuest.questName;
            qRow.questGiver.text = completeQuest.questGiver;

            qRow.isActive = false;
            qRow.isTracking = false;

            qRow.coinAmount.text = $"{completeQuest.info.coinReward}";

            // qRow.firstRewardAmount.text = $"{actvieQuest.info.firstRequirmentAmount}";
            qRow.firstRewardAmount.text = "";

            qRow.secondRewardAmount.text = "";
        }
    }
}
