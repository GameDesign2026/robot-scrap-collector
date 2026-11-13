
using UnityEngine;
using UnityEngine.UI;

// SAHÝBÝ: Furkan
public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Image frame;

    [SerializeField]
    private Color idleColor =
        new Color(0f, 0f, 0f, 0.47f);

    [SerializeField]
    private Color activeColor =
        new Color(1f, 0.84f, 0.31f, 0.9f);

    private BatController bat;

    void Start()
    {
        bat = FindFirstObjectByType<BatController>();
    }

    void Update()
    {
        if (frame == null) return;

        bool swinging = bat != null && bat.IsSwinging();

        if (swinging)
        {
            frame.color = activeColor;
        }
        else
        {
            frame.color = idleColor;
        }
    }
}
