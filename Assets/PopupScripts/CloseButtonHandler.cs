using UnityEngine;
using UnityEngine.UI;

public class HidePanelButton : MonoBehaviour
{
    // Reference to the panel you want to hide
    GameObject panel;

    // Reference to the button
    private Button button;

    void Start()
    {
        // Get the Button component attached to this GameObject
        button = GetComponent<Button>();

        panel = GameObject.Find("PopupPanel");

        // Add a listener to the button to call the HidePanel method when clicked
        if (button != null)
        {
            button.onClick.AddListener(HidePanel);
        }
    }

    // Method to hide the panel
    void HidePanel()
    {
        if (panel != null)
        {
			panel.SetActive(false);
        }
    }
}
