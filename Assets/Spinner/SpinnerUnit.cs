using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpinnerUnit : SpinnerControl, ISpinnerUnitAdapter
{
    public JointMotor Motor { get => _joint.motor; set => _joint.motor = value; }
    public float SpeedFactor => _speedFactor;

    [Space()]
    [Header("Dynamics Configuration")]
    [SerializeField] private PhysicMaterial _friction;

    [Space()]
    [SerializeField] private float _mass = 1;
    [SerializeField] private bool _useGravity = false;
    [SerializeField] private RigidbodyInterpolation _interpolation;
    [SerializeField] private CollisionDetectionMode _collisionDetection;

    [Space()]
    [SerializeField] private float _speedFactor = 0.5f;
    [SerializeField] private float _motorForce = 10;
    [SerializeField] private Vector3 _spinAxis = Vector3.up;

    [Space()]
    [SerializeField] private SpinnerGroup _group;
    public bool IsGrouped => _isGrouped;

    private HingeJoint _joint;
    private SpinnerSimulation _spinnerSimulation;
    public SpinnerSimulation SpinnerSimulation => _spinnerSimulation;

    private void Awake()
    {
        _spinnerSimulation = new SpinnerSimulation(this);

        if (_group)
        {
            _group.SimulationUnitRegister(_spinnerSimulation);
        }
        else
        {
            _spinnerLogic = new SpinnerLogic(this, _spinnerSimulation);
        }

        AddRigidbody();
        AddFrictionMaterial();
        AddHingeJoint();
    }

    private void AddRigidbody()
    {
        var rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = _mass;
        rigidBody.useGravity = _useGravity;
        rigidBody.interpolation = _interpolation;
        rigidBody.collisionDetectionMode = _collisionDetection;
    }

    private void AddFrictionMaterial()
    {
        gameObject.GetComponents<Collider>().ToList().ForEach(c =>
        {
            c.material = _friction;
        });
    }

    private void AddHingeJoint()
    {
        _joint = gameObject.AddComponent<HingeJoint>();
        _joint.axis = _spinAxis;
        _joint.useMotor = true;
        _joint.anchor = Vector3.zero;

        var motor = _joint.motor;
        motor.force = 0;
        _joint.motor = motor;

        if (transform.parent)
        {
            var parentRb = transform.parent.GetComponent<Rigidbody>();
            if (parentRb) _joint.connectedBody = parentRb;
        }
    }

    private void OnValidate()
    {
        _isGrouped = _group;

        if (!transform.parent)
        {
            Debug.LogWarning("Spinners Dynamics must has a exclusive parent Component.");
        }
#if UNITY_EDITOR
        else if (!transform.parent.GetComponent<Rigidbody>())
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorUtility.DisplayDialog("The parent must has a Rigidbody Component",
                "RigidBody will be added in the parent.", "Ok"))
                {
                    var rb = transform.parent.gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            };
        }
#endif
    }
}

public interface ISpinnerUnitAdapter
{
    public JointMotor Motor { get; set; }
    public float SpeedFactor { get; }
}
