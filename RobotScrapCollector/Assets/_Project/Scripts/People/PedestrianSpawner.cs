
using UnityEngine;

// Bir yol: başlangıç ve bitiş noktası (Inspector'da düzenlenebilir)
[System.Serializable]
public class PedestrianRoute
{
    public Vector3 start;
    public Vector3 end;

    public PedestrianRoute(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }
}

// SAHİBİ: Nagihan — saate göre yoldan yaya gönderir
public class PedestrianSpawner : MonoBehaviour
{
    [Header("Neler gelecek?")]
    [SerializeField] private GameObject[] pedestrianPrefabs;
    [SerializeField] private TrashTable trashTable;

    [Header("Yollar (Furkan'ın dış yolu: bahçenin 4 yanı)")]
    [SerializeField]
    private PedestrianRoute[] routes =
    {
        new PedestrianRoute(new Vector3(-25f, 0f,  23f), new Vector3( 25f, 0f,  23f)),
        new PedestrianRoute(new Vector3(-25f, 0f, -23f), new Vector3( 25f, 0f, -23f)),
        new PedestrianRoute(new Vector3( 23f, 0f, -25f), new Vector3( 23f, 0f,  25f)),
        new PedestrianRoute(new Vector3(-23f, 0f, -25f), new Vector3(-23f, 0f,  25f)),
    };

    [Header("Yoğunluk: oyun saati başına kaç yaya (X = saat, Y = yaya)")]
    [SerializeField]
    private AnimationCurve pedestriansPerHour = new AnimationCurve(
        new Keyframe(8f, 3f), new Keyframe(9f, 5f), new Keyframe(10f, 2f),
        new Keyframe(12f, 2f), new Keyframe(14f, 2f), new Keyframe(16f, 3f),
        new Keyframe(17f, 7f), new Keyframe(18f, 8f), new Keyframe(19f, 0f),
        new Keyframe(20f, 0f));

    [Header("Yaya davranışı")]
    [SerializeField] private float throwChance = 0.8f;
    [SerializeField] private Vector2 walkSpeedRange = new Vector2(1.2f, 1.8f);
    [SerializeField] private Vector2 throwPointRange = new Vector2(0.2f, 0.8f);

    [Header("Çöplerin düşebileceği alan")]
    [SerializeField] private Vector2 targetMin = new Vector2(-17f, -17f);
    [SerializeField] private Vector2 targetMax = new Vector2(17f, 17f);
    [SerializeField] private Collider[] noTargetZones;

    [Header("Test (Ali'nin saati yokken)")]
    [SerializeField] private bool useOwnTestClock = false;
    [SerializeField] private float testHoursPerSecond = 0.2f;

    private DayClock clock;
    private float lastHour;
    private float pending;
    private float difficulty = 1f;
    private float testHour = 8f;

    void Start()
    {
        clock = FindFirstObjectByType<DayClock>();
        lastHour = CurrentHour();
    }

    void Update()
    {
        if (useOwnTestClock)
        {
            testHour += testHoursPerSecond * Time.deltaTime;
            if (testHour >= 20f) testHour = 8f;
        }

        float hour = CurrentHour();
        float passed = hour - lastHour;
        lastHour = hour;

        if (passed <= 0f) return;

        pending += pedestriansPerHour.Evaluate(hour) * difficulty * passed;

        while (pending >= 1f)
        {
            pending -= 1f;
            SpawnOne();
        }
    }

    private float CurrentHour()
    {
        if (useOwnTestClock || clock == null) return testHour;
        return clock.GetHour();
    }

    public bool CanThrow()
    {
        if (useOwnTestClock || clock == null) return testHour < 19f;
        return clock.CanThrow();
    }

    private void SpawnOne()
    {
        if (pedestrianPrefabs == null || pedestrianPrefabs.Length == 0) return;
        if (routes == null || routes.Length == 0) return;

        PedestrianRoute route = routes[Random.Range(0, routes.Length)];
        bool reverse = Random.value < 0.5f;
        Vector3 from = reverse ? route.end : route.start;
        Vector3 to = reverse ? route.start : route.end;

        GameObject prefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)];
        GameObject obj = Instantiate(prefab, from, Quaternion.identity, transform);

        Pedestrian pedestrian = obj.AddComponent<Pedestrian>();
        pedestrian.Init(this, from, to,
            Random.Range(walkSpeedRange.x, walkSpeedRange.y),
            Random.Range(throwPointRange.x, throwPointRange.y),
            Random.value < throwChance);
    }

    public bool TryPickTarget(out Vector3 target)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 point = new Vector3(
                Random.Range(targetMin.x, targetMax.x), 0f,
                Random.Range(targetMin.y, targetMax.y));

            if (IsInNoTargetZone(point)) continue;

            target = point;
            return true;
        }

        target = Vector3.zero;
        return false;
    }

    private bool IsInNoTargetZone(Vector3 point)
    {
        foreach (Collider zone in noTargetZones)
        {
            if (zone == null) continue;
            Vector3 test = point;
            test.y = zone.bounds.center.y;
            if (zone.bounds.Contains(test)) return true;
        }
        return false;
    }

    public TrashItem PickTrashPrefab()
    {
        if (trashTable == null) return null;
        TrashData data = trashTable.PickRandom();
        if (data == null) return null;
        return data.GetPrefab();
    }

    // ===== SÖZLEŞME (Aşama 3) =====

    public void SetDifficulty(int day)
    {
        difficulty = 1f + (day - 1) * 0.15f;
    }

    public void ClearAll()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        pending = 0f;
    }
}