using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StopCars : MonoBehaviour
{
    private Light red = new Light();
    private Light yellow = new Light();
    private Light green = new Light();

    private float distance = 0;
    private void Update()
    {
        GameObject rlight;
        GameObject ylight;
        GameObject glight;

        rlight = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        //Debug.Log(rlight.name);
        ylight = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
        //Debug.Log(ylight.name);
        glight = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
        //Debug.Log(glight.name);

        if (rlight.GetComponentInChildren<Light>() != null)
            red = rlight.GetComponentInChildren<Light>();

        if (ylight.GetComponentInChildren<Light>() != null)
            yellow = ylight.GetComponentInChildren<Light>();

        if (glight.GetComponentInChildren<Light>() != null)
            green = glight.GetComponentInChildren<Light>();
    }

    // Cand vad semaforul
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collider Enter");
        //Debug.Log(other.name);
        //Debug.Log(other.tag);
        //Debug.Log(other.transform.parent.gameObject.name);
        //Debug.Log(other.transform.parent.gameObject.tag);
        if (other.gameObject.tag == "Car")
        {
            // reduc puțin viteza când văd că semaforul se face roșu
            if (yellow.intensity > 0 || yellow.enabled == true || red.intensity > 0 || red.enabled == true)
            {
                other.gameObject.GetComponent<CarEngine>().IsStop = true;
            }
            else
            {
                other.gameObject.GetComponent<CarEngine>().IsStop = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("Collider Stay");
        if (other.gameObject.tag == "Car")
        {

            if (green.intensity > 0 || green.enabled == true)
            {
                other.gameObject.GetComponent<CarEngine>().IsStop = false;
            }

            // Dacă mașina trebuie să oprească la semafor reduc viteza până la zero
            else if (yellow.intensity > 0 || yellow.enabled == true || red.intensity > 0 || red.enabled == true)
            {
                other.gameObject.GetComponent<CarEngine>().IsStop = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            if (red.intensity == 0 || red.enabled == false)
            {
                other.gameObject.GetComponent<CarEngine>().IsStop = false;
            }
            else
            {
                other.gameObject.GetComponent<CarEngine>().IsStop = false;
            }
        }
    }

}