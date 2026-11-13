
using UnityEngine;

// SAHİBİ: Nagihan — bütün çöp türlerinin listesi, nadirliğe göre rastgele seçim
[CreateAssetMenu(menuName = "Robot Scrap/Trash Table", fileName = "TT_New")]
public class TrashTable : ScriptableObject
{
    [SerializeField] private TrashData[] entries;
    [SerializeField] private int rareValueThreshold = 6;

    // Eski: bonussuz (sözleşme, silinmedi)
    public TrashData PickRandom()
    {
        return PickRandom(0f);
    }

    // Yeni: değerli çöplerin şansı (1 + rareBoost) katına çıkar
    public TrashData PickRandom(float rareBoost)
    {
        if (entries == null || entries.Length == 0) return null;

        float total = 0f;
        foreach (TrashData data in entries)
        {
            if (data != null) total += WeightOf(data, rareBoost);
        }
        if (total <= 0f) return null;

        float roll = Random.Range(0f, total);

        float cumulative = 0f;
        foreach (TrashData data in entries)
        {
            if (data == null) continue;
            cumulative += WeightOf(data, rareBoost);
            if (roll < cumulative) return data;
        }

        for (int i = entries.Length - 1; i >= 0; i--)
        {
            if (entries[i] != null) return entries[i];
        }
        return null;
    }

    private float WeightOf(TrashData data, float rareBoost)
    {
        float weight = data.GetChance();
        if (data.GetValue() >= rareValueThreshold)
        {
            weight *= 1f + rareBoost;
        }
        return weight;
    }

    public int GetCount()
    {
        if (entries == null) return 0;
        return entries.Length;
    }
}