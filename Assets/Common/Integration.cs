using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using System.Diagnostics;
#endif

[Serializable]
public class Integration
{
    [Space()]
    [SerializeField]
    private StateChangeEvent<bool>[] logicStateIntegrations;

    [Space()]
    [SerializeField]
    private StateChangeEvent<float>[] numericStateIntegrations;

    [Space()]
    [SerializeField]
    private StateChangeEvent<string>[] textStateIntegrations;

    [Space()]
    [SerializeField]
    private AudioIntegration[] audioIntegrations;

    public void HandleStateChange(bool state, int index = 0)
    {
        if (logicStateIntegrations == null || index >= logicStateIntegrations.Length)
        {
#if UNITY_EDITOR
            ThrowWarningMessage(
                $"The requested Logic Event Index ({index}) is out of bounds.");
#endif
            return;
        }

        logicStateIntegrations[index].HandleStateChange(state);
    }

    public void HandleStateChange(float value, int index = 0)
    {
        if (numericStateIntegrations == null || index >= numericStateIntegrations.Length)
        {
#if UNITY_EDITOR
            ThrowWarningMessage(
                $"The requested Numeric Event Index ({index}) is out of bounds.");
#endif
            return;
        }

        numericStateIntegrations[index].HandleStateChange(value);
    }

    public void HandleStateChange(string text, int index = 0)
    {
        if (textStateIntegrations == null || index >= textStateIntegrations.Length)
        {
#if UNITY_EDITOR
            ThrowWarningMessage(
                $"The requested Text Event Index ({index}) is out of bounds.");
#endif
            return;
        }

        textStateIntegrations[index].HandleStateChange(text);
    }

    public void HandleNoiseChange(float intensity = 0, params int[] indexes)
    {
        if (audioIntegrations == null) return;
        if (indexes.Length == 0 || indexes == null)
        {
            for (int i = 0; i < audioIntegrations.Length; i++)
            {
                audioIntegrations[i].HandleNoiseIntensityChange(intensity);
            }
        }
        else
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] < audioIntegrations.Length)
                {
                    audioIntegrations[indexes[i]].HandleNoiseIntensityChange(intensity);
                }
#if UNITY_EDITOR
                else
                {
                    ThrowWarningMessage(
                        $"[The requested Audio Event Index ({indexes[i]}) is out of bounds.");
                }
#endif
            }
        }
    }

#if UNITY_EDITOR
    private void ThrowWarningMessage(string message)
    {
        // var stackFrame = new StackFrame(2, true);

        // UnityEngine.Debug.LogWarning(string.Format("{3} ({0}.{1}:{2})",
        //     stackFrame.GetMethod().DeclaringType.Name,
        //     stackFrame.GetMethod().Name,
        //     stackFrame.GetFileLineNumber(),
        //     message));
    }
#endif
}

[Serializable]
public class StateChangeEvent<T>
{
    [SerializeField]
    private string integrationLabel;

    [SerializeField]
    [Space()]
    private UnityEvent<T> onStateChange;

    public void HandleStateChange(T state)
    {
        onStateChange?.Invoke(state);
    }
}

[Serializable]
public class AudioIntegration
{
    [SerializeField]
    private string integrationLabel;

    [SerializeField]
    private AudioSource audioSource;

    [Space()]
    [SerializeField]
    private UnityEvent<AudioSource, float> onIntensityChange;

    [SerializeField]
    private UnityEvent<AudioSource> onZeroIntensity;

    public void HandleNoiseIntensityChange(float intensity)
    {
        intensity = Mathf.Abs(intensity);
        if (intensity > 1) intensity = 1;

        if (intensity == 0)
        {
            onZeroIntensity?.Invoke(audioSource);
        }
        else
        {
            onIntensityChange?.Invoke(audioSource, intensity);
        }
    }
}
