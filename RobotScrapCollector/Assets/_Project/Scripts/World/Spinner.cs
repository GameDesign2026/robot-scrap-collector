using UnityEngine;

public class Spinner : MonoBehaviour
{
    // Saniyede kaç derece dönecek (Inspector'dan değiştirilebilir)
    [SerializeField] private float rotationSpeed = 90f;

    void Update()
    {
        // Her karede, Y ekseni etrafında biraz döndür
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}