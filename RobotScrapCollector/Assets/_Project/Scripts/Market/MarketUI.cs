using UnityEngine;
using UnityEngine.UI;
using TMPro;

// SAHİBİ: Esra — market paneli: fiyatları gösterir, butonlar satın alır
public class MarketUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;   // açılıp kapanacak kutu

    [Header("Yazılar")]
    [SerializeField] private TMP_Text scrapInfo;
    [SerializeField] private TMP_Text slotInfo;
    [SerializeField] private TMP_Text batteryInfo;
    [SerializeField] private TMP_Text speedInfo;

    [Header("Butonlar")]
    [SerializeField] private Button slotButton;
    [SerializeField] private Button batteryButton;
    [SerializeField] private Button speedButton;
    [SerializeField] private Button closeButton;

    private UpgradeShop shop;       // Nazife
    private RobotStorage storage;   // Furkan
    private RobotMove move;         // Hamza

    void Start()
    {
        // Butonlara "tıklanınca şu fonksiyonu çalıştır" de
        slotButton.onClick.AddListener(OnBuySlot);
        batteryButton.onClick.AddListener(OnBuyBattery);
        speedButton.onClick.AddListener(OnBuySpeed);
        closeButton.onClick.AddListener(Close);

        // Oyun başında panel kapalı
        panel.SetActive(false);
    }

    // MarketZone çağırır: robot içeri girdi
    public void Open(GameObject robot)
    {
        shop = robot.GetComponent<UpgradeShop>();
        storage = robot.GetComponent<RobotStorage>();
        move = robot.GetComponent<RobotMove>();

        panel.SetActive(true);
        if (move != null) move.SetControlEnabled(false);   // robot dursun, imleç serbest
        Refresh();
    }

    // "Kapat" butonu
    public void Close()
    {
        panel.SetActive(false);
        if (move != null) move.SetControlEnabled(true);    // robot hareket etsin, imleç kilitli
    }

    private void OnBuySlot()
    {
        if (shop == null) return;
        shop.BuySlot();
        Refresh();
    }

    private void OnBuyBattery()
    {
        if (shop == null) return;
        shop.BuyBattery();
        Refresh();
    }

    private void OnBuySpeed()
    {
        if (shop == null) return;
        shop.BuySpeed();
        Refresh();
    }

    // Yazıları ve butonları güncelle
    private void Refresh()
    {
        if (shop == null || storage == null) return;

        int scrap = storage.GetScrap();
        scrapInfo.text = "Scrap: " + scrap;

        SetRow(slotInfo, slotButton, shop.GetSlotCost(), scrap);
        SetRow(batteryInfo, batteryButton, shop.GetBatteryCost(), scrap);
        SetRow(speedInfo, speedButton, shop.GetSpeedCost(), scrap);
    }

    // Bir satırı ayarla: fiyat yazısı ve butonun tıklanabilirliği
    private void SetRow(TMP_Text info, Button button, int cost, int scrap)
    {
        if (cost < 0)
        {
            info.text = "MAKS";
            button.interactable = false;          // maksimum seviye: tıklanamaz
            return;
        }

        info.text = cost + " Scrap";
        button.interactable = scrap >= cost;     // para yetiyorsa tıklanabilir
    }
}