using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data",menuName = "ScriptableObjects/QuestInfo",order = 1)]
public class QuestInfo : ScriptableObject
{
    [TextArea(5,10)]
    public List<string> initialDialog; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문

    [Header("Options")]
    [TextArea(5, 10)]
    public string acceptOption; //퀘스트를 수락하는 옵션. ex) 네 저는 당신(npc)의 부탁을 들어드릴게요. 
    [TextArea(5, 10)]
    public string acceptAnswer; //퀘스트를 수락할 때 알려주는 수락 답변. ex)그래요? 감사해요. 
    [TextArea(5, 10)]
    public string declineOption; //퀘스트를 거절하는 옵션, ex) 아니요 저는 당신(npc)의 부탁을 들어드리기 불편해요. 
    [TextArea(5, 10)]
    public string declineAnswer; //퀘스트를 거절할 때 알려주는 거절 답변. ex) 알겠어요. 다시 생각해봐요
    [TextArea(5, 10)]
    public string combackAfterDecline; //퀘스트를 거절한 후 다시 npc에게 돌아갔을 때
    [TextArea(5, 10)]
    public string combackInProgress; //퀘스트가 완료되지 않았는데(진행 중인데) npc에게 돌아갔다면 , npc는 아직 몇가지의 조건이 더 필요하다
    [TextArea(5, 10)]
    public string combackCompleted;
    [TextArea(5, 10)]
    public string finalWords;

    [Header("Rewards")] //보상, 여기에 더 많은 것을 추가할 수 있다.
    public int coinReward;
    public string rewardItem1;
    public string rewardItem2;

    [Header("Requirments")] //요구사항, 더 많은 것을 추가할 수 있다.
    public string firstRequirmentItem; //첫번째 요구사항, ex ) 돌이 필요함
    public int firstRequirmentAmount; //  ex) 돌이 5개 정도 필요함. 

    public string secondRequirmentItem; //두번째 요구사항, ex) 막대기도 필요함
    public int secondRequirmentAmount; // ex)막대기 2개 정도 필요함

    // => 즉, 돌과 막대기를 요구했고, 돌 5개와 막대기 2개가 필요하다는 말.
}