using UnityEngine;

// SAHİBİ: Ali — gidilen mesafe kadar batarya harcar
public class BatteryUsage : MonoBehaviour
{
    [SerializeField] private float drainPerMeter = 1f;
    [SerializeField] private float extraDrainPerWeight = 0.1f;

    private Rigidbody rb;
    private RobotBattery battery;
    private RobotStorage storage;
    private Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        battery = GetComponent<RobotBattery>();
        storage = GetComponent<RobotStorage>();

        lastPosition = rb.position;
        lastPosition.y = 0f;
    }

    void FixedUpdate()
    {
        Vector3 now = rb.position;
        now.y = 0f;

        // YENİ: markette (şarjda) iken harcama yok
        if (battery.IsCharging())
        {
            lastPosition = now;   // konumu yine de güncelle
            return;
        }

        float distance = Vector3.Distance(now, lastPosition);
        lastPosition = now;

        if (distance < 0.001f) return;

        float load = 1f;
        if (storage != null)
        {
            load = 1f + storage.GetCarriedWeight() * extraDrainPerWeight;
        }

        battery.Drain(distance * drainPerMeter * load);
    }
}