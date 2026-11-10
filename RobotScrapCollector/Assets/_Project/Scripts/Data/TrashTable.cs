
using UnityEngine;

// SAHİBİ: Nagihan — bütün çöp türlerinin listesi, nadirliğe göre rastgele seçim
[CreateAssetMenu(menuName = "Robot Scrap/Trash Table", fileName = "TT_New")]
public class TrashTable : ScriptableObject
{
    [SerializeField] private TrashData[] entries;

    public TrashData PickRandom()
    {
        if (entries == null || entries.Length == 0) return null;

        float total = 0f;
        foreach (TrashData data in entries)
        {
            if (data != null) total += data.GetChance();
        }
        if (total <= 0f) return null;

        float roll = Random.Range(0f, total);

        float cumulative = 0f;
        foreach (TrashData data in entries)
        {
            if (data == null) continue;
            cumulative += data.GetChance();
            if (roll < cumulative) return data;
        }

        for (int i = entries.Length - 1; i >= 0; i--)
        {
            if (entries[i] != null) return entries[i];
        }
        return null;
    }

    public int GetCount()
    {
        if (entries == null) return 0;
        return entries.Length;
    }
}