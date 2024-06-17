using UnityEngine;
using UnityEngine.EventSystems;

public class PanelFocusHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // This will be called when the mouse pointer enters the panel
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Focus on the panel to ensure other UI elements are not interacted with
        EventSystem.current.SetSelectedGameObject(gameObject);
        Debug.Log("Mouse entered the panel. Focus set on the panel.");
    }

    // This will be called when the mouse pointer exits the panel
    public void OnPointerExit(PointerEventData eventData)
    {
        // Clear the focus to allow interaction with other UI elements
        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("Mouse exited the panel. Focus cleared.");
    }
}
