using UnityEngine;

// SAHİBİ: Nazife — çamur ve yağ bölgeleri
public class SurfaceZone : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.5f;   // çamur 0.5, yağ 1.5
    [SerializeField] private float grip = 1f;                // çamur 1, yağ 0.15 (küçük = kaygan)

    // Robot bölgeye GİRDİĞİNDE
    void OnTriggerEnter(Collider other)
    {
        RobotSpeed speed = other.GetComponent<RobotSpeed>();
        if (speed == null) return;

        speed.SetSurface(speedMultiplier);
        speed.SetGrip(grip);
        Debug.Log(name + ": speed x" + speedMultiplier + " · grip " + grip);
    }

    // Robot bölgeden ÇIKTIĞINDA
    void OnTriggerExit(Collider other)
    {
        RobotSpeed speed = other.GetComponent<RobotSpeed>();
        if (speed == null) return;

        speed.ResetSurface();   // hem hız hem grip normale döner
        Debug.Log(name + ": back to normal");
    }
}
