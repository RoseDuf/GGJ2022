using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        public event Action OnScoreAdded;
        
        private GlobalSettings settings;
        
        private int score = 0;
        private int currentCombo = 0;
        private float currentpoints = 0;

        private Coroutine comboEndCoroutine = null;
        
        public int Score => score;
        public int CurrentCombo => currentCombo;
        public float CurrentPoints => currentpoints;

        private float ComboMultiplier => 1.0f + currentCombo * settings.ComboScoreMultiplier;

        private void Start()
        {
            settings = SettingsSystem.Instance.Settings;
        }

        public void AddScoreForKilling(int fatnessLevel)
        {
            currentpoints = (int)(settings.BasePointsForKilling * Math.Pow(settings.FatnessMultiplier, fatnessLevel) * ComboMultiplier);
            score += (int) (settings.BasePointsForKilling * Math.Pow(settings.FatnessMultiplier, fatnessLevel) * ComboMultiplier);
            
            ++currentCombo;
            StartComboEndTimer();
            LastScoreSystem.Instance.UpdateLastScore(score);
            OnScoreAdded?.Invoke();
        }
        
        private void StartComboEndTimer()
        {
            if (comboEndCoroutine != null)
            {
                StopCoroutine(comboEndCoroutine);
                comboEndCoroutine = null;
            }
            comboEndCoroutine = StartCoroutine(ComboEndRoutine());
        }

        private IEnumerator ComboEndRoutine()
        {
            yield return new WaitForSeconds(settings.SecondsBeforeEndOfCombo);
            currentCombo = 0;
        }
    }
}