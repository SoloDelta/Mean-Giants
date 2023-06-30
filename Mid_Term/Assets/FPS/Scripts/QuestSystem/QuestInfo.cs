using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QuestInfo", menuName = "ScriptableObject/QuestInfo", order = 1)]
public class QuestInfo : ScriptableObject
{
    [field: SerializeField]public string questId { get; private set; }

    [Header("---- Quest Details ----")]
    public string questName;

    [Header("---- Requirements ----")]
    public QuestInfo[] questPrequisites;

    [Header("---- Steps ----")]
    public GameObject[] questStepsPrefab;

    [Header("---- Rewards ----")]
    public int currencyReward;

    // makes sure that the questId is the same as the scriptable object
    private void OnValidate()
    {
    #if UNITY_EDITOR
        questId = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
    #endif
    }
}
