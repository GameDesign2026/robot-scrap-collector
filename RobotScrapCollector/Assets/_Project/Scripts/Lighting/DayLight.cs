using UnityEngine;

// SAHİBİ: Nazife — güneş, saate göre döner ve renk değiştirir
[RequireComponent(typeof(Light))]
public class DayLight : MonoBehaviour
{
    [Header("Günün saatleri")]
    [SerializeField] private float startHour = 8f;
    [SerializeField] private float endHour = 20f;

    [Header("Güneşin yolu")]
    [SerializeField] private float sunriseAngle = 15f;    // 08:00'de yükseklik (doğuda alçak)
    [SerializeField] private float sunsetAngle = 165f;    // 20:00'de (batıda alçak)
    [SerializeField] private float direction = -30f;      // güneşin geldiği yön (Y açısı)

    [Header("Renk ve parlaklık (0 = 08:00, 1 = 20:00)")]
    [SerializeField] private Gradient sunColor = DefaultColors();
    [SerializeField] private AnimationCurve intensity = new AnimationCurve(
        new Keyframe(0f, 0.8f),     // sabah
        new Keyframe(0.5f, 1.2f),   // öğlen: en parlak
        new Keyframe(0.85f, 0.9f),  // akşam
        new Keyframe(1f, 0.5f));    // gün batımı

    [Header("Test (Ali'nin saati yokken)")]
    [SerializeField] private bool useTestHour = false;
    [SerializeField, Range(8f, 20f)] private float testHour = 8f;

    private Light sun;
    private DayClock clock;

    void Awake()
    {
        sun = GetComponent<Light>();
    }

    void Start()
    {
        clock = FindFirstObjectByType<DayClock>();
    }

    void Update()
    {
        // Saati al
        float hour = testHour;
        if (!useTestHour && clock != null) hour = clock.GetHour();

        // 08:00 → 0, 20:00 → 1
        float t = Mathf.InverseLerp(startHour, endHour, hour);

        // 1) Güneşin açısı: doğudan batıya bir yay
        float angle = Mathf.Lerp(sunriseAngle, sunsetAngle, t);
        transform.rotation = Quaternion.Euler(angle, direction, 0f);

        // 2) Renk
        sun.color = sunColor.Evaluate(t);

        // 3) Parlaklık
        sun.intensity = intensity.Evaluate(t);
    }

    // Varsayılan renk şeridi: sabah sarımsı → öğlen beyaz → akşam turuncu → kızıl
    private static Gradient DefaultColors()
    {
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(1f, 0.85f, 0.63f), 0f),    // 08:00 FFD9A0
                new GradientColorKey(Color.white, 0.5f),                  // 14:00
                new GradientColorKey(new Color(1f, 0.69f, 0.4f), 0.75f),  // 17:00 FFB066
                new GradientColorKey(new Color(1f, 0.48f, 0.27f), 1f),    // 20:00 FF7A45
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            });
        return g;
    }
}
