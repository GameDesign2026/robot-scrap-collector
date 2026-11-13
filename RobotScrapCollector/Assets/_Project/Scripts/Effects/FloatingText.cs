using UnityEngine;
using TMPro;

// SAHİBİ: Sümeyra — bir noktada beliren, yukarı süzülüp kaybolan yazı
public class FloatingText : MonoBehaviour
{
    private const float Lifetime = 1.2f;    // kaç saniye görünür
    private const float RiseSpeed = 1.2f;   // saniyede kaç metre yükselir

    private TextMeshPro label;
    private Color baseColor;
    private float timer;
    private Transform cam;

    // Herkes çağırabilir: FloatingText.Spawn(konum, "yazı", renk);
    public static void Spawn(Vector3 position, string text, Color color)
    {
        GameObject obj = new GameObject("FloatingText");
        obj.transform.position = position;

        FloatingText floating = obj.AddComponent<FloatingText>();
        floating.Setup(text, color);
    }

    private void Setup(string text, Color color)
    {
        label = gameObject.AddComponent<TextMeshPro>();
        label.text = text;
        label.fontSize = 5;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.NoWrap;   // tek satırda kalsın
        label.color = color;

        // Siyah dış çizgi: açık renkli gökyüzünde de okunsun
        label.outlineWidth = 0.25f;
        label.outlineColor = Color.black;

        baseColor = color;
        if (Camera.main != null) cam = Camera.main.transform;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Yukarı süzül
        transform.position += Vector3.up * RiseSpeed * Time.deltaTime;

        // Yavaşça saydamlaş: başta tam görünür, sonda görünmez
        Color c = baseColor;
        c.a = 1f - (timer / Lifetime);
        label.color = c;

        if (timer >= Lifetime) Destroy(gameObject);
    }

    void LateUpdate()
    {
        // Billboard: her zaman kameraya bak
        if (cam != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
        }
    }
}
