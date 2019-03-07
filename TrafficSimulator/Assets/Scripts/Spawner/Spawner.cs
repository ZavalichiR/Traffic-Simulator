using UnityEngine;

public class Spawner : MonoBehaviour {

    public Waypoint StartNode;
    public int MaxCars;
    public GameObject[] Cars;

    private bool _ready = true;
    private static int _counter = 0;

    // Update is called once per frame
    void Update () {
        if (MaxCars != 0 && _ready)
                SpawnCar();
    }

    private void SpawnCar()
    {
        _counter++;
        int i = Random.Range(0, Cars.Length);
        Cars[i].GetComponent<CarEngine>().StartNode = StartNode;
        Cars[i].GetComponent<CarEngine>().MaxSpeed = Random.Range(60,80);
        string carModel = Cars[i].name;
        Cars[i].name = "IS" + _counter.ToString() + "CAR";
        Instantiate(Cars[i], gameObject.transform.GetChild(0).gameObject.transform.position, gameObject.transform.rotation);

        MaxCars--;
        if (MaxCars < 0)
            MaxCars = 0;
        _ready = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        _ready = false;
        if(other.tag == "Car")
            other.GetComponent<CarEngine>().MotorTorque = 1000;
    }

    private void OnTriggerStay(Collider other)
    {
        _ready = false;
        if (other.tag == "Car")
            other.GetComponent<CarEngine>().MotorTorque = 1000;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Exit");
        _ready = true;
    }
}
