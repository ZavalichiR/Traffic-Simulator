using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundAout : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            Debug.Log("ENTEER");
            other.gameObject.GetComponent<CarEngine>().RoundAbout = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            other.gameObject.GetComponent<CarEngine>().RoundAbout = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            other.gameObject.GetComponent<CarEngine>().RoundAbout = false;
        }
    }
}
