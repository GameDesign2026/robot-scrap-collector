using UnityEngine;

// SAHİBİ: Sümeyra
public class TrashItem : MonoBehaviour
{
    private enum State { OnGround, InAir, Carried, Delivered }

    [Header("Veri (Nagihan'ın dosyası)")]
    [SerializeField] private TrashData data;

    [Header("Parçalar")]
    [SerializeField] private Transform visual;
    [SerializeField] private Collider pickupTrigger;
    [SerializeField] private Collider bodyCollider;

    [Header("Ayarlar")]
    [SerializeField] private float growTime = 0.5f;
    [SerializeField] private float pickupDelay = 0.5f;
    [SerializeField] private float throwForce = 3f;
    [SerializeField] private float maxAirTime = 5f;

    [Header("Efektler (YENİ)")]
    [SerializeField] private GameObject landDustFx;    // FX_Dust
    [SerializeField] private GameObject pickupFx;      // FX_Sparkle

    private State state = State.OnGround;
    private Rigidbody rb;
    private Renderer visualRenderer;
    private Color baseColor;
    private Vector3 fullScale;
    private float growTimer;
    private float canPickUpAt;
    private float airTimer;
    private int groundLayer;
    private TrashSpawner spawner;

    private GameObject marker;      // YENİ: Nagihan'ın halkası
    private Pedestrian thrower;     // YENİ: beni atan yaya
    private bool wasHit;            // YENİ: sopayla vuruldum mu?

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        visualRenderer = visual.GetComponent<Renderer>();
        groundLayer = LayerMask.NameToLayer("Ground");

        if (data == null)
        {
            Debug.LogError(name + ": TrashData yok! Prefabın Data slotunu doldur.");
        }
        else
        {
            visualRenderer.material.color = data.GetColor();
        }

        baseColor = visualRenderer.material.color;
        fullScale = visual.localScale;

        visual.localScale = Vector3.zero;
        growTimer = 0f;
    }

    void Start()
    {
        if (state == State.OnGround && GardenTracker.Instance != null)
        {
            if (GardenTracker.Instance.IsInsideGarden(transform.position))
            {
                GardenTracker.Instance.Register(this);
            }
        }
    }

    void Update()
    {
        if (growTimer < growTime)
        {
            growTimer += Time.deltaTime;
            float growth = growTimer / growTime;
            if (growth > 1f) growth = 1f;
            visual.localScale = fullScale * growth;
        }

        if (state == State.InAir)
        {
            airTimer += Time.deltaTime;
            if (airTimer >= maxAirTime) Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (state != State.OnGround) return;
        if (rb.isKinematic) return;
        if (Time.time < canPickUpAt) return;
        if (rb.linearVelocity.magnitude > 0.05f) return;

        rb.isKinematic = true;
        bodyCollider.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.InAir) return;
        if (collision.gameObject.layer != groundLayer) return;

        Land();
    }

    void OnDestroy()
    {
        if (GardenTracker.Instance != null) GardenTracker.Instance.Unregister(this);
        RemoveMarker();   // YENİ: çöp nasıl silinirse silinsin, halka da gitsin
    }

    // ===== AŞAMA 2 =====

    public void Launch(Vector3 velocity)
    {
        state = State.InAir;
        airTimer = 0f;

        visual.gameObject.SetActive(true);
        visual.localScale = fullScale;
        growTimer = growTime;

        pickupTrigger.enabled = false;
        bodyCollider.enabled = true;

        rb.isKinematic = false;
        rb.linearVelocity = velocity;
    }

    public bool IsInAir()
    {
        return state == State.InAir;
    }

    private void Land()
    {
        RemoveMarker();   // YENİ: yere değdi, halka gitsin

        if (GardenTracker.Instance != null && !GardenTracker.Instance.IsInsideGarden(transform.position))
        {
            Destroy(gameObject);
            return;
        }

        state = State.OnGround;
        pickupTrigger.enabled = true;
        canPickUpAt = Time.time + 0.3f;

        SpawnFx(landDustFx, transform.position);   // YENİ: toz

        if (GardenTracker.Instance != null) GardenTracker.Instance.Register(this);
    }

    // ===== AŞAMA 3 (YENİ) =====

    // Nagihan verir: düşüş halkası
    public void SetMarker(GameObject newMarker)
    {
        marker = newMarker;
    }

    // Nagihan verir: beni atan yaya
    public void SetThrower(Pedestrian newThrower)
    {
        thrower = newThrower;
    }

    // Sopa sorar: beni vurabilir mi?
    public bool CanBeHit()
    {
        return state == State.InAir && !wasHit;
    }

    // Sopa çağırır: yeni bir hızla fırla
    public void HitAway(Vector3 velocity)
    {
        if (!CanBeHit()) return;

        wasHit = true;
        airTimer = 0f;

        rb.isKinematic = false;
        bodyCollider.enabled = true;
        rb.linearVelocity = velocity;

        RemoveMarker();   // eski hedef artık geçersiz

        // Beni atan yaya korksun ve kaçsın
        if (thrower != null)
        {
            thrower.Flee();
            thrower = null;
        }
    }

    private void RemoveMarker()
    {
        if (marker != null)
        {
            Destroy(marker);
            marker = null;
        }
    }

    private void SpawnFx(GameObject fxPrefab, Vector3 position)
    {
        if (fxPrefab != null) Instantiate(fxPrefab, position, Quaternion.identity);
    }

    // ===== ÖNCEKİ AŞAMALAR =====

    public bool CanBePickedUp()
    {
        return state == State.OnGround && Time.time >= canPickUpAt;
    }

    public void OnPickedUp()
    {
        SpawnFx(pickupFx, visual.position);   // YENİ: parıltı (gizlenmeden önce, görünüşün olduğu yerde)

        state = State.Carried;
        SetHighlight(false);
        visual.gameObject.SetActive(false);
        pickupTrigger.enabled = false;
        bodyCollider.enabled = false;
        rb.isKinematic = true;

        if (GardenTracker.Instance != null) GardenTracker.Instance.Unregister(this);
    }

    public void OnDropped(Vector3 position, Vector3 throwDirection)
    {
        state = State.OnGround;
        transform.position = position;

        visual.gameObject.SetActive(true);
        visual.localScale = fullScale;
        growTimer = growTime;

        pickupTrigger.enabled = true;
        bodyCollider.enabled = true;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        Vector3 push = throwDirection.normalized + Vector3.up;
        rb.AddForce(push * throwForce, ForceMode.Impulse);

        canPickUpAt = Time.time + pickupDelay;

        if (GardenTracker.Instance != null) GardenTracker.Instance.Register(this);
    }

    public void OnDelivered()
    {
        state = State.Delivered;
        if (spawner != null) spawner.OnTrashRemoved(this);
        Destroy(gameObject);
    }

    public int GetValue()
    {
        if (data == null) return 0;
        return data.GetValue();
    }

    public float GetWeight()
    {
        if (data == null) return 0f;
        return data.GetWeight();
    }

    public TrashData GetData() { return data; }

    public void SetHighlight(bool on)
    {
        if (on) visualRenderer.material.color = Color.Lerp(baseColor, Color.white, 0.5f);
        else visualRenderer.material.color = baseColor;
    }

    public void SetSpawner(TrashSpawner newSpawner)
    {
        spawner = newSpawner;
    }
}
