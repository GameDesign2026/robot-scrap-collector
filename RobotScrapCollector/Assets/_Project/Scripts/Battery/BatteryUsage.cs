using UnityEngine;

// SAHİBİ: Ali
public class BatteryUsage : MonoBehaviour
{
    [SerializeField] private float drainPerMeter = 1f;          // 1 metre yol kaç batarya harcar
    [SerializeField] private float extraDrainPerWeight = 0.1f;  // her 1 ağırlık harcamayı ne kadar artırır (0.1 = %10)

    private Rigidbody rb;
    private RobotBattery battery;     // senin geçen haftaki script'in
    private RobotStorage storage;     // Furkan'ın: ağırlığı soracağız
    private Vector3 lastPosition;     // bir önceki fizik adımındaki konum (sadece yatay)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        battery = GetComponent<RobotBattery>();
        storage = GetComponent<RobotStorage>();

        lastPosition = rb.position;
        lastPosition.y = 0f;          // yüksekliği hesaba katma
    }

    void FixedUpdate()
    {
        // Şu anki konum (sadece yatay)
        Vector3 now = rb.position;
        now.y = 0f;

        // Son adımdan beri ne kadar yol gittik?
        float distance = Vector3.Distance(now, lastPosition);
        lastPosition = now;           // bir sonraki adım için şimdiki konumu hatırla

        // Neredeyse hiç kıpırdamadıysa harcama yapma
        if (distance < 0.001f) return;

        // Yük çarpanı: boşken 1, metal (ağırlık 3) taşırken 1.3
        float load = 1f;
        if (storage != null)
        {
            load = 1f + storage.GetCarriedWeight() * extraDrainPerWeight;
        }

        // Gidilen yol × metre başına harcama × yük
        battery.Drain(distance * drainPerMeter * load);
    }
}