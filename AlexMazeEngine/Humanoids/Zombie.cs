using AlexMazeEngine.Humanoids;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AlexMazeEngine
{
    public class Zombie : Humanoid
    {
        private const int StepCount = 10;
        private const double MinSpeed = 0.5;
        private const double MaxSpeed = 10;
        private const double ZombieWidth = 20;
        private const double Acceleration = 0.05;
        private const int AttackCount = 7;

        private readonly List<string> _imagesWalk;
        private readonly List<string> _imagesAttack;

        private int _attackCounter;
        private bool _zombieWalksHorizontally;

        public Zombie(List<string> imagesWalk, List<string> imagesAttack)
            : base(imagesWalk[0], ZombieWidth, MinSpeed)
        {
            _imagesWalk = imagesWalk;
            _imagesAttack = imagesAttack;
            _speed = 0.5;
            _zombieWalksHorizontally = true;
            _lookDirection = LookDirection.Left;
        }

        public bool IsAttack { get; private set; }
        public ZombieState State { get; set; }

        public override void SetMove()
        {
            Random random = new();
            MoveDirection horizontalDirection = (MoveDirection)random.Next((int)MoveDirection.Left, (int)MoveDirection.Right + 1);
            MoveDirection verticalDirection = (MoveDirection)random.Next((int)MoveDirection.Up, (int)MoveDirection.Down + 1);
            _moveDirection = (_zombieWalksHorizontally) ? horizontalDirection : verticalDirection;
            _zombieWalksHorizontally = !_zombieWalksHorizontally;
            TryMakeTurn();
        }

        public static void Step(List<Zombie> zombies)
        {
            foreach (Zombie zombie in zombies)
            {
                zombie.SetImage(zombie._imagesWalk[zombie._stepCounter]);
                zombie._stepCounter++;
                zombie._stepCounter = (zombie._stepCounter == StepCount) ? 0 : zombie._stepCounter;
            }

        }

        public void Hunt(MoveDirection direction)
        {
            if (direction != _moveDirection && direction != MoveDirection.None)
            {
                _moveDirection = direction;
                TryMakeTurn();
            }
        }

        public void Attack()
        {
            _attackCounter = (_attackCounter == 7) ? 0 : _attackCounter;
            SetImage(_imagesAttack[_attackCounter]);
            _attackCounter++;
            State = (_attackCounter > AttackCount - 1) ? ZombieState.kill : State;
        }

        public void TryCatchPlayer(Player player)
        {
            Rect zombieCatchBox = new(Canvas.GetLeft(Image), Canvas.GetTop(Image), Width, Height);
            Rect playerCatchBox = new(Canvas.GetLeft(player.Image), Canvas.GetTop(player.Image), player.Width, Height);
            State = zombieCatchBox.IntersectsWith(playerCatchBox) ? ZombieState.Attack : State;
            player.State = zombieCatchBox.IntersectsWith(playerCatchBox) ? PlayerState.Caught : player.State;
        }

        public void Accelerate()
        {
            if (_speed < MaxSpeed)
            {
                _speed += _speed * Acceleration;
            }
        }

        private void TryMakeTurn()
        {
            if (_moveDirection == (MoveDirection)LookDirection.Left && _lookDirection != LookDirection.Left)
            {
                _lookDirection = LookDirection.Left;
                Image.FlowDirection = FlowDirection.LeftToRight;
            }
            else if (_moveDirection == (MoveDirection)LookDirection.Right && _lookDirection != LookDirection.Right)
            {
                _lookDirection = LookDirection.Right;
                Image.FlowDirection = FlowDirection.RightToLeft;
            }
        }
    }
}