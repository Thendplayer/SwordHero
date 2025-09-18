using UnityEngine;

namespace SwordHero.Core.Joystick
{
    public enum JoystickState
    {
        Idle,
        Dragging
    }
    
    public class JoystickModel
    {
        private readonly float _maxDistance;
        private readonly float _relocateThreshold;
        private readonly float _inputDeadZone;

        private Vector2 _originalPosition;
        private Vector2 _currentOffset;
        private JoystickState _state;

        public Vector2 OriginalPosition => _originalPosition;
        public Vector2 Direction => _currentOffset.normalized;
        public float Magnitude => Mathf.Clamp01(_currentOffset.magnitude / _maxDistance);
        public bool IsActive => Magnitude > _inputDeadZone;
        public bool IsDragging => _state == JoystickState.Dragging;
        public JoystickState State => _state;

        public JoystickModel(JoystickData data)
        {
            _maxDistance = data.MaxDistance;
            _relocateThreshold = data.RelocateThreshold;
            _inputDeadZone = data.InputDeadZone;
            _state = JoystickState.Idle;
        }

        public void SetOriginalPosition(Vector2 originalPosition)
        {
            _originalPosition = originalPosition;
        }

        public void StartDrag()
        {
            _state = JoystickState.Dragging;
            _currentOffset = Vector2.zero;
        }

        public void UpdateDrag(Vector2 offset)
        {
            if (_state != JoystickState.Dragging) return;
            _currentOffset = Vector2.ClampMagnitude(offset, _maxDistance);
        }

        public void EndDrag()
        {
            _state = JoystickState.Idle;
            _currentOffset = Vector2.zero;
        }

        public Vector2 CalculateRelocationOffset(Vector2 inputOffset)
        {
            if (inputOffset.magnitude <= _relocateThreshold)
                return Vector2.zero;

            return inputOffset.normalized * (inputOffset.magnitude - _maxDistance);
        }
    }
}