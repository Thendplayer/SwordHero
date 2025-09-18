using System.Collections.Generic;
using UnityEngine;

namespace SwordHero.Core.Pawn.Adapters
{
    public enum Layer
    {
        Player,
        Enemy
    }
    
    public interface IPhysicsBody {
        Vector3 Position { get; }
        void SetPosition(Vector3 position);
        bool Equals(IPhysicsBody other);
    }

    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyAdapter : MonoBehaviour, IPhysicsBody
    {
        [SerializeField] private Layer layer;

        private static readonly Dictionary<Layer, List<RigidbodyAdapter>> _entitiesByType = new();
        
        private Rigidbody _rigidbody;

        public Layer Layer => layer;
        public Vector3 Position => _rigidbody.position;

        public static IReadOnlyList<RigidbodyAdapter> GetEntitiesOfType(Layer type)
        {
            if (_entitiesByType.TryGetValue(type, out var list))
                return list.AsReadOnly();
            
            return new List<RigidbodyAdapter>().AsReadOnly();
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            if (!_entitiesByType.ContainsKey(layer))
                _entitiesByType[layer] = new List<RigidbodyAdapter>();

            _entitiesByType[layer].Add(this);
        }

        private void OnDisable()
        {
            if (_entitiesByType.TryGetValue(layer, out var list))
                list.Remove(this);
        }

        public void SetPosition(Vector3 position) => _rigidbody.position = position;

        public bool Equals(IPhysicsBody other) => ReferenceEquals(this, other);
    }
}