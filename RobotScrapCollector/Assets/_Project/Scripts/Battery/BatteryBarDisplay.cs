using UnityEngine;

// SAHİBİ: Ali — robotun bataryasını bir çubukla gösterir
public class BatteryBarDisplay : MonoBehaviour
{
    [SerializeField] private RobotBattery battery;   // hangi robotun bataryası? (Inspector'dan sürükle)

    void Update()
    {
        if (battery == null) return;

        // Çubuğun genişliği = bataryanın doluluk oranı (0 ile 1 arası)
        transform.localScale = new Vector3(battery.GetPercent(), 1f, 1f);
    }
}