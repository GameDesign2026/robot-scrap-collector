using UnityEngine;

// SAHİBİ: Furkan
public class CarryIndicator : MonoBehaviour
{
    [SerializeField] private Transform cargoBox;          // robotun sırtındaki sandık (Inspector'dan sürükle)
    [SerializeField] private float baseHeight = 0.2f;     // en hafif yükte sandığın yüksekliği
    [SerializeField] private float heightPerWeight = 0.1f; // her 1 ağırlık sandığı ne kadar yükseltir

    private RobotStorage storage;

    void Awake()
    {
        storage = GetComponent<RobotStorage>();
    }

    void Update()
    {
        if (storage == null || cargoBox == null) return;   // güvenlik: bir şey eksikse hiçbir şey yapma

        // Depoda çöp var mı?
        bool carrying = storage.GetItemCount() > 0;

        // Varsa sandığı göster, yoksa gizle
        cargoBox.gameObject.SetActive(carrying);
        if (!carrying) return;

        // Sandığın yüksekliği: taban + ağırlık × artış
        float height = baseHeight + storage.GetCarriedWeight() * heightPerWeight;

        // Ölçeği değiştir: kopyala → değiştir → geri ver
        Vector3 scale = cargoBox.localScale;
        scale.y = height;
        cargoBox.localScale = scale;
    }
}