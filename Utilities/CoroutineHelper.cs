using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LagMenu.Utilities
{
    public class CoroutineHelper : MonoBehaviour
    {
        public static CoroutineHelper Instance { get; private set; }

        public static IEnumerator DestroyAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            if ((Object)(object)obj != (Object)null)
            {
                Object.Destroy((Object)(object)obj);
            }
        }

        private void Awake()
        {
            if ((Object)(object)Instance != (Object)null)
            {
                Object.Destroy((Object)(object)gameObject);
                return;
            }
            Instance = this;
            Object.DontDestroyOnLoad((Object)(object)gameObject);
        }

        private static IEnumerator InvokeAfterDelayCoroutine(float time, Action afterDelay)
        {
            yield return new WaitForSeconds(time);
            try
            {
                afterDelay();
            }
            catch (Exception ex)
            {
                Exception e = ex;
                Debug.LogError($"Delay coroutine caught exception: {e}");
            }
        }

        public static void InvokeAfterDelay(float time, Action afterDelay)
        {
            if (afterDelay == null)
            {
                Debug.LogError("Delay called with null action!");
            }
            else
            {
                Instance.StartCoroutine(InvokeAfterDelayCoroutine(time, afterDelay));
            }
        }
    }
}
