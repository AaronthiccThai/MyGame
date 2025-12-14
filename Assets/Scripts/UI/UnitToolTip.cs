using TMPro;
using UnityEngine;

public class UnitToolTip : MonoBehaviour
{
    public static UnitToolTip Instance;

    public TextMeshProUGUI tooltipText; 
    void Awake()
    {
        gameObject.SetActive(false);
        Instance = this;
    }

    void Update()
    {
        // Follow mouse position
        Vector2 pos = Input.mousePosition;
        transform.position = pos + new Vector2(15f, -15f);
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
