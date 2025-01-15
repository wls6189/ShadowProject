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

    [Header("# Sub Quest NPC")]
    public TextMeshProUGUI dialogText;

    public Button optionBtn1;
    public Button optionBtn2;

    public Canvas QuestNpcCanvas; //���̾�α� UI

    public bool isdialogueCanvas; //���̾�α� UI�� �����ִ��� ����

    [Header("# Main Story NPC")]
    public TextMeshProUGUI StoryDialogText;

    public Button optBtn1;
    public Button optBtn2;
    public Button FinalBtn;

    public Canvas StoryNpcCanvas; //���̾�α� UI

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

    public void OpenDialogUI(Canvas Canvas)
    {
        Canvas.gameObject.SetActive(true);
        isdialogueCanvas = true;

        MouseMoveStop();
    }

    public void CloseDialogUI(Canvas Canvas)
    {
        Canvas.gameObject.SetActive(false);
        isdialogueCanvas = false;

        MouseMoveStart();
    }

    void MouseMoveStop()
    {
        Cursor.lockState = CursorLockMode.None; //���콺 �̵� xxx
        Cursor.visible = true;
    }
    void MouseMoveStart()
    {

        Cursor.lockState = CursorLockMode.Locked; //���콺 �̵� xxx
        Cursor.visible = false;
    }
}
