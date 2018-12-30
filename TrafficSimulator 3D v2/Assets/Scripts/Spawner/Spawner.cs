using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Waypoint StartNode;
    public int MaxCars;
    public GameObject[] Cars;
    private float _delay = 0;
    private bool _ready = true;

    // Update is called once per frame
    void Update () {
        if (MaxCars != 0 && _ready)
                SpawnCar();
    }

    private void SpawnCar()
    {
        int i = Random.Range(0, Cars.Length);
        Cars[i].GetComponent<CarEngine>().start = StartNode;
        Instantiate(Cars[i], StartNode.gameObject.transform.position, gameObject.transform.rotation);

        MaxCars--;
        if (MaxCars < 0)
            MaxCars = 0;
        _ready = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enter");
        _ready = false;
    }
    private void OnTriggerStay(Collider other)
    {
        _ready = false;        
    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Exit");
        _ready = true;
    }
}
