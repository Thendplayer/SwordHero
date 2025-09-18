using System;
using System.Collections.Generic;
using System.Linq;
using MessagePipe;
using SwordHero.Core.Events;
using SwordHero.Core.Pawn.Adapters;
using UnityEngine;

namespace SwordHero.Core.Pawn.UseCases
{
    public class AttackClosestTargetUseCase
    {
        private readonly IPublisher<OnPawnAttackedEvent> _onPawnAttackedEventPublisher;

        public AttackClosestTargetUseCase(IPublisher<OnPawnAttackedEvent> onPawnAttackedEventPublisher)
        {
            _onPawnAttackedEventPublisher = onPawnAttackedEventPublisher;
        }

        public void Execute(Vector3 currentPosition, Layer targetType, float radius, int damage, Action<Vector3> onAttack)
        {
            var enemies = RigidbodyAdapter.GetEntitiesOfType(targetType);
            if (!enemies.Any()) return;

            var closestTarget = FindClosestEnemyWithinRadius(enemies, currentPosition, radius);
            if (closestTarget == null) return;

            var directionToTarget = closestTarget.Position - currentPosition;

            var onPawnAttackedEvent = new OnPawnAttackedEvent(closestTarget, damage);
            _onPawnAttackedEventPublisher.Publish(onPawnAttackedEvent);

            onAttack(directionToTarget);
        }

        private RigidbodyAdapter FindClosestEnemyWithinRadius(IReadOnlyList<RigidbodyAdapter> enemies, Vector3 playerPosition, float radius)
        {
            RigidbodyAdapter closestEnemy = null;
            var closestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                var distance = Vector3.Distance(playerPosition, enemy.Position);

                if (distance <= radius && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }
    }
}