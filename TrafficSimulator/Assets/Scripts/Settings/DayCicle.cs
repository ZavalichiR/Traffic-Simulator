using UnityEngine;

public class DayCicle : MonoBehaviour {
    
    public Vector3 StartPositionSun;
    public Vector3 StartPositionMoon;

    public float Delay;
    public float Steps;
    public bool Day;
    public bool Night;

    public GameObject Sun;
    public GameObject Moon;
    public GameObject Sun_Top;
    public GameObject Sun_Bot;
    public GameObject Moon_Top;
    public GameObject Moon_Bot;

    private double _timeLeft;

    void Start () {
        Sun.transform.position = StartPositionSun;
        Moon.transform.position = StartPositionMoon;
        _timeLeft = Delay;
	}
	
	// Update is called once per frame
	void Update () {
        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0)
        {
            Sun.transform.Rotate(Steps, 0, 0);
            Moon.transform.Rotate(Steps, 0, 0);
            _timeLeft = Delay;
        }
        if (Sun.transform.eulerAngles.x < 170)
        {
            Day = true;
            Night = false;
            Sun.GetComponent<Light>().intensity = 1;
            Sun_Top.SetActive(true);
            Sun_Bot.SetActive(true);

            Moon_Top.SetActive(false);
            Moon_Bot.SetActive(false);
        }
        else
        {
            Day = false;
            Night = true;
            Sun.GetComponent<Light>().intensity = 0;
            Sun_Top.SetActive(false);
            Sun_Bot.SetActive(false);

            Moon_Top.SetActive(true);
            Moon_Bot.SetActive(true);

        }
    }
}
