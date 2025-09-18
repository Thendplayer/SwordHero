using SwordHero.Repositories;
using SwordHero.Core.Joystick.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SwordHero.Core.Joystick
{
    [CreateAssetMenu(menuName = "Recipes/Joystick", fileName = "JoystickRecipe")]
    public class JoystickRecipe : StandardRecipe
    {
        [SerializeField] private JoystickView _view;
        [SerializeField] private JoystickData _data;

        public override void Register(IContainerBuilder builder)
        {
            var canvas = FindObjectOfType<Canvas>(); //Get Main Canvas before Injection is ready
            
            builder.RegisterInstance(_data);
            builder.RegisterComponentInNewPrefab(_view, Lifetime.Singleton).UnderTransform(canvas.transform);
            builder.Register<JoystickModel>(Lifetime.Singleton);
            builder.Register<JoystickController>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<IJoystickService, UnityJoystickService>(Lifetime.Singleton);
        }
    }
}