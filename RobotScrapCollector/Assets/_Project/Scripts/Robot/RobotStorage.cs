using UnityEngine;
using UnityEngine.InputSystem;

// SAHİBİ: Furkan — slotlu envanter
public class RobotStorage : MonoBehaviour
{
    [SerializeField] private int maxSlots = 5;          // toplam slot (kilitliler dahil)
    [SerializeField] private int unlockedSlots = 1;     // başta kaç slot açık
    [SerializeField] private float dropDistance = 1.2f; // çöp robotun ne kadar önüne atılsın
    [SerializeField] private float dropHeight = 0.8f;   // ne kadar yukarıdan atılsın

    private TrashItem[] slots;      // slotlar: her biri bir çöp ya da null (boş)
    private int selectedIndex;      // seçili slot (0'dan başlar)
    private int scrap;              // toplam kazanılan Scrap

    private InputAction dropAction;     // Q
    private InputAction scrollAction;   // fare tekerleği

    void Awake()
    {
        // 5 boş kutuluk diziyi oluştur
        slots = new TrashItem[maxSlots];
    }

    void Start()
    {
        dropAction = InputSystem.actions.FindAction("Drop");
        scrollAction = InputSystem.actions.FindAction("SlotScroll");
    }

    void Update()
    {
        ReadNumberKeys();
        ReadScroll();

        // Q: seçili slottaki çöpü at
        if (dropAction != null && dropAction.WasPressedThisFrame())
        {
            DropSelected();
        }
    }

    // ===== SLOT SEÇİMİ =====

    // 1–5 tuşları (doğrudan klavyeden)
    private void ReadNumberKeys()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;   // klavye takılı değilse

        if (keyboard.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (keyboard.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (keyboard.digit3Key.wasPressedThisFrame) SelectSlot(2);
        if (keyboard.digit4Key.wasPressedThisFrame) SelectSlot(3);
        if (keyboard.digit5Key.wasPressedThisFrame) SelectSlot(4);
    }

    // Fare tekerleği (action ile): aşağı = sonraki, yukarı = önceki
    private void ReadScroll()
    {
        if (scrollAction == null) return;

        float scroll = scrollAction.ReadValue<float>();
        if (scroll < 0f) SelectSlot((selectedIndex + 1) % unlockedSlots);
        if (scroll > 0f) SelectSlot((selectedIndex - 1 + unlockedSlots) % unlockedSlots);
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= unlockedSlots) return;   // kilitli slot seçilemez
        selectedIndex = index;
    }

    // ===== ÇÖP ALMA (Sümeyra'nın etkileşimi çağırır) =====

    // Açık slotlardan en az biri boş mu?
    public bool CanCollect()
    {
        for (int i = 0; i < unlockedSlots; i++)
        {
            if (slots[i] == null) return true;
        }
        return false;
    }

    // Çöpü ilk boş slota koy. Koyabildiyse true. (Çöpü gizlemez, onu Sümeyra yapar.)
    public bool TryAddItem(TrashItem item)
    {
        if (item == null) return false;

        for (int i = 0; i < unlockedSlots; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                Debug.Log(item.name + " → slot " + (i + 1));
                return true;
            }
        }
        return false;   // boş slot yok
    }

    // ===== ÇÖP ATMA (Q) =====

    public void DropSelected()
    {
        TrashItem item = slots[selectedIndex];
        if (item == null) return;   // seçili slot boş

        slots[selectedIndex] = null;   // slotu boşalt

        // Atılacak nokta: robotun önü, biraz yukarısı
        Vector3 start = transform.position + Vector3.up * dropHeight;
        Vector3 forward = transform.forward;
        float distance = dropDistance;

        // Önümüzde duvar varsa çöp duvarın içine girmesin: duvarın biraz önüne koy
        RaycastHit hit;
        if (Physics.Raycast(start, forward, out hit, dropDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            distance = hit.distance - 0.3f;
            if (distance < 0f) distance = 0f;
        }

        Vector3 dropPosition = start + forward * distance;
        item.OnDropped(dropPosition, forward);   // Sümeyra'nın fonksiyonu: görünür ol, fırla
    }

    // ===== TESLİM (Esra'nın teslim noktası çağırır) =====

    public void Unload()
    {
        int total = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;   // boş slotu atla

            total += slots[i].GetValue();
            slots[i].OnDelivered();   // Sümeyra'nın fonksiyonu: üreticiye haber ver, yok ol
            slots[i] = null;
        }

        if (total == 0) return;   // teslim edilecek bir şey yoktu

        scrap += total;
        Debug.Log("Delivered " + total + " · Scrap: " + scrap);
    }

    // ===== MARKET (Nazife'nin UpgradeShop'u çağırır) =====

    public void UnlockSlot()
    {
        if (unlockedSlots >= maxSlots) return;   // zaten hepsi açık
        unlockedSlots++;
    }

    // Scrap harca. Yeterliyse düşer ve true döner.
    public bool SpendScrap(int amount)
    {
        if (amount < 0) return false;
        if (scrap < amount) return false;   // yetmiyor
        scrap -= amount;
        return true;
    }

    // ===== BİLGİ (diğer script'ler okur) =====

    public int GetScrap() { return scrap; }
    public int GetSelectedIndex() { return selectedIndex; }
    public int GetUnlockedSlots() { return unlockedSlots; }
    public int GetMaxSlots() { return slots.Length; }
    public int GetCapacity() { return unlockedSlots; }

    public TrashItem GetItemInSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return null;   // geçersiz numara
        return slots[index];
    }

    // Kaç slot dolu
    public int GetItemCount()
    {
        int count = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) count++;
        }
        return count;
    }

    // Taşınan toplam ağırlık
    public float GetCarriedWeight()
    {
        float total = 0f;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) total += slots[i].GetWeight();
        }
        return total;
    }
        // ===== AŞAMA 3 SÖZLEŞMESİ (Furkan dolduracak) =====

    // Ceza: Scrap'tan düş, eksiye düşebilir (Ali çağırır)
    public void RemoveScrap(int amount) { }
}
