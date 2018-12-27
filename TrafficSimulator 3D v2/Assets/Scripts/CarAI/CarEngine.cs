using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarEngine : MonoBehaviour
{
    public Waypoint start;
    private Waypoint _currentNode;
    private Waypoint _nextNode;
    private Waypoint _previousNode;
    private bool _setNodes = false;

    public float CurrentSpeed = 0f;

    [Header("GearBox")]
    public bool ForcedBraking = false;
    public float MaxSpeed = 100f;
    public float MotorTorque = 100;
    public float BrakingTorque = 100f;
    public float WheelsMaxSteerAngle = 70f;

    [Header("CarWheels")]
    public WheelCollider WheelFR;
    public WheelCollider WheelFL;
    public WheelCollider WheelBR;
    public WheelCollider WheelBL;


    [Header("Breaking")]
    private bool _isBraking = false;

    [Header("Sensors for speed")]
    [Header("Front")]
    public Vector3 frontSensorsPosition0 = new Vector3(0f, 0f, 2.13f);
    public float frontSensorsLength0 = 2.5f;
    public float frontSidePosition0 = 0.76f;
    public float FrontOVF = 1f;
    public float frontSensorAngle0 = 30f;

    [Header("Sides")]
    public Vector3 sideSensorsPosition = new Vector3(0f, 0f, 0f);
    public float sideSensorsLength = 1;
    public float SidePosition = 0.76f;

    [Header("Avoiding Sensors")]
    [Header("Front")]
    public Vector3 frontSensorsPosition = new Vector3(0f, 0f, 2.13f);
    public float frontSensorsLength = 2.5f;
    public float frontSidePosition = 0.76f;
    public float indexFront = 1f;
    public float frontSensorAngle = 30f;

    [Header("Back")]
    public Vector3 backSensorsPosition = new Vector3(0f, 0f, -1.62f);
    public float backSensorsLength = 3.06f;
    public float indexBack = 1f;
    public float backSidePosition = 0.75f;
    public float backSensorAngle = 10f;

    public bool IsStop = false;
    private bool _carAhead = false;
    private bool _reverse = false;
    private bool _avoiding = false;
    private float _targetSteerAngle = 0;
    private float _turnSpeed = 5;

    private float _angle = 0;
    private bool _isCarAhead = false;
    private bool _isTooFast = false;
    private bool _spawned = true;
    private float delay = 0;
    private Rigidbody _rb;

    [HideInInspector]
    public bool GoForward = false;
    public bool GoLeft = false;
    public bool GoRight = false;

    private void Start()
    {
        _spawned = true;

        _previousNode = start;
        _currentNode = start;
        _nextNode = _currentNode.getNextWaypoint();

        GetAngle();
        SetDirection();

        _rb = GetComponent<Rigidbody>();
        
    }

    private void FixedUpdate()
    {
        if (_spawned == true)
            SpeedBooster();

        ApplySteer();
        Drive();
        IsTooFast();
        Braking();
        SpeedoMeter();
        Sensors();
        CitySensors();
        //Reverse();
        LerpSteerAngle();
    }

    private void Sensors()
    {

        RaycastHit hit;
        Color colorSensor = new Color();
        Vector3 sensorsStartPosition = transform.position;
        sensorsStartPosition += transform.forward * frontSensorsPosition.z; // are aceeasi directie ca si masina
        sensorsStartPosition += transform.up * frontSensorsPosition.y; // se afla la aceeasi inaltime indiferent de pozitia masinii

        Vector3 backSensorsStartPosition = transform.position;
        backSensorsStartPosition += transform.forward * backSensorsPosition.z; // are aceeasi directie ca si masina
        backSensorsStartPosition += transform.up * backSensorsPosition.y; // se afla la aceeasi inaltime indiferent de pozitia masinii

        float avoidCounter = 0f;
        _avoiding = false;
        _reverse = false;


        // Pozitionez senzori
        sensorsStartPosition += transform.right * frontSidePosition;
        backSensorsStartPosition += transform.right * backSidePosition;

        // First Right Sensor
        if (Physics.Raycast(sensorsStartPosition, transform.forward, out hit, frontSensorsLength))
        {
            //if (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("Car"))
            if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
            {
                float distance = Vector3.Distance(sensorsStartPosition, hit.point);

                if (distance < frontSensorsLength / 6)
                {
                    _reverse = true;
                }

                if (distance < frontSensorsLength / 3)
                {
                    colorSensor = Color.red;
                    avoidCounter -= 0.5f;
                }
                else if (distance < frontSensorsLength / 2)
                {
                    colorSensor = Color.yellow;
                }
                else
                {
                    colorSensor = Color.green;
                }

                Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);
                _avoiding = true;
                avoidCounter -= 0.5f;
            }

        }

        // Second Right Sensor
        if (Physics.Raycast(sensorsStartPosition, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, frontSensorsLength + 0.5f))
        {
            //if (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("Car"))
            if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
            {
                float distance = Vector3.Distance(sensorsStartPosition, hit.point);

                if (distance < frontSensorsLength / 6)
                {
                    _reverse = true;
                }

                if (distance < frontSensorsLength / 3)
                {
                    colorSensor = Color.red;
                    avoidCounter -= 0.25f;
                }
                else if (distance < frontSensorsLength / 2)
                {
                    colorSensor = Color.yellow;
                }
                else
                {
                    colorSensor = Color.green;
                }

                Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);
                _avoiding = true;
                avoidCounter -= 0.25f;
            }

        }

        // Back First Right Sensor
        if (Physics.Raycast(backSensorsStartPosition, Quaternion.AngleAxis(backSensorAngle, transform.up) * transform.forward, out hit, backSensorsLength))
        {
            //if (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("Car"))
            if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
            {
                float distance = Vector3.Distance(backSensorsStartPosition, hit.point);

                if (distance < backSensorsLength / 3)
                {
                    colorSensor = Color.red;
                }

                else if (distance < backSensorsLength / 2)
                {
                    colorSensor = Color.yellow;
                }
                else
                {
                    colorSensor = Color.green;
                }

                Debug.DrawLine(backSensorsStartPosition, hit.point, colorSensor);
                _avoiding = true;
                avoidCounter -= 0.15f;
            }

        }

        // Left First Sensor
        sensorsStartPosition -= 2* indexFront * transform.right * frontSidePosition;
        if (Physics.Raycast(sensorsStartPosition, transform.forward, out hit, frontSensorsLength))
        {
             if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
            {
                float distance = Vector3.Distance(sensorsStartPosition, hit.point);

                if (distance < frontSensorsLength / 6)
                {
                    _reverse = true;
                }

                if (distance < frontSensorsLength / 3)
                {
                    colorSensor = Color.red;
                    avoidCounter += 0.5f;
                }
                else if (distance < frontSensorsLength / 2)
                {
                    colorSensor = Color.yellow;
                }
                else
                {
                    colorSensor = Color.green;
                }

                Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);
                _avoiding = true;
                avoidCounter += 0.5f;
            }

        }

        // Left Second Sensor
        if (Physics.Raycast(sensorsStartPosition, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, frontSensorsLength + 0.5f))
        {
            //if (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("Car"))
            if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
            {
                float distance = Vector3.Distance(sensorsStartPosition, hit.point);
                if (distance < frontSensorsLength / 6)
                {
                    _reverse = true;
                }

                if (distance < frontSensorsLength / 3)
                {
                    colorSensor = Color.red;
                    avoidCounter += 0.25f;
                }
                else if (distance < frontSensorsLength / 2)
                {
                    colorSensor = Color.yellow;
                }
                else
                {
                    colorSensor = Color.green;
                }

                Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);
                _avoiding = true;
                avoidCounter += 0.25f;
            }

        }

        // Left Side Sensor

        // Back First Left Sensor
        backSensorsStartPosition -= 2 * indexBack* transform.right * backSidePosition;
        if (Physics.Raycast(backSensorsStartPosition, Quaternion.AngleAxis(-backSensorAngle, transform.up) * transform.forward, out hit, backSensorsLength))
        {
            //if (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("Car"))
            if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
            {
                float distance = Vector3.Distance(backSensorsStartPosition, hit.point);

                if (distance < backSensorsLength / 3)
                {
                    colorSensor = Color.red;
                }
                else if (distance < backSensorsLength / 2)
                {
                    colorSensor = Color.yellow;
                }
                else
                {
                    colorSensor = Color.green;
                }

                Debug.DrawLine(backSensorsStartPosition, hit.point, colorSensor);
                _avoiding = true;
                avoidCounter += 0.15f;
            }

        }


        // Center
        if (avoidCounter == 0)
        {

            if (Physics.Raycast(sensorsStartPosition, transform.forward, out hit, frontSensorsLength + 0.5f))
            {
                // if (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("Car"))
                if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
                {
                    float distance = Vector3.Distance(sensorsStartPosition, hit.point);

                    if (distance < frontSensorsLength / 6)
                    {
                        _reverse = true;
                    }

                    if (distance < frontSensorsLength / 3)
                    {
                        colorSensor = Color.red;
                    }
                    else if (distance < frontSensorsLength / 2)
                    {
                        colorSensor = Color.yellow;
                    }
                    else
                    {
                        colorSensor = Color.green;
                    }


                    Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);
                    _avoiding = true;
                    if (hit.normal.x < 0)
                        avoidCounter = -1f;
                    else
                        avoidCounter = 1f;
                }
            }


        }

        if (_avoiding)
        {
            _targetSteerAngle = WheelsMaxSteerAngle * avoidCounter;
        }

    }

    private void CitySensors()
    {
        _isCarAhead = false;
        float newDim = 0;
        if (CurrentSpeed < 50 && CurrentSpeed > 40)
            newDim = frontSensorsLength0 - 0.3f;
        else if (CurrentSpeed < 40 && CurrentSpeed > 35)
            newDim = frontSensorsLength0 - 0.6f;
        else if (CurrentSpeed < 35 && CurrentSpeed > 30)
            newDim = frontSensorsLength0 - 0.9f;
        else if (CurrentSpeed < 30 && CurrentSpeed > 20)
            newDim = frontSensorsLength0 - 1f;
        else if (CurrentSpeed < 25 && CurrentSpeed > 20)
            newDim = frontSensorsLength0 - 1.5f;
        else if (CurrentSpeed < 20 && CurrentSpeed > 15)
            newDim = frontSensorsLength0 - 2f;
        else if (CurrentSpeed < 15)
            newDim = frontSensorsLength0 - 2.5f;
        else
            newDim = frontSensorsLength0;

        RaycastHit hit;
        Color colorSensor = new Color();
        Vector3 sensorsStartPosition = transform.position;
        sensorsStartPosition += transform.forward * frontSensorsPosition0.z; // are aceeasi directie ca si masina
        sensorsStartPosition += transform.up * frontSensorsPosition0.y; // se afla la aceeasi inaltime indiferent de pozitia masinii

        Vector3 sideSensorsStartPosition = transform.position;
        sideSensorsStartPosition += transform.forward * sideSensorsPosition.z; // are aceeasi directie ca si masina
        sideSensorsStartPosition += transform.up * sideSensorsPosition.y; // se afla la aceeasi inaltime indiferent de pozitia masinii


        // Center
        if (Physics.Raycast(sensorsStartPosition, transform.forward, out hit, newDim))
        {
            if (hit.collider.CompareTag("Car"))
            {
                float distance = Vector3.Distance(sensorsStartPosition, hit.point);

                if (distance < 3f / 6)
                {
                    _reverse = true;
                }

                if (distance < 3f / 3)
                {
                    colorSensor = Color.red;
                }
                else if (distance < 3f / 2)
                {
                    colorSensor = Color.yellow;
                }
                else
                {
                    colorSensor = Color.blue;
                }

                Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);
                _isCarAhead = true;
                return;
            }
        }

        // Pozitionez senzori
        sensorsStartPosition += transform.right * frontSidePosition0;
        
            // First Right Sensor
            if ( Physics.Raycast(sensorsStartPosition, transform.forward, out hit, frontSensorsLength0 / 2) )
            {
                if ( hit.collider.CompareTag("Car") )
                {
                    float distance = Vector3.Distance(sensorsStartPosition, hit.point);

                    if (distance < 3f / 6)
                    {
                        _reverse = true;
                    }

                    if (distance < 3f / 3)
                    {
                        colorSensor = Color.red;
                    }
                    else if (distance < 3f / 2)
                    {
                        colorSensor = Color.yellow;
                    }
                    else
                    {
                        colorSensor = Color.blue;
                    }
                    Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);

                Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);
                _isCarAhead = true;
            }          
        }


        // Right Side Sensor
        sideSensorsStartPosition += transform.right * SidePosition;
        if (Physics.Raycast(sideSensorsStartPosition, Quaternion.AngleAxis(90, transform.up) * transform.forward, out hit, sideSensorsLength))
        {
            if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
                Debug.DrawLine(sideSensorsStartPosition, hit.point, Color.blue);

        }

        // Left First Sensor
        sensorsStartPosition -= 2 * FrontOVF * transform.right * frontSidePosition0;
        if ( Physics.Raycast(sensorsStartPosition, transform.forward, out hit, frontSensorsLength0 / 2) )
        {
            if (hit.collider.CompareTag("Car"))
            {
                float distance = Vector3.Distance(sensorsStartPosition, hit.point);

                if (distance  < 3f / 6)
                {
                    _reverse = true;
                }

                if (distance < 3f / 3)
                {
                    colorSensor = Color.red;
                }
                else if (distance < 3f / 2)
                {
                    colorSensor = Color.yellow;
                }
                else
                {
                    colorSensor = Color.blue;
                }

                Debug.DrawLine(sensorsStartPosition, hit.point, colorSensor);
                _isCarAhead = true;
            }          
        }

        // Left Side Sensor
        sideSensorsStartPosition -= 2 * transform.right * SidePosition;
        if (Physics.Raycast(sideSensorsStartPosition, Quaternion.AngleAxis(-90, transform.up) * transform.forward, out hit, sideSensorsLength))
        {
            if (!hit.collider.CompareTag("WayPoint") && hit.collider.CompareTag("Car"))
                Debug.DrawLine(sideSensorsStartPosition, hit.point, Color.blue);

        }

    }

    private void SpeedBooster()
    {
        if (delay > 3.0f)
        {
            _spawned = false;
        }
        if (_spawned == true)
        {
            delay += Time.deltaTime;
        }

        if (_spawned == true)
            MotorTorque = 300;
        else
            MotorTorque = MotorTorque/2;
    }

    private void ApplySteer()
    {
        if (_avoiding)
            return;

        float distance = Vector3.Distance(gameObject.transform.position, _currentNode.transform.position);
        GetAngle();

        if ( (distance < 2f) && _setNodes)
        {
            _setNodes = false;           
            SetDirection();
            SetNodes();
        }
        else
            _setNodes = true;
            

        Vector3 relativeVector = transform.InverseTransformPoint(_currentNode.transform.position);

        float DMax = 45;
        float DA = (((WheelsMaxSteerAngle - DMax) / MaxSpeed) * CurrentSpeed) + DMax;

        float newSteer = (relativeVector.x / relativeVector.magnitude) * DA;
        _targetSteerAngle = newSteer;

        WheelFL.steerAngle = newSteer;
        WheelFL.steerAngle = newSteer;
    }

    private void LerpSteerAngle()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, _targetSteerAngle, Time.deltaTime * _turnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFL.steerAngle, _targetSteerAngle, Time.deltaTime * _turnSpeed);
    }

    private void IsTooFast()
    {
        if ( ( (_angle < 150f)  || (_angle > 210) ) && (CurrentSpeed > 10))
            _isTooFast = true;
        else
            _isTooFast = false;
    }

    private void Drive()
    {
        if (ForcedBraking == true)
        {
            _isBraking = true;
            IsStop = false;
            _isCarAhead = false;
            _isTooFast = false;
        }
            
        else
        {
            if (IsStop)
                _isBraking = true;
            else if (_isCarAhead)
                _isBraking = true;
            else if (_isTooFast)
                _isBraking = true;
            else
                _isBraking = false;
        }
    }

    private void Reverse()
    {
        if (_reverse)
            MotorTorque--;
    }

    private void Braking()
    {
        if (_isBraking)
        {
            TurnONBackLights();

            GetComponent<Rigidbody>().drag = 2;
            WheelFL.brakeTorque = BrakingTorque;
            WheelFR.brakeTorque = BrakingTorque;
            WheelBL.brakeTorque = BrakingTorque;
            WheelBR.brakeTorque = BrakingTorque;

            WheelFL.motorTorque = 0;
            WheelFR.motorTorque = 0;          
        }
        else
        {
            TurnOFFBackLights();

            GetComponent<Rigidbody>().drag = 0;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
            WheelBL.brakeTorque = 0;
            WheelBR.brakeTorque = 0;

            WheelFL.motorTorque = MotorTorque;
            WheelFR.motorTorque = MotorTorque;
        }

    }

    private void SpeedoMeter()
    {
        CurrentSpeed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        _rb.centerOfMass = new Vector3(0, -0.296f, 0);
    }

    private void SetNodes()
    {
        _previousNode = _currentNode;
        _currentNode = _nextNode;
        _nextNode = _currentNode.getNextWaypoint();
    }

    private void SetDirection()
    {
        if (_angle <= 100)
        {
            GoRight = true;
            GoLeft = false;
            GoForward = false;
        }
        else if(_angle >= 280)
        {
            GoLeft = true;
            GoRight = false;
            GoForward = false;
        }
        else
        {
            GoForward = true;
            GoLeft = false;
            GoRight = false;
        }
    }

    private void GetAngle()
    {
        _angle = Vector3.SignedAngle(transform.position - _currentNode.transform.position, _nextNode.transform.position - _currentNode.transform.position, _currentNode.transform.position);
        if (_angle < 0)
            _angle += 360;
    }

    private bool SetPriority(GameObject car2)
    {
        if (GoForward)
            return true;
        else if (GoRight && !car2.GetComponent<CarEngine>().GoForward)
            return true;
        else if (GoLeft && !car2.GetComponent<CarEngine>().GoForward)
            return true;
        else
            return false;
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
