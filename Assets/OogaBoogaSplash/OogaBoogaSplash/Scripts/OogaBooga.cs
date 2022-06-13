using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public class OogaBooga : MonoBehaviour
    {
        [SerializeField] private AnimationClip splashAnimation;

        [SerializeField] private float secondToWaitBeforeTrigger = 0f;

        [Tooltip("Set this scene name to load after oogabooga splash")] [SerializeField]
        private string nextSceneName = "MainMenu";

        private Animator _splashAnimator;

        private void Start()
        {
            _splashAnimator = GetComponent<Animator>();
            StartCoroutine(OogaBoogaRoutine());
        }

        private IEnumerator OogaBoogaRoutine()
        {
            Debug.Log("OOGA BOOGA");
            yield return new WaitForSeconds(secondToWaitBeforeTrigger);
            _splashAnimator.SetTrigger("SplashTrigger");

            SoundSystem.Instance.PlaySplashSound();
        }

        /**
         * Link to an animation event a then of the animation
         */
        public void AnimationObservers(string message)
        {
            if (message == "AnimationEnded")
            {
                Debug.Log("LOAAD NEXT SCENE");
                SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
            }
        }
    }
}