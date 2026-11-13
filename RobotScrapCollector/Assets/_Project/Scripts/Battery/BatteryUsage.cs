using UnityEngine;

// SAHİBİ: Ali — gidilen mesafe kadar batarya harcar
public class BatteryUsage : MonoBehaviour
{
    [SerializeField] private float drainPerMeter = 1f;
    [SerializeField] private float extraDrainPerWeight = 0.1f;
    [SerializeField] private float sprintDrainMultiplier = 2f;   // YENİ: koşarken metre başına 2 kat

    private Rigidbody rb;
    private RobotBattery battery;
    private RobotStorage storage;
    private RobotMove move;            // YENİ
    private Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        battery = GetComponent<RobotBattery>();
        storage = GetComponent<RobotStorage>();
        move = GetComponent<RobotMove>();

        lastPosition = rb.position;
        lastPosition.y = 0f;
    }

    void FixedUpdate()
    {
        Vector3 now = rb.position;
        now.y = 0f;

        if (battery.IsCharging())
        {
            lastPosition = now;
            return;
        }

        float distance = Vector3.Distance(now, lastPosition);
        lastPosition = now;

        // YENİ: ışınlandıysa (çekici) bu bir "yol" değil
        if (distance > 3f) return;

        if (distance < 0.001f) return;

        float load = 1f;
        if (storage != null)
        {
            load = 1f + storage.GetCarriedWeight() * extraDrainPerWeight;
        }

        // YENİ: koşuyorsa metre başına daha çok
        if (move != null && move.IsSprinting())
        {
            load *= sprintDrainMultiplier;
        }

        battery.Drain(distance * drainPerMeter * load);
    }
}