using System;
using UnityEditor;
using UnityEngine;

/**
 * Base on this tutorial
 * https://www.youtube.com/watch?v=33RL196x4LI&ab_channel=Zenva
 */

[Serializable]
public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)] [Tooltip("* 0 = 00:00\n* 0.5 = 12h\n* 1 = 23:59")]
    public float time;

    public float fullDayLength;
    public float startTime = 0.35f;

    private float timeRate = 30;
    public Vector3 noon;

    [Header("Sun")] public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")] public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")] public AnimationCurve lightIntensityMultiplier;
    public AnimationCurve reflectiontIntensityMultiplier;
    
    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;

        RenderSettings.sun = sun;
    }

    private void Update()
    {
        timeRate = 1.0f / fullDayLength;
        time += timeRate * Time.deltaTime;

        if (time > 1.0f)
        {
            time = 0;
        }

        sun.transform.eulerAngles = (time - 0.25f) * noon * 4.0f;
        moon.transform.eulerAngles = (time - 0.75f) * noon * 4.0f;

        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(false);
        }
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(true);
        }
        
        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(false);
        }
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(true);
        }

        RenderSettings.ambientIntensity = lightIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectiontIntensityMultiplier.Evaluate(time);

        if (time < 0.6)
        {
            ThresholdReachedEventArgs args = new ThresholdReachedEventArgs();
            args.Period = DaytimeManager.TimeOfDay.Day;
            args.TimeReached = time;
            OnNightThresholdReached(args);
        }
        else if (time >= 0.6 && time < 0.8)
        {
            ThresholdReachedEventArgs args = new ThresholdReachedEventArgs();
            args.Period = DaytimeManager.TimeOfDay.Evening;
            args.TimeReached = time;
            OnNightThresholdReached(args);
        }
        else if (time >= 0.8)
        {
            ThresholdReachedEventArgs args = new ThresholdReachedEventArgs();
            args.Period = DaytimeManager.TimeOfDay.Night;
            args.TimeReached = time;
            OnNightThresholdReached(args);
        }
        
    }
    
    protected virtual void OnNightThresholdReached(ThresholdReachedEventArgs e)
    {
        EventHandler<ThresholdReachedEventArgs> handler = nightThreshold;
        if (handler != null)
        {
            handler(this, e);
        }
    }
    
    public event EventHandler<ThresholdReachedEventArgs> nightThreshold;
    
    public class ThresholdReachedEventArgs : EventArgs
    {
        public DaytimeManager.TimeOfDay Period { get; set; }
        public float TimeReached { get; set; }
    }
}