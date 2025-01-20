using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UIManager"); //EventBus��� �� ��ü�� �����
                instance = go.AddComponent<UIManager>(); //EventBus �� ��ü�� EventBus ��ũ��Ʈ(������Ʈ)�� �߰�
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public bool IsGameMenuOpen;

    public Button[] TabButtons;
    private bool isChoose = false;

    EventSystem system;
    public Selectable firstInput;
    private Button lastSelectedButton; // ���������� ���õ� ��ư
    void Start()
    {
        system = EventSystem.current;

        if (TabButtons != null && TabButtons.Length > 0)
        {
            TabButtons[0].Select(); // ù ��° ��ư ����
            lastSelectedButton = TabButtons[0];
            ColorChange(lastSelectedButton); // �ʱ� ���� ����
        }

    }

 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NavigateButtons(true); 
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E");
            NavigateButtons(false); 
        }
    }
    private void NavigateButtons(bool isShiftPressed)
    {
        // ���� ���õ� Selectable ��������
        Selectable current = system.currentSelectedGameObject?.GetComponent<Selectable>();
      
        if (current == null && lastSelectedButton != null) //���콺�� �� ���� ���ý� current�� null�̹Ƿ�, null�� ���
        {
            lastSelectedButton.Select(); //������ ��ư ����
            current = lastSelectedButton; //���������� ��ư�� current�� ����.
        }

        if (current != null)
        {
            Selectable next = isShiftPressed ? current.FindSelectableOnLeft() : current.FindSelectableOnRight();  //true/false�� ���� ���� �̵��� �������̵�.

            if (next != null)
            {
                Debug.Log("2");
                next.Select();

                Button nextButton = next.GetComponent<Button>(); //��
                if (nextButton != null && nextButton != lastSelectedButton)
                {
                    Debug.Log("3");
                    nextButton.onClick.Invoke();
                    lastSelectedButton = nextButton; // ������ ���õ� ��ư ������Ʈ
                }
            }
        }
    }

    [SerializeField]
    GameObject GameMenu;
    public void GameMenuOpen()
    {
        GameMenu.gameObject.SetActive(true);
        IsGameMenuOpen = true;
        DialogSystem.Instance.MouseMoveStop();
    }

    public void GameMenuClose()
    {
        GameMenu.gameObject.SetActive(false);
        IsGameMenuOpen = false;
        DialogSystem.Instance.MouseMoveStart(); // �޴��� �� �� ���콺 �̵� ����
    }
    public void ColorChange(Button clickedButton)
    {
 
        foreach(Button button in TabButtons)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (button == clickedButton)
            {
                // Ŭ���� ��ư�� �Ķ���
                buttonImage.color = Color.blue;
            }
            else
            {
                // ������ ��ư�� ������
                buttonImage.color = Color.black;
            }
        }
    }
}
