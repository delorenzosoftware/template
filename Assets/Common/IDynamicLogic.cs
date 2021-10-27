public abstract class IDynamicLogic
{
    protected Integration _integration;
    protected bool _isPending;

    public abstract void UpdateSimulation();

    protected bool UpdateLogicValue(bool current, bool released,
        int forced, int eventIndex = 0)
    {
        var newValue = released;
        if(forced >= 0) newValue = forced > 0;

        if(newValue != current)
        {
            _integration?.HandleStateChange(newValue, eventIndex);
            _isPending = true;
        }

        return newValue;
    }

    protected float UpdateNumericValue(float current, float released,
    bool status, float forced, int eventIndex = 0)
    {
        var newValue = status ? released : forced;

        if(newValue != current)
        {
            if(newValue < 0) { newValue = 0; }
            else if(newValue > 10) { newValue = 10; }

            _integration?.HandleStateChange(newValue, eventIndex);
            _integration?.HandleStateChange(newValue.ToString("0.00"), eventIndex);
            _isPending = true;
        }

        return newValue;
    }
}