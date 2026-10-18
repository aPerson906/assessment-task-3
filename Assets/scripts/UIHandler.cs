using UnityEngine.UIElements;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    private VisualElement M_HealthBar;
    public static UIHandler instance { get; private set; }
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        M_HealthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealBar");
        SetHealthValue( 1.0f);
    }
    public void SetHealthValue(float percentage)
    {
        M_HealthBar.style.width = (Length.Percent(percentage * 100));
    }
}

