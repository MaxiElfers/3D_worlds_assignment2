using CesiumForUnity;
using UnityEngine;
using Npgsql;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.Mathematics;

public class TouchRaycaster : MonoBehaviour
{
    public GameObject cubePrefab;  // Reference to the cube prefab
    private LineRenderer lineRenderer;
    private Vector3 hitPoint;
    private bool hitDetected;
    public float lineHeight = 10;
    public float lineWidth = 0.5f;
    public GameObject GeoRefFolder;
    public GameObject panel;  // Reference to the panel GameObject
    public PopupManager panelScript;  // Reference to the PopupManager script
    public CesiumCameraController CesiumCamera; // Reference to the CesiumCamera GameObject
    public bool isPanelOpen = false;

    void Start()
    {
        // Initialize the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component missing from this GameObject. Please add a LineRenderer component.");
            return;
        }

        // Set some default properties for the line renderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        hitDetected = false;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (isPanelOpen == false){
            HandleMouseInput();
        }
        else {
            CesiumCamera.enableMovement = false;
            CesiumCamera.enableRotation = false;
        }
#else
        if (isPanelOpen == false){
            HandleTouchInput();
        }
        else {
            CesiumCamera.enableMovement = false;
            CesiumCamera.enableRotation = false;
        }
#endif
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            VisualizeRay(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && hitDetected)
        {
            float groundLevel = GetGroundLevel(hitPoint);
            Vector3 spawnPosition = new Vector3(hitPoint.x, groundLevel, hitPoint.z);
            GameObject instantiatedObject = Instantiate(cubePrefab, spawnPosition, Quaternion.Euler(0, 0, 0));
            CesiumGlobeAnchor globeAnchor = instantiatedObject.AddComponent<CesiumGlobeAnchor>();
            instantiatedObject.transform.parent = GeoRefFolder.transform;

            double3 pos = new double3(globeAnchor.longitudeLatitudeHeight.x, globeAnchor.longitudeLatitudeHeight.y, globeAnchor.longitudeLatitudeHeight.z);

            // Insert the point into the PostGIS database
            ShowPopup(instantiatedObject, pos);
            // InsertPointIntoDatabase(hitPoint);
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                VisualizeRay(touch.position);
            }

            if (touch.phase == TouchPhase.Ended && hitDetected)
            {
                float groundLevel = GetGroundLevel(hitPoint);
                Vector3 spawnPosition = new Vector3(hitPoint.x, groundLevel, hitPoint.z);
                var instantiatedObject = Instantiate(cubePrefab, spawnPosition, Quaternion.Euler(0, 0, 0));
                instantiatedObject.AddComponent<CesiumGlobeAnchor>();
                instantiatedObject.transform.parent = GeoRefFolder.transform;

                // Insert the point into the PostGIS database
                // InsertPointIntoDatabase(hitPoint);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    void VisualizeRay(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            hitPoint = hit.point;
            hitDetected = true;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new Vector3(hit.point.x, hit.point.y + lineHeight, hit.point.z));
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new Vector3(ray.origin.x, ray.origin.y + lineHeight, ray.origin.z));
            lineRenderer.SetPosition(1, ray.origin + ray.direction * 100);
            hitDetected = false;
        }
    }

    float GetGroundLevel(Vector3 point)
    {
        RaycastHit hit;
        // Perform a raycast downwards from a high point (e.g., 1000 units) to find the ground level
        if (Physics.Raycast(new Vector3(point.x, 1000, point.z), Vector3.down, out hit))
        {
            return hit.point.y;
        }
        return point.y;
    }

    void ShowPopup(GameObject instantiatedObject, double3 point)
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        var mouseClickPos = Input.mousePosition;
        
        Vector3 panelPosition;

            // Determine whether the click is on the left or right side of the screen
            if (mouseClickPos.x < screenWidth / 2)
            {
                // If click is on the left side, position the panel in the middle right side of the screen
                panelPosition = new Vector3((screenWidth / 2) + 800 , (screenHeight / 2) - 200, 100);
            }
            else
            {
                // If click is on the right side, position the panel in the middle left side of the screen
                panelPosition = new Vector3((screenWidth / 2) - 800, (screenHeight / 2) - 200, 100);
            }
        
        panel.transform.position = panelPosition;
        isPanelOpen = true;
        panel.SetActive(true);
        panelScript.instantiatedObject = instantiatedObject;
        panelScript.point = point;
        Debug.Log("Point" + point);  
        panelScript.startPopup();
    }

    public void startHidePopup()
    {
        CesiumCamera.enableMovement = true;
        CesiumCamera.enableRotation = true;
        panel.SetActive(false);
        StartCoroutine(HidePopup());
    } 
    private IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(1);
        isPanelOpen = false;
    }
}
