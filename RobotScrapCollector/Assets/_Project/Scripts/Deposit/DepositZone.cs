using UnityEngine;

// SAHİBİ: Esra
public class DepositZone : MonoBehaviour
{
    // Bir şey bu trigger'ın içine GİRDİĞİ anda çalışır
    void OnTriggerEnter(Collider other)
    {
        // KİM? Giren şeyin deposu var mı? (yani robot mu?)
        RobotStorage storage = other.GetComponent<RobotStorage>();
        if (storage == null) return;   // deposu yoksa robot değil, hiçbir şey yapma

        // YAP: depoyu boşalt, Scrap'e çevir
        storage.Unload();
        Debug.Log("Deposit zone: robot arrived");
    }
}