using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera[] Cameras;
    private int currentCameraIndex;

    void Start()
    {
        currentCameraIndex = 0;

        // Pornim doar prima camera
        for (int i = 1; i < Cameras.Length; i++)
        {
            Cameras[i].gameObject.SetActive(false);
        }

        //If any cameras were added to the controller, enable the first one
        if (Cameras.Length > 0)
        {
            Cameras[0].gameObject.SetActive(true);
            Debug.Log("Camera with name: " + Cameras[0].GetComponent<Camera>().name + ", is now enabled");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If the c button is pressed, switch to the next camera
        //Set the camera at the current index to inactive, and set the next one in the array to active
        //When we reach the end of the camera array, move back to the beginning or the array.
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCameraIndex++;
            Debug.Log("C button has been pressed. Switching to the next camera");
            if (currentCameraIndex < Cameras.Length)
            {
                Cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                Cameras[currentCameraIndex].gameObject.SetActive(true);
                Debug.Log("Camera with name: " + Cameras[currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
            }
            else
            {
                Cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                Cameras[currentCameraIndex].gameObject.SetActive(true);
                Debug.Log("Camera with name: " + Cameras[currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
            }
        }
    }
}