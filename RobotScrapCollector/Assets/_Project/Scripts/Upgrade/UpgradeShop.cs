using UnityEngine;

// SAHİBİ: Nazife. İSKELET: fonksiyon adlarını değiştirmeyin (Esra'nın market butonları kullanıyor).
public class UpgradeShop : MonoBehaviour
{
    public bool BuySlot()    { return false; }
    public bool BuyBattery() { return false; }
    public bool BuySpeed()   { return false; }

    // Bir sonraki seviyenin fiyatı (-1 = maksimum seviyeye ulaşıldı)
    public int GetSlotCost()    { return 0; }
    public int GetBatteryCost() { return 0; }
    public int GetSpeedCost()   { return 0; }
}