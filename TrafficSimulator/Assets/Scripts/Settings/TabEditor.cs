using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CarEngine))]
public class TabEditor : Editor {

    private CarEngine _myTarget;

    private SerializedObject _serTarget;

    private SerializedProperty _startNode;

    /// <summary>
    /// Board CAR
    /// </summary>
    private SerializedProperty _currentSpeed;
    private SerializedProperty _coefAcc;
    private SerializedProperty _maxSpeed;
    private SerializedProperty _motorTorque;

    /// <summary>
    /// GearBOX
    /// </summary>
    private SerializedProperty _forceBraking;
    private SerializedProperty _brakingTorque;
    private SerializedProperty _wheelsMaxSteerAngle;

    /// <summary>
    /// Priority Sensors
    /// </summary>
    //FRONT
    private SerializedProperty _frontSensPosition;
    private SerializedProperty _frontSensLength;
    //FRONT OBLIQUE
    private SerializedProperty _FrontObliqSensorsPositionf;
    private SerializedProperty _FrontObliqSensorsAngle;
    private SerializedProperty  _FrontSideSensorsLength;
    private SerializedProperty _FrontOVF;
    //SIDE
    private SerializedProperty _sideSensorsPosition;
    private SerializedProperty _sideSensorsLength;
    private SerializedProperty _sidePosition;
    //BACK OBLIQUE
    private SerializedProperty _backSensorsPosition;
    private SerializedProperty _backObliqSensorsPosition;
    private SerializedProperty _backObliqSensorsAngle;
    private SerializedProperty _backObliqSensorsLength;
    private SerializedProperty _backOVF;

    /// <summary>
    /// Avoiding Sensors
    /// </summary>
    private SerializedProperty _avoidingFrontSensPosition;
    private SerializedProperty _avoidingFrontSensLength;
    private SerializedProperty _avoidingFrontSideSensPosition;
    private SerializedProperty _avoidingFrontSensIndex;
    private SerializedProperty _avoidingFrontSensAngle;
    private SerializedProperty _avoidingBackSensPosition;
    private SerializedProperty _avoidingBackSensLengh;
    private SerializedProperty _avoidingBackSensIndex;
    private SerializedProperty _avoidingBackSideSensPosition;
    private SerializedProperty _avoidingBackSideSensAngle;

    /// <summary>
    /// Info
    /// </summary>
    private SerializedProperty _targetSteerAngle;
    private SerializedProperty _dirAngle;
    private SerializedProperty _isSotp;
    private SerializedProperty _roundAbout;
    private SerializedProperty _intersection;
    private SerializedProperty _isCarAhead;
    private SerializedProperty _carAhead;
    private SerializedProperty _priority;
    private SerializedProperty _changeLane;
    private SerializedProperty _goForward;
    private SerializedProperty _goLeft;
    private SerializedProperty _goRight;


