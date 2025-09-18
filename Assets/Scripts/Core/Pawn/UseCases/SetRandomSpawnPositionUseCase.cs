using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwordHero.Core.Pawn.UseCases
{
    public class SetRandomSpawnPositionUseCase
    {
        private static readonly float RADIUS = 8; //TODO: Expose variable
        
        private readonly PawnModel _playerModel;

        public SetRandomSpawnPositionUseCase(PawnModel playerModel)
        {
            _playerModel = playerModel;
        }

        public void Execute(Action<Vector3> setPosition)
        {
            if (RADIUS <= _playerModel.HitRadius)
            {
                Debug.LogError($"Cannot execute: radius ({RADIUS}) must be greater than ({_playerModel.HitRadius}).");
                return;
            }

            var randomPosition = GetRandomPositionAroundPoint(_playerModel.Position, RADIUS, _playerModel.HitRadius);
            setPosition(randomPosition);
        }

        private Vector3 GetRandomPositionAroundPoint(Vector3 centerPoint, float radius, float minRadius)
        {
            var randomDirection = Random.insideUnitCircle.normalized;
            var randomDistance = Random.Range(minRadius, radius);

            var randomOffset = new Vector3(randomDirection.x, 0f, randomDirection.y) * randomDistance;
            return centerPoint + randomOffset;
        }
    }
}