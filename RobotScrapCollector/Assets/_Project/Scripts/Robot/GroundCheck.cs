using UnityEngine;

// SAHİBİ: Esra
public class GroundCheck : MonoBehaviour
{
    // Hangi layer'lar zemin sayılır? Inspector'da "Ground" seçilmeli.
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private float rayStartHeight = 0.5f;   // ray, robotun tabanından ne kadar yukarıda başlasın
    [SerializeField] private float rayLength = 0.6f;        // ray ne kadar aşağı insin (0.5 + 0.1 pay)

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Güvenlik: Inspector'da maske "Nothing" bırakılmışsa, Ground'u kendimiz seç
        if (groundLayers == 0)
        {
            groundLayers = LayerMask.GetMask("Ground");
            Debug.LogWarning(name + ": Ground Layers was Nothing, set to Ground automatically");
        }
    }

    // Robot şu an yerde mi?
        public bool IsGrounded()
    {
        Vector3 start = rb.position + Vector3.up * rayStartHeight;
        bool grounded = Physics.Raycast(start, Vector3.down, rayLength, groundLayers);
        return grounded;
    }
}