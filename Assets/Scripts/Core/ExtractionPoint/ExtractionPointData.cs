using System;
using UnityEngine;

namespace SwordHero.Core.ExtractionPoint
{
    [Serializable]
    public class ExtractionPointData
    {
        [SerializeField, Range(5f, 15f)] private float _minRadius = 5f;
        [SerializeField, Range(10f, 30f)] private float _maxRadius = 10f;
        
        public Vector2 RadiusRange => new(_minRadius, _maxRadius);
    }
}