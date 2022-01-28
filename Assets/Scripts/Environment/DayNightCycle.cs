using System;
using Game;
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
    public Material _skyboxDayMaterial;
    public Material _skyboxNightMaterial;

    public void Stop() => enabled = false;
    public void Resume() => enabled = true;

    [SerializeField] private bool timePause = false;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;

        RenderSettings.sun = sun;
        if (time > DaytimeManager.DayStartTime && time < DaytimeManager.DayEndTime)
            RenderSettings.skybox = _skyboxDayMaterial;
        else
        {
            RenderSettings.skybox = _skyboxNightMaterial;
        }
    }

    private void Update()
    {
        timeRate = 1.0f / fullDayLength;
        
        if(!timePause)
            time += timeRate * Time.deltaTime;

        if (time > 1.0f)
            time = 0;

        sun.transform.eulerAngles = (time -  DaytimeManager.DayStartTime) * noon * 4.0f;
        moon.transform.eulerAngles = (time -  DaytimeManager.DayEndTime) * noon * 4.0f;

        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(false);
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(true);
            RenderSettings.skybox = _skyboxDayMaterial;
        }


        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(false);
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(true);
            RenderSettings.skybox = _skyboxNightMaterial;
        }

        RenderSettings.ambientIntensity = lightIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectiontIntensityMultiplier.Evaluate(time);
        
        _skyboxDayMaterial.SetFloat("_Exposure", sun.intensity);
        _skyboxNightMaterial.SetFloat("_Exposure", moon.intensity);
        
            
            
        
        
    }
}