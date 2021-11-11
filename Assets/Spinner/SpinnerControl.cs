using Bazinga.Extensions;
using UnityEngine;

public abstract class SpinnerControl : MonoBehaviour, ISpinnerControlAdapter
{
    [Space()]
    [ConditionalField("_isGrouped", true)]
    [SerializeField] private Integration _spinnerIntegrations;
    public Integration Integration => _spinnerIntegrations;

    [Space()]
    [Header("Dynamics Debugger")]
    [SerializeField] [Inactive] private bool _isOn;
    public bool _IsOn => _isOn;
    [SerializeField] [Inactive] private bool _reverse;
    public bool _Reverse => _reverse;
    [SerializeField] [Inactive] private float _speed;
    public float _Speed => _speed;

    [SerializeField] [HideInInspector] protected bool _isGrouped;

    private bool isOn;
    public bool IsOn
    {
        get => isOn;

        set
        {
            isOn = value;
            _isOn = _spinnerLogic.OnStatus;
        }
    }

    private bool reverse;
    public bool Reverse
    {
        get => reverse;

        set
        {
            reverse = value;
            _reverse = _spinnerLogic.ReverseStatus;
        }
    }

    private float speed;
    public float Speed
    {
        get => speed;

        set
        {
            speed = value;
            _speed = _spinnerLogic.SpeedValue;
        }

    }

    private int forceOnStatus;
    public int ForceOnStatus
    {
        get => forceOnStatus;

        set
        {
            forceOnStatus = value;
            _isOn = _spinnerLogic.OnStatus;
        }
    }

    public int ForceReverseStatus { get; set; }
    public bool ReleaseSpeedStatus { get; set; }
    public float ForcedSpeedValue { get; set; }

    protected SpinnerLogic _spinnerLogic;

    protected virtual void FixedUpdate()
    {
        if (_spinnerLogic == null) return;

        _spinnerLogic.UpdateSimulation();
    }

    public void SetSpeedValue(string stringValue)
    {
        if (float.TryParse(stringValue, out float value))
        {
            Speed = value;
        }
    }
}

public interface ISpinnerControlAdapter
{
    public Integration Integration { get; }

    public bool _IsOn { get; }
    public bool _Reverse { get; }
    public float _Speed { get; }

    public bool IsOn { get; set; }
    public bool Reverse { get; set; }
    public float Speed { get; set; }

    public int ForceOnStatus { get; set; }
    public int ForceReverseStatus { get; set; }
    public bool ReleaseSpeedStatus { get; set; }
    public float ForcedSpeedValue { get; set; }
}

public class SpinnerLogic : IDynamicLogic
{
    private ISpinnerControlAdapter _adapter;
    private SpinnerSimulation[] _simulations;

    public SpinnerLogic(ISpinnerControlAdapter adapter, params SpinnerSimulation[] simulations)
    {
        _adapter = adapter;
        _integration = adapter.Integration;
        _simulations = simulations;
    }


    public bool OnStatus
    {
        get
        {
            return UpdateLogicValue(_adapter._IsOn, _adapter.IsOn, _adapter.ForceOnStatus, 0);
        }
    }

    public bool ReverseStatus
    {
        get
        {
            return UpdateLogicValue(_adapter._Reverse, _adapter.Reverse, _adapter.ForceReverseStatus, 1);
        }
    }

    public float SpeedValue
    {
        get
        {
            var result = UpdateNumericValue(_adapter._Speed, _adapter.Speed,
           _adapter.ReleaseSpeedStatus, _adapter.ForcedSpeedValue);
            _adapter.ForcedSpeedValue = result;
            return result;

        }
    }

    public override void UpdateSimulation()
    {
        if (_isPending && _simulations != null)
        {
            var speed = 0f;

            if (_adapter._IsOn)
            {
                speed = _adapter._Speed * (_adapter._Reverse ? -1 : 1);
                _adapter.Integration.HandleNoiseChange(speed / 10);
            }
            else
            {
                _adapter.Integration.HandleNoiseChange();
            }

            for (int i = 0; i < _simulations.Length; i++)
            {
                _simulations[i].UpdateSpinnerMotor(speed);
            }
        }
    }
}

public class SpinnerSimulation
{
    private ISpinnerUnitAdapter _adapter;

    public SpinnerSimulation(ISpinnerUnitAdapter adapter)
    {
        _adapter = adapter;
    }

    public void UpdateSpinnerMotor(float speed)
    {
        var motor = _adapter.Motor;
        motor.targetVelocity = speed * 50 * _adapter.SpeedFactor;
        _adapter.Motor = motor;
    }
}
