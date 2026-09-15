using System;
using LagMenu.Utilities;
using UnityEngine;

namespace LagMenu.Menu
{
    public class TabletButtonPress : MonoBehaviour
    {
        public Action onPress;
        public GameObject touchReference;

        private float _cooldown;

        private void Update()
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (touchReference == null || other.gameObject != touchReference) return;
            if (_cooldown > 0f) return;

            _cooldown = 0.25f;
            ResourceManager.PlayButtonSound();
            onPress?.Invoke();
        }
    }
}
