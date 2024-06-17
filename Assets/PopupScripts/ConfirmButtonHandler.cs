using UnityEngine;
using UnityEngine.UI;
using Npgsql;

public class ConfirmButtonHandler : MonoBehaviour
{
    // Reference to the panel you want to hide
    GameObject panel;

    // Reference to the button
    private Button button;

    void Start()
    {
        // Get the Button component attached to this GameObject
        button = GetComponent<Button>();

        // Find the panel in the scene
        panel = GameObject.Find("PopupPanel");

        // Add a listener to the button to call the LoadDataToDatabank method when clicked
        if (button != null)
        {
            button.onClick.AddListener(LoadDataToDatabank);
        }
        else
        {
            Debug.LogError("Button component is missing.");
        }

        // Check if the panel reference is set
        if (panel == null)
        {
            Debug.LogError("Panel reference is not set.");
        }
    }

    // Method to load data to the databank and hide the panel
    void LoadDataToDatabank()
    {
        try
        {
            var query = "INSERT INTO my_table(geom, metadata) VALUES( ST_GeomFromText('POINTZ(1.0 2.0 3.0)', 4326), 'tree')";
            
            var connection = DbCommonFunctions.GetNpgsqlConnection();
            
            // Ensure the connection object is not null
            if (connection == null)
            {
                Debug.LogError("Failed to get database connection.");
                return;
            }

            connection.Open();
            
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
            }

            connection.Close();

            // Hide the panel after successfully executing the query
            if (panel != null)
            {
                panel.SetActive(false);
                Debug.Log("Panel has been hidden.");
            }
            else
            {
                Debug.LogError("Panel reference is null. Cannot hide panel.");
            }
        }
        catch (NpgsqlException ex)
        {
            Debug.LogError($"PostgreSQL error: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"General error: {ex.Message}");
        }
    }
}
