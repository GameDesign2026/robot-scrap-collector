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
    [SerializeField] private float maxAirTime = 5f;   // bu kadar havada kalırsa (çatıya takıldıysa) silinir

    private State state = State.OnGround;
    private Rigidbody rb;
    private Renderer visualRenderer;
    private Color baseColor;
    private Vector3 fullScale;
    private float growTimer;
    private float canPickUpAt;
    private float airTimer;
    private int groundLayer;
    private TrashSpawner spawner;   // eski üretici (birleştirmede kalkacak)

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
        // Yerde doğan çöp (eski üretici) hemen sayılsın.
        // Fırlatılan çöpte Launch, Start'tan ÖNCE çağrıldığı için durum zaten InAir olur.
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

        // Havada çok uzun kaldıysa (bir çatıya takıldı): sil
        if (state == State.InAir)
        {
            airTimer += Time.deltaTime;
            if (airTimer >= maxAirTime) Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        // Sadece yerdeyken: hareket bitti mi?
        if (state != State.OnGround) return;
        if (rb.isKinematic) return;
        if (Time.time < canPickUpAt) return;
        if (rb.linearVelocity.magnitude > 0.05f) return;

        rb.isKinematic = true;
        bodyCollider.enabled = false;
    }

    // Havadayken bir şeye çarptı
    void OnCollisionEnter(Collision collision)
    {
        if (state != State.InAir) return;
        if (collision.gameObject.layer != groundLayer) return;   // zemin değilse sekmeye devam

        Land();
    }

    // Unity, obje silinmeden hemen önce çağırır: ne olursa olsun sayaçtan düş
    void OnDestroy()
    {
        if (GardenTracker.Instance != null) GardenTracker.Instance.Unregister(this);
    }

    // ===== AŞAMA 2 =====

    // Yaya fırlattı (Nagihan çağırır)
    public void Launch(Vector3 velocity)
    {
        state = State.InAir;
        airTimer = 0f;

        // Elden tam boyda çıksın (doğma animasyonu olmasın)
        visual.gameObject.SetActive(true);
        visual.localScale = fullScale;
        growTimer = growTime;

        pickupTrigger.enabled = false;   // havadayken alınamaz
        bodyCollider.enabled = true;     // yere çarpabilsin

        rb.isKinematic = false;
        rb.linearVelocity = velocity;
    }

    public bool IsInAir()
    {
        return state == State.InAir;
    }

    // Zemine indi
    private void Land()
    {
        // Bahçenin dışına düştüyse (yol, çim): yok ol
        if (GardenTracker.Instance != null && !GardenTracker.Instance.IsInsideGarden(transform.position))
        {
            Destroy(gameObject);
            return;
        }

        state = State.OnGround;
        pickupTrigger.enabled = true;
        canPickUpAt = Time.time + 0.3f;   // sekerken hemen alınmasın

        if (GardenTracker.Instance != null) GardenTracker.Instance.Register(this);
    }

    // ===== SÖZLEŞME (önceki aşamalar) =====

    public bool CanBePickedUp()
    {
        return state == State.OnGround && Time.time >= canPickUpAt;
    }

    public void OnPickedUp()
    {
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

        // Robot bahçenin içinde, düşürdüğü çöp de bahçeye düştü
        if (GardenTracker.Instance != null) GardenTracker.Instance.Register(this);
    }

    public void OnDelivered()
    {
        state = State.Delivered;
        if (spawner != null) spawner.OnTrashRemoved(this);
        Destroy(gameObject);   // OnDestroy sayaçtan düşürür
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

    // ESKİ üretici için (birleştirmede üretici kalkınca da zararsız)
    public void SetSpawner(TrashSpawner newSpawner)
    {
        spawner = newSpawner;
    }
}