    /// <summary>
    /// Wheels
    /// </summary>
    private SerializedProperty _wheelFR;
    private SerializedProperty _wheelFL;
    private SerializedProperty _wheelBR;
    private SerializedProperty _wheelBL;
    private void OnEnable()
    {
        _myTarget = (CarEngine)target;
        _serTarget = new SerializedObject(target);

        _startNode = _serTarget.FindProperty("StartNode");

        _coefAcc = _serTarget.FindProperty("CoefAcc");
        _currentSpeed = _serTarget.FindProperty("CurrentSpeed");
        _maxSpeed = _serTarget.FindProperty("MaxSpeed");
        _motorTorque = _serTarget.FindProperty("MotorTorque");

        _forceBraking = _serTarget.FindProperty("ForcedBraking");
        _brakingTorque = _serTarget.FindProperty("BrakingTorque");
        _wheelsMaxSteerAngle = _serTarget.FindProperty("WheelsMaxSteerAngle");

        _wheelFR = _serTarget.FindProperty("WheelFR");
        _wheelFL = _serTarget.FindProperty("WheelFL");
        _wheelBR = _serTarget.FindProperty("WheelBR");
        _wheelBL = _serTarget.FindProperty("WheelBL");

        _frontSensPosition = _serTarget.FindProperty("FrontSensorsPosition");
        _frontSensLength = _serTarget.FindProperty("FrontSensorsLength");
        _FrontObliqSensorsPositionf = _serTarget.FindProperty("FrontObliqSensorsPosition");
        _FrontObliqSensorsAngle = _serTarget.FindProperty("FrontObliqSensorsAngle");
        _FrontSideSensorsLength = _serTarget.FindProperty("FrontSideSensorsLength");
        _FrontOVF = _serTarget.FindProperty("FrontOVF");
        _sideSensorsPosition = _serTarget.FindProperty("SideSensorsPosition");
        _sideSensorsLength = _serTarget.FindProperty("SideSensorsLength");
        _sidePosition = _serTarget.FindProperty("SidePosition");
        _backSensorsPosition = _serTarget.FindProperty("BackSensorsPosition");
        _backObliqSensorsPosition = _serTarget.FindProperty("BackObliqSensorsPosition");
        _backObliqSensorsAngle = _serTarget.FindProperty("BackObliqSensorsAngle");
        _backObliqSensorsLength = _serTarget.FindProperty("BackObliqSensorsLength");
        _backOVF = _serTarget.FindProperty("BackOVF");

        _avoidingFrontSensPosition = _serTarget.FindProperty("frontSensorsPosition");
        _avoidingFrontSensLength = _serTarget.FindProperty("frontSensorsLength");
        _avoidingFrontSideSensPosition = _serTarget.FindProperty("frontSidePosition");
        _avoidingFrontSensIndex = _serTarget.FindProperty("indexFront");
        _avoidingFrontSensAngle = _serTarget.FindProperty("frontSensorAngle");
        _avoidingBackSensPosition = _serTarget.FindProperty("backSensorsPosition");
        _avoidingBackSensLengh = _serTarget.FindProperty("backSensorsLength");
        _avoidingBackSensIndex = _serTarget.FindProperty("indexBack");
        _avoidingBackSideSensPosition = _serTarget.FindProperty("backSidePosition");
        _avoidingBackSideSensAngle = _serTarget.FindProperty("backSensorAngle");

        _targetSteerAngle = _serTarget.FindProperty("_targetSteerAngle");
        _dirAngle = _serTarget.FindProperty("_dirAngle");
        _isSotp = _serTarget.FindProperty("IsStop");
        _roundAbout = _serTarget.FindProperty("RoundAbout");
        _intersection = _serTarget.FindProperty("Intersection");
        _carAhead = _serTarget.FindProperty("CarAhead");
        _isCarAhead = _serTarget.FindProperty("_isCarAhead");
        _priority = _serTarget.FindProperty("_priority");
        _changeLane = _serTarget.FindProperty("ChangeLane");
        _goForward = _serTarget.FindProperty("GoForward");
        _goLeft = _serTarget.FindProperty("GoLeft");
        _goRight = _serTarget.FindProperty("GoRight");

    }

