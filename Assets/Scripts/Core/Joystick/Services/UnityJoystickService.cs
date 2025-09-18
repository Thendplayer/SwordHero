using UnityEngine;

namespace SwordHero.Core.Joystick.Services
{
    public class UnityJoystickService : IJoystickService
    {
        private readonly Canvas _canvas;
        private readonly Camera _camera;
        private readonly JoystickView _joystickView;
        private bool _isEnabled = true;

        public UnityJoystickService(Canvas canvas, Camera camera, JoystickView joystickView)
        {
            _canvas = canvas;
            _camera = camera;
            _joystickView = joystickView;
        }

        public Touch? GetCurrentInputState()
        {
            if (!_isEnabled) return null;

#if UNITY_EDITOR
            return GetMouseInputState();
#else
            return GetTouchInputState();
#endif
        }

        public Vector2 ScreenToCanvasPosition(Vector2 screenPosition)
        {
            var camera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _camera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                screenPosition,
                camera,
                out var canvasPosition);

            return canvasPosition;
        }

        public Vector2 ScreenToJoystickLocal(Vector2 screenPosition)
        {
            var camera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _camera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _joystickView.RectTransform,
                screenPosition,
                camera,
                out var localPoint);

            return localPoint;
        }
        
        public void SetEnabled(bool enabled) => _isEnabled = enabled;

#if UNITY_EDITOR
        private Touch? GetMouseInputState()
        {
            var position = Input.mousePosition;
            TouchPhase phase;

            if (Input.GetMouseButtonDown(0))
                phase = TouchPhase.Began;
            else if (Input.GetMouseButtonUp(0))
                phase = TouchPhase.Ended;
            else if (Input.GetMouseButton(0))
                phase = TouchPhase.Moved;
            else
                return null; // No input activity

            return new Touch
            {
                fingerId = 0,
                position = position,
                phase = phase
            };
        }
#else
        private Touch? GetTouchInputState()
        {
            if (Input.touchCount == 0)
                return null; // No touch input

            return Input.GetTouch(0);
        }
#endif
    }
}