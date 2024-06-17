using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public TouchRaycaster touchRaycaster;

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) // 0 is the left mouse button
        {
            ShowPopup();
        }
    }

    void ShowPopup()
    {
        touchRaycaster.isPanelOpen = true;
        popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}
