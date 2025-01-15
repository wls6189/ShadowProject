using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour 
{
    // 1. NPC 근처에 있을 경우에만 대화를 할 수 있는 기능.

    [SerializeField]
    private bool playerInRange; //대화 시작 범위안에 플레이어가 있는 경우에 대한 여부
    [SerializeField]
    GameObject player;

    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    private bool isTalkingwithPlayer;
    [SerializeField]
    GameObject NpcTalkImage;

    Quaternion initRot;

    [Header("퀘스트 관련 UI")]
    TextMeshProUGUI npcDialogText;

    Button NpcAcceptBtn;
    TextMeshProUGUI AcceptText;

    Button NpcDeclineBtn;
    TextMeshProUGUI DeclineText;

    Canvas QuestCanvas;

    [Header("퀘스트 대화 내용 객체")]
    public List<Quest> quests; //NPC가 가지고 있는 퀘스트 목록을 관리하는 List
    public Quest currentActiveQuest; //현재 활성화된 퀘스트를 나타냄( 리스트에서 특정 퀘스트를 선택)
    public int ActiveQuestIndex = 0; //현재 활성화된 퀘스트의 인덱스, 초기값은 0으로 설정되어 첫 번째 퀘스트부터 시작한다. 
    public bool isFirstTimeInteraction = true; //플레이어와 NPC의 첫 상호작용 여부, 
    public int CurrentDialog; //대화 진행 순서를 int로 관리.


    private void Start()
    {
        QuestCanvas = DialogSystem.Instance.QuestNpcCanvas;


        npcDialogText = DialogSystem.Instance.dialogText;

        NpcAcceptBtn = DialogSystem.Instance.optionBtn1;
        AcceptText = DialogSystem.Instance.optionBtn1.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        NpcDeclineBtn = DialogSystem.Instance.optionBtn2;
        DeclineText = DialogSystem.Instance.optionBtn2.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        initRot = transform.rotation;
    }
    private void Update()
    {
        if(playerInRange)
        {
            //텍스트 띄우기
            NpcTalkImage.gameObject.SetActive(true); // 캔버스 비활성화 -> 활성화 or 캔버스 활성화 된 상태에서 텍스트만 변경할 지

            if(Input.GetKey(KeyCode.F) && isTalkingwithPlayer == false) // 1. 대화 시작 전
            {
                //2. 대화 시작
                NpcTalkImage.GetComponentInChildren<TextMeshProUGUI>().text = "...";
                StartTalk();
            }
        }
        else
        {
            NpcTalkImage.gameObject.SetActive(false);
        }
    }

    public void StartTalk()
    {
        isTalkingwithPlayer = true;

        Vector3 dir = transform.position - player.transform.position;
        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        StartCoroutine(LookAtPlayerRoutine(targetRotation));

        Debug.Log("대화 시작");

        if(isFirstTimeInteraction) //대화 첫 시작 할 경우
        {
            isFirstTimeInteraction = false; 
            currentActiveQuest = quests[ActiveQuestIndex]; //quests 리스트에 저장된 퀘스트 중 ActiveQuestIndex에 해당하는
                                                           //퀘스트를 currentActiveQuest 변수에 저장.
            StartQuestInitialDialog();
            CurrentDialog = 0; //현재 대화를 0으로 초기화.
        }

        else //대화 첫 시작이 아닐 경우
        {
            if(currentActiveQuest.isDeclined) //이전에 대화 첫 시작했을 때 퀘스트를 거절했을 것이다. 따라서 isDeclined가 당연히 true 였을 것임. 
            {
                DialogSystem.Instance.OpenDialogUI(QuestCanvas); //

                npcDialogText.text = currentActiveQuest.info.combackAfterDecline;

                SetAcceptAndDecline();



            } // 퀘스트를 거절 했었을 경우

            if(currentActiveQuest.isAccepted && currentActiveQuest.isCompleted == false) //퀘스트는 수락했지만 아직 완료하지 못한 경우
            {
                if(AreQuestRequirmentsCompleted()) //정말 플레이어가 npc가 요구한 조건들을 만족했는지 bool 메서드를 통해 참,거짓 판별
                {
                    SubmitRequiredItems();

                    DialogSystem.Instance.OpenDialogUI(QuestCanvas);

                    npcDialogText.text = currentActiveQuest.info.combackCompleted; // combackCompleted 는 퀘스트 완료 했다고 알려주는 string

                    AcceptText.text = "[Take Reward]";
                    NpcAcceptBtn.onClick.RemoveAllListeners();
                    NpcAcceptBtn.onClick.AddListener(() =>
                    {
                        ReceiveReward(); //보상 받기.
                        
                    });
                }
                else //퀘스트를 수락했지만, 완료하지 못했을 경우
                {
                    DialogSystem.Instance.OpenDialogUI(QuestCanvas);

                    npcDialogText.text = currentActiveQuest.info.combackInProgress; //combackInProgress는 아직 퀘스트 진행 중이라고 알려주는 텍스트

                    AcceptText.text = "[Close]";
                    NpcAcceptBtn.onClick.RemoveAllListeners();
                    NpcAcceptBtn.onClick.AddListener(() =>
                    {
                        DialogSystem.Instance.CloseDialogUI(QuestCanvas);
                        isTalkingwithPlayer = false;
                        NpcRotation();
                    });
                }
            } // 퀘스트를 받았지만 완료하지 못했을 경우

            if(currentActiveQuest.isCompleted == true) //퀘스트를 완료했을 경우. 즉, 이미 이전에 동일한 퀘스트를 완료했는데, 또 보상 받을려고? ㅋ
            {
                DialogSystem.Instance.OpenDialogUI(QuestCanvas);

                npcDialogText.text = currentActiveQuest.info.finalWords;


                AcceptText.text = "[Close]";
                NpcAcceptBtn.onClick.RemoveAllListeners();
                NpcAcceptBtn.onClick.AddListener(() =>
                {
                    DialogSystem.Instance.CloseDialogUI(QuestCanvas);
                    isTalkingwithPlayer = false;
                    NpcRotation();
                });
            } //이미 이전에 동일한 퀘스트를 완료했을 경우 
            if(currentActiveQuest.isInitialDialogCompleted == false) //처음에 받은 퀘스트를 완료하지 않았을 때
            {
                StartQuestInitialDialog();
            }//이전 퀘스트를 완료 했지만, 여전히 새 퀘스트가 있는 경우.
             //즉, 한 npc에 여러 퀘스트가 있을 경우

        }

    }

    private void SetAcceptAndDecline()
    {
        AcceptText.text = currentActiveQuest.info.acceptOption;
        NpcAcceptBtn.onClick.RemoveAllListeners();
        NpcAcceptBtn.onClick.AddListener(() =>
        {
            AcceptedQuest();
        });

        //거절하기 위해 두번째 버튼을 거절 버튼으로 할 것임.
        NpcDeclineBtn.gameObject.SetActive(true);
        DeclineText.text = currentActiveQuest.info.declineOption;
        NpcDeclineBtn.onClick.RemoveAllListeners();
        NpcDeclineBtn.onClick.AddListener(() =>
        {
            DeclinedQuest();
        });
    }

    private void SubmitRequiredItems() //NPC가 요구했던 아이템들을 플레이어가 제출하는 메서드
    {
        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirmentAmount;

        if(firstRequiredItem != "")
        {
            player.GetComponent<PlayerControllerTest>().RemoveItem(firstRequiredItem, firstRequiredAmount);
        }

        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequirmentAmount = currentActiveQuest.info.secondRequirmentAmount;

        if (secondRequiredItem != "")
        {
            player.GetComponent<PlayerControllerTest>().RemoveItem(secondRequiredItem, secondRequirmentAmount);
        }

        //@@@@@@@ NPC가 요구했던 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 
    }

    private bool AreQuestRequirmentsCompleted() //Npc가 요청한 요구사항들을 플레이어가 갖고 있는지 체크
    {

        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirmentAmount;

        var firstItemCounter = 0; //현재 내가 들고있는 첫번째 요구한 아이템의 개수 0으로 잡고

         
        foreach (var item in player.GetComponent<PlayerControllerTest>().collectedItems)
        {
            if(item.Key == firstRequiredItem)
            {
                firstItemCounter += item.Value; //현재 내가 들고있는 첫번째 요구한 아이템의 개수를 1씩 증가
            }
        }

        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequirmentAmount = currentActiveQuest.info.secondRequirmentAmount;

        var secondItemCounter = 0;

        //@@@@@@@ NPC가 요구했던 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 


        //현재 내가 들고있는 첫번째 또는 두번째 요구한 아이템의 개수가 >= 목표치보다 많을 경우
        if (firstItemCounter >= firstRequiredAmount && secondItemCounter >= secondRequirmentAmount)  
        {          
            return true; 
        }
        else
        {
            return false;
        }
    }

    private void StartQuestInitialDialog() //대화 시작 초기 대화 상자 메서드
    {
        DialogSystem.Instance.OpenDialogUI(QuestCanvas); //1. 퀘스트 ui 열고

        //첫번째 대화 상자 표시
        npcDialogText.text = currentActiveQuest.info.initialDialog[CurrentDialog]; //현재 npc의 대화 text를 초기 대화 리스트 중
                                                                                   //현재 대화 순서(CurrentDialog)에 해당하는 텍스트를 반환
        AcceptText.text = "Next"; //옵션 버튼1의 대화 텍스트를 Next로 변경
        NpcAcceptBtn.onClick.RemoveAllListeners(); //버튼에 기존에 등록된 모든 리스너를 제거한다.
        NpcAcceptBtn.onClick.AddListener(() =>  //새로운 리스너를 등록한다.
        {
            CurrentDialog++; //대화 순서를 1 증가
            CheckIfDialogDone(); //현재 대화가 완료되었는지 확인하는 메서드 호출
        });

        NpcDeclineBtn.gameObject.SetActive(false); //대화를 하고 있는 거절 버튼을 보여주면 안되므로, 잠시 비활성화. 마지막 대화 표시할 때 보여줄 것
    }

    private void CheckIfDialogDone()
    {
        if(CurrentDialog == currentActiveQuest.info.initialDialog.Count - 1) //마지막 대화라면
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[CurrentDialog]; //마지막 대화 표시

            currentActiveQuest.isInitialDialogCompleted = true; //퀘스트 초기 대화 완료.

            //수락하기 위해 첫번째 버튼을 수락 버튼으로 할 것임. 
            SetAcceptAndDecline();

        }
        else
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[CurrentDialog]; //현재 npc의 대화 text를 초기 대화 리스트 중
                                                                                       //현재 대화 순서(CurrentDialog)에 해당하는 텍스트를 반환
            AcceptText.text = "Next"; //옵션 버튼1의 대화 텍스트를 Next로 변경
            NpcAcceptBtn.onClick.RemoveAllListeners(); //버튼에 기존에 등록된 모든 리스너를 제거한다.
            NpcAcceptBtn.onClick.AddListener(() =>  //새로운 리스너를 등록한다.
            {
                CurrentDialog++; //대화 순서를 1 증가
                CheckIfDialogDone(); //현재 대화가 완료되었는지 확인하는 메서드 호출
            });
        }
    }
    private void AcceptedQuest()
    {
        QuestManager.Instance.AddActiveQuest(currentActiveQuest);

        currentActiveQuest.isAccepted = true; //수락 완료
        currentActiveQuest.isDeclined = false; //거절 여부는 false

        if(currentActiveQuest.isHasNoRequirements) //퀘스트에 보상있는 퀘스트라면 
        {
            npcDialogText.text = currentActiveQuest.info.combackCompleted;
            AcceptText.text = "[Take Reward]";
            NpcAcceptBtn.onClick.RemoveAllListeners();
            NpcAcceptBtn.onClick.AddListener(() =>
            {
                ReceiveReward();
                NpcRotation();
            });
            NpcDeclineBtn.gameObject.SetActive(false); //거절 버튼 비활성화.
        }
        else ////퀘스트에 보상이 없는 퀘스트 라면 
        {
            npcDialogText.text = currentActiveQuest.info.acceptAnswer;
            CloseDialogUI();
        }
    }

    private void CloseDialogUI()
    {
        AcceptText.text = "[Close]";
        NpcAcceptBtn.onClick.RemoveAllListeners();
        NpcAcceptBtn.onClick.AddListener(() =>
        {
            DialogSystem.Instance.CloseDialogUI(QuestCanvas); //대화 UI 비활성화 메서드 호출.
            isTalkingwithPlayer = false; //플레이어와 대화 종료.
            NpcRotation();
            //여기서 NPC가 플레이어를 바라봤었기 때문에 원래 각도로 회전해야함. 
        });
        NpcDeclineBtn.gameObject.SetActive(false); //거절 버튼 비활성화.
    }

    private void ReceiveReward()
    {
        QuestManager.Instance.MarkQuestCompleted(currentActiveQuest);

        currentActiveQuest.isCompleted = true; //퀘스트 완료

        var coinRecived = currentActiveQuest.info.coinReward;
        Debug.Log("보상의 개수는" + coinRecived + "이다.");

        if(currentActiveQuest.info.rewardItem1 != "")
        {
            player.GetComponent<PlayerControllerTest>().CollectItem(currentActiveQuest.info.rewardItem1);
        }

        if (currentActiveQuest.info.rewardItem2 != "")
        {
            player.GetComponent<PlayerControllerTest>().CollectItem(currentActiveQuest.info.rewardItem2);
        }

        //@@@@@@@ NPC가 보상해줄 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 




        //ActiveQuestIndex++; //NPC_A에게 전달 받았던 퀘스트를 완료하고, NPC_A의 두번째 퀘스트를 시작하려고 인덱스 증가.

        //NPC_A의 두번째 퀘스트 시작.
        if(ActiveQuestIndex <= quests.Count)
        {
            currentActiveQuest = quests[ActiveQuestIndex]; //이전과는 다른 퀘스트 이고, 위에서 ActiveQuestIndex을 1증가 시켰기 때문에 
            //퀘스트도 다를 것임. 따라서 다른 퀘스트를 가져와서 현재 활성화된 퀘스트에 저장

            CurrentDialog = 0; //CurrentDialog을 0으로 해서 대화상자를 재 설정
            DialogSystem.Instance.CloseDialogUI(QuestCanvas); //대화 UI 비활성화
            isTalkingwithPlayer = false;
            NpcRotation();
        }
        else
        {
            DialogSystem.Instance.CloseDialogUI(QuestCanvas);
            isTalkingwithPlayer = false;

            Debug.Log(this.gameObject.name + "의 퀘스트는 더 이상 없다.");
        }
    }

    private void DeclinedQuest()
    {
        currentActiveQuest.isDeclined = true;

        npcDialogText.text = currentActiveQuest.info.declineAnswer;
        CloseDialogUI();

    }

    void NpcRotation()
    {

        StartCoroutine(LookAtPlayerRoutine(initRot));
    }

    private IEnumerator LookAtPlayerRoutine(Quaternion targetRotation)
    {

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f) // 목표 회전 값을 기준으로 각도를 계산, 1도 이상일 때 회전시키기.
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null; 
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

}
