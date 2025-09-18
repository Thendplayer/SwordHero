using UnityEngine;

namespace SwordHero.Core.Events
{
    public struct ExtractionPointTriggeredEvent
    {
        public Vector3 Position { get; }

        public ExtractionPointTriggeredEvent(Vector3 position)
        {
            Position = position;
        }
    }
}