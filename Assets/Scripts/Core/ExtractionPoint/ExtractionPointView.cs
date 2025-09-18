using System;
using UnityEngine;

namespace SwordHero.Core.ExtractionPoint
{
    public class ExtractionPointView : MonoBehaviour
    {
        [SerializeField] private Collider _triggerCollider;

        public Action OnTriggerActivated;

        private void OnTriggerEnter(Collider other) => OnTriggerActivated?.Invoke();

        public void SetPosition(Vector3 position) => transform.position = position;
    }
}