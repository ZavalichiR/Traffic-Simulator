using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour {

    public GameObject Sun;

    public float Delay;
    public float Steps;

    private double _timeLeft;
    void Start () {
        _timeLeft = Delay;
    }
	
	// Update is called once per frame
	void Update () {
        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0)
        {
            Sun.transform.Rotate(Steps, 0, 0);
            _timeLeft = Delay;
        }
    }
}
