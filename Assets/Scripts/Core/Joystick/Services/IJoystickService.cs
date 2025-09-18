using UnityEngine;

namespace SwordHero.Core.Joystick.Services
{
    public interface IJoystickService
    {
        Touch? GetCurrentInputState();
        Vector2 ScreenToCanvasPosition(Vector2 screenPosition);
        Vector2 ScreenToJoystickLocal(Vector2 screenPosition);
        void SetEnabled(bool enabled);
    }
}