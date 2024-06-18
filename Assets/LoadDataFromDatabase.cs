using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using CesiumForUnity;
using Unity.Mathematics;


public class LoadDataFromDatabase : MonoBehaviour
{
    public GameObject cubePrefab;  // Reference to the cube prefab

    public GameObject GeoRefFolder;

    private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=Maxi0310;Database=postgres";

    // Start is called before the first frame update
    void Start()
    {
        var sql = $"SELECT id,geom,marke,batterietime,rotation FROM public.table_scooter";
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        connection.TypeMapper.UseNetTopologySuite();
        var cmd = new NpgsqlCommand(sql, connection);
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                // Assuming geom column is of type Geometry (NetTopologySuite.Geometries.Geometry)
                NetTopologySuite.Geometries.Geometry geometry = reader.GetFieldValue<NetTopologySuite.Geometries.Geometry>(1);

                // Example: Extracting coordinates from Point geometry
                if (geometry is Point point)
                {
                    Point geoemtryPoint = (Point)geometry;
                    double x = geoemtryPoint.X;
                    double y = geoemtryPoint.Y;
                    double z = geoemtryPoint.Z; 
                

                    // Read marke
                    Debug.Log(reader.GetString(2));

                    // Read batterietime
                    Debug.Log(reader.GetString(3));

                    //Vector3 spawnPosition = new Vector3((float)x, (float)y, (float)z);
                    GameObject instantiatedObject = Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
                    CesiumGlobeAnchor globeAnchor = instantiatedObject.AddComponent<CesiumGlobeAnchor>();
                    globeAnchor.longitudeLatitudeHeight = new double3( x, y, z );
                    instantiatedObject.transform.parent = GeoRefFolder.transform;
                
                    Debug.Log("Point: " + x + ", " + y + ", " + z);
                }
            }
        }
        connection.Close();

    }
}
