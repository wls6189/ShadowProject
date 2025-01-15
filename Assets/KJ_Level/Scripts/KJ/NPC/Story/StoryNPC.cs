using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryNPC : MonoBehaviour
{
    [SerializeField]
    private bool playerInRange; // 대화 시작 범위 안에 플레이어가 있는 경우에 대한 여부
    [SerializeField]
    GameObject player;

    [SerializeField]
    float rotationSpeed;

    [SerializeField]
    private bool isTalkingwithPlayer;

    Quaternion initRot;

    [Header("Story 관련 UI")]
    TextMeshProUGUI npcDialogText;

    Button NpcFirstBtn;
    TextMeshProUGUI NpcFirstText;

    Button NpcSecondBtn;
    TextMeshProUGUI NpcSecondText;

    Button NpcThirdBtn;
    TextMeshProUGUI NpcThirdText;

    Canvas StoryCanvas;

    [SerializeField]
    GameObject NpcTalkImage;


    [Header("퀘스트 대화 내용 객체")]
    public List<StoryQuest> quests; // NPC가 가지고 있는 퀘스트 목록 관리
    public StoryQuest currentActiveQuest;
    public int ActiveQuestIndex = 0;
    public bool isFirstTimeInteraction = true;
    public int CurrentFirstDialog = 0;
    public int CurrentSecondDialog = 0;

    void Start()
    {
        StoryCanvas = DialogSystem.Instance.StoryNpcCanvas;

        npcDialogText = DialogSystem.Instance.StoryDialogText;

        NpcFirstBtn = DialogSystem.Instance.optBtn1;
        NpcFirstText = DialogSystem.Instance.optBtn1.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        NpcSecondBtn = DialogSystem.Instance.optBtn2;
        NpcSecondText = DialogSystem.Instance.optBtn2.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        NpcThirdBtn = DialogSystem.Instance.FinalBtn;
        NpcThirdText = DialogSystem.Instance.FinalBtn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        initRot = transform.rotation;
    }

    void Update()
    {
        if (playerInRange)
        {
            NpcTalkImage.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F) && !isTalkingwithPlayer)
            {
                NpcTalkImage.GetComponentInChildren<TextMeshProUGUI>().text = "...";
                StartTalk();
            }
        }
        else
        {
            NpcTalkImage.gameObject.SetActive(false);
        }
    }

    private void StartTalk()
    {
        isTalkingwithPlayer = true;

        Vector3 dir = transform.position - player.transform.position;
        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        StartCoroutine(LookAtPlayerRoutine(targetRotation));

        if (isFirstTimeInteraction)
        {
            isFirstTimeInteraction = false;

            currentActiveQuest = quests[ActiveQuestIndex]; 
        }

        UpdateDialogUI(); // 대화 UI 초기화 및 시작
    }

    private void UpdateDialogUI()
    {
        DialogSystem.Instance.OpenDialogUI(StoryCanvas);

        NpcFirstBtn.gameObject.SetActive(true);
        NpcSecondBtn.gameObject.SetActive(true);

        CurrentFirstDialog = 0;
        CurrentSecondDialog = 0;

        // 대화 텍스트 업데이트
        npcDialogText.text = currentActiveQuest.storyinfo.initialDialog;

        NpcFirstText.text = currentActiveQuest.storyinfo.firstAnswer[CurrentFirstDialog];
        NpcFirstBtn.onClick.RemoveAllListeners();
        NpcFirstBtn.onClick.AddListener(() =>
        {
            npcDialogText.text = currentActiveQuest.storyinfo.firstDialog[CurrentFirstDialog]; //마지막 대화 표시
            NpcSecondBtn.gameObject.SetActive(false);
            AdvanceFirstDialog();
        });

        NpcSecondText.text = currentActiveQuest.storyinfo.secondAnswer[CurrentSecondDialog];
        NpcSecondBtn.onClick.RemoveAllListeners();
        NpcSecondBtn.onClick.AddListener(() =>
        {
            npcDialogText.text = currentActiveQuest.storyinfo.secondDialog[CurrentSecondDialog]; //마지막 대화 표시
            NpcFirstBtn.gameObject.SetActive(false);
            AdvanceSecondDialog();
        });
    }

    private void AdvanceFirstDialog()
    {
        if (CurrentFirstDialog == currentActiveQuest.storyinfo.firstDialog.Count - 1) //마지막 대화라면
        {
            currentActiveQuest.isFirstCompleted = true; //퀘스트 초기 대화 완료.
            NpcFirstBtn.gameObject.SetActive(false);
            NpcSecondBtn.gameObject.SetActive(true);
            //수락하기 위해 첫번째 버튼을 수락 버튼으로 할 것임. 
            Debug.Log("첫번째 버튼 마지막 대화");
            CheckIfDialogDone();

        }
        else
        {
            CurrentFirstDialog++;

            NpcFirstText.text = currentActiveQuest.storyinfo.firstAnswer[CurrentFirstDialog];
            NpcFirstBtn.onClick.RemoveAllListeners();
            NpcFirstBtn.onClick.AddListener(() =>
            {
                npcDialogText.text = currentActiveQuest.storyinfo.firstDialog[CurrentFirstDialog]; //마지막 대화 표시

                AdvanceFirstDialog();
            });
        }
    }

    private void AdvanceSecondDialog()
    {
        if (CurrentSecondDialog == currentActiveQuest.storyinfo.secondDialog.Count - 1) //마지막 대화라면
        {
            currentActiveQuest.isSecondCompleted = true; //퀘스트 초기 대화 완료.
            NpcSecondBtn.gameObject.SetActive(false);
            NpcFirstBtn.gameObject.SetActive(true);
            //수락하기 위해 첫번째 버튼을 수락 버튼으로 할 것임. 
            CheckIfDialogDone();

        }
        else
        {
            CurrentSecondDialog++;

            NpcSecondText.text = currentActiveQuest.storyinfo.secondAnswer[CurrentSecondDialog];
            NpcSecondBtn.onClick.RemoveAllListeners();
            NpcSecondBtn.onClick.AddListener(() =>
            {
                npcDialogText.text = currentActiveQuest.storyinfo.secondDialog[CurrentSecondDialog]; //마지막 대화 표시

                AdvanceSecondDialog();
            });
        }


    }


    private void CheckIfDialogDone()
    {
        if (currentActiveQuest.isFirstCompleted && currentActiveQuest.isSecondCompleted)
        {
            Debug.Log("대화 끝");
            npcDialogText.text = currentActiveQuest.storyinfo.finalDialog; // 최종 대화 표시
            NpcSecondBtn.gameObject.SetActive(false);
            NpcFirstBtn.gameObject.SetActive(false);

            NpcThirdBtn.gameObject.SetActive(true); // 세 번째 버튼 활성화

            // 세 번째 버튼 리스너 설정
            NpcThirdText.text = currentActiveQuest.storyinfo.finalAnswer;

            NpcThirdBtn.onClick.RemoveAllListeners();
            NpcThirdBtn.onClick.AddListener(() =>
            {
                currentActiveQuest.isFirstCompleted = false;
                currentActiveQuest.isSecondCompleted = false;
                NpcThirdBtn.gameObject.SetActive(false);
                isTalkingwithPlayer = false;
                DialogSystem.Instance.CloseDialogUI(StoryCanvas);

                NpcRotation();
            });
        }
    }

    void NpcRotation()
    {

        StartCoroutine(LookAtPlayerRoutine(initRot));
    }

    private IEnumerator LookAtPlayerRoutine(Quaternion targetRotation)
    {


        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
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
