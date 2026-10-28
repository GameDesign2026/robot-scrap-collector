using UnityEngine;

// SAHİBİ: Esra — robot market bölgesine girince paneli açar
public class MarketZone : MonoBehaviour
{
    private MarketUI marketUI;   // sahnedeki market paneli

    void Start()
    {
        // Sahnede MarketUI script'i olan objeyi bul
        marketUI = FindFirstObjectByType<MarketUI>();

        if (marketUI == null)
        {
            Debug.LogWarning("MarketZone: sahnede MarketUI yok, panel açılamaz");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // KİM? Sadece robot (Player tag'i)
        if (!other.CompareTag("Player")) return;
        if (marketUI == null) return;

        // YAP: paneli aç
        marketUI.Open(other.gameObject);
    }
}