using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public class OogaBooga : MonoBehaviour
    {
        private Animator SplashAnimator;
        private void Start()
        {
            SplashAnimator = GetComponent<Animator>();
            StartCoroutine(OogaBoogaRoutine());
        }

        private IEnumerator OogaBoogaRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                Debug.Log("OOGA BOOGA");
                SplashAnimator.SetTrigger("SplashTrigger");
                SoundSystem.Instance.PlayOogaBoogaSplashSound();
            }
        }
    }
}