using UnityEngine;

public class LookAround : MonoBehaviour
{
    [SerializeField] private float lookAngle = 30f; // sağa ve sola en fazla kaç derece dönecek
    [SerializeField] private float lookSpeed = 1f;  // ne kadar hızlı bakınacak

    void Update()
    {
        // -lookAngle ile +lookAngle arasında gidip gelen bir açı
        float angle = Mathf.Sin(Time.time * lookSpeed) * lookAngle;

        // Bu açıyı Y ekseni etrafında dönüşe çevir ve uygula
        transform.localRotation = Quaternion.Euler(0f, angle, 0f);
    }
}