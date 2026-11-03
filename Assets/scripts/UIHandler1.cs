using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    public Image healthFill; // The red/green bar image that shrinks as health drops

    void Awake()
    {
        instance = this;
    }

    // Called by PlayerController: 1 = full health, 0.5 = half, 0 = empty
    public void SetHealthValue(float percentage)
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = percentage;
        }
    }
}
