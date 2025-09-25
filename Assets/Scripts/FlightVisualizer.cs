using System.Collections.Generic;
using UnityEngine;

public class FlightVisualizer : MonoBehaviour
{
    public FlightDataFetcher fetcher;   // Reference to the FlightDataFetcher
    public GameObject planePrefab;      // Prefab for planes
    public float globeRadius = 10f;     // Radius of globe
    public float moveSpeed = 5f;        // Interpolation speed

    // Dictionary to track planes by callsign
    private Dictionary<string, GameObject> planeObjects = new Dictionary<string, GameObject>();

    void Update()
    {
        if (fetcher == null || fetcher.Flights == null) return;

        foreach (FlightData f in fetcher.Flights)
        {
            // Ensure latitude and longitude are valid
            if (f == null) continue;

            Vector3 targetPos = LatLonToXYZ(f.Latitude, f.Longitude, globeRadius);

            if (planeObjects.ContainsKey(f.Callsign))
            {
                // move existing plane
                planeObjects[f.Callsign].transform.position = Vector3.Lerp(
                    planeObjects[f.Callsign].transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                // Spawn a new plane
                GameObject plane = Instantiate(planePrefab, targetPos, Quaternion.identity);
                plane.name = f.Callsign;

                // rotate plane to point outward from globe center
                Vector3 direction = (targetPos - plane.transform.position).normalized;
                plane.transform.rotation = Quaternion.LookRotation(direction, plane.transform.position.normalized);


                planeObjects[f.Callsign] = plane;
            }
        }

        // Remove planes that are no longer in fetcher.Flights
        RemoveMissingPlanes();
    }

    public bool TryGetPlane(string callsign, out GameObject plane, out FlightData data)
    {
        plane = null;
        data = null;

        if (!planeObjects.ContainsKey(callsign)) return false;

        plane = planeObjects[callsign];
        data = fetcher.Flights.Find(f => f.Callsign == callsign);
        return data != null;
    }

    Vector3 LatLonToXYZ(float lat, float lon, float radius)
    {
        float latRad = lat * Mathf.Deg2Rad;
        float lonRad = lon * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float y = radius * Mathf.Sin(latRad);
        float z = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);

        return new Vector3(x, y, z);
    }

    void RemoveMissingPlanes()
    {
        // Create a temporary list
        List<string> toRemove = new List<string>();

        foreach (var kvp in planeObjects)
        {
            bool exists = fetcher.Flights.Exists(f => f.Callsign == kvp.Key);
            if (!exists)
            {
                Destroy(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (string key in toRemove)
            planeObjects.Remove(key);
    }
}
