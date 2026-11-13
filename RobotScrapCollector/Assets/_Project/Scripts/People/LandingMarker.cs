using UnityEngine;

// SAHİBİ: Nagihan — çöpün düşeceği yerde küçülen kırmızı halka
public class LandingMarker : MonoBehaviour
{
    [SerializeField] private float startSize = 1.6f;
    [SerializeField] private float endSize = 0.5f;
    [SerializeField] private float thickness = 0.01f;
    [SerializeField] private float pulseSpeed = 8f;

    private float duration = 1f;
    private float timer;

    // Yaya fırlatınca çağırır: çöpün uçuş süresi
    public void Init(float flightTime)
    {
        duration = Mathf.Max(0.1f, flightTime);

        // Güvenlik: çöp beni silmeyi unutursa, uçuş + 1.5 sn sonra kendimi sil
        Destroy(gameObject, duration + 1.5f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / duration);
        float size = Mathf.Lerp(startSize, endSize, t);
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.05f;

        transform.localScale = new Vector3(size * pulse, thickness, size * pulse);
    }
}