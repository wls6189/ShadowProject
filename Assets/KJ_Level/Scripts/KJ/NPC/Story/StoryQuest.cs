using UnityEngine;
[System.Serializable] //����ȭ -> ������ ���� ����
public class StoryQuest
{
    [Header("Bools")]
    public bool isFirstCompleted;
    public bool isSecondCompleted;
    [Header("StoryQuest Info")]
    public StoryQuestInfo storyinfo; //����Ʈ�� ���� ���� ������ ��� �ִ� ��ü.
}
