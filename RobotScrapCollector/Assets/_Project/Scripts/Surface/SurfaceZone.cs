using UnityEngine;

// SAHİBİ: Nazife
public class SurfaceZone : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.5f;   // çamur 0.5, yağ 1.5

    // Robot bölgeye GİRDİĞİNDE
    void OnTriggerEnter(Collider other)
    {
        // KİM? Giren şeyin hız bileşeni var mı? (yani robot mu?)
        RobotSpeed speed = other.GetComponent<RobotSpeed>();
        if (speed == null) return;

        // YAP: hızı değiştir
        speed.SetSurface(speedMultiplier);
        Debug.Log(name + ": speed x" + speedMultiplier);
    }

    // Robot bölgeden ÇIKTIĞINDA
    void OnTriggerExit(Collider other)
    {
        RobotSpeed speed = other.GetComponent<RobotSpeed>();
        if (speed == null) return;

        // YAP: normal hıza dön
        speed.ResetSurface();
        Debug.Log(name + ": speed back to normal");
    }
}