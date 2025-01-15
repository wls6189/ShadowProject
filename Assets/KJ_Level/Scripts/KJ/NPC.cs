using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour 
{
    // 1. NPC ��ó�� ���� ��쿡�� ��ȭ�� �� �� �ִ� ���.

    [SerializeField]
    private bool playerInRange; //��ȭ ���� �����ȿ� �÷��̾ �ִ� ��쿡 ���� ����
    [SerializeField]
    GameObject player;

    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    private bool isTalkingwithPlayer;
    [SerializeField]
    GameObject NpcTalkImage;

    Quaternion initRot;

    [Header("����Ʈ ���� UI")]
    TextMeshProUGUI npcDialogText;

    Button NpcAcceptBtn;
    TextMeshProUGUI AcceptText;

    Button NpcDeclineBtn;
    TextMeshProUGUI DeclineText;

    Canvas QuestCanvas;

    [Header("����Ʈ ��ȭ ���� ��ü")]
    public List<Quest> quests; //NPC�� ������ �ִ� ����Ʈ ����� �����ϴ� List
    public Quest currentActiveQuest; //���� Ȱ��ȭ�� ����Ʈ�� ��Ÿ��( ����Ʈ���� Ư�� ����Ʈ�� ����)
    public int ActiveQuestIndex = 0; //���� Ȱ��ȭ�� ����Ʈ�� �ε���, �ʱⰪ�� 0���� �����Ǿ� ù ��° ����Ʈ���� �����Ѵ�. 
    public bool isFirstTimeInteraction = true; //�÷��̾�� NPC�� ù ��ȣ�ۿ� ����, 
    public int CurrentDialog; //��ȭ ���� ������ int�� ����.


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
            //�ؽ�Ʈ ����
            NpcTalkImage.gameObject.SetActive(true); // ĵ���� ��Ȱ��ȭ -> Ȱ��ȭ or ĵ���� Ȱ��ȭ �� ���¿��� �ؽ�Ʈ�� ������ ��

            if(Input.GetKey(KeyCode.F) && isTalkingwithPlayer == false) // 1. ��ȭ ���� ��
            {
                //2. ��ȭ ����
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

        Debug.Log("��ȭ ����");

        if(isFirstTimeInteraction) //��ȭ ù ���� �� ���
        {
            isFirstTimeInteraction = false; 
            currentActiveQuest = quests[ActiveQuestIndex]; //quests ����Ʈ�� ����� ����Ʈ �� ActiveQuestIndex�� �ش��ϴ�
                                                           //����Ʈ�� currentActiveQuest ������ ����.
            StartQuestInitialDialog();
            CurrentDialog = 0; //���� ��ȭ�� 0���� �ʱ�ȭ.
        }

        else //��ȭ ù ������ �ƴ� ���
        {
            if(currentActiveQuest.isDeclined) //������ ��ȭ ù �������� �� ����Ʈ�� �������� ���̴�. ���� isDeclined�� �翬�� true ���� ����. 
            {
                DialogSystem.Instance.OpenDialogUI(QuestCanvas); //

                npcDialogText.text = currentActiveQuest.info.combackAfterDecline;

                SetAcceptAndDecline();



            } // ����Ʈ�� ���� �߾��� ���

            if(currentActiveQuest.isAccepted && currentActiveQuest.isCompleted == false) //����Ʈ�� ���������� ���� �Ϸ����� ���� ���
            {
                if(AreQuestRequirmentsCompleted()) //���� �÷��̾ npc�� �䱸�� ���ǵ��� �����ߴ��� bool �޼��带 ���� ��,���� �Ǻ�
                {
                    SubmitRequiredItems();

                    DialogSystem.Instance.OpenDialogUI(QuestCanvas);

                    npcDialogText.text = currentActiveQuest.info.combackCompleted; // combackCompleted �� ����Ʈ �Ϸ� �ߴٰ� �˷��ִ� string

                    AcceptText.text = "[Take Reward]";
                    NpcAcceptBtn.onClick.RemoveAllListeners();
                    NpcAcceptBtn.onClick.AddListener(() =>
                    {
                        ReceiveReward(); //���� �ޱ�.
                        
                    });
                }
                else //����Ʈ�� ����������, �Ϸ����� ������ ���
                {
                    DialogSystem.Instance.OpenDialogUI(QuestCanvas);

                    npcDialogText.text = currentActiveQuest.info.combackInProgress; //combackInProgress�� ���� ����Ʈ ���� ���̶�� �˷��ִ� �ؽ�Ʈ

                    AcceptText.text = "[Close]";
                    NpcAcceptBtn.onClick.RemoveAllListeners();
                    NpcAcceptBtn.onClick.AddListener(() =>
                    {
                        DialogSystem.Instance.CloseDialogUI(QuestCanvas);
                        isTalkingwithPlayer = false;
                        NpcRotation();
                    });
                }
            } // ����Ʈ�� �޾����� �Ϸ����� ������ ���

            if(currentActiveQuest.isCompleted == true) //����Ʈ�� �Ϸ����� ���. ��, �̹� ������ ������ ����Ʈ�� �Ϸ��ߴµ�, �� ���� ��������? ��
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
            } //�̹� ������ ������ ����Ʈ�� �Ϸ����� ��� 
            if(currentActiveQuest.isInitialDialogCompleted == false) //ó���� ���� ����Ʈ�� �Ϸ����� �ʾ��� ��
            {
                StartQuestInitialDialog();
            }//���� ����Ʈ�� �Ϸ� ������, ������ �� ����Ʈ�� �ִ� ���.
             //��, �� npc�� ���� ����Ʈ�� ���� ���

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

        //�����ϱ� ���� �ι�° ��ư�� ���� ��ư���� �� ����.
        NpcDeclineBtn.gameObject.SetActive(true);
        DeclineText.text = currentActiveQuest.info.declineOption;
        NpcDeclineBtn.onClick.RemoveAllListeners();
        NpcDeclineBtn.onClick.AddListener(() =>
        {
            DeclinedQuest();
        });
    }

    private void SubmitRequiredItems() //NPC�� �䱸�ߴ� �����۵��� �÷��̾ �����ϴ� �޼���
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

        //@@@@@@@ NPC�� �䱸�ߴ� �͵��� �� ���ٸ� ���� �Ʒ��� �� �߰� �� �� ����@@@@@@@@@ 
    }

    private bool AreQuestRequirmentsCompleted() //Npc�� ��û�� �䱸���׵��� �÷��̾ ���� �ִ��� üũ
    {

        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirmentAmount;

        var firstItemCounter = 0; //���� ���� ����ִ� ù��° �䱸�� �������� ���� 0���� ���

         
        foreach (var item in player.GetComponent<PlayerControllerTest>().collectedItems)
        {
            if(item.Key == firstRequiredItem)
            {
                firstItemCounter += item.Value; //���� ���� ����ִ� ù��° �䱸�� �������� ������ 1�� ����
            }
        }

        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequirmentAmount = currentActiveQuest.info.secondRequirmentAmount;

        var secondItemCounter = 0;

        //@@@@@@@ NPC�� �䱸�ߴ� �͵��� �� ���ٸ� ���� �Ʒ��� �� �߰� �� �� ����@@@@@@@@@ 


        //���� ���� ����ִ� ù��° �Ǵ� �ι�° �䱸�� �������� ������ >= ��ǥġ���� ���� ���
        if (firstItemCounter >= firstRequiredAmount && secondItemCounter >= secondRequirmentAmount)  
        {          
            return true; 
        }
        else
        {
            return false;
        }
    }

    private void StartQuestInitialDialog() //��ȭ ���� �ʱ� ��ȭ ���� �޼���
    {
        DialogSystem.Instance.OpenDialogUI(QuestCanvas); //1. ����Ʈ ui ����

        //ù��° ��ȭ ���� ǥ��
        npcDialogText.text = currentActiveQuest.info.initialDialog[CurrentDialog]; //���� npc�� ��ȭ text�� �ʱ� ��ȭ ����Ʈ ��
                                                                                   //���� ��ȭ ����(CurrentDialog)�� �ش��ϴ� �ؽ�Ʈ�� ��ȯ
        AcceptText.text = "Next"; //�ɼ� ��ư1�� ��ȭ �ؽ�Ʈ�� Next�� ����
        NpcAcceptBtn.onClick.RemoveAllListeners(); //��ư�� ������ ��ϵ� ��� �����ʸ� �����Ѵ�.
        NpcAcceptBtn.onClick.AddListener(() =>  //���ο� �����ʸ� ����Ѵ�.
        {
            CurrentDialog++; //��ȭ ������ 1 ����
            CheckIfDialogDone(); //���� ��ȭ�� �Ϸ�Ǿ����� Ȯ���ϴ� �޼��� ȣ��
        });

        NpcDeclineBtn.gameObject.SetActive(false); //��ȭ�� �ϰ� �ִ� ���� ��ư�� �����ָ� �ȵǹǷ�, ��� ��Ȱ��ȭ. ������ ��ȭ ǥ���� �� ������ ��
    }

    private void CheckIfDialogDone()
    {
        if(CurrentDialog == currentActiveQuest.info.initialDialog.Count - 1) //������ ��ȭ���
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[CurrentDialog]; //������ ��ȭ ǥ��

            currentActiveQuest.isInitialDialogCompleted = true; //����Ʈ �ʱ� ��ȭ �Ϸ�.

            //�����ϱ� ���� ù��° ��ư�� ���� ��ư���� �� ����. 
            SetAcceptAndDecline();

        }
        else
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[CurrentDialog]; //���� npc�� ��ȭ text�� �ʱ� ��ȭ ����Ʈ ��
                                                                                       //���� ��ȭ ����(CurrentDialog)�� �ش��ϴ� �ؽ�Ʈ�� ��ȯ
            AcceptText.text = "Next"; //�ɼ� ��ư1�� ��ȭ �ؽ�Ʈ�� Next�� ����
            NpcAcceptBtn.onClick.RemoveAllListeners(); //��ư�� ������ ��ϵ� ��� �����ʸ� �����Ѵ�.
            NpcAcceptBtn.onClick.AddListener(() =>  //���ο� �����ʸ� ����Ѵ�.
            {
                CurrentDialog++; //��ȭ ������ 1 ����
                CheckIfDialogDone(); //���� ��ȭ�� �Ϸ�Ǿ����� Ȯ���ϴ� �޼��� ȣ��
            });
        }
    }
    private void AcceptedQuest()
    {
        QuestManager.Instance.AddActiveQuest(currentActiveQuest);

        currentActiveQuest.isAccepted = true; //���� �Ϸ�
        currentActiveQuest.isDeclined = false; //���� ���δ� false

        if(currentActiveQuest.isHasNoRequirements) //����Ʈ�� �����ִ� ����Ʈ��� 
        {
            npcDialogText.text = currentActiveQuest.info.combackCompleted;
            AcceptText.text = "[Take Reward]";
            NpcAcceptBtn.onClick.RemoveAllListeners();
            NpcAcceptBtn.onClick.AddListener(() =>
            {
                ReceiveReward();
                NpcRotation();
            });
            NpcDeclineBtn.gameObject.SetActive(false); //���� ��ư ��Ȱ��ȭ.
        }
        else ////����Ʈ�� ������ ���� ����Ʈ ��� 
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
            DialogSystem.Instance.CloseDialogUI(QuestCanvas); //��ȭ UI ��Ȱ��ȭ �޼��� ȣ��.
            isTalkingwithPlayer = false; //�÷��̾�� ��ȭ ����.
            NpcRotation();
            //���⼭ NPC�� �÷��̾ �ٶ�þ��� ������ ���� ������ ȸ���ؾ���. 
        });
        NpcDeclineBtn.gameObject.SetActive(false); //���� ��ư ��Ȱ��ȭ.
    }

    private void ReceiveReward()
    {
        QuestManager.Instance.MarkQuestCompleted(currentActiveQuest);

        currentActiveQuest.isCompleted = true; //����Ʈ �Ϸ�

        var coinRecived = currentActiveQuest.info.coinReward;
        Debug.Log("������ ������" + coinRecived + "�̴�.");

        if(currentActiveQuest.info.rewardItem1 != "")
        {
            player.GetComponent<PlayerControllerTest>().CollectItem(currentActiveQuest.info.rewardItem1);
        }

        if (currentActiveQuest.info.rewardItem2 != "")
        {
            player.GetComponent<PlayerControllerTest>().CollectItem(currentActiveQuest.info.rewardItem2);
        }

        //@@@@@@@ NPC�� �������� �͵��� �� ���ٸ� ���� �Ʒ��� �� �߰� �� �� ����@@@@@@@@@ 




        //ActiveQuestIndex++; //NPC_A���� ���� �޾Ҵ� ����Ʈ�� �Ϸ��ϰ�, NPC_A�� �ι�° ����Ʈ�� �����Ϸ��� �ε��� ����.

        //NPC_A�� �ι�° ����Ʈ ����.
        if(ActiveQuestIndex <= quests.Count)
        {
            currentActiveQuest = quests[ActiveQuestIndex]; //�������� �ٸ� ����Ʈ �̰�, ������ ActiveQuestIndex�� 1���� ���ױ� ������ 
            //����Ʈ�� �ٸ� ����. ���� �ٸ� ����Ʈ�� �����ͼ� ���� Ȱ��ȭ�� ����Ʈ�� ����

            CurrentDialog = 0; //CurrentDialog�� 0���� �ؼ� ��ȭ���ڸ� �� ����
            DialogSystem.Instance.CloseDialogUI(QuestCanvas); //��ȭ UI ��Ȱ��ȭ
            isTalkingwithPlayer = false;
            NpcRotation();
        }
        else
        {
            DialogSystem.Instance.CloseDialogUI(QuestCanvas);
            isTalkingwithPlayer = false;

            Debug.Log(this.gameObject.name + "�� ����Ʈ�� �� �̻� ����.");
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

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f) // ��ǥ ȸ�� ���� �������� ������ ���, 1�� �̻��� �� ȸ����Ű��.
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
