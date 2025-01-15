using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryQuestInfo", order = 1)]
public class StoryQuestInfo : ScriptableObject
{
    [TextArea(5, 10)]
    public string initialDialog; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문

    [Header("Options")]
    [TextArea(5, 10)]
    public List<string> firstDialog;
    [TextArea(5, 10)]
    public List<string> firstAnswer; // 플레이어가 길을 물어본다 or 여기가 어디냐고 물어봤을 때 길을 물어본다의 대답. 
    [TextArea(5, 10)]
    public List<string> secondDialog;
    [TextArea(5, 10)]
    public List<string> secondAnswer; // 플레이어가 길을 물어본다 or 여기가 어디냐고 물어봤을 때 길을 물어본다의 대답. 

    public string finalDialog;
    public string finalAnswer;

}
