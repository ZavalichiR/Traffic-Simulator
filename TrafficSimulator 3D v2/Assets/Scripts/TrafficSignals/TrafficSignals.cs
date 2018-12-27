using UnityEngine;

public class TrafficSignals
{
    // ID-ul nodului
    public ulong ID { get; set; }

    // Obiectul care reprezint[ semafor
    public GameObject TrafficLight { get; set; }

    public TrafficSignals(ulong id, GameObject tf)
    {
        this.ID = id;
        GameObject aux = null;
        TrafficLight = tf;
        aux = TrafficLight.transform.GetChild(0).gameObject;
        aux= aux.transform.GetChild(0).gameObject;
    }
}