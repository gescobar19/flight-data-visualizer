using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public FlightDataFetcher fetcher;
    public TextMeshProUGUI totalFlightsText;

    void Update()
    {
        if (fetcher != null && totalFlightsText != null)
        {
            totalFlightsText.text = "Flights: " + fetcher.Flights.Count;
        }
    }
}