    public override void OnInspectorGUI()
    {
        _serTarget.Update();

        EditorGUI.BeginChangeCheck();

        _myTarget.ToolBar1 = GUILayout.Toolbar(_myTarget.ToolBar1, new string[] { "Board", "GearBox", "Info" });       
        switch (_myTarget.ToolBar1)
        {
            case 0:
                _myTarget.ToolBar2 = 3;
                _myTarget.CurrentTab = "Board"; 
                break;              
            case 1:
                _myTarget.ToolBar2 = 3;
                _myTarget.CurrentTab = "GearBox";
                break;
            case 2:
                _myTarget.ToolBar2 = 3;
                _myTarget.CurrentTab = "Info";
                break;
        }

        _myTarget.ToolBar2 = GUILayout.Toolbar(_myTarget.ToolBar2, new string[] { "Priority Sensors", "Avoiding Sensors", "Wheels"});
        switch (_myTarget.ToolBar2)
        {
            case 0:
                _myTarget.ToolBar1 = 3;
                _myTarget.CurrentTab = "Priority Sensors";
                break;

            case 1:
                _myTarget.ToolBar1 = 3;
                _myTarget.CurrentTab = "Avoiding Sensors";
                break;
            case 2:
                _myTarget.ToolBar1 = 3;
                _myTarget.CurrentTab = "Wheels";
                break;
        }


        EditorGUI.BeginChangeCheck();

        switch (_myTarget.CurrentTab)
        {
            case "Board":
                EditorGUILayout.PropertyField(_startNode);
                EditorGUILayout.PropertyField(_coefAcc);
                EditorGUILayout.PropertyField(_currentSpeed);
                EditorGUILayout.PropertyField(_dirAngle);
                EditorGUILayout.PropertyField(_targetSteerAngle);
                EditorGUILayout.PropertyField(_maxSpeed);
                EditorGUILayout.PropertyField(_motorTorque);
                break;
            case "GearBox":
                EditorGUILayout.PropertyField(_forceBraking);
                EditorGUILayout.PropertyField(_brakingTorque);
                EditorGUILayout.PropertyField(_wheelsMaxSteerAngle);
                break;
            case "Info":
                EditorGUILayout.PropertyField(_targetSteerAngle);
                EditorGUILayout.PropertyField(_dirAngle);
                EditorGUILayout.PropertyField(_isSotp);
                EditorGUILayout.PropertyField(_roundAbout);
                EditorGUILayout.PropertyField(_intersection);
                EditorGUILayout.PropertyField(_isCarAhead);
                EditorGUILayout.PropertyField(_carAhead);
                EditorGUILayout.PropertyField(_priority);
                EditorGUILayout.PropertyField(_changeLane);
                EditorGUILayout.PropertyField(_goForward);
                EditorGUILayout.PropertyField(_goLeft);
                EditorGUILayout.PropertyField(_goRight);
                break;
            case "Priority Sensors":

                EditorGUILayout.PropertyField(_frontSensPosition);
                EditorGUILayout.PropertyField(_frontSensLength);
                EditorGUILayout.PropertyField(_FrontObliqSensorsPositionf);
                EditorGUILayout.PropertyField(_FrontObliqSensorsAngle);
                EditorGUILayout.PropertyField(_FrontSideSensorsLength);
                EditorGUILayout.PropertyField(_FrontOVF);
                EditorGUILayout.PropertyField(_sideSensorsPosition);
                EditorGUILayout.PropertyField(_sideSensorsLength);
                EditorGUILayout.PropertyField(_sidePosition);
                EditorGUILayout.PropertyField(_backSensorsPosition);
                EditorGUILayout.PropertyField(_backObliqSensorsPosition);
                EditorGUILayout.PropertyField(_backObliqSensorsAngle);
                EditorGUILayout.PropertyField(_backObliqSensorsLength);
                EditorGUILayout.PropertyField(_backOVF);

                break;
            case "Avoiding Sensors":
                EditorGUILayout.PropertyField(_avoidingFrontSensPosition);
                EditorGUILayout.PropertyField(_avoidingFrontSensLength);
                EditorGUILayout.PropertyField(_avoidingFrontSideSensPosition);
                EditorGUILayout.PropertyField(_avoidingFrontSensIndex);
                EditorGUILayout.PropertyField(_avoidingFrontSensAngle);
                EditorGUILayout.PropertyField(_avoidingBackSensPosition);
                EditorGUILayout.PropertyField(_avoidingBackSensLengh);
                EditorGUILayout.PropertyField(_avoidingBackSensIndex);
                EditorGUILayout.PropertyField(_avoidingBackSideSensPosition);
                EditorGUILayout.PropertyField(_avoidingBackSideSensAngle);
                break;
            case "Wheels":
                EditorGUILayout.PropertyField(_wheelFR);
                EditorGUILayout.PropertyField(_wheelFL);
                EditorGUILayout.PropertyField(_wheelBR);
                EditorGUILayout.PropertyField(_wheelBL);
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            _serTarget.ApplyModifiedProperties();
        }
    }

}
