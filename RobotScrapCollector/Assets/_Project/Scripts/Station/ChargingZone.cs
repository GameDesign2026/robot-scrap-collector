
using UnityEngine;

//  Nagihan
public class ChargingZone : MonoBehaviour
{
    // Bir şey bu trigger'ın içine GİRDİĞİ anda çalışır
    void OnTriggerEnter(Collider other)
    {
        // KİM? Giren şeyin bataryası var mı? (yani robot mu?)
        RobotBattery battery = other.GetComponent<RobotBattery>();
        if (battery == null) return;

        // YAP: bataryayı doldur
        battery.RechargeFull();
        Debug.Log("Charging station: robot charged");
    }
}