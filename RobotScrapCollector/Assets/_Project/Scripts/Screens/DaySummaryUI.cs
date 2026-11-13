using UnityEngine;
using UnityEngine.UI;
using TMPro;

// SAHİBİ: Esra — gün sonu özet ekranı
public class DaySummaryUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private TMP_Text penaltyText;    // YENİ
    [SerializeField] private Button nextDayButton;
    [SerializeField] private int penaltyPerTrash = 1; // YENİ: Ali'nin ayarıyla aynı olmalı

    [SerializeField] private Color successColor = new Color(0.24f, 0.86f, 0.52f);
    [SerializeField] private Color failColor = new Color(1f, 0.32f, 0.32f);

    void Awake()
    {
        panel.SetActive(false);
    }

    void Start()
    {
        nextDayButton.onClick.AddListener(OnNextDay);
    }

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

        string sign = scrapEarned >= 0 ? "+" : "";
        scrapText.text = "Bugün kazanılan: " + sign + scrapEarned + " Scrap";

        // YENİ: ceza satırı sadece çöp kaldıysa görünür
        if (penaltyText != null)
        {
            if (trashLeft > 0)
            {
                int penalty = trashLeft * penaltyPerTrash;
                penaltyText.text = "Ceza: -" + penalty + " Scrap (" + trashLeft + " çöp kaldı)";
                penaltyText.gameObject.SetActive(true);
            }
            else
            {
                penaltyText.gameObject.SetActive(false);
            }
        }

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

    private void OnNextDay()
    {
        DayClock clock = FindFirstObjectByType<DayClock>();
        if (clock != null)
        {
            clock.StartNewDay();
        }
        else
        {
            Hide();
        }
    }
}