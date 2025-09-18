using SwordHero.Repositories;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SwordHero.Core.ExtractionPoint
{
    [CreateAssetMenu(menuName = "Recipes/ExtractionPoint", fileName = "ExtractionPointRecipe")]
    public class ExtractionPointRecipe : StandardRecipe
    {
        [SerializeField] private ExtractionPointView _view;
        [SerializeField] private ExtractionPointData _data;

        public override void Register(IContainerBuilder builder)
        {
            builder.RegisterInstance(_data);
            builder.RegisterComponentInNewPrefab(_view, Lifetime.Singleton);
            builder.Register<ExtractionPointModel>(Lifetime.Singleton);
            builder.Register<ExtractionPointController>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}