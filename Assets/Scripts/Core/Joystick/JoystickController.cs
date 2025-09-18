using MessagePipe;
using SwordHero.Core.Events;
using SwordHero.Core.Joystick.Services;
using UnityEngine;
using VContainer.Unity;

namespace SwordHero.Core.Joystick
{
    public class JoystickController : ITickable
    {
        private readonly JoystickModel _model;
        private readonly JoystickView _view;
        private readonly IJoystickService _service;
        
        private readonly IPublisher<JoystickDragEvent> _dragEventPublisher;
        private readonly IPublisher<JoystickPressedEvent> _pressedEventPublisher;
        private readonly IPublisher<JoystickReleasedEvent> _releasedEventPublisher;

        public JoystickController(
            JoystickModel model,
            JoystickView view,
            IJoystickService service,
            IPublisher<JoystickDragEvent> dragEventPublisher,
            IPublisher<JoystickPressedEvent> pressedEventPublisher,
            IPublisher<JoystickReleasedEvent> releasedEventPublisher)
        {
            _model = model;
            _view = view;
            _service = service;

            _dragEventPublisher = dragEventPublisher;
            _pressedEventPublisher = pressedEventPublisher;
            _releasedEventPublisher = releasedEventPublisher;

            _model.SetOriginalPosition(_view.RectTransform.anchoredPosition);
        }

        public void Tick()
        {
            var touch = _service.GetCurrentInputState();

            // Process input when starting interaction or during ongoing interaction
            if (touch.HasValue && (_model.IsDragging || touch.Value.phase == TouchPhase.Began))
            {
                var canvasPosition = _service.ScreenToCanvasPosition(touch.Value.position);
                ProcessInput(touch.Value, canvasPosition);
            }
        }

        private void ProcessInput(Touch touch, Vector2 canvasPosition)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchBegan(canvasPosition);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    HandleTouchMoved(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnded();
                    break;
            }
        }

        private void HandleTouchBegan(Vector2 canvasPosition)
        {
            _view.SetPosition(canvasPosition);
            _view.SetHandlePosition(Vector2.zero);
            _model.StartDrag();
            _pressedEventPublisher.Publish(new JoystickPressedEvent());
        }

        private void HandleTouchMoved(Vector2 screenPosition)
        {
            var localOffset = _service.ScreenToJoystickLocal(screenPosition);

            var relocationOffset = _model.CalculateRelocationOffset(localOffset);
            if (relocationOffset != Vector2.zero)
            {
                var currentPosition = _view.RectTransform.anchoredPosition;
                _view.SetPosition(currentPosition + relocationOffset);
                localOffset = _service.ScreenToJoystickLocal(screenPosition);
            }

            _model.UpdateDrag(localOffset);
            _view.SetHandlePosition(localOffset);

            if (!_model.IsActive) return;

            var dragEvent = new JoystickDragEvent(_model.Direction, _model.Magnitude);
            _dragEventPublisher.Publish(dragEvent);
        }

        private void HandleTouchEnded()
        {
            _model.EndDrag();
            _view.SetHandlePosition(Vector2.zero);
            _view.SetPosition(_model.OriginalPosition);
            _releasedEventPublisher.Publish(new JoystickReleasedEvent());
        }
    }
}