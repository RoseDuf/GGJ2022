using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public class OogaBooga : MonoBehaviour
    {
        Scene scene;

        private Animator SplashAnimator;
        private float AnimationLength;
        private AudioSource SplashSound;

        //----Did want this member but dont find a sexy way of getting animationClip length
        [SerializeField] private AnimationClip SplashAnimation;
        
        [SerializeField] private float SecondToWaitBeforeTrigger = 1.0f;
        
        [Tooltip("Set this scene index to the last one and et your first scene index here (probably 0)"), SerializeField]
        private int NextSceneIndex = 0;

        private void Start()
        {
            scene = SceneManager.GetActiveScene();
            Debug.Log("Active Scene name is: " + scene.name + "\nActive Scene index: " + scene.buildIndex);

            SplashAnimator = GetComponent<Animator>();
            SplashSound = GetComponent<AudioSource>();
            
            AnimationLength = SplashAnimation.length;

            StartCoroutine(OogaBoogaRoutine());
        }

        private void FindAnimationLength()
        {
            
            var animatorClipInfos = SplashAnimator.GetCurrentAnimatorClipInfo(0);
            Debug.Log("FINNNNNDDD : "  +animatorClipInfos.Length);
            foreach (var clipInfo in animatorClipInfos)
            {
                Debug.Log("clipName: " + clipInfo.clip.name);
                if (clipInfo.clip.name == "SplashAnimation")
                {
                    
                    AnimationLength = clipInfo.clip.length;
                    break;
                }
            }
            
            Debug.Log("AnimationLength: "+AnimationLength);
        }

        private IEnumerator OogaBoogaRoutine()
        {
            Debug.Log("OOGA BOOGA");
            yield return new WaitForSeconds(SecondToWaitBeforeTrigger);

            SplashAnimator.SetTrigger("SplashTrigger");
            SplashSound.Play();
        }

        public void AnimationObservers(string message)
        {
            if (message == "AnimationEnded")
            {
                Debug.Log("LOAAD NEXT SCENE" );
            }
        }
        
        private IEnumerator LoadNextSceneAfter()
        {
            yield return new WaitForSeconds(AnimationLength);
                
            Debug.Log("LOAAD NEXT SCENE" );
            
            SceneManager.LoadScene(NextSceneIndex, LoadSceneMode.Single);
        }
    }
}