using UnityEngine;

public class Bobber : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.15f; // kaç metre yukarı ve aşağı
    [SerializeField] private float bobSpeed = 2f;     // ne kadar hızlı süzülecek

    private Vector3 startLocalPosition;

    void Start()
    {
        // Başladığımız yeri hatırla
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // -bobHeight ile +bobHeight arasında gidip gelen bir sayı
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        // Başlangıç noktasına göre yukarı veya aşağı kaydır
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}
