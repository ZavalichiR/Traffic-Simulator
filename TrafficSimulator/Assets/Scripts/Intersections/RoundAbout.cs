using UnityEngine;

public class RoundAbout : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
            other.GetComponent<CarEngine>().RoundAbout = true;   
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Car")
            other.GetComponent<CarEngine>().RoundAbout = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
            other.GetComponent<CarEngine>().RoundAbout = false;
    }
}
