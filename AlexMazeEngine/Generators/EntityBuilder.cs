using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AlexMazeEngine.Generators
{
    public class EntityBuilder
    {
        private readonly List<Zombie> _zombies = new();
        private readonly List<Coin> _coins = new();
        private readonly Canvas _canvas = new();
        private readonly bool[,] _maze;
        private readonly int _mazeBlockSize;
        private readonly int _coinsQuantity;

        private Player _player;

        public EntityBuilder(bool[,] maze, int mazeBlockSize, int coinsQuantity)
        {
            _mazeBlockSize = mazeBlockSize;
            _coinsQuantity = coinsQuantity;
            _maze = maze;
            CreatePlayer();
            CreateZombie(0);
            CreateCoins();
        }

        public Canvas Canvas => _canvas;
        public Player Player => _player;
        public List<Zombie> Zombies => _zombies;
        public List<Coin> Coins => _coins;

        public void CreateAnotherCoin()
        {
            System.Drawing.Point coinPosition = StartPositionGenerator.GetNewCoinPosition(_maze, _coins);
            _coins.Add(new(GetCoinImages()));
            _coins[^1].Position = coinPosition;
            MapBuilder.AddUiElementToCanvas(_canvas, _coins[^1].Image,
                    (coinPosition.X * _mazeBlockSize) + Coin.DistanceToWall,
                    (coinPosition.Y * _mazeBlockSize) + Coin.DistanceToWall);
        }

        public void CreateZombie(int zombieNumber)
        {
            _zombies.Add(new(GetZombieImages().Item1, GetZombieImages().Item2));
            System.Drawing.Point zombiePosition = (zombieNumber == 0) ?
                StartPositionGenerator.GetFirstZombiePosition(_maze) :
                StartPositionGenerator.GetSecondZombiePosition(_maze);
            MapBuilder.AddUiElementToCanvas(_canvas, _zombies[zombieNumber].Image,
                zombiePosition.X * _mazeBlockSize + Zombie.DistanceToWall,
                zombiePosition.Y * _mazeBlockSize + Zombie.DistanceToWall);
            _zombies[zombieNumber].SetMove();
        }

        private void CreatePlayer()
        {
            _player = new(@"Images\Player.png");
            System.Drawing.Point playerPosition = StartPositionGenerator.GetPlayerPosition(_maze);
            MapBuilder.AddUiElementToCanvas(_canvas, _player.Image,
                playerPosition.X * _mazeBlockSize + Player.DistanceToWall,
                playerPosition.Y * _mazeBlockSize + Player.DistanceToWall);
        }

        private void CreateCoins()
        {
            List<System.Drawing.Point> coinPositions = StartPositionGenerator.GetCoinsPositions(_maze, _coinsQuantity);
            for (int i = 0; i < coinPositions.Count; i++)
            {
                _coins.Add(new(GetCoinImages()));
                _coins[i].Position = coinPositions[i];
                MapBuilder.AddUiElementToCanvas(_canvas, _coins[i].Image,
                    (coinPositions[i].X * _mazeBlockSize) + Coin.DistanceToWall,
                    (coinPositions[i].Y * _mazeBlockSize) + Coin.DistanceToWall);
            }
        }

        private static List<string> GetCoinImages()
        {
            List<string> coinImages = new();
            for (int i = 1; i < 5; i++)
            {
                coinImages.Add($@"Images\Coins\Coin{i}.png");
            }

            return coinImages;
        }

        private static (List<string>, List<string>) GetZombieImages()
        {
            List<string> imagesWalk = new();
            for (int i = 1; i < 11; i++)
            {
                imagesWalk.Add($@"Images\Zombie Walk\go_{i}.png");
            }

            List<string> imagesAttack = new();
            for (int i = 1; i < 8; i++)
            {
                imagesAttack.Add($@"Images\\Zombie Attack\hit_{i}.png");
            }

            return (imagesWalk, imagesAttack);
        }
    }
}
