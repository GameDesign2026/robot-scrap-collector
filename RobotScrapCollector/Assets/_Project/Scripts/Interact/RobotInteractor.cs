using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// SAHİBİ: Sümeyra — bakılan çöpü bulur, vurgular, E ile alır
public class RobotInteractor : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;     // ışının çıktığı yer (robotun CameraPivot'u)
    [SerializeField] private float range = 2.5f;      // en fazla kaç metre uzaktaki çöp alınabilir
    [SerializeField] private TMP_Text promptText;     // ekrandaki "E – Al" yazısı

    private RobotStorage storage;
    private InputAction interactAction;               // E tuşu
    private TrashItem current;                        // şu an bakılan çöp

    void Awake()
    {
        storage = GetComponent<RobotStorage>();

        // Slot boşsa robotun CameraPivot'unu kendisi bul
        if (rayOrigin == null) rayOrigin = transform.Find("CameraPivot");
    }

    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        SetPrompt("");
    }

    void Update()
    {
        // 1) Neye bakıyoruz?
        TrashItem target = FindTarget();

        // 2) Bakılan çöp değiştiyse: eskisinin vurgusunu kaldır, yenisini vurgula
        if (target != current)
        {
            if (current != null) current.SetHighlight(false);
            current = target;
            if (current != null) current.SetHighlight(true);
        }

        // 3) Hiçbir çöpe bakmıyorsak yazıyı temizle
        if (current == null)
        {
            SetPrompt("");
            return;
        }

        // 4) Envanterde yer var mı?
        if (!storage.CanCollect())
        {
            SetPrompt("Envanter dolu");
            return;
        }

        SetPrompt("E – Al");

        // 5) E'ye basıldıysa al
        if (!interactAction.WasPressedThisFrame()) return;

        if (storage.TryAddItem(current))
        {
            current.OnPickedUp();   // çöpü gizle (TryAddItem sadece slota koyar)
            current = null;
            SetPrompt("");
        }
        else
        {
            Debug.Log("Storage refused " + current.name);
        }
    }

    // Kameradan ileri ışın gönder, en yakın alınabilir çöpü bul
    private TrashItem FindTarget()
    {
        if (rayOrigin == null) return null;

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        // Işının çarptığı HER ŞEY (trigger'lar dahil)
        RaycastHit[] hits = Physics.RaycastAll(ray, range, ~0, QueryTriggerInteraction.Collide);

        TrashItem best = null;
        float bestDistance = range;   // en yakın çöpün uzaklığı
        float wallDistance = range;   // en yakın katı engelin uzaklığı

        foreach (RaycastHit hit in hits)
        {
            TrashItem item = hit.collider.GetComponent<TrashItem>();

            if (item != null)
            {
                // Bir çöp: alınabilir ve şimdiye kadarkinden yakınsa aday
                if (item.CanBePickedUp() && hit.distance < bestDistance)
                {
                    best = item;
                    bestDistance = hit.distance;
                }
            }
            else if (!hit.collider.isTrigger && hit.distance < wallDistance)
            {
                // Çöp değil ve katı: duvar, bina... en yakınını hatırla
                wallDistance = hit.distance;
            }
            // Çöp olmayan trigger'lar (çamur, şarj...) görmezden gelinir
        }

        // Çöp, engelden önce geliyorsa al. Duvarın arkasındaysa alma.
        if (best != null && bestDistance <= wallDistance) return best;
        return null;
    }

    private void SetPrompt(string text)
    {
        if (promptText != null) promptText.text = text;
    }
        // ===== AŞAMA 3 SÖZLEŞMESİ (Sümeyra dolduracak) =====

    // Mıknatıs geliştirmesi: E ile alma mesafesi (Nazife çağırır)
    public void SetRange(float newRange)
    {
        if (newRange > 0f) range = newRange;
    }
}
