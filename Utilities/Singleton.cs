using UnityEngine;

namespace LagMenu.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }
      

        public  void Awake()
        {
            if (Instance != null && Instance != this)
            {
                gameObject.Obliterate();

                return;
            }

            Instance = this as T;
        }
    }
}