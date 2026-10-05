using UnityEngine;

public class Blinker : MonoBehaviour
{
    [SerializeField] private float blinkInterval = 0.5f; // kaç saniyede bir yanıp sönecek

    private float timer;            // geçen süreyi sayan sayaç
    private Renderer lampRenderer;  // objeyi ekranda çizen bileşen

    void Start()
    {
        // Bu objenin Renderer bileşenini bul ve hatırla
        lampRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Her karede, geçen süreyi sayaca ekle
        timer += Time.deltaTime;

        // Süre dolduysa
        if (timer >= blinkInterval)
        {
            timer = 0f;                                // sayacı sıfırla
            lampRenderer.enabled = !lampRenderer.enabled; // yanıksa söndür, sönükse yak
        }
    }
}