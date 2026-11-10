using UnityEngine;

// SAHİBİ: Nagihan. İSKELET: fonksiyon adlarını değiştirmeyin.
// Bir çöp türünün verisi (elma, teneke, kahve kutusu, telefon)
public class TrashData : ScriptableObject
{
    public string GetDisplayName() { return ""; }
    public int GetValue() { return 0; }
    public float GetWeight() { return 0f; }
    public float GetChance() { return 0f; }
    public Color GetColor() { return Color.white; }
    public TrashItem GetPrefab() { return null; }
}