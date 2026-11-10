using UnityEngine;

// ESKİ: Aşama 2'de silinecek (eski kodlar derlenmeye devam etsin diye şimdilik duruyor)
public enum TrashType { Normal, Metal, Electronic }

// SAHİBİ: Sümeyra
public class TrashItem : MonoBehaviour
{
    private enum State { OnGround, Carried, Delivered }

    [Header("Veri (Nagihan'ın dosyası)")]
    [SerializeField] private TrashData data;         // TD_Apple, TD_Can, TD_Coffee ya da TD_Phone

    [Header("Parçalar")]
    [SerializeField] private Transform visual;
    [SerializeField] private Collider pickupTrigger;
    [SerializeField] private Collider bodyCollider;

    [Header("Ayarlar")]
    [SerializeField] private float growTime = 0.5f;
    [SerializeField] private float pickupDelay = 0.5f;
    [SerializeField] private float throwForce = 3f;

    private State state = State.OnGround;
    private Rigidbody rb;
    private Renderer visualRenderer;
    private Color baseColor;
    private Vector3 fullScale;
    private float growTimer;
    private float canPickUpAt;
    private TrashSpawner spawner;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        visualRenderer = visual.GetComponent<Renderer>();

        // Veri slotu boşsa KIRMIZI hata: sessizce yanlış çalışmasın
        if (data == null)
        {
            Debug.LogError(name + ": TrashData yok! Prefabın Data slotunu doldur.");
        }
        else
        {
            // Rengi veri dosyasından al (bu çöpe özel kopya üzerinde)
            visualRenderer.material.color = data.GetColor();
        }

        baseColor = visualRenderer.material.color;
        fullScale = visual.localScale;

        visual.localScale = Vector3.zero;
        growTimer = 0f;
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
    }

    void FixedUpdate()
    {
        if (rb.isKinematic) return;
        if (Time.time < canPickUpAt) return;
        if (rb.linearVelocity.magnitude > 0.05f) return;

        rb.isKinematic = true;
        bodyCollider.enabled = false;
    }

    // ===== SÖZLEŞME =====

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
    }

    public void OnDelivered()
    {
        state = State.Delivered;
        if (spawner != null) spawner.OnTrashRemoved(this);
        Destroy(gameObject);
    }

    // Değer ve ağırlık artık veri dosyasından
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

    // ESKİ: kimse kullanmıyor, Aşama 2'de silinecek
    public TrashType GetTrashType() { return TrashType.Normal; }

    public void SetHighlight(bool on)
    {
        if (on) visualRenderer.material.color = Color.Lerp(baseColor, Color.white, 0.5f);
        else visualRenderer.material.color = baseColor;
    }

    public void SetSpawner(TrashSpawner newSpawner)
    {
        spawner = newSpawner;
    }
        // ===== AŞAMA 2 SÖZLEŞMESİ (Sümeyra dolduracak) =====

    // Yaya fırlattı: çöp bu hızla havaya fırlar (Nagihan çağırır)
    public void Launch(Vector3 velocity) { }

    // Çöp şu an havada mı? (Aşama 3'te sopa sorar)
    public bool IsInAir() { return false; }
}
