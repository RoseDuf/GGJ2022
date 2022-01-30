using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace System
{
    public class SceneManager : PersistentSingleton<SceneManager>
    {
        
        public event Action<string> OnSceneLoaded;
        private readonly HashSet<MonoBehaviour> scriptsToTransfer = new HashSet<MonoBehaviour>();
        private Coroutine loadSceneCoroutine;

        public bool IsLoading => loadSceneCoroutine != null;
        public float Progress { get; private set; }
        public string ActiveScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        public void RegisterForWorldTransfer(MonoBehaviour scriptToTransfer)
        {
            scriptsToTransfer.Add(scriptToTransfer);
        }
        
        public void UnregisterForWorldTransfer(MonoBehaviour scriptToStopTransferring)
        {
            scriptsToTransfer.Remove(scriptToStopTransferring);
        }

        public void LoadScene(SceneField sceneToLoad)
        {
            LoadScene(sceneToLoad.SceneName);
        }

        public void LoadScene(string sceneToLoad)
        {
            if (IsLoading)
            {
                Debug.LogWarning($"Trying to call {nameof(SceneManager)}.{nameof(LoadScene)} while a scene is already loading.");
            }

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneToLoad)
            {
                throw new InvalidOperationException($"Trying to load the same scene twice ({sceneToLoad})");
            }

            loadSceneCoroutine = StartCoroutine(LoadSceneRoutine(sceneToLoad));
        }

        private IEnumerator LoadSceneRoutine(string sceneToLoad)
        {
            yield return null;
            
            var currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            yield return StartCoroutine(LoadNewSceneSubRoutine(sceneToLoad));
            ActivateScene(sceneToLoad);
            yield return StartCoroutine(UnloadSceneSubRoutine(currentActiveScene));

            yield return null;
            
            loadSceneCoroutine = null;
            OnSceneLoaded?.Invoke(sceneToLoad);
        }

        private IEnumerator LoadNewSceneSubRoutine(string sceneToLoad)
        {
            yield return null;
            
            Progress = 0.0f;
            
            var loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = false;
            
            //Keep going
            while (!loadOperation.isDone)
            {
                Progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                
                if (Math.Abs(loadOperation.progress - 0.9f) < 0.01f)
                {
                    loadOperation.allowSceneActivation = true;
                }
                
                yield return null;
            }

            Progress = 1.0f;

            yield return null;
        }

        private IEnumerator UnloadSceneSubRoutine(string sceneToUnload)
        {
            yield return null;

            var unloadOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneToUnload);
            while (unloadOperation != null && !unloadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
        }

        private void ActivateScene(string destinationScene)
        {
            var sceneToActivate = UnityEngine.SceneManagement.SceneManager.GetSceneByName(destinationScene);
            if (sceneToActivate.IsValid())
            {
                foreach (var scriptToTransfer in scriptsToTransfer)
                {
                    if (scriptToTransfer)
                    {
                        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(scriptToTransfer.gameObject, sceneToActivate);
                    }
                }
                
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(sceneToActivate);
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            loadSceneCoroutine = null;

            Progress = 1.0f;
            
            RegisterForWorldTransfer(this);
        }

        private void OnDestroy()
        {
            if (loadSceneCoroutine != null)
            {
                StopCoroutine(loadSceneCoroutine);
            }
        }
    }
}