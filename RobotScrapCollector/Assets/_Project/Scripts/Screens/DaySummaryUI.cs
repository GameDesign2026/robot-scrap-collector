using UnityEngine;
using UnityEngine.UI;
using TMPro;

// SAHİBİ: Esra — gün sonu özet ekranı
public class DaySummaryUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;        // açılıp kapanan kısım (Canvas'ın içindeki Panel)
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private Button nextDayButton;

    [SerializeField] private Color successColor = new Color(0.24f, 0.86f, 0.52f);   // yeşil
    [SerializeField] private Color failColor = new Color(1f, 0.32f, 0.32f);         // kırmızı

    void Awake()
    {
        // Oyun başında gizli
        panel.SetActive(false);
    }

    void Start()
    {
        nextDayButton.onClick.AddListener(OnNextDay);
    }

    // Ali'nin saati çağırır: gün bitti
    public void Show(int day, int scrapEarned, int trashLeft, bool success)
    {
        titleText.text = "GÜN " + day + " BİTTİ";

        if (success)
        {
            resultText.text = "BAHÇE TEMİZ!";
            resultText.color = successColor;
        }
        else
        {
            resultText.text = "BAHÇEDE " + trashLeft + " ÇÖP KALDI";
            resultText.color = failColor;
        }

        // Pozitifse başına + koy: "+23", negatifse zaten "-5" yazılır
        string sign = scrapEarned >= 0 ? "+" : "";
        scrapText.text = "Bugün kazanılan: " + sign + scrapEarned + " Scrap";

        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public bool IsOpen()
    {
        return panel.activeSelf;
    }

    // "Sonraki Gün" butonu
    private void OnNextDay()
    {
        DayClock clock = FindFirstObjectByType<DayClock>();
        if (clock != null)
        {
            clock.StartNewDay();   // Ali'nin fonksiyonu: saat 08:00'e döner, o da Hide()'ı çağırır
        }
        else
        {
            Hide();   // saat yoksa en azından ekranı kapat
        }
    }
}