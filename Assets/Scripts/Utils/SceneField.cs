// This file was taken and adapted from https://forum.unity.com/threads/using-createassetmenu-and-scriptableobject.518022/
// It allows us to assign scenes asset in serialized scripts properties directly in the editor

using UnityEngine;

namespace Utils
{
    [System.Serializable]
    public class SceneField
    {
        [SerializeField] 
        private Object _sceneAsset;
        
        [SerializeField] 
        private string _sceneName = "";

        public string SceneName => _sceneName;

        public static implicit operator string(SceneField sceneField)
        {
            // Makes it work with the existing Unity methods (LoadLevel/LoadScene)
            return sceneField.SceneName;
        }
    }
}