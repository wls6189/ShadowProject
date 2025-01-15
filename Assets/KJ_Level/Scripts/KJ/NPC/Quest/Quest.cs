using UnityEngine;

[System.Serializable] //����ȭ -> ������ ���� ����
public class Quest //������ �����ϴ� Ŭ����
{
    public string questName;
    public string questGiver;

    [Header("Bools")]
    public bool isAccepted; //����Ʈ ���� ����
    public bool isDeclined; //����Ʈ �ź� ����
    public bool isInitialDialogCompleted; //����Ʈ �ʱ� ��ȭ �Ϸ� ����
    public bool isCompleted; //����Ʈ �Ϸ� ����

    public bool isHasNoRequirements; //����Ʈ�� ���� ���� ���� �ٷ� ���� �������� ����

    [Header("Quest Info")]
    public QuestInfo info; //����Ʈ�� ���� ���� ������ ��� �ִ� ��ü.

}
