using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleCarController : MonoBehaviour {

    public Text TextSpeed;
    public float Speed;
    public float MaxSpeed = 120;
    public float CoefAcc = 10f;
    

    [Header("GearBox")]
	public float WheelsMaxAngle = 10;
    public float DMax = 35f;
    public float Torque = 100;
    private float _maxMF = 500;
    public Vector3 CentreOfMass = Vector3.zero;

    [Header("Wheels")]
    public WheelCollider WheelFR;
    public WheelCollider WheelFL;
    public WheelCollider WheelBR;
    public WheelCollider WheelBL;

    private float _oldFriction;
    private bool _upArrow = false;
    private bool _downArrow = false;
    private bool _leftArrow = false;
    private bool _rightArrow = false;
    private bool _space = false;
    private bool _drift = false;

    private void Start()
    {
        _oldFriction = WheelBL.sidewaysFriction.stiffness;
    }

    private void Update()
	{
        SetCOM();
        SpeedoMeter();
        GetInput();
        Acceleration();
        Direction();
        Braking();
        Drift();
    }

    private void GetInput()
    {
        _upArrow = Input.GetKey(KeyCode.UpArrow);
        _downArrow = Input.GetKey(KeyCode.DownArrow);
        _leftArrow = Input.GetKey(KeyCode.LeftArrow);
        _rightArrow = Input.GetKey(KeyCode.RightArrow);
        _space = Input.GetKey(KeyCode.Space);        
    }

    private void SpeedoMeter()
    {
        Speed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        TextSpeed.text = "Speed: " + (int)Speed;
    }
    private void SetCOM()
    {
        GetComponent<Rigidbody>().centerOfMass = CentreOfMass;
        //GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.296f, 0);
    }

    private void Acceleration()
    {
        if ( (_upArrow || _downArrow) && (Speed < MaxSpeed) )
        {
            CoefAcc ++;
            TurnOFFBackLights();
            WheelBL.motorTorque = Input.GetAxis("Vertical") * Torque * CoefAcc * Time.deltaTime;
            WheelBR.motorTorque = Input.GetAxis("Vertical") * Torque * CoefAcc * Time.deltaTime;
        }
        else
        {
            WheelBL.motorTorque = 0;
            WheelBR.motorTorque = 0;
        }
    
    }

    private void Direction()
    {
        if (_rightArrow || _leftArrow)
        {
            float DA = (((WheelsMaxAngle - DMax) / MaxSpeed) * Speed) + DMax;
            Debug.Log(DA);
            var steerAngle = Input.GetAxis("Horizontal") * DMax;
            WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, steerAngle, Time.deltaTime * 5);
            WheelFR.steerAngle = Mathf.Lerp(WheelFL.steerAngle, steerAngle, Time.deltaTime * 5);

        }
    }

    private void Drift()
    {
        WheelFrictionCurve frictionBL = WheelBL.sidewaysFriction;
        WheelFrictionCurve frictionBR = WheelBR.sidewaysFriction;

        if ( (_leftArrow || _rightArrow) && _space)
        {
            _drift = true;
            frictionBL.stiffness = 0.2f;          
            frictionBR.stiffness = 0.2f;
            
            WheelBL.motorTorque = 0;
            WheelBR.motorTorque = 0;

            WheelBL.brakeTorque = Mathf.Infinity;           
            WheelBL.brakeTorque = Mathf.Infinity;
        }
        else
        {
            _drift = false;
            frictionBL.stiffness = _oldFriction;
            frictionBR.stiffness = _oldFriction;
        }

        WheelBL.sidewaysFriction = frictionBL;
        WheelBR.sidewaysFriction = frictionBR;

    }

    private void Braking()
    {
        if (_space && !_drift)
        {
            WheelFR.brakeTorque = Mathf.Infinity;
            WheelFL.brakeTorque = Mathf.Infinity;
            WheelBR.brakeTorque = Mathf.Infinity;
            WheelBL.brakeTorque = Mathf.Infinity;
            TurnONBackLights();
        }
        else
        {
            WheelFR.brakeTorque = 0;
            WheelFL.brakeTorque = 0;
            WheelBR.brakeTorque = 0;
            WheelBL.brakeTorque = 0;
            TurnOFFBackLights();
        }
    }

    private void TurnONBackLights()
    {
        GameObject red;
        GameObject blue;
        Light r = new Light();
        Light b = new Light();
        red = gameObject.transform.GetChild(3).gameObject;
        blue = gameObject.transform.GetChild(4).gameObject;
        if (red.GetComponentInChildren<Light>() != null)
            r = red.GetComponentInChildren<Light>();
        if (blue.GetComponentInChildren<Light>() != null)
            b = blue.GetComponentInChildren<Light>();
        r.enabled = true;
        b.enabled = true;
    }

    private void TurnOFFBackLights()
    {
        GameObject red;
        GameObject blue;
        Light r = new Light();
        Light b = new Light();
        red = gameObject.transform.GetChild(3).gameObject;
        blue = gameObject.transform.GetChild(4).gameObject;
        if (red.GetComponentInChildren<Light>() != null)
            r = red.GetComponentInChildren<Light>();
        if (blue.GetComponentInChildren<Light>() != null)
            b = blue.GetComponentInChildren<Light>();
        r.enabled = false;
        b.enabled = false;
    }

}
