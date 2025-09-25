using UnityEngine;
using TMPro; // for TextMeshPro
using UnityEngine.UI;

public class FlightSearch : MonoBehaviour
{
    public FlightVisualizer visualizer;
    public TMP_InputField searchInput;
    public GameObject flightInfoPanel;
    public TMP_Text callsignText;
    public TMP_Text altitudeText;
    public TMP_Text speedText;

    private GameObject highlightedPlane;
    private Color originalColor;

    public void OnSearchButton()
    {
        string callsign = searchInput.text.Trim().ToUpper();

        if (visualizer.TryGetPlane(callsign, out GameObject plane, out FlightData data))
        {
            // Highlight the plane
            if (highlightedPlane != null)
                ResetHighlight(); // remove highlight from previous

            highlightedPlane = plane;
            Renderer rend = plane.GetComponent<Renderer>();
            if (rend != null)
            {
                originalColor = rend.material.color;
                rend.material.color = Color.yellow; // highlight color
            }

            // Show info panel
            flightInfoPanel.SetActive(true);
            callsignText.text = $"Flight: {data.Callsign}";
            altitudeText.text = $"Altitude: {data.Altitude:F0} m";
            speedText.text = $"Speed: {data.Velocity:F0} m/s";
        }
        else
        {
            flightInfoPanel.SetActive(false);
            Debug.Log("Flight not found");
        }
    }

    void ResetHighlight()
    {
        if (highlightedPlane != null)
        {
            Renderer rend = highlightedPlane.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = originalColor;
        }
    }
}
