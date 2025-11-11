using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private List<IQuest> _activeObjectives = new List<IQuest>();

    void Start()
    {
        AddQuestObjective(new KillObjective("Wolf", 5));
    }


    public void AddQuestObjective(IQuest objective)
    {
        _activeObjectives.Add(objective);
        objective.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    public void SubscribeToEnemyDeath(Idestoryable enemy)
    {
        enemy.OnDestory += HandleEnemyDied;
    }

    private void HandleEnemyDied(Idestoryable enemy)
    {
        string enemyType = enemy.GetType().Name;
        foreach (var obj in _activeObjectives)
        {
            if (obj is KillObjective killObj && killObj.IsComplete == false)
            {
                killObj.UpdateProgress(1);
                Debug.Log($"Quest Progress: {obj.GetProgressText()}");
            }
        }
    }

    private void HandleObjectiveCompleted()
    {
        Debug.Log("An objective has been successfully completed!");
    }
}
