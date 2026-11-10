using UnityEngine;

// SAHİBİ: Nagihan — bir çöp türünün verisi
[CreateAssetMenu(menuName = "Robot Scrap/Trash Data", fileName = "TD_New")]
public class TrashData : ScriptableObject
{
    [SerializeField] private string displayName = "Çöp";
    [SerializeField] private int value = 1;
    [SerializeField] private float weight = 1f;
    [SerializeField] private float chance = 10f;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private TrashItem prefab;

    public string GetDisplayName() { return displayName; }
    public int GetValue() { return value; }
    public float GetWeight() { return weight; }
    public float GetChance() { return chance; }
    public Color GetColor() { return color; }
    public TrashItem GetPrefab() { return prefab; }

    void OnValidate()
    {
        if (value < 0) value = 0;
        if (weight < 0f) weight = 0f;
        if (chance < 0f) chance = 0f;
    }
}