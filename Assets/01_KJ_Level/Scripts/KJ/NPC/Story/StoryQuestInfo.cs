using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryQuestInfo", order = 1)]
public class StoryQuestInfo : ScriptableObject
{
    [TextArea(5, 10)]
    public string initialDialog; //�ʱ� ��ȭ���ڰ� �� ���ڿ� ����Ʈ -> ���� ���� �ѹ��� ��ü ��ȭ ���ڸ� ǥ���ϰ� ���� �ʱ� ����

    [Header("Options")]
    [TextArea(5, 10)]
    public List<string> firstDialog;
    [TextArea(5, 10)]
    public List<string> firstAnswer; // �÷��̾ ���� ����� or ���Ⱑ ���İ� ������� �� ���� ������� ���. 
    [TextArea(5, 10)]
    public List<string> secondDialog;
    [TextArea(5, 10)]
    public List<string> secondAnswer; // �÷��̾ ���� ����� or ���Ⱑ ���İ� ������� �� ���� ������� ���. 

    public string finalDialog;
    public string finalAnswer;

}
