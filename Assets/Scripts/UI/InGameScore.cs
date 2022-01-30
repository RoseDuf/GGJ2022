using Game;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InGameScore : DayNightSensibleMonoBehaviour
    {
        private const int NUMBER_OF_RANDOM_SCORE_ANIMS = 4;
        
        private static readonly int SCORE_UP = Animator.StringToHash("ScoreUp");
        private static readonly int SCORE_RANDOM = Animator.StringToHash("ScoreRandom");

        [Header("Links")] 
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private Animator scoreAnimator;
        [SerializeField] private Animator comboAnimator;
        
        [Header("Fonts")]
        [SerializeField] private TMP_FontAsset dayFont;
        [SerializeField] private TMP_FontAsset nightFont;

        protected override void OnDay(int dayNumber)
        {
            if (scoreText && dayFont)
            {
                scoreText.font = dayFont;
            }
        }

        protected override void OnNight(int nightNumber)
        {
            if (scoreText && nightFont)
            {
                scoreText.font = nightFont;
            }
        }

        private void Start()
        {
            if (!ScoreManager.HasInstance)
                gameObject.SetActive(false);

            ScoreManager.Instance.OnScoreAdded += OnScoreAdded;
        }

        private void OnDestroy()
        {
            if (ScoreManager.HasInstance)
                ScoreManager.Instance.OnScoreAdded -= OnScoreAdded;
        }

        private void OnScoreAdded()
        {
            if (!scoreAnimator || !comboAnimator)
                return;
            
            scoreAnimator.SetInteger(SCORE_RANDOM, Random.Range(0, NUMBER_OF_RANDOM_SCORE_ANIMS));
            
            scoreAnimator.SetTrigger(SCORE_UP);
            comboAnimator.SetTrigger(SCORE_UP);
        }
        
        private void Update()
        {
            scoreText.text = $"{ScoreManager.Instance.Score:n0}";

            var currentCombo = ScoreManager.Instance.CurrentCombo;
            comboText.enabled = currentCombo != 0 && GameManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night;
            comboText.text = $"x{ScoreManager.Instance.CurrentCombo}";
        }
    }
}