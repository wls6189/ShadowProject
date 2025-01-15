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
                GameObject go = new GameObject("DialogSystem"); //EventBus라는 빈 객체를 만들고
                instance = go.AddComponent<QuestManager>(); //EventBus 빈 객체에 EventBus 스크립트(컴포넌트)을 추가
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

    public List<Quest> allActiveQuests; //활성화된 퀘스트를 보관할 리스트. 즉, 새로운 퀘스트가 시작 될 때 이 리스트에 추가됨
    public List<Quest> allCompletedQuests; //활성화된 퀘스트를 완료하고 나서 보관할 리스트. 즉, 퀘스트를 완료했을 때 이 리스트에 추가됨.


    [Header("QuestMenu")]
    public GameObject questMenu; //내 퀘스트 목록 UI
    public bool isQuestMenuOpen; //내 퀘스트 UI 활성화 여부

    public GameObject activeQuestPrefab;  //현재 활성화된 퀘스트를 표시하기 위한 UI 프리팹
    public GameObject completeQuestPrefab; //완료된 퀘스트를 표시하기 위한 UI 프리팹

    public GameObject questMenuContent; //퀘스트 메뉴의 스크롤뷰 콘텐츠 객체를 참조, 활성화된 퀘스트와 완료된 퀘스트를 UI에 동적으로 추가할 때,
                                        //이 객체의 자식으로 프리팹이 생성됩니다.

    [Header("QuestTracker")]
    public GameObject questTrackerContent; //현재 추적 중인 퀘스트 정보를 표시하는 UI의 콘텐츠 객체


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

    public void RefreshQuestList()//활성화된 퀘스트 목록 또는 완료된 퀘스트 목록에 대한 퀘스트 새로고침 기능이 필요.
    {
        foreach(Transform child in questMenuContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(Quest actvieQuest in allActiveQuests)
        {
            GameObject questPrefab = Instantiate(activeQuestPrefab, Vector3.zero, Quaternion.identity); //활성화된 퀘스트 프리펩 생성
            questPrefab.transform.SetParent(questMenuContent.transform, false); //활성화된 퀘스트 프리펩의 부모를 Content로 설정

            QuestRow qRow = questPrefab.GetComponent<QuestRow>(); //활성화된 퀘스트 프리펩의 QuestRow 컴포넌트 가져옴

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
            GameObject questPrefab = Instantiate(completeQuestPrefab, Vector3.zero, Quaternion.identity); //활성화된 퀘스트 프리펩 생성
            questPrefab.transform.SetParent(questMenuContent.transform, false); //활성화된 퀘스트 프리펩의 부모를 Content로 설정

            QuestRow qRow = questPrefab.GetComponent<QuestRow>(); //활성화된 퀘스트 프리펩의 QuestRow 컴포넌트 가져옴

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
