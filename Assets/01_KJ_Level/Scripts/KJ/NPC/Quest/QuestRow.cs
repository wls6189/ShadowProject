using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRow : MonoBehaviour
{

    public TextMeshProUGUI quesetName; //����Ʈ �̸�
    public TextMeshProUGUI questGiver; //����Ʈ ������(npc)

    public Button trackingButton;  //��ư

    public bool isActive;
    public bool isTracking;

    public TextMeshProUGUI coinAmount;  //������ ��

    public Image firstReward; //ù��° ���� �̹���
    public TextMeshProUGUI firstRewardAmount; //ù��° ������ ��

    public Image secondReward; //�ι�° ���� �̹���
    public TextMeshProUGUI secondRewardAmount; //�ι�° ������ ��


}
