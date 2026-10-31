using UnityEngine;
using UnityEngine.UI;


// SAHİBİ: Nagihan — envanteri ekranın altında gösterir
public class HotbarUI : MonoBehaviour
{
    [SerializeField] private RobotStorage storage;
    [SerializeField] private Image[] slotBackgrounds;
    [SerializeField] private Image[] slotIcons;

    [Header("Slot renkleri")]
    [SerializeField] private Color emptyColor = new Color(0f, 0f, 0f, 0.47f);
    [SerializeField] private Color lockedColor = new Color(0f, 0f, 0f, 0.9f);
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 1f, 0.6f);

    [Header("Çöp renkleri (yerdeki çöplerle aynı)")]
    [SerializeField] private Color normalColor = new Color(0.42f, 0.39f, 0.35f);
    [SerializeField] private Color metalColor = new Color(0.85f, 0.47f, 0.17f);
    [SerializeField] private Color electronicColor = new Color(0.18f, 0.8f, 0.44f);

    void Start()
    {
        if (storage == null)
        {
            GameObject robot = GameObject.FindWithTag("Player");
            if (robot != null) storage = robot.GetComponent<RobotStorage>();
        }
    }

    void Update()
    {
        if (storage == null) return;

        int unlocked = storage.GetUnlockedSlots();
        int selected = storage.GetSelectedIndex();

        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            if (i >= unlocked)
            {
                slotBackgrounds[i].color = lockedColor;
            }
            else if (i == selected)
            {
                slotBackgrounds[i].color = selectedColor;
            }
            else
            {
                slotBackgrounds[i].color = emptyColor;
            }

            TrashItem item = null;
            if (i < unlocked) item = storage.GetItemInSlot(i);

            if (item == null)
            {
                slotIcons[i].enabled = false;
            }
            else
            {
                slotIcons[i].enabled = true;
                slotIcons[i].color = GetColorFor(item.GetTrashType());
            }
        }
    }

    private Color GetColorFor(TrashType type)
    {
        switch (type)
        {
            case TrashType.Metal:
                return metalColor;
            case TrashType.Electronic:
                return electronicColor;
            default:
                return normalColor;
        }
    }
}