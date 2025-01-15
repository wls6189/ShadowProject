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
                GameObject go = new GameObject("DialogSystem"); //EventBus라는 빈 객체를 만들고
                instance = go.AddComponent<DialogSystem>(); //EventBus 빈 객체에 EventBus 스크립트(컴포넌트)을 추가
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

    public Canvas QuestNpcCanvas; //다이얼로그 UI

    public bool isdialogueCanvas; //다이얼로그 UI가 열려있는지 여부

    [Header("# Main Story NPC")]
    public TextMeshProUGUI StoryDialogText;

    public Button optBtn1;
    public Button optBtn2;
    public Button FinalBtn;

    public Canvas StoryNpcCanvas; //다이얼로그 UI

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else // 1. go.AddComponent<EventBus>(); -> 2. Awake 실행이므로 evetbus가 아직 null이다. 그래서 eventbus = this를 해준다. 
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
        Cursor.lockState = CursorLockMode.None; //마우스 이동 xxx
        Cursor.visible = true;
    }
    void MouseMoveStart()
    {

        Cursor.lockState = CursorLockMode.Locked; //마우스 이동 xxx
        Cursor.visible = false;
    }
}
