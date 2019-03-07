using UnityEngine;

public class StopCars : MonoBehaviour
{
    private Light _red = new Light();
    private Light _yellow = new Light();
    private Light _green = new Light();

    /// <summary>
    /// Get colors of traffic lights 
    /// </summary>
    private void Update()
    {
        GameObject rlight;
        GameObject ylight;
        GameObject glight;

        rlight = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        ylight = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
        glight = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;

        if (rlight.GetComponentInChildren<Light>() != null)
            _red = rlight.GetComponentInChildren<Light>();

        if (ylight.GetComponentInChildren<Light>() != null)
            _yellow = ylight.GetComponentInChildren<Light>();

        if (glight.GetComponentInChildren<Light>() != null)
            _green = glight.GetComponentInChildren<Light>();

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Car")
        {
            // Reduce speed when the car see red or yellow light

            if ( _yellow.enabled == true || _red.enabled == true)
                other.gameObject.GetComponent<CarEngine>().IsStop = true;

            else
                other.gameObject.GetComponent<CarEngine>().IsStop = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Car")
        {

            if (_green.enabled == true)
                other.gameObject.GetComponent<CarEngine>().IsStop = false;

            // The car has to stop
            else if (_yellow.enabled == true ||  _red.enabled == true)
                other.gameObject.GetComponent<CarEngine>().IsStop = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            // The car forced the traffic light
            if ( _red.enabled == false)
                other.gameObject.GetComponent<CarEngine>().IsStop = false;
            else
                other.gameObject.GetComponent<CarEngine>().IsStop = false;
        }
    }

}