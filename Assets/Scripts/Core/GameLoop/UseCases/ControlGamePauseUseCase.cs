using SwordHero.Core.Joystick;
using SwordHero.Core.Joystick.Services;
using SwordHero.Core.Pawn.Player;
using SwordHero.Core.Spawner;

namespace SwordHero.Core.GameLoop.UseCases
{
    public class ControlGamePauseUseCase
    {
        private readonly JoystickView _joystickView;
        private readonly IJoystickService _joystickService;
        private readonly EnemySpawnerController _spawnerController;
        private readonly PlayerPawnController _playerController;

        public ControlGamePauseUseCase(JoystickView joystickView, IJoystickService joystickService, EnemySpawnerController spawnerController, PlayerPawnController playerController)
        {
            _joystickView = joystickView;
            _joystickService = joystickService;
            _spawnerController = spawnerController;
            _playerController = playerController;
        }

        public void Execute(bool isActive)
        {
            _joystickView.SetActive(isActive);
            _joystickService.SetEnabled(isActive);

            if (isActive)
            {
                _spawnerController.StartSpawning();
            }
            else
            {
                _spawnerController.StopSpawning();
                _playerController.StopMovement();
            }
        }
    }
}