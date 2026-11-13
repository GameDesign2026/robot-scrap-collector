using UnityEngine;
using UnityEngine.UI;
using TMPro;

// SAHİBİ: Esra — market paneli
public class MarketUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text promptText;

    [Header("Yazılar")]
    [SerializeField] private TMP_Text scrapInfo;
    [SerializeField] private TMP_Text slotInfo;
    [SerializeField] private TMP_Text batteryInfo;
    [SerializeField] private TMP_Text speedInfo;
    [SerializeField] private TMP_Text magnetInfo;     // YENİ
    [SerializeField] private TMP_Text batInfo;        // YENİ

    [Header("Butonlar")]
    [SerializeField] private Button slotButton;
    [SerializeField] private Button batteryButton;
    [SerializeField] private Button speedButton;
    [SerializeField] private Button magnetButton;     // YENİ
    [SerializeField] private Button batButton;        // YENİ
    [SerializeField] private Button closeButton;

    private UpgradeShop shop;
    private RobotStorage storage;
    private RobotMove move;
    private bool promptAllowed;

    void Start()
    {
        slotButton.onClick.AddListener(OnBuySlot);
        batteryButton.onClick.AddListener(OnBuyBattery);
        speedButton.onClick.AddListener(OnBuySpeed);
        magnetButton.onClick.AddListener(OnBuyMagnet);    // YENİ
        batButton.onClick.AddListener(OnBuyBat);          // YENİ
        closeButton.onClick.AddListener(Close);

        panel.SetActive(false);
        RefreshPrompt();
    }

    public void Open(GameObject robot)
    {
        shop = robot.GetComponent<UpgradeShop>();
        storage = robot.GetComponent<RobotStorage>();
        move = robot.GetComponent<RobotMove>();

        panel.SetActive(true);
        if (move != null) move.SetControlEnabled(false);
        Refresh();
        RefreshPrompt();
    }

    public void Close()
    {
        panel.SetActive(false);
        if (move != null) move.SetControlEnabled(true);
        RefreshPrompt();
    }

    public bool IsOpen()
    {
        return panel.activeSelf;
    }

    public void SetPromptVisible(bool allowed)
    {
        promptAllowed = allowed;
        RefreshPrompt();
    }

    private void RefreshPrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(promptAllowed && !panel.activeSelf);
    }

    private void OnBuySlot()    { if (shop == null) return; shop.BuySlot();     Refresh(); }
    private void OnBuyBattery() { if (shop == null) return; shop.BuyBattery();  Refresh(); }
    private void OnBuySpeed()   { if (shop == null) return; shop.BuySpeed();    Refresh(); }
    private void OnBuyMagnet()  { if (shop == null) return; shop.BuyMagnet();   Refresh(); }   // YENİ
    private void OnBuyBat()     { if (shop == null) return; shop.BuyBatPower(); Refresh(); }   // YENİ

    private void Refresh()
    {
        if (shop == null || storage == null) return;

        int scrap = storage.GetScrap();
        scrapInfo.text = "Scrap: " + scrap;

        SetRow(slotInfo, slotButton, shop.GetSlotCost(), scrap);
        SetRow(batteryInfo, batteryButton, shop.GetBatteryCost(), scrap);
        SetRow(speedInfo, speedButton, shop.GetSpeedCost(), scrap);
        SetRow(magnetInfo, magnetButton, shop.GetMagnetCost(), scrap);      // YENİ
        SetRow(batInfo, batButton, shop.GetBatPowerCost(), scrap);          // YENİ
    }

    private void SetRow(TMP_Text info, Button button, int cost, int scrap)
    {
        if (info == null || button == null) return;   // YENİ: slot boşsa hata vermesin

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