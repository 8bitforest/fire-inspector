using UnityEngine;

namespace FireInspector.Editor.Extensions
{
    public static class GameObjectExtensions
    {
        public static string GetObjectPath(this GameObject gameObject)
        {
            var scene = gameObject.scene;
            if (scene.IsValid())
            {
                var sceneName = scene.name;
                var objectPath = GetObjectPath(gameObject.transform);
                return $"[{sceneName}] {objectPath}";
            }

            return GetObjectPath(gameObject.transform);
        }

        private static string GetObjectPath(Transform current)
        {
            if (current.parent == null)
                return "/" + current.name;
            return GetObjectPath(current.parent) + "/" + current.name;
        }
    }
}