using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Npgsql;
using Unity.Mathematics;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public TouchRaycaster touchRaycaster;
    public Button closeButton;  // Reference to the close button
    public Button confirmButton;  // Reference to the confirm button
    public Slider rotationSlider;
    public TMP_InputField inputMarke;
    public TMP_InputField inputBatterie;
    public Toggle saveToDatabaseToggle;
    public GameObject instantiatedObject;
    public double3 point;
    private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=Maxi0310;Database=postgres";


    void Update()
    {

    }

    public void startPopup()
    {
        closeButton.onClick.AddListener(touchRaycaster.startHidePopup);
        confirmButton.onClick.AddListener(loadDataToDatabase);
        rotationSlider.onValueChanged.AddListener(OnRotationSliderValueChanged);
    }


    void OnRotationSliderValueChanged(float value)
    {
        instantiatedObject.transform.rotation = Quaternion.Euler(0, value*360, 0);
    }
    
    void loadDataToDatabase()
    {
        if(saveToDatabaseToggle.isOn != false){
            string query = $"INSERT INTO table_scooter(geom, marke, batterietime, rotation) " +
                $"VALUES(ST_GeomFromText('POINTZ({point.x} {point.y} {point.z})', 4326), " +
                $"'{inputMarke.text}', " + // Quote text input
                $"'{inputBatterie.text}', " + // Quote text input
                $"{rotationSlider.value * 360})"; // Numeric values do not need quotes

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
                Debug.Log("Data saved to Database");
            }
        }
        else{
            Debug.Log("Don't save to Database");
        }
    }
}
