using UnityEngine;
using UnityEngine.UI;
using TMPro;

// SAHİBİ: Esra — market paneli
public class MarketUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;        // açılıp kapanan kutu (MarketPanel'in içindeki Panel)
    [SerializeField] private TMP_Text promptText;     // YENİ: "E – Market" yazısı

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

    private UpgradeShop shop;
    private RobotStorage storage;
    private RobotMove move;
    private bool promptAllowed;   // YENİ: robot markette mi? (yazı gösterilebilir mi?)

    void Start()
    {
        slotButton.onClick.AddListener(OnBuySlot);
        batteryButton.onClick.AddListener(OnBuyBattery);
        speedButton.onClick.AddListener(OnBuySpeed);
        closeButton.onClick.AddListener(Close);

        panel.SetActive(false);
        RefreshPrompt();   // oyun başında yazı gizli
    }

    public void Open(GameObject robot)
    {
        shop = robot.GetComponent<UpgradeShop>();
        storage = robot.GetComponent<RobotStorage>();
        move = robot.GetComponent<RobotMove>();

        panel.SetActive(true);
        if (move != null) move.SetControlEnabled(false);
        Refresh();
        RefreshPrompt();   // panel açıkken yazıyı gizle
    }

    public void Close()
    {
        panel.SetActive(false);
        if (move != null) move.SetControlEnabled(true);
        RefreshPrompt();   // robot hâlâ içerideyse yazı tekrar görünsün
    }

    // YENİ: panel şu an açık mı? (MarketZone sorar)
    public bool IsOpen()
    {
        return panel.activeSelf;
    }

    // YENİ: MarketZone, robot girince true, çıkınca false verir
    public void SetPromptVisible(bool allowed)
    {
        promptAllowed = allowed;
        RefreshPrompt();
    }

    // Yazı sadece robot içerideyken VE panel kapalıyken görünür
    private void RefreshPrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(promptAllowed && !panel.activeSelf);
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

    private void Refresh()
    {
        if (shop == null || storage == null) return;

        int scrap = storage.GetScrap();
        scrapInfo.text = "Scrap: " + scrap;

        SetRow(slotInfo, slotButton, shop.GetSlotCost(), scrap);
        SetRow(batteryInfo, batteryButton, shop.GetBatteryCost(), scrap);
        SetRow(speedInfo, speedButton, shop.GetSpeedCost(), scrap);
    }

    private void SetRow(TMP_Text info, Button button, int cost, int scrap)
    {
        if (cost < 0)
        {
            info.text = "MAKS";
            button.interactable = false;
            return;
        }

        info.text = cost + " Scrap";
        button.interactable = scrap >= cost;
    }
}