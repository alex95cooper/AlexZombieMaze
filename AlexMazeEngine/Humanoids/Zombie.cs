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
            Speed = 0.5;
            _zombieWalksHorizontally = true;
            LookDirection = LookDirection.Left;
        }

        public bool IsAttack { get; private set; }
        public ZombieState State { get; set; }

        public void SetMove()
        {
            Random random = new();
            MoveDirection horizontalDirection = (MoveDirection)random.Next((int)MoveDirection.Left, (int)MoveDirection.Right + 1);
            MoveDirection verticalDirection = (MoveDirection)random.Next((int)MoveDirection.Up, (int)MoveDirection.Down + 1);
            MoveDirection = (_zombieWalksHorizontally) ? horizontalDirection : verticalDirection;
            _zombieWalksHorizontally = !_zombieWalksHorizontally;
            TryMakeTurn();
        }

        public static void Step(List<Zombie> zombies)
        {
            foreach (Zombie zombie in zombies)
            {
                zombie.SetImage(zombie._imagesWalk[zombie.StepCounter]);
                zombie.StepCounter++;
                zombie.StepCounter = (zombie.StepCounter == StepCount) ? 0 : zombie.StepCounter;
            }

        }

        public void Hunt(MoveDirection direction)
        {
            if (direction != MoveDirection && direction != MoveDirection.None)
            {
                MoveDirection = direction;
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
            if (Speed < MaxSpeed)
            {
                Speed += Speed * Acceleration;
            }
        }

        private void TryMakeTurn()
        {
            if (MoveDirection == (MoveDirection)LookDirection.Left && LookDirection != LookDirection.Left)
            {
                LookDirection = LookDirection.Left;
                Image.FlowDirection = FlowDirection.LeftToRight;
            }
            else if (MoveDirection == (MoveDirection)LookDirection.Right && LookDirection != LookDirection.Right)
            {
                LookDirection = LookDirection.Right;
                Image.FlowDirection = FlowDirection.RightToLeft;
            }
        }

        public override void AvoidTouchingWall()
        {
            SetMove();
        }
    }
}