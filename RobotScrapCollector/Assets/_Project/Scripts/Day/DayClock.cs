using UnityEngine;

// SAHİBİ: Ali — oyunun saati: 08:00'den 20:00'e
public class DayClock : MonoBehaviour
{
    [SerializeField] private float realMinutesPerDay = 6f;
    [SerializeField] private float startHour = 8f;
    [SerializeField] private float throwEndHour = 19f;
    [SerializeField] private float endHour = 20f;
    [SerializeField] private int penaltyPerTrash = 1;   // YENİ: kalan her çöp için ceza

    private float hour;
    private int day = 1;
    private bool dayOver;
    private int scrapAtDayStart;

    private RobotStorage storage;
    private RobotMove move;
    private DaySummaryUI summary;
    private PedestrianSpawner spawner;   // YENİ

    // YENİ: saat Awake'te başlıyor (ilk gün hatası buradaydı)
    void Awake()
    {
        hour = startHour;
    }

    void Start()
    {
        GameObject robot = GameObject.FindWithTag("Player");
        if (robot != null)
        {
            storage = robot.GetComponent<RobotStorage>();
            move = robot.GetComponent<RobotMove>();
        }

        summary = FindFirstObjectByType<DaySummaryUI>(FindObjectsInactive.Include);
        spawner = FindFirstObjectByType<PedestrianSpawner>();

        if (storage != null) scrapAtDayStart = storage.GetScrap();
        if (spawner != null) spawner.SetDifficulty(day);   // YENİ: 1. günün zorluğu
    }

    void Update()
    {
        if (dayOver) return;

        float hoursPerSecond = (endHour - startHour) / (realMinutesPerDay * 60f);
        hour += hoursPerSecond * Time.deltaTime;

        if (hour >= endHour)
        {
            hour = endHour;
            EndDay();
        }
    }

    private void EndDay()
    {
        dayOver = true;

        int trashLeft = 0;
        if (GardenTracker.Instance != null) trashLeft = GardenTracker.Instance.GetCount();

        // YENİ: kalan her çöp için ceza (eksiye düşebilir)
        if (trashLeft > 0 && storage != null)
        {
            storage.RemoveScrap(trashLeft * penaltyPerTrash);
        }

        // Bugün kazanılan: cezadan SONRA hesaplanır
        int scrapEarned = 0;
        if (storage != null) scrapEarned = storage.GetScrap() - scrapAtDayStart;

        bool success = trashLeft == 0;

        Debug.Log("Gün " + day + " bitti · net Scrap " + scrapEarned + " · bahçede " + trashLeft + " çöp");

        if (move != null) move.SetControlEnabled(false);
        if (summary != null) summary.Show(day, scrapEarned, trashLeft, success);
    }

    // YENİ: bahçedeki (yerdeki ve havadaki) bütün çöpleri sil, taşınanlara dokunma
    private void ClearGarden()
    {
        TrashItem[] all = FindObjectsByType<TrashItem>(FindObjectsSortMode.None);
        foreach (TrashItem item in all)
        {
            if (item.IsInAir() || item.CanBePickedUp())
            {
                Destroy(item.gameObject);
            }
        }
    }

    // ===== SÖZLEŞME =====

    public float GetHour() { return hour; }

    public string GetTimeText()
    {
        int h = Mathf.FloorToInt(hour);
        int m = Mathf.FloorToInt((hour - h) * 60f);
        return h.ToString("00") + ":" + m.ToString("00");
    }

    public bool CanThrow() { return !dayOver && hour < throwEndHour; }
    public bool IsCleanupTime() { return !dayOver && hour >= throwEndHour; }
    public bool IsDayOver() { return dayOver; }
    public int GetDay() { return day; }

    public void StartNewDay()
    {
        ClearGarden();                         // YENİ: ceza ödendi, bahçe temizlensin

        day++;
        hour = startHour;
        dayOver = false;
        if (storage != null) scrapAtDayStart = storage.GetScrap();

        if (spawner != null) spawner.SetDifficulty(day);   // YENİ: her gün biraz daha zor

        if (summary != null) summary.Hide();
        if (move != null) move.SetControlEnabled(true);

        Debug.Log("Gün " + day + " başladı");
    }

    // YENİ: LivesManager (çekici) çağırır: saati ileri al, en fazla 20:00
    public void AddHours(float hours)
    {
        if (dayOver) return;

        hour = Mathf.Min(hour + hours, endHour);
        if (hour >= endHour) EndDay();
    }

    // ----- HATA AYIKLAMA -----
    [ContextMenu("DEBUG: Jump to 18:50")]
    private void DebugJumpToEvening() { hour = 18.83f; }

    [ContextMenu("DEBUG: Start New Day")]
    private void DebugStartNewDay() { StartNewDay(); }
}