using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerGroup : SpinnerControl
{
    [SerializeField] private SpinnerUnit[] _spinnerUnits;
    private List<SpinnerSimulation> _spinnerSimulations = new List<SpinnerSimulation>();

    public void SimulationUnitRegister(SpinnerSimulation spinnerSimulation)
    {
        _spinnerSimulations.Add(spinnerSimulation);

        if(_spinnerSimulations.Count == _spinnerUnits.Length)
        {
            _spinnerLogic = new SpinnerLogic(this, _spinnerSimulations.ToArray());
        }
    }

    private void OnValidate()
    {
        _spinnerUnits = GetComponentsInChildren<SpinnerUnit>()
            .Where(u => u.IsGrouped).ToArray();
    }
}
