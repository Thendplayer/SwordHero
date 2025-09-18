using System;
using SwordHero.Core.Pawn;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwordHero.Core.ExtractionPoint.UseCases
{
    public class SetExtractionPointPositionUseCase
    {
        private readonly PawnModel _playerModel;

        public SetExtractionPointPositionUseCase(PawnModel playerModel)
        {
            _playerModel = playerModel;
        }

        public void Execute(Vector2 radiusRange, Action<Vector3> setPosition)
        {
            var randomDirection = Random.insideUnitCircle.normalized;
            var randomDistance = Random.Range(radiusRange.x, radiusRange.y);

            var randomOffset = new Vector3(randomDirection.x, 0f, randomDirection.y) * randomDistance;
            setPosition(_playerModel.Position + randomOffset);
        }
    }
}