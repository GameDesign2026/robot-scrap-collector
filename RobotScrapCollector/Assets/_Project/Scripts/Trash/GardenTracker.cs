using System.Collections.Generic;
using UnityEngine;

// SAHİBİ: Sümeyra — bahçede (yerde) kaç çöp var?
public class GardenTracker : MonoBehaviour
{
    // Herkesin ulaşabildiği tek sayaç
    public static GardenTracker Instance;

    [SerializeField] private Vector2 gardenMin = new Vector2(-19.8f, -19.8f);  // çitin iç köşesi (X, Z)
    [SerializeField] private Vector2 gardenMax = new Vector2(19.8f, 19.8f);
    [SerializeField] private int countForDebug;   // Play'de Inspector'dan izlemek için

    private List<TrashItem> items = new List<TrashItem>();

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Bir çöp bahçeye düştü
    public void Register(TrashItem item)
    {
        if (item == null) return;
        if (items.Contains(item)) return;   // aynı çöp iki kere sayılmasın
        items.Add(item);
        countForDebug = items.Count;
    }

    // Bir çöp bahçeden gitti
    public void Unregister(TrashItem item)
    {
        items.Remove(item);   // listede yoksa hiçbir şey olmaz
        countForDebug = items.Count;
    }

    // Bahçede kaç çöp var?
    public int GetCount()
    {
        return items.Count;
    }

    // Bu nokta çitin içinde mi? (yükseklik önemli değil)
    public bool IsInsideGarden(Vector3 point)
    {
        return point.x >= gardenMin.x && point.x <= gardenMax.x
            && point.z >= gardenMin.y && point.z <= gardenMax.y;
    }
}
