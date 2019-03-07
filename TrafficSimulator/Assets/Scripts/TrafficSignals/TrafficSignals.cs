using UnityEngine;

public class TrafficSignals
{
    // ID-ul nodului
    public ulong ID { get; set; }

    public GameObject TrafficLight { get; set; }

    public TrafficSignals(ulong id, GameObject trafficLight)
    {
        this.ID = id;
        GameObject aux = null;
        TrafficLight = trafficLight;
        aux = TrafficLight.transform.GetChild(0).gameObject;
        aux = aux.transform.GetChild(0).gameObject;
    }
}