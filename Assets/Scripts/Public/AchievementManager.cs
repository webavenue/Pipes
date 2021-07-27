using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudOnce;
using CloudOnce.Internal;
using CloudOnce.CloudPrefs;

public class AchievementManager : MonoBehaviour
{
    private static readonly int[] CompleteLevels_Marks = { 1, 5, 10, 50, 100, 200, 500, 1000 };
    private static readonly int[] CompleteDailyQuests_Marks = { 1, 3, 21, 90 };
    private static readonly int[] PlayConsecutiveDays_Marks = { 3, 7, 30 };
    //First Hint
    //First InfiniteMove

    public static void CheckAchievements_LevelCompleted(int index)
    {
        for (int i = 0; i < CompleteLevels_Marks.Length; i++)
        {
            if(index == CompleteLevels_Marks[i])
            {
                UnifiedAchievement theAchievement = GetAchievement_LevelCompleted(CompleteLevels_Marks[i]);
                if (theAchievement != null)
                {
                    theAchievement.Unlock();
                    break;
                }
            }
        }
    }

    public static void CheckAchievements_QuestCompleted()
    {
        Manager.instance.numberOfCompletedQuests++;
        for (int i = 0; i < CompleteDailyQuests_Marks.Length; i++)
        {
            if(Manager.instance.numberOfCompletedQuests == CompleteDailyQuests_Marks[i])
            {
                UnifiedAchievement theAchievement = GetAchievement_QuestCompleted(CompleteDailyQuests_Marks[i]);
                if (theAchievement != null)
                {
                    theAchievement.Unlock();
                    break;
                }
            }
        }
    }

    public static void CheckAchievements_PlayedForXdays()
    {
        DateTime now = DateTime.Now;
        if((now - Manager.instance.startOfConsecutiveDays).Days == Manager.instance.numberOfConsecutiveDays)
        {
            Manager.instance.numberOfConsecutiveDays++;
        }
        else
        {
            Manager.instance.startOfConsecutiveDays = now;
            Manager.instance.numberOfConsecutiveDays = 1;
        }
        for (int i = 0; i < PlayConsecutiveDays_Marks.Length; i++)
        {
            if(Manager.instance.numberOfConsecutiveDays == PlayConsecutiveDays_Marks[i])
            {
                UnifiedAchievement theAchievement = GetAchievement_PlayedForXdays(PlayConsecutiveDays_Marks[i]);
                if (theAchievement != null)
                {
                    theAchievement.Unlock();
                    break;
                }
            }
        }
    }

    public static void UnlockAchievement_FirstHint()
    {
        if (!Manager.instance.firstHintUsed)
        {
            Manager.instance.firstHintUsed = true;
            UnifiedAchievement theAchievement = Achievements.firstHint;
            if (theAchievement != null)
            {
                theAchievement.Unlock();
            }
        }
    }

    public static void UnlockAchievement_FirstInfiniteMoves()
    {
        if (!Manager.instance.firstIMUsed)
        {
            Manager.instance.firstIMUsed = true;
            UnifiedAchievement theAchievement = Achievements.firstInfiniteMoves;
            if (theAchievement != null)
            {
                theAchievement.Unlock();
            }
        }
    }

    private static UnifiedAchievement GetAchievement_LevelCompleted(int markNumber)
    {
        switch (markNumber)
        {
            case 1:
                return Achievements.level1;
            case 5:
                return Achievements.level5 ;
            case 10:
                return Achievements.level10;
            case 50:
                return Achievements.level50;
            case 100:
                return Achievements.level100;
            case 200:
                return Achievements.level200;
            case 500:
                return Achievements.level500;
            case 1000:
                return Achievements.level1000;
            default:
                return null;
        }
    }

    private static UnifiedAchievement GetAchievement_QuestCompleted(int numberOfCompletedQuests)
    {
        switch (numberOfCompletedQuests)
        {
            case 1:
                return Achievements.daily1;
            case 3:
                return Achievements.daily3 ;
            case 21:
                return Achievements.daily21;
            case 90:
                return Achievements.daily90;
            default:
                return null;
        }
    }

    private static UnifiedAchievement GetAchievement_PlayedForXdays(int x)
    {
        switch (x)
        {
            case 3:
                return Achievements.play3;
            case 21:
                return Achievements.play7;
            case 30:
                return Achievements.play30;
            default:
                return null;
        }
    }
}
