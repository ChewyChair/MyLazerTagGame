using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrenadeController : MonoBehaviour
{
    public TMP_Text grenadeCountText;
    public Button grenadeButton;

    private int grenadeCount = 2; // Starting grenades

    private void Start()
    {
        UpdateGrenadeText();

        grenadeButton.onClick.AddListener(ThrowGrenade);
    }

    private void ThrowGrenade()
    {
        if (grenadeCount > 0)
        {
            grenadeCount--;
            UpdateGrenadeText();
        }

        // If grenades run out, disable the grenade button
        if (grenadeCount <= 0)
        {
            grenadeButton.interactable = false;
        }
    }

    private void UpdateGrenadeText()
    {
        grenadeCountText.text = grenadeCount + " x";
    }

    public void ResetGrenades()
    {
        grenadeCount = 2;
        UpdateGrenadeText();
        grenadeButton.interactable = true;
    }

}

