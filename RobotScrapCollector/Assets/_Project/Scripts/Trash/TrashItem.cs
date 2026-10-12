using UnityEngine;

// SAHİBİ: Sümeyra
public class TrashItem : MonoBehaviour
{
    [SerializeField] private int value = 1;            // kaç Scrap değerinde
    [SerializeField] private float weight = 1f;        // ne kadar ağır (robotu yavaşlatacak)
    [SerializeField] private float respawnTime = 5f;   // toplandıktan kaç saniye sonra geri gelecek
    [SerializeField] private float growTime = 0.5f;    // geri gelirken kaç saniyede tam boyuta ulaşacak
    [SerializeField] private Transform visual;         // gizlenip gösterilecek görünüş parçası

    private Collider pickupTrigger;   // toplama alanı (bu objedeki Sphere Collider)
    private Vector3 fullScale;        // Visual'ın normal boyutu
    private bool isCollected;         // şu an toplanmış (gizli) mı?
    private float timer;              // yeniden doğma sayacı
    private float growTimer;          // büyüme sayacı

    void Awake()
    {
        pickupTrigger = GetComponent<Collider>();
        fullScale = visual.localScale;
        growTimer = growTime;          // oyun başında büyüme animasyonu olmasın
    }

    // ALGILA: bir şey toplama alanına girdi
    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;                                   // zaten toplanmışsa çık

        // KİM? Giren şeyin deposu var mı? (yani robot mu?)
        RobotStorage storage = other.GetComponent<RobotStorage>();
        if (storage == null) return;                               // depo yoksa robot değil
        if (!storage.CanCollect()) return;                         // depo doluysa toplanamaz

        // YAP: çöpü depoya ver ve gizlen
        storage.AddTrash(value, weight);
        Collect();
    }

    void Update()
    {
        // Toplandıysa: sayaç ilerlesin, süre dolunca geri gel
        if (isCollected)
        {
            timer += Time.deltaTime;
            if (timer >= respawnTime)
            {
                Respawn();
            }
            return;
        }

        // Yeni geri geldiyse: küçükten büyüsün
        if (growTimer < growTime)
        {
            growTimer += Time.deltaTime;
            float growth = growTimer / growTime;
            if (growth > 1f)
            {
                growth = 1f;
            }
            visual.localScale = fullScale * growth;
        }
    }

    private void Collect()
    {
        isCollected = true;
        timer = 0f;
        visual.gameObject.SetActive(false);   // görünmez ol
        pickupTrigger.enabled = false;        // bir daha toplanamasın
        Debug.Log(name + " collected (+" + value + ")");
    }

    private void Respawn()
    {
        isCollected = false;
        growTimer = 0f;
        visual.localScale = Vector3.zero;     // sıfır boyuttan başla
        visual.gameObject.SetActive(true);    // tekrar görün
        pickupTrigger.enabled = true;         // tekrar toplanabilsin
        Debug.Log(name + " respawned");
    }
}