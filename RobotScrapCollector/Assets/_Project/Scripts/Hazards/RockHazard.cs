using UnityEngine;
using Unity.Cinemachine;

// SAHİBİ: Nazife — robot taşa çarparsa seçili çöpü düşürür ve kamera sarsılır
public class RockHazard : MonoBehaviour
{
    [SerializeField] private float cooldown = 1f;   // aynı taş kaç saniye sonra tekrar düşürebilir

    private CinemachineImpulseSource shake;   // sarsıntı gönderici (yoksa null)
    private float readyAt;                    // bu zamandan önce tekrar çalışmaz

    void Start()
    {
        // Bu taşın üzerindeki sarsıntı göndericiyi bul
        shake = GetComponent<CinemachineImpulseSource>();
    }

    // Detect: bir şey taşın alanına girdi
    void OnTriggerEnter(Collider other)
    {
        // Who? sadece robot
        if (!other.CompareTag("Player")) return;

        // Bekleme süresi dolmadıysa bir şey yapma
        if (Time.time < readyAt) return;

        RobotStorage storage = other.GetComponent<RobotStorage>();
        if (storage == null) return;

        // Seçili slotta çöp var mı? Yoksa düşecek bir şey de yok
        TrashItem selected = storage.GetItemInSlot(storage.GetSelectedIndex());
        if (selected == null) return;

        // Act: seçili çöpü düşür
        storage.DropSelected();
        Debug.Log(name + " knocked off " + selected.name + "!");

        // ...ve kamerayı kısaca sars (sadece gerçekten bir şey düştüyse)
        if (shake != null) shake.GenerateImpulse();

        // Bir sonraki düşürme için bekle
        readyAt = Time.time + cooldown;
    }
}
