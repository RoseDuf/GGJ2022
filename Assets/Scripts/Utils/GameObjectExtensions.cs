using UnityEngine;

namespace Game
{
    public static class GameObjectExtensions
    {
        public static int GetNumberOfComponents(this GameObject gameObject)
        {
            return gameObject.GetComponentsInChildren<Component>().Length - 1;
        }
    }
}