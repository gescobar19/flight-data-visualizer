using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq; // Make sure Newtonsoft.Json is installed

public class FlightDataFetcher : MonoBehaviour
{
    public string apiUrl = "https://opensky-network.org/api/states/all";
    public float refreshInterval = 10f; // seconds

    public List<FlightData> Flights = new List<FlightData>();

    void Start()
    {
        StartCoroutine(FetchLoop());
    }

    IEnumerator FetchLoop()
    {
        while (true)
        {
            yield return FetchFlights();
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    IEnumerator FetchFlights()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching flight data: " + www.error);
            }
            else
            {
                Flights.Clear();
                JObject json = JObject.Parse(www.downloadHandler.text);
                JArray states = (JArray)json["states"];

                if (states == null) yield break;

                foreach (JArray state in states)
                {
                    // Check for null latitude and longitude
                    if (state[5].Type == JTokenType.Null || state[6].Type == JTokenType.Null)
                        continue;

                    float lat = state[6].ToObject<float>();
                    float lon = state[5].ToObject<float>();

                    // Optional: handle other fields safely
                    float alt = state[7].Type != JTokenType.Null ? state[7].ToObject<float>() : 0f;
                    float vel = state[9].Type != JTokenType.Null ? state[9].ToObject<float>() : 0f;

                    Flights.Add(new FlightData
                    {
                        Callsign = state[1].ToString(),
                        Latitude = lat,
                        Longitude = lon,
                        Altitude = alt,
                        Velocity = vel
                    });
                }
            }
        }
    }
}

[System.Serializable]
public class FlightData
{
    public string Callsign;
    public float Latitude;
    public float Longitude;
    public float Altitude;
    public float Velocity;
}
