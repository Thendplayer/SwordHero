using System.Linq;
using SwordHero.Core.Pawn.Adapters;
using UnityEngine;

namespace SwordHero.Core.Pawn.UseCases
{
    public class SetTargetDirectionUseCase
    {
        public void Execute(Vector3 currentPosition, float hitRadius, Layer targetLayer, out bool targetReached, out Vector2 targetDirection)
        {
            targetDirection = Vector2.zero;

            var target = RigidbodyAdapter.GetEntitiesOfType(targetLayer);
            if (!target.Any())
            {
                targetReached = true;
                return;
            }

            var direction = target.First().Position - currentPosition;
            var distance = direction.magnitude;

            if (distance <= hitRadius)
            {
                targetReached = true;
                return;
            }

            targetDirection = new Vector2(-direction.x, -direction.z).normalized;
            targetReached = false;
        }
    }
}