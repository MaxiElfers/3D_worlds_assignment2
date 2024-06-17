using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Npgsql;
using UnityEngine;

public class retrieve_gis_data : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var sql = $"SELECT id,geom,metadata FROM public.table_scooter";
        var connection = DbCommonFunctions.GetNpgsqlConnection();
        connection.Open();
        connection.TypeMapper.UseNetTopologySuite();
        var cmd = new NpgsqlCommand(sql, connection);
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                // Read id
                var id = reader.GetInt32(0);
                Debug.Log(id.ToString());

                // Read geom
                Debug.Log((Point)reader[1]);

                // Read metadata
                Debug.Log(reader.GetString(2));
            }
        }
        connection.Close();

    }
}
