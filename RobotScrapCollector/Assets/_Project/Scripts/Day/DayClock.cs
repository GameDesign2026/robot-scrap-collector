using UnityEngine;

// SAHİBİ: Ali — oyunun saati: 08:00'den 20:00'e
public class DayClock : MonoBehaviour
{
    [SerializeField] private float realMinutesPerDay = 6f;   // bir oyun günü kaç gerçek dakika
    [SerializeField] private float startHour = 8f;
    [SerializeField] private float throwEndHour = 19f;       // bu saatten sonra çöp atılmaz
    [SerializeField] private float endHour = 20f;

    private float hour;
    private int day = 1;
    private bool dayOver;
    private int scrapAtDayStart;    // gün başındaki Scrap (bugün ne kadar kazanıldı?)

    private RobotStorage storage;
    private RobotMove move;
    private DaySummaryUI summary;

    void Start()
    {
        hour = startHour;

        GameObject robot = GameObject.FindWithTag("Player");
        if (robot != null)
        {
            storage = robot.GetComponent<RobotStorage>();
            move = robot.GetComponent<RobotMove>();
        }

        // Özet ekranı başta gizli olabilir: gizli objeleri de ara
        summary = FindFirstObjectByType<DaySummaryUI>(FindObjectsInactive.Include);

        if (storage != null) scrapAtDayStart = storage.GetScrap();
    }

    void Update()
    {
        if (dayOver) return;

        // Gerçek saniye → oyun saati
        float hoursPerSecond = (endHour - startHour) / (realMinutesPerDay * 60f);
        hour += hoursPerSecond * Time.deltaTime;

        if (hour >= endHour)
        {
            hour = endHour;
            EndDay();
        }
    }

    // Gün bitti: bir kez çalışır
    private void EndDay()
    {
        dayOver = true;

        int trashLeft = 0;
        if (GardenTracker.Instance != null) trashLeft = GardenTracker.Instance.GetCount();

        int scrapEarned = 0;
        if (storage != null) scrapEarned = storage.GetScrap() - scrapAtDayStart;

        bool success = trashLeft == 0;

        Debug.Log("Gün " + day + " bitti · Scrap +" + scrapEarned + " · bahçede " + trashLeft + " çöp");

        if (move != null) move.SetControlEnabled(false);
        if (summary != null) summary.Show(day, scrapEarned, trashLeft, success);
    }

    // ===== SÖZLEŞME =====

    public float GetHour() { return hour; }

    // "14:35" gibi
    public string GetTimeText()
    {
        int h = Mathf.FloorToInt(hour);
        int m = Mathf.FloorToInt((hour - h) * 60f);
        return h.ToString("00") + ":" + m.ToString("00");
    }

    public bool CanThrow()
    {
        return !dayOver && hour < throwEndHour;
    }

    public bool IsCleanupTime()
    {
        return !dayOver && hour >= throwEndHour;
    }

    public bool IsDayOver() { return dayOver; }

    public int GetDay() { return day; }

    // "Sonraki Gün" butonu çağırır
    public void StartNewDay()
    {
        day++;
        hour = startHour;
        dayOver = false;
        if (storage != null) scrapAtDayStart = storage.GetScrap();

        if (summary != null) summary.Hide();
        if (move != null) move.SetControlEnabled(true);

        Debug.Log("Gün " + day + " başladı");
    }

    // ----- HATA AYIKLAMA (Inspector'da ⋮ menüsünden) -----
    [ContextMenu("DEBUG: Jump to 18:50")]
    private void DebugJumpToEvening() { hour = 18.83f; }

    [ContextMenu("DEBUG: Start New Day")]
    private void DebugStartNewDay() { StartNewDay(); }
}