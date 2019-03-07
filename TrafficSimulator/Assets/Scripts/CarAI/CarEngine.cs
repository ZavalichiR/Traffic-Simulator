using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class CarEngine : MonoBehaviour
{
    [HideInInspector]
    public int ToolBar1;
    public int ToolBar2;
    public string CurrentTab;

    public Waypoint StartNode;

    /// <summary>
    /// Board CAR
    /// </summary>
    public float CoefAcc;
    public float CurrentSpeed = 0f;
    public float MaxSpeed = 70;
    public float MotorTorque = 100;
    private float _oldMotorTorque;

    /// <summary>
    /// GearBOX
    /// </summary>
    public bool ForcedBraking = false;
    public float BrakingTorque = 100f;
    public float WheelsMaxSteerAngle = 70f;

    /// <summary>
    /// Wheels
    /// </summary>
    public WheelCollider WheelFR;
    public WheelCollider WheelFL;
    public WheelCollider WheelBR;
    public WheelCollider WheelBL;

    /// <summary>
    /// Priority sensors
    /// </summary>
    [Header("-------------------------Front-------------------------")]
    public Vector3 FrontSensorsPosition;
    public float FrontSensorsLength;

    [Header("-------------------------Oblique-------------------------")]
    public float FrontObliqSensorsPosition;
    public float FrontObliqSensorsAngle;
    public float FrontSideSensorsLength;
    public float FrontOVF;

    [Header("-------------------------Sides-------------------------")]
    public Vector3 SideSensorsPosition;
    public float SideSensorsLength;
    public float SidePosition;

    [Header("-------------------------Back-------------------------")]
    public Vector3 BackSensorsPosition;
    public float BackObliqSensorsPosition;
    public float BackObliqSensorsAngle;
    public float BackObliqSensorsLength;
    public float BackOVF;

    /// <summary>
    /// Avoiding Sensors
    /// </summary>
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

    /// <summary>
    /// Info
    /// </summary>
    public float _targetSteerAngle = 0;
    public float _dirAngle = 0;

    public bool IsStop = false;
    public bool RoundAbout = false;
    public bool Intersection = false;

    public string CarAhead = "";
    public bool _isCarAhead = false;

    public bool _priority = false;
    public bool ChangeLane = false;
    public bool GoForward = false;
    public bool GoLeft = false;
    public bool GoRight = false;

    /// <summary>
    /// Private variables
    /// </summary>
    private Rigidbody _rb;
    public GameObject FrontCar;
    private Waypoint _currentNode;
    private Waypoint _nextNode;
    private Waypoint _nextNextNode;
    private Waypoint _previousNode;
    private float _distance;
    private float _turnSpeed = 5;
    private float delay = 0;
    private bool _reduceSpeed;
    private bool _setNodes = false;
    private bool _isBraking = false;

    private bool _reverse = false;
    private bool _avoiding = false;
    private bool _isTooFast = false;
    private bool _spawned = true;

    /// <summary>
    ///  Sensors
    /// </summary>
    public bool FrontCenterSensorPriority;
    public bool FrontLeftSensorPriority;
    public bool FrontObliqueLeftSensorPriority;
    public bool FrontRightSensorPriority;
    public bool FrontObliqueRightSensorPriority;
    public bool LeftSideSensorPriority;
    public bool RightSideSeonsorPriority;
    public bool BackLeftSensorPriority;
    public bool BackRightSensorPriority;

    // For ExecuteInEditMode
    private void Update()
    {
        //PrioritySensors();
        //AvoidingSensors();
    }

    private void Start()
    {
        FrontCar = gameObject.transform.GetChild(5).gameObject;
        _spawned = true;

        _previousNode = StartNode;
        _currentNode = StartNode;
        _nextNode = _currentNode.getNextWaypoint();

        GetAngle();
        SetDirection();

        _rb = GetComponent<Rigidbody>();

        _oldMotorTorque = MotorTorque;

    }

    private void FixedUpdate()
    {

        if (_spawned == true)
            SpeedBooster();
        else
            MotorTorque = _oldMotorTorque;

        ApplySteer();
        LerpSteerAngle();
        GetAngle();
        SetDirection();
        Drive();
        IsTooFast();
        Braking();
        Accelerate();
        SpeedoMeter();
        //if(!_avoiding)
        PrioritySensors();
        //AvoidingSensors();
        //GameObject sun = GameObject.Find("Sun");
        //ToggleFrontLights(sun.GetComponent<sun>().Night);
        if(ChangeLane)
            StartCoroutine(ChangeLaneLights());

    }

    /// <summary>
    /// Aplica o viteza mai mare pentru primele 2 secunde
    /// </summary>
    private void SpeedBooster()
    {
        if (delay > 2.0f)
            _spawned = false;
        if (_spawned == true)
            delay += Time.deltaTime;
        if (_spawned == true)
            MotorTorque = 1000;
    }

    /// <summary>
    /// Find the next point
    /// </summary>
    private void ApplySteer()
    {
        if (_avoiding)
            return;

        _distance = Vector3.Distance(FrontCar.transform.position, _currentNode.transform.position);
        //Debug.DrawLine(FrontCar.transform.position, _currentNode.transform.position);
        GetAngle();

        // Distance between car and the current point
        if ((_distance < 1.2f) && _setNodes)
        {
            _setNodes = false;
            SetDirection();
            SetNodes();
        }
        else
            _setNodes = true;

        Vector3 relativeVector = FrontCar.transform.InverseTransformPoint(_currentNode.transform.position);

        // Adaptive steer
        float maxAngle = 45;
        float angle = (((WheelsMaxSteerAngle - maxAngle) / MaxSpeed) * CurrentSpeed) + maxAngle;
        float newSteer = (relativeVector.x / relativeVector.magnitude) * angle;

        _targetSteerAngle = newSteer;
        WheelFL.steerAngle = newSteer;
        WheelFL.steerAngle = newSteer;
    }

    /// <summary>
    /// Smooth direction
    /// </summary>
    private void LerpSteerAngle()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, _targetSteerAngle, Time.deltaTime * _turnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFL.steerAngle, _targetSteerAngle, Time.deltaTime * _turnSpeed);
    }

    /// <summary>
    /// Check all flags and apply a corect dicision for the car
    /// </summary>
    private void Drive()
    {

        if (RoundAbout && CurrentSpeed > 20)
        {
            _isBraking = true;
            return;
        }

        if (CurrentSpeed > MaxSpeed)
            _isBraking = true;
        // daca masina are prioritate atunci aceasta nu se opreste cand detecteaza o alta masina
        if (_priority)
            _isCarAhead = false;

        // Oprire fortata
        if (ForcedBraking == true)
        {
            _isBraking = true;
            IsStop = false;
            _isCarAhead = false;
            _isTooFast = false;
        }
        else
        {
            // Verificare flag-uri pentru oprire
            if (IsStop || _isCarAhead || _isTooFast)
                _isBraking = true;
            else
                _isBraking = false;
        }


    }

    /// <summary>
    /// Reduce viteza pentru intersecții
    /// </summary>
    private void IsTooFast()
    {
        // Baga constante - 

        // Debug.Log("Distance: " + _distance + "   Angle: " + _angle);
        if ((_dirAngle > 15 || _dirAngle < 15) && (CurrentSpeed > 40) && (_distance < 15)) // Curba
            _isTooFast = true;
        else if ((_dirAngle > 30 || _dirAngle < -30) && (CurrentSpeed > 25) && (_distance < 15)) // Intersecții
            _isTooFast = true;
        else
            _isTooFast = false;
    }

    /// <summary>
    /// Opreste masina
    /// </summary>
    private void Braking()
    {
        if (_isBraking)
        {
            _reduceSpeed = true;
            if (GoForward)
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
            _reduceSpeed = false;
            TurnOFFBackLights();

            GetComponent<Rigidbody>().drag = 0;
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
            WheelBL.brakeTorque = 0;
            WheelBR.brakeTorque = 0;

            WheelFL.motorTorque = CoefAcc + MotorTorque;
            WheelFR.motorTorque = CoefAcc + MotorTorque;
        }

    }

    /// <summary>
    /// Reduce viteza masinii
    /// </summary>
    private void Accelerate()
    {
        if (_reverse )
            CoefAcc--;
        else if (_reduceSpeed)
            CoefAcc = 0;
        else if ((CurrentSpeed <= MaxSpeed) && (!_reduceSpeed))
        {
            if (CoefAcc < 0)
                CoefAcc = 0;
            CoefAcc++;
        }
    }

    /// <summary>
    /// Calculeaza viteza si face masina mai stabila
    /// </summary>
    private void SpeedoMeter()
    {
        CurrentSpeed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        _rb.centerOfMass = new Vector3(0, -0.296f, 0);
    }

    private void PrioritySensors()
    {
        if (_currentNode == null)
            return;

        _reverse = false;
        CarAhead = "";
        _priority = false;
        _isCarAhead = false;

        FrontCenterSensorPriority = false;
        FrontLeftSensorPriority = false;
        FrontObliqueLeftSensorPriority = false;
        FrontRightSensorPriority = false;
        FrontObliqueRightSensorPriority = false;
        LeftSideSensorPriority = false;
        RightSideSeonsorPriority = false;
        BackLeftSensorPriority = false;
        BackRightSensorPriority = false;

        // dimensiunea senzorului va fi adaptata in functie de viteza
        float newFrontSensorsLength = 0;
        // speed 15 -------------- FrontSL
        // speed x  -------------- newFrontSL
        if (CurrentSpeed > 100)
            newFrontSensorsLength = 40;
        else if (CurrentSpeed > 80)
            newFrontSensorsLength = 35;
        else if (CurrentSpeed > 60)
            newFrontSensorsLength = 30;
        else if (CurrentSpeed > 40)
            newFrontSensorsLength = 25;
        else if (CurrentSpeed > 30)
            newFrontSensorsLength = 20;
        else if (CurrentSpeed > 20)
            newFrontSensorsLength = 15;
        else if (CurrentSpeed > 15)
            newFrontSensorsLength = 10;
        else
            newFrontSensorsLength = 3;
        if (Intersection)
            newFrontSensorsLength += 2;

        // Stabilim dimensiunea senzorilor in intersectii
        float newFronSideSensorsLength = FrontSideSensorsLength;
        float newFrontObliqSensorsAngle = FrontObliqSensorsAngle;
        if (RoundAbout)
            newFronSideSensorsLength *= 1.5f;
        else if (Intersection && !GoRight)
        {
            newFronSideSensorsLength = 12;
            if (GoForward)
                newFrontObliqSensorsAngle = 5;
        }
        float dim = FrontSideSensorsLength;

        RaycastHit hit = new RaycastHit();
        Color colorSensor = new Color();
        colorSensor = Color.green;

        Vector3 frontSensorsStartPosition = transform.position;
        frontSensorsStartPosition += transform.forward * FrontSensorsPosition.z; // are aceeasi directie ca si masina
        frontSensorsStartPosition += transform.up * FrontSensorsPosition.y; // se afla la aceeasi inaltime indiferent de pozitia masinii

        Vector3 sideSensorsStartPosition = transform.position;
        sideSensorsStartPosition += transform.forward * SideSensorsPosition.z; // are aceeasi directie ca si masina
        sideSensorsStartPosition += transform.up * SideSensorsPosition.y; // se afla la aceeasi inaltime indiferent de pozitia masinii

        Vector3 backObliqSensorsStartPosition = transform.position;
        backObliqSensorsStartPosition += transform.forward * (-1) * BackSensorsPosition.z; // are aceeasi directie ca si masina
        backObliqSensorsStartPosition += transform.up * BackSensorsPosition.y; // se afla la aceeasi inaltime indiferent de pozitia masinii

        RaycastHit[] hits;
        hits = Physics.RaycastAll(frontSensorsStartPosition, transform.forward, newFrontSensorsLength);
        // Front Center
        bool hited = false;
        if (hits.Length > 0)
        {
            foreach (var h in hits)
            {
                // reduc viteza in intersectii
                if (h.collider.gameObject.tag == "Intersection" && CurrentSpeed > 30)
                {
                    _isCarAhead = true;
                    return;
                }
                if (h.collider.gameObject.tag == "Car")
                {
                    hit = h;
                    hited = true;
                    break;
                }
            }
            if (hited)
            {
                FrontCenterSensorPriority = true;

                Debug.DrawLine(frontSensorsStartPosition, hit.point, colorSensor);
                CarAhead = hit.collider.gameObject.name;

                _priority = GetPriority(hit.collider.gameObject, hit);
                _isCarAhead = !_priority;
                return;
            }

        }

        // Front Left
        frontSensorsStartPosition -= transform.right * FrontObliqSensorsPosition;
        dim = (Intersection && !GoRight) ? newFronSideSensorsLength : newFrontSensorsLength + 1;
        if (Physics.Raycast(frontSensorsStartPosition, transform.forward, out hit, dim))
        {
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors") && !hit.collider.CompareTag("Intersection"))
            {
                //Debug.Log("FL Dim: " + dim + " NewLength" + newFrontSensorsLength + " Inter: " + newFronSideSensorsLength);
                FrontLeftSensorPriority = true;

                Debug.DrawLine(frontSensorsStartPosition, hit.point, colorSensor);
                CarAhead = hit.collider.gameObject.name;

                if(hit.collider.CompareTag("Player"))
                {
                    _isCarAhead = true;
                    return;
                }
                _priority = GetPriority(hit.collider.gameObject, hit);
                _isCarAhead = !_priority;
                return;

            }
        }

        // Oblique Left        
        dim = (Intersection && !GoRight) ? newFronSideSensorsLength : FrontSideSensorsLength;
        if (Physics.Raycast(frontSensorsStartPosition, Quaternion.AngleAxis(-newFrontObliqSensorsAngle, transform.up) * transform.forward, out hit, dim))
        {
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors") && !hit.collider.CompareTag("Intersection"))
            {
                //Debug.Log("OL Dim: " + dim + " NewLength" + FrontSideSensorsLength + " Inter: " + newFronSideSensorsLength);
                FrontObliqueLeftSensorPriority = true;

                Debug.DrawLine(frontSensorsStartPosition, hit.point, colorSensor);
                CarAhead = hit.collider.gameObject.name;

                if (hit.collider.CompareTag("Player"))
                {
                    _isCarAhead = true;
                    return;
                }

                _priority = GetPriority(hit.collider.gameObject, hit);
                _isCarAhead = !_priority;
                return;

            }
        }

        // Front Right
        frontSensorsStartPosition += 2 * transform.right * FrontObliqSensorsPosition;
        dim = (Intersection && !GoRight) ? newFronSideSensorsLength : newFrontSensorsLength + 1;
        if (Physics.Raycast(frontSensorsStartPosition, transform.forward, out hit, dim))
        {
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors") && !hit.collider.CompareTag("Intersection"))
            {
                //Debug.Log("FR Dim: " + dim + " NewLength" + newFrontSensorsLength + " Inter: "+ newFronSideSensorsLength);

                FrontRightSensorPriority = true;

                Debug.DrawLine(frontSensorsStartPosition, hit.point, colorSensor);
                CarAhead = hit.collider.gameObject.name;

                if (hit.collider.CompareTag("Player"))
                {
                    _isCarAhead = true;
                    return;
                }

                _priority = GetPriority(hit.collider.gameObject, hit);
                _isCarAhead = !_priority;
                return;
            }
        }

        // Oblique Right      
        dim = (Intersection && !GoRight) ? newFronSideSensorsLength : FrontSideSensorsLength;
        if (Physics.Raycast(frontSensorsStartPosition, Quaternion.AngleAxis(FrontObliqSensorsAngle, transform.up) * transform.forward, out hit, dim))
        {
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors") && !hit.collider.CompareTag("Intersection"))
            {
                //Debug.Log("OR Dim: " + dim + " NewLength" + FrontSideSensorsLength + " Inter: " + newFronSideSensorsLength);
                FrontObliqueRightSensorPriority = true;

                Debug.DrawLine(frontSensorsStartPosition, hit.point, colorSensor);
                CarAhead = hit.collider.gameObject.name;

                if (hit.collider.CompareTag("Player"))
                {
                    _isCarAhead = true;
                    return;

                }
                _priority = GetPriority(hit.collider.gameObject, hit);
                _isCarAhead = !_priority;
                return;
            }
        }

        // RIGHT SIDE
        sideSensorsStartPosition += transform.right * SidePosition;
        // Acest senzor se activeaza atunci cand masina isi schimba banda
        if (!Intersection && GoRight)
        {
            if (Physics.Raycast(sideSensorsStartPosition, Quaternion.AngleAxis(90, transform.up) * transform.forward, out hit, SideSensorsLength))
            {
                if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors") && !hit.collider.CompareTag("Intersection"))
                {
                    RightSideSeonsorPriority = true;

                    Debug.DrawLine(sideSensorsStartPosition, hit.point, colorSensor);
                    CarAhead = hit.collider.gameObject.name;

                    if (hit.collider.CompareTag("Player"))
                    {
                        _isCarAhead = true;
                        return;
                    }

                    _priority = GetPriority(hit.collider.gameObject, hit);
                    if (_priority)
                        _isCarAhead = false;
                    else
                        _isCarAhead = true;
                    return;
                }
            }
        }
            

        // LEFT SIDE
        sideSensorsStartPosition -= 2 * transform.right * SidePosition;
        if (!Intersection && GoLeft)
        {
            if (Physics.Raycast(sideSensorsStartPosition, Quaternion.AngleAxis(-90, transform.up) * transform.forward, out hit, SideSensorsLength))
            {
                if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors") && !hit.collider.CompareTag("Intersection"))
                {
                    LeftSideSensorPriority = true;

                    Debug.DrawLine(sideSensorsStartPosition, hit.point, colorSensor);
                    CarAhead = hit.collider.gameObject.name;

                    if (hit.collider.CompareTag("Player"))
                    {
                        _isCarAhead = true;
                        return;
                    }

                    _priority = GetPriority(hit.collider.gameObject, hit);
                    _isCarAhead = !_priority;
                    return;
                }
            }

        }
        

        // RIGHT OBLIQUE BACK
        if (GoRight && !Intersection)
        {
            backObliqSensorsStartPosition += transform.right * BackObliqSensorsPosition;
            // Acest senzor se activeaza atunci cand masina isi schimba banda
            if (Physics.Raycast(backObliqSensorsStartPosition, Quaternion.AngleAxis(BackObliqSensorsAngle, transform.up) * transform.forward, out hit, BackObliqSensorsLength))
            {
                //Debug.Log("RIGHT");
                if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors") && !hit.collider.CompareTag("Intersection"))
                {

                    BackRightSensorPriority = true;

                    Debug.DrawLine(backObliqSensorsStartPosition, hit.point, colorSensor);
                    CarAhead = hit.collider.gameObject.name;

                    if (hit.collider.CompareTag("Player"))
                    {
                        _isCarAhead = true;
                        return;
                    }

                    _priority = GetPriority(hit.collider.gameObject, hit);
                    _isCarAhead = !_priority;
                    return;
                }
            }
        }

        // LEFT OBLIQUE BACK
        if (GoLeft && !Intersection)
        {
            backObliqSensorsStartPosition -= 2 * transform.right * BackObliqSensorsPosition;
            if (Physics.Raycast(backObliqSensorsStartPosition, Quaternion.AngleAxis(-BackObliqSensorsAngle, transform.up) * transform.forward, out hit, BackObliqSensorsLength))
            {
                //Debug.Log("LEFT");
                if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors") && !hit.collider.CompareTag("Intersection"))
                {
                    BackLeftSensorPriority = true;

                    Debug.DrawLine(backObliqSensorsStartPosition, hit.point, colorSensor);
                    CarAhead = hit.collider.gameObject.name;

                    if (hit.collider.CompareTag("Player"))
                    {
                        _isCarAhead = true;
                        return;
                    }

                    _priority = GetPriority(hit.collider.gameObject, hit);
                    _isCarAhead = !_priority;
                    return;
                }
            }
        }

    }

    // Cand ocoleste, mai intai sa verifice daca are unde sa se duca
    // poti face asta adunand sau scazand puncte 
    private void AvoidingSensors()
    {
        float avoidCounter = 0f;
        _avoiding = false;
        _reverse = false;

        //if (_priority)
        //    return;

        RaycastHit hit;
        Color colorSensor = new Color();
        Vector3 sensorsStartPosition = transform.position;
        sensorsStartPosition += transform.forward * frontSensorsPosition.z; // are aceeasi directie ca si masina
        sensorsStartPosition += transform.up * frontSensorsPosition.y; // se afla la aceeasi inaltime indiferent de pozitia masinii

        Vector3 backSensorsStartPosition = transform.position;
        backSensorsStartPosition += transform.forward * backSensorsPosition.z; // are aceeasi directie ca si masina
        backSensorsStartPosition += transform.up * backSensorsPosition.y; // se afla la aceeasi inaltime indiferent de pozitia masinii

        // Pozitionez senzori
        sensorsStartPosition += transform.right * frontSidePosition;
        backSensorsStartPosition += transform.right * backSidePosition;

        // First Right Sensor
        if (Physics.Raycast(sensorsStartPosition, transform.forward, out hit, frontSensorsLength))
        {
            //if (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("Car"))
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors"))
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
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors"))
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
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors"))
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
        sensorsStartPosition -= 2 * indexFront * transform.right * frontSidePosition;
        if (Physics.Raycast(sensorsStartPosition, transform.forward, out hit, frontSensorsLength))
        {
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors"))
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
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors"))
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
        backSensorsStartPosition -= 2 * indexBack * transform.right * backSidePosition;
        if (Physics.Raycast(backSensorsStartPosition, Quaternion.AngleAxis(-backSensorAngle, transform.up) * transform.forward, out hit, backSensorsLength))
        {
            //if (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("Car"))
            if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors"))
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
                if (!hit.collider.CompareTag("WayPoint") && !hit.collider.CompareTag("NotForSensors"))
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

    /// <summary>
    /// Functii suplimentare
    /// </summary>
    private void SetNodes()
    {
        _previousNode = _currentNode;
        _currentNode = _nextNode;
        //Debug.Log(_currentNode.outs.Count);        
        if ((_currentNode.outs.Count == 0) && (_distance < 1))
        {
            Debug.Log("Destrory");
            Destroy();

        }
        else if (_currentNode.outs.Count != 0)
        {
            _nextNode = _currentNode.getNextWaypoint();
            if (_nextNode.outs.Count != 0)
                _nextNextNode = _nextNode.getNextWaypoint();
        }

    }

    private void SetDirection()
    {

        //if (_dirAngle < 168)
        if (_dirAngle < 20 && _dirAngle > -20)
            ChangeLane = false;
        else
            ChangeLane = true;

        if (_dirAngle > 25)
        {
            GoRight = true;
            GoLeft = false;
            GoForward = false;
        }
        else if (_dirAngle < -25)
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
        Vector3 a = transform.position;
        Vector3 o = _currentNode.transform.position;
        Vector3 b = _nextNode.transform.position;

        //_dirAngle = Vector3.Angle(a - o, b - o);
        //_dirAngle = Vector3.SignedAngle(a - o, b - o, o.normalized);

        Vector3 relativeVector = Vector3.zero;
        if (_nextNextNode != null )
            relativeVector = FrontCar.transform.InverseTransformPoint(_nextNextNode.transform.position);
        else
            relativeVector = FrontCar.transform.InverseTransformPoint(_nextNode.transform.position);

        _dirAngle = (relativeVector.x / relativeVector.magnitude) * 90;

        //_dirAngle = Vector3.SignedAngle(transform.position - _currentNode.transform.position, _nextNode.transform.position - _currentNode.transform.position, _currentNode.transform.position);
        //if (_dirAngle < 0)
        //    _dirAngle += 360;
        //Debug.Log(_dirAngle + "   " + _targetSteerAngle);
    }

    private bool GetPriority(GameObject car2, RaycastHit hit)
    {

        var hitedCar = car2.GetComponent<CarEngine>();
        var distance = Vector3.Distance(FrontCar.transform.position, hit.point);
        //Debug.Log(name + "Hited "+hitedCar.CarAhead  + " " + distance);

        if ( distance < 2 && !LeftSideSensorPriority  && !RightSideSeonsorPriority && !BackLeftSensorPriority && !BackRightSensorPriority)
        { 
            _reverse = true;
            return true;
        }
        else
            _reverse = false;
        
        // Senzor Fata Centru 
        if (FrontCenterSensorPriority)
        {
            //Debug.Log(name + "FrontCenterSensorPriority");
            if (Intersection)
            {
                if (GoLeft && hitedCar.IsStop)
                    return true;
                if (GoLeft && ((hitedCar.GoLeft) && (hitedCar.CarAhead == name)))
                    return true;
                return false;
            }
            return false;
        }
        // Senzor Fata Stanga 
        if (FrontLeftSensorPriority)
        {
            //Debug.Log(name + "FrontLeftSensorPriority");
            if (Intersection)
            {
                if (GoLeft && hitedCar.IsStop)
                    return true;
                if (GoLeft && ((hitedCar.GoLeft || hitedCar.GoRight) && (distance >= 3)) || (hitedCar.FrontCenterSensorPriority && (hitedCar.CarAhead == name)))
                    return true;
                if (GoForward && hitedCar.GoLeft)
                    return false;
                if (GoLeft && (hitedCar.GoRight || hitedCar.GoForward) && (hitedCar.CarAhead == name))
                    return true;
                return false;
            }
            return false;
        }
        // Senzor Fata Dreapta 
        if (FrontRightSensorPriority)
        {
            //Debug.Log(name + "FrontRightSensorPriority");
            if (Intersection)
            {
                if (GoRight)
                    return false;

                if (GoLeft && hitedCar.IsStop)
                    return true;
                if (((hitedCar.GoLeft || hitedCar.GoRight) && (distance >= 3)) || (hitedCar.FrontCenterSensorPriority && (hitedCar.CarAhead == name)))
                    return true;
                if (GoLeft && (hitedCar.GoRight || hitedCar.GoForward) &&  (hitedCar.CarAhead == name))
                    return true;
                return false;
            }
            return false;
        }
        // Senzor Fata Stanga Oblic
        if (FrontObliqueLeftSensorPriority)
        {
            //Debug.Log(name + "FrontObliqueLeftSensorPriority");
            if (Intersection)
            {
                if (GoLeft && hitedCar.IsStop)
                    return true;
                if (GoLeft && ((hitedCar.GoLeft || hitedCar.GoRight) && (distance >= 3)) || (hitedCar.FrontCenterSensorPriority && (hitedCar.CarAhead == name)))
                    return true;
                if (GoForward && hitedCar.GoLeft && (hitedCar.CarAhead == name))
                    return false;
                if (GoLeft && hitedCar.GoForward && (hitedCar.FrontCenterSensorPriority || hitedCar.FrontRightSensorPriority || hitedCar.FrontLeftSensorPriority) && (hitedCar.CarAhead == name))
                    return true;
                return false;
            }
            return false;
        }
        // Senzor Fata Dreapta Oblic
        if (FrontObliqueRightSensorPriority)
        {
            //Debug.Log(name + "FrontObliqueRightSensorPriority");
            if (Intersection)
            {
                if (GoRight)
                    return false;
                if (GoLeft && hitedCar.IsStop)
                    return true;
                if (Intersection && GoForward && (hitedCar.GoForward || GoRight))
                    return true;
                if (((hitedCar.GoLeft || hitedCar.GoRight) && (distance >= 2)) || (hitedCar.FrontCenterSensorPriority && (hitedCar.CarAhead == name)))
                    return true;
                if (GoLeft && hitedCar.GoForward && hitedCar.FrontObliqueLeftSensorPriority)
                    return true;
                if (GoLeft && hitedCar.GoForward && (hitedCar.FrontCenterSensorPriority || hitedCar.FrontRightSensorPriority || hitedCar.FrontLeftSensorPriority) && (hitedCar.CarAhead == name))
                    return true;
                return false;
            }
            return false;
        }
        // Senzor Stanga Lateral
        if (LeftSideSensorPriority)
        {
            //Debug.Log(name + "LeftSideSensorPriority");
            if (GoLeft)
                return false;
            return true;
        }
        // Senzor Dreapta Lateral
        if (RightSideSeonsorPriority)
        {
            // Debug.Log(name + "RightSideSeonsorPriority");
            if (GoRight)
                return false;
            return true;
        }
        // Senzor Spate Stanga
        if (BackLeftSensorPriority)
        {
            // Debug.Log(name + "BackLeftSensorPriority");
            if (GoLeft)
                return false;
            return true;
        }
        // Senzor Spate Dreapta
        if (BackRightSensorPriority)
        {
            //Debug.Log(name + "BackRightSensorPriority");
            if (GoRight)
                return false;
            return true;
        }
        return false;
    }

    private bool ChangeLanePriority(GameObject car2)
    {
        if (GoForward)
            return true;
        else if (!GoForward && !car2.GetComponent<CarEngine>().GoForward)
            return true;
        //else if ((GoRight && car2.GetComponent<CarEngine>().GoLeft))
        //return true;        
        else
            return false;
    }

    private void ToggleBackLights(bool flag)
    {
        GameObject right;
        GameObject left;
        Light rLight = new Light();
        Light lLight = new Light();
        right = gameObject.transform.GetChild(4).gameObject;
        left = gameObject.transform.GetChild(3).gameObject;
        if (right.GetComponentInChildren<Light>() != null)
            rLight = right.GetComponentInChildren<Light>();
        if (left.GetComponentInChildren<Light>() != null)
            lLight = left.GetComponentInChildren<Light>();
        rLight.enabled = flag;
        lLight.enabled = flag;
    }

    private void TurnONBackLights()
    {
        ToggleBackLights(true);
    }

    private void TurnOFFBackLights()
    {
        ToggleBackLights(false);
    }

    IEnumerator ChangeLaneLights()
    {
        int index = 0;
        if (GoLeft)
            index = 3;
        else if(GoRight)
            index = 4;

        GameObject oLight;
        Light light = new Light();
        oLight = gameObject.transform.GetChild(index).gameObject;
        if (oLight.GetComponentInChildren<Light>() != null)
        {
            light = oLight.GetComponentInChildren<Light>();

            light.enabled = true;

            yield return new WaitForSeconds(1f);

            light.enabled = false;
        }
            

    }

    private void ToggleFrontLights(bool flag)
    {
        GameObject frontRightPoint;
        GameObject frontRightSpot;
        GameObject frontLeftPoint;
        GameObject frontLeftSpot;

        Light lifhtFrontRightPoint = new Light();
        Light lifhtFrontRightSpot = new Light();
        Light lifhtFrontLeftPoint = new Light();
        Light lifhtFrontLeftSpot = new Light();

        frontRightPoint = gameObject.transform.GetChild(6).gameObject;
        frontRightSpot = gameObject.transform.GetChild(7).gameObject;

        if (frontRightPoint.GetComponentInChildren<Light>() != null)
            lifhtFrontRightPoint = frontRightPoint.GetComponentInChildren<Light>();
        if (frontRightSpot.GetComponentInChildren<Light>() != null)
            lifhtFrontRightSpot = frontRightSpot.GetComponentInChildren<Light>();

        lifhtFrontRightPoint.enabled = flag;
        lifhtFrontRightSpot.enabled = flag;

        frontLeftPoint = gameObject.transform.GetChild(8).gameObject;
        frontLeftSpot = gameObject.transform.GetChild(9).gameObject;

        if (frontLeftPoint.GetComponentInChildren<Light>() != null)
            lifhtFrontLeftPoint = frontLeftPoint.GetComponentInChildren<Light>();
        if (frontLeftSpot.GetComponentInChildren<Light>() != null)
            lifhtFrontLeftSpot = frontLeftSpot.GetComponentInChildren<Light>();

        lifhtFrontLeftPoint.enabled = flag;
        lifhtFrontLeftSpot.enabled = flag;
    }

    private void Destroy()
    {
        //Debug.Log(name + " deleted");
        Destroy(gameObject);
        Destroy(this);
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());
    }



}
