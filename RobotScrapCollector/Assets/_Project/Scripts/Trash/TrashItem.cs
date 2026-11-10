using UnityEngine;

// Çöp türleri (hotbar renkleri ve üretici kullanıyor)
public enum TrashType { Normal, Metal, Electronic }

// SAHİBİ: Sümeyra
public class TrashItem : MonoBehaviour
{
    // Çöpün içinde bulunabileceği durumlar
    private enum State { OnGround, Carried, Delivered }

    [Header("Kimlik")]
    [SerializeField] private TrashType trashType = TrashType.Normal;
    [SerializeField] private int value = 1;          // kaç Scrap değerinde
    [SerializeField] private float weight = 1f;      // ne kadar ağır

    [Header("Parçalar")]
    [SerializeField] private Transform visual;       // görünüş (dönen, süzülen küp)
    [SerializeField] private Collider pickupTrigger; // alma alanı (Sphere Collider, trigger)
    [SerializeField] private Collider bodyCollider;  // gövde (Box Collider, sadece fırlatılırken açık)

    [Header("Ayarlar")]
    [SerializeField] private float growTime = 0.5f;    // doğarken kaç saniyede büyüsün
    [SerializeField] private float pickupDelay = 0.5f; // atıldıktan kaç saniye sonra alınabilsin
    [SerializeField] private float throwForce = 3f;    // ne kadar kuvvetle fırlatılsın

    private State state = State.OnGround;
    private Rigidbody rb;
    private Renderer visualRenderer;
    private Color baseColor;        // normal renk (vurgu kalkınca buna dönülür)
    private Vector3 fullScale;      // Visual'ın normal boyutu
    private float growTimer;
    private float canPickUpAt;      // bu zamandan önce alınamaz
    private TrashSpawner spawner;   // beni kim yarattı? (Nagihan'ın üreticisi)

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        visualRenderer = visual.GetComponent<Renderer>();
        baseColor = visualRenderer.material.color;
        fullScale = visual.localScale;

        // Doğarken sıfır boyuttan başla, Update'te büyüyecek
        visual.localScale = Vector3.zero;
        growTimer = 0f;
    }

    void Update()
    {
        // Doğma animasyonu: küçükten büyüye
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
        // Sadece fırlatılmışken: yere oturdu mu?
        if (rb.isKinematic) return;                       // zaten duruyor
        if (Time.time < canPickUpAt) return;              // daha yeni atıldı, biraz bekle
        if (rb.linearVelocity.magnitude > 0.05f) return;  // hâlâ hareket ediyor

        // Durdu: fiziği kapat, gövdeyi kapat (robot artık çarpmasın)
        rb.isKinematic = true;
        bodyCollider.enabled = false;
    }

    // ===== SÖZLEŞME =====

    // Şu an alınabilir mi?
    public bool CanBePickedUp()
    {
        return state == State.OnGround && Time.time >= canPickUpAt;
    }

    // E ile alındı (RobotInteractor çağırır)
    public void OnPickedUp()
    {
        state = State.Carried;
        SetHighlight(false);
        visual.gameObject.SetActive(false);   // görünmez ol
        pickupTrigger.enabled = false;        // bakılınca bulunmasın
        bodyCollider.enabled = false;
        rb.isKinematic = true;                // fizik dursun
    }

    // Q ile bırakıldı (Furkan'ın RobotStorage'ı çağırır)
    public void OnDropped(Vector3 position, Vector3 throwDirection)
    {
        state = State.OnGround;
        transform.position = position;

        visual.gameObject.SetActive(true);
        visual.localScale = fullScale;
        growTimer = growTime;                 // atılınca büyüme animasyonu olmasın

        pickupTrigger.enabled = true;
        bodyCollider.enabled = true;          // yere çarpabilsin

        rb.isKinematic = false;               // fizik başlasın
        rb.linearVelocity = Vector3.zero;
        Vector3 push = throwDirection.normalized + Vector3.up;   // ileri ve yukarı
        rb.AddForce(push * throwForce, ForceMode.Impulse);

        canPickUpAt = Time.time + pickupDelay;   // hemen geri alınmasın
    }

    // Teslim edildi (Furkan'ın RobotStorage'ı çağırır)
    public void OnDelivered()
    {
        state = State.Delivered;
        if (spawner != null) spawner.OnTrashRemoved(this);   // üreticiye haber ver
        Destroy(gameObject);                                  // sahneden sil
    }

    public int GetValue() { return value; }
    public float GetWeight() { return weight; }
    public TrashType GetTrashType() { return trashType; }

    // Bakılınca parla
    public void SetHighlight(bool on)
    {
        if (on)
        {
            visualRenderer.material.color = Color.Lerp(baseColor, Color.white, 0.5f);
        }
        else
        {
            visualRenderer.material.color = baseColor;
        }
    }

    public void SetSpawner(TrashSpawner newSpawner)
    {
        spawner = newSpawner;
    }
        // ===== AŞAMA 1 SÖZLEŞMESİ (Sümeyra dolduracak) =====

    // Bu çöpün veri dosyası (hotbar rengi için Nagihan kullanır)
    public TrashData GetData() { return null; }
}
