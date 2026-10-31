
using UnityEngine;

// SAHİBİ: Nagihan — haritada rastgele çöp yaratır
public class TrashSpawner : MonoBehaviour
{
    [Header("Çöp prefabları (Sümeyra'nın)")]
    [SerializeField] private TrashItem normalPrefab;
    [SerializeField] private TrashItem metalPrefab;
    [SerializeField] private TrashItem electronicPrefab;

    [Header("Çıkma şansları (ağırlık)")]
    [SerializeField] private float normalChance = 65f;
    [SerializeField] private float metalChance = 27f;
    [SerializeField] private float electronicChance = 8f;

    [Header("Miktar ve zaman")]
    [SerializeField] private int maxTrash = 8;
    [SerializeField] private float respawnDelay = 5f;

    [Header("Nereye çıkabilir?")]
    [SerializeField] private Vector2 areaMin = new Vector2(-18f, -18f);
    [SerializeField] private Vector2 areaMax = new Vector2(18f, 18f);
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float clearRadius = 1f;
    [SerializeField] private Collider[] noSpawnZones;
    [SerializeField] private int maxTries = 20;

    private int aliveCount;
    private float timer;

    void Start()
    {
        if (groundLayers == 0)
        {
            groundLayers = LayerMask.GetMask("Ground");
        }

        for (int i = 0; i < maxTrash; i++)
        {
            SpawnOne();
        }
    }

    void Update()
    {
        if (aliveCount >= maxTrash)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= respawnDelay)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    // Bir çöp teslim edilip yok olunca çöpün kendisi bunu çağırır
    public void OnTrashRemoved(TrashItem item)
    {
        aliveCount--;
    }

    private void SpawnOne()
    {
        Vector3 position;
        if (!TryFindPosition(out position)) return;

        TrashItem prefab = PickRandomPrefab();
        if (prefab == null) return;

        TrashItem newTrash = Instantiate(prefab, position, Quaternion.identity, transform);
        newTrash.SetSpawner(this);
        aliveCount++;
    }

    private TrashItem PickRandomPrefab()
    {
        float total = normalChance + metalChance + electronicChance;
        float roll = Random.Range(0f, total);

        if (roll < normalChance) return normalPrefab;
        if (roll < normalChance + metalChance) return metalPrefab;
        return electronicPrefab;
    }

    private bool TryFindPosition(out Vector3 position)
    {
        for (int i = 0; i < maxTries; i++)
        {
            float x = Random.Range(areaMin.x, areaMax.x);
            float z = Random.Range(areaMin.y, areaMax.y);

            Vector3 top = new Vector3(x, 10f, z);
            RaycastHit hit;
            if (!Physics.Raycast(top, Vector3.down, out hit, 20f, groundLayers)) continue;

            Vector3 candidate = hit.point;

            Vector3 checkCenter = candidate + Vector3.up * (clearRadius + 0.05f);
            if (Physics.CheckSphere(checkCenter, clearRadius, ~groundLayers, QueryTriggerInteraction.Ignore)) continue;

            if (IsInNoSpawnZone(candidate)) continue;

            position = candidate;
            return true;
        }

        position = Vector3.zero;
        return false;
    }

    private bool IsInNoSpawnZone(Vector3 point)
    {
        foreach (Collider zone in noSpawnZones)
        {
            if (zone != null && zone.bounds.Contains(point)) return true;
        }
        return false;
    }
}