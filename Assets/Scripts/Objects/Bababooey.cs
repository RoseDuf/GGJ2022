using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class Bababooey : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(BababooeyRoutine());
        }

        private IEnumerator BababooeyRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                SoundSystem.Instance.PlayBababooeySound();
            }
        }
    }
}