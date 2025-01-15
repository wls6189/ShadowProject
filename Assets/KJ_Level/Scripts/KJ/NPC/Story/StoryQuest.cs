using UnityEngine;
[System.Serializable] //직렬화 -> 에디터 편집 가능
public class StoryQuest
{
    [Header("Bools")]
    public bool isFirstCompleted;
    public bool isSecondCompleted;
    [Header("StoryQuest Info")]
    public StoryQuestInfo storyinfo; //퀘스트에 대한 세부 정보를 담고 있는 객체.
}
