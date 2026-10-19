// SAHİBİ: Sümeyra

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;                            // takip edilecek obje: robot (Inspector'dan sürükle)
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -10f); // kameranın robota göre konumu: 12 m yukarıda, 10 m arkada
    [SerializeField] private float followSpeed = 5f;                      // ne kadar hızlı yetişsin (büyük = sıkı, küçük = gevşek)

    void Start()
    {
        if (target == null) return;

        // Oyun başında kamerayı direkt doğru yere koy (uzaktan süzülerek gelmesin)
        transform.position = target.position + offset;

        // Robota bir kere bak, bu açı oyun boyunca sabit kalacak
        transform.LookAt(target.position);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Kameranın olması gereken yer: robotun konumu + offset
        Vector3 wanted = target.position + offset;

        // Oraya bir anda değil, yumuşakça yaklaş
        transform.position = Vector3.Lerp(transform.position, wanted, followSpeed * Time.deltaTime);
    }
}