using UnityEngine;

[System.Serializable] //직렬화 -> 에디터 편집 가능
public class Quest //정보를 포함하는 클래스
{
    public string questName;
    public string questGiver;

    [Header("Bools")]
    public bool isAccepted; //퀘스트 수락 여부
    public bool isDeclined; //퀘스트 거부 여부
    public bool isInitialDialogCompleted; //퀘스트 초기 대화 완료 여부
    public bool isCompleted; //퀘스트 완료 여부

    public bool isHasNoRequirements; //퀘스트가 수행 조건 없이 바로 시작 가능한지 여부

    [Header("Quest Info")]
    public QuestInfo info; //퀘스트에 대한 세부 정보를 담고 있는 객체.

}
