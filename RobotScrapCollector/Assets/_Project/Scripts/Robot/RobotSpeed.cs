using UnityEngine;

// SAHİBİ: Nazife
public class RobotSpeed : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3f;          // normal hız (market ile artacak)
    [SerializeField] private float slowPerWeight = 0.1f;    // her 1 ağırlık hızı ne kadar düşürür (0.1 = %10)
    [SerializeField] private float minWeightFactor = 0.4f;  // ağırlık hızı en fazla bu orana kadar düşürebilir

    private float surfaceMultiplier = 1f;   // zemin çarpanı: normal 1, çamur 0.5, yağ 1.5
    private RobotStorage storage;           // ağırlığı buradan soracağız (Furkan'ın script'i)

    void Awake()
    {
        // Aynı robottaki depoyu bir kere bul ve sakla
        storage = GetComponent<RobotStorage>();
    }

    // ----- ZEMİN (geçen haftadan, değişmedi) -----

    public void SetSurface(float multiplier)
    {
        if (multiplier <= 0f) return;
        surfaceMultiplier = multiplier;
    }

    public void ResetSurface()
    {
        surfaceMultiplier = 1f;
    }

    public float GetMultiplier()
    {
        return surfaceMultiplier;
    }

    // ----- AĞIRLIK (yeni) -----

    // Taşınan ağırlığa göre hız çarpanı: boşken 1, ağırlaştıkça azalır
    public float GetWeightFactor()
    {
        if (storage == null) return 1f;   // depo yoksa ağırlık da yok

        float weight = storage.GetCarriedWeight();
        float factor = 1f - weight * slowPerWeight;

        // Alt sınır: hız hiçbir zaman minWeightFactor'ın altına inmesin
        return Mathf.Max(factor, minWeightFactor);
    }

    // ----- SONUÇ -----

    // Robotun şu anki gerçek hızı (RobotMove her fizik adımında bunu sorar)
    public float GetSpeed()
    {
        return baseSpeed * surfaceMultiplier * GetWeightFactor();
    }
        // ===== 5. HAFTA SÖZLEŞMESİ (Nazife dolduracak) =====

    // Temel hızı artır (market)
    public void IncreaseBaseSpeed(float amount) { }

    // Zemin tutuşu: 1 = normal, küçük = kaygan (yağ)
    public float GetGrip() { return 1f; }
    public void SetGrip(float grip) { }
}
