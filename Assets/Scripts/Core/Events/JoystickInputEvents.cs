using UnityEngine;

namespace SwordHero.Core.Events
{
    public struct JoystickDragEvent
    {
        public Vector2 Direction { get; }
        public float Magnitude { get; }

        public JoystickDragEvent(Vector2 direction, float magnitude)
        {
            Direction = direction;
            Magnitude = magnitude;
        }
    }
    
    public struct JoystickPressedEvent { }
    
    public struct JoystickReleasedEvent { }
}