using UnityEngine;

// SAHİBİ: Nazife
public class RobotSpeed : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3f;          // normal hız (market ile artar)
    [SerializeField] private float slowPerWeight = 0.1f;    // her 1 ağırlık hızı ne kadar düşürür
    [SerializeField] private float minWeightFactor = 0.4f;  // ağırlık hızı en fazla bu orana düşürebilir

    private float surfaceMultiplier = 1f;   // zemin hız çarpanı: normal 1, çamur 0.5, yağ 1.5
    private float grip = 1f;                // zemin tutuşu: normal 1, yağ 0.15
    private RobotStorage storage;

    void Awake()
    {
        storage = GetComponent<RobotStorage>();
    }

    // ----- ZEMİN -----

    public void SetSurface(float multiplier)
    {
        if (multiplier <= 0f) return;
        surfaceMultiplier = multiplier;
    }

    // Zeminden çıkınca hem hız çarpanı hem tutuş normale döner
    public void ResetSurface()
    {
        surfaceMultiplier = 1f;
        grip = 1f;
    }

    public float GetMultiplier()
    {
        return surfaceMultiplier;
    }

    // ----- GRIP (yeni) -----

    public float GetGrip()
    {
        return grip;
    }

    public void SetGrip(float newGrip)
    {
        // 0.01 ile 1 arasında tut: 0 olursa robot hiç hızlanamaz, 1'den büyük anlamsız
        grip = Mathf.Clamp(newGrip, 0.01f, 1f);
    }

    // ----- MARKET (yeni) -----

    public void IncreaseBaseSpeed(float amount)
    {
        if (amount <= 0f) return;   // eksi ya da sıfır artış anlamsız
        baseSpeed += amount;
        Debug.Log("Base speed: " + baseSpeed);
    }

    // ----- AĞIRLIK (geçen hafta) -----

    public float GetWeightFactor()
    {
        if (storage == null) return 1f;
        float weight = storage.GetCarriedWeight();
        float factor = 1f - weight * slowPerWeight;
        return Mathf.Max(factor, minWeightFactor);
    }

    // ----- SONUÇ -----

    public float GetSpeed()
    {
        return baseSpeed * surfaceMultiplier * GetWeightFactor();
    }
}
