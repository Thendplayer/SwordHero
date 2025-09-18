using UnityEngine;

namespace SwordHero.Core.ExtractionPoint
{
    public class ExtractionPointModel
    {
        private readonly Vector2 _radiusRange;
        public Vector3 Position { get; private set; }
        public Vector2 RadiusRange => _radiusRange;

        public ExtractionPointModel(ExtractionPointData data)
        {
            _radiusRange = data.RadiusRange;
        }

        public void SetPosition(Vector3 position) => Position = position;
    }
}