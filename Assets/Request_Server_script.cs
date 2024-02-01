using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;

public class Request_Server_script : MonoBehaviour
{
    public string RequestServerURL = "http://localhost:8057/";

    public float Request_tacx_last_update           = 0;
    public float Request_tacx_elapsed_time          = 0;
    public int Request_tacx_distance_travelled      = 0;
    public float Request_tacx_speed                 = 0;
    public int Request_tacx_basic_resistance        = 0;
    public float Request_tacx_road_feel_type        = 0;
    public float Request_tacx_road_feel_intesity    = 0;
    public float Request_elite_angle                = 0;
    public float Request_elite_last_update          = 0;
    public float Request_break_least_update         = 0;    
    public float Request_break_front                = 0;
    public float Request_break_back                 = 0;



    public class MyData
    {
        public float tacx_last_update;
        public float tacx_elapsed_time;
        public int tacx_distance_travelled;
        public float tacx_speed;
        public int tacx_basic_resistance;
        public float tacx_road_feel_type; // Use nullable types for nullable JSON values
        public float tacx_road_feel_intesity; // Use nullable types for nullable JSON values
        public float elite_angle;
        public float elite_last_update;
        public float break_last_update;
        public float break_front; // Use nullable types for nullable JSON values
        public float break_back; // Use nullable types for nullable JSON values
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Add your update logic here if needed
        StartCoroutine(GetJsonData());
    }

    IEnumerator GetJsonData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(RequestServerURL))
        {
            // Wait for the web request to return
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(webRequest.downloadHandler.text);

                MyData myData = JsonUtility.FromJson<MyData>(webRequest.downloadHandler.text);
                if(myData.elite_last_update != 0) 
                { 
                    Request_elite_angle = myData.elite_angle; 
                    Request_elite_last_update = myData.elite_last_update;
                }

                if(myData.tacx_last_update != 0)
                {
                    Request_tacx_last_update = myData.tacx_last_update;
                    Request_tacx_elapsed_time = myData.tacx_elapsed_time;
                    Request_tacx_distance_travelled = myData.tacx_distance_travelled;
                    Request_tacx_basic_resistance = myData.tacx_basic_resistance;
                    Request_tacx_speed = myData.tacx_speed;
                    Request_tacx_road_feel_intesity = myData.tacx_road_feel_intesity;
                    Request_tacx_road_feel_type = myData.tacx_road_feel_type;

                }

                if(myData.break_last_update != 0)
                {
                    Request_break_least_update = myData.break_last_update;
                    Request_break_front = myData.break_front;
                    Request_break_back = myData.break_back;
                }


                
                

                //Debug.Log("elite_angle: " + myData.elite_angle);


            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }
}