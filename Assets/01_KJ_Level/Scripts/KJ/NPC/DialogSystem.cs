using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{

    private static DialogSystem instance;
    public static DialogSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DialogSystem"); //EventBus��� �� ��ü�� �����
                instance = go.AddComponent<DialogSystem>(); //EventBus �� ��ü�� EventBus ��ũ��Ʈ(������Ʈ)�� �߰�
                Debug.Log("instance");
                DontDestroyOnLoad(go);
            }
            return instance;
        }



    }

    public bool isdialogueCanvas; //���̾�α� UI�� �����ִ��� ����

    [Header("# NPC UI")]
    public TextMeshProUGUI StoryDialogText;

    public TextMeshProUGUI NpcGiverText;

    public Button optBtn1;
    public Button optBtn2;
    public Button BackBtn;

    public Canvas DialougeCanvas; //���̾�α� UI


    public Button FirstBtn;
    public Button SecondBtn;
    public Button ReceiveBtn;


    public GameObject TalkDialouge;
    public GameObject QuestDialouge;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else // 1. go.AddComponent<EventBus>(); -> 2. Awake �����̹Ƿ� evetbus�� ���� null�̴�. �׷��� eventbus = this�� ���ش�. 
        {
            instance = this;
        }
    }

    public void OpenDialogUI()
    {
        DialougeCanvas.gameObject.SetActive(true);
        isdialogueCanvas = true;

        MouseMoveStop();
    }

    public void CloseDialogUI()
    {
        DialougeCanvas.gameObject.SetActive(false);
        isdialogueCanvas = false;

        MouseMoveStart();
    }

    public void MouseMoveStop()
    {
        Cursor.lockState = CursorLockMode.None; //���콺 �����ֱ�
        Cursor.visible = true;
    }
    public void MouseMoveStart()
    {
        Cursor.lockState = CursorLockMode.Locked; //���콺 �������� �ʱ�
        Cursor.visible = false;
    }
}
