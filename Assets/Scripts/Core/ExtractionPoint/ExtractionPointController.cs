using System;
using MessagePipe;
using SwordHero.Core.Events;
using SwordHero.Core.ExtractionPoint.UseCases;
using UnityEngine;
using VContainer.Unity;

namespace SwordHero.Core.ExtractionPoint
{
    public class ExtractionPointController : IInitializable, IDisposable
    {
        private readonly ExtractionPointModel _model;
        private readonly ExtractionPointView _view;
        private readonly SetExtractionPointPositionUseCase _setPositionUseCase;
        private readonly IPublisher<ExtractionPointTriggeredEvent> _triggeredEventPublisher;

        public ExtractionPointController(
            ExtractionPointModel model,
            ExtractionPointView view,
            SetExtractionPointPositionUseCase setPositionUseCase,
            IPublisher<ExtractionPointTriggeredEvent> triggeredEventPublisher
        )
        {
            _model = model;
            _view = view;
            _setPositionUseCase = setPositionUseCase;
            _triggeredEventPublisher = triggeredEventPublisher;
        }

        public void Initialize()
        {
            _setPositionUseCase.Execute(_model.RadiusRange, SetPosition);
            _view.OnTriggerActivated += OnTriggerActivated;
        }

        private void SetPosition(Vector3 position)
        {
            _model.SetPosition(position);
            _view.SetPosition(position);
        }
        
        public void Dispose()
        {
            _view.OnTriggerActivated -= OnTriggerActivated;
        }

        private void OnTriggerActivated()
        {
            var triggerEvent = new ExtractionPointTriggeredEvent(_model.Position);
            _triggeredEventPublisher.Publish(triggerEvent);
            _setPositionUseCase.Execute(_model.RadiusRange, SetPosition);
        }
    }
}