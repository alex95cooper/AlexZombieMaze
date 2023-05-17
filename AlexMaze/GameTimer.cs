using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using AlexMaze;

namespace AlexMazeEngine
{
    internal class GameTimer
    {
        private readonly DispatcherTimer _gameTimer = new();
        private readonly DispatcherTimer _animationTimer = new();
        private readonly DispatcherTimer _motionTimer = new();
        private readonly DispatcherTimer _coinAddTimer = new();
        private readonly Stopwatch _stopwatch = new();
        private readonly MainWindow _mainWindow;

        public GameTimer(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Start()
        {
            InitialStopWatch();
            InitializeMobility();
            InitialAnimation();
            InitialCoinAdd();
        }

        public void Stop()
        {
            _gameTimer.Stop();
            _animationTimer.Stop();
            _motionTimer.Stop();
            _coinAddTimer.Stop();
        }

        private async void InitialStopWatch()
        {
            _gameTimer.Interval = new(0, 0, 1);
            _gameTimer.IsEnabled = true;
            await Task.Delay(10);
            _gameTimer.Tick += (obj, e) => { _mainWindow.TimeBlock.Text = "    " + _stopwatch.Elapsed.ToString(@"hh\:mm\:ss"); };
            _stopwatch.Start();
        }

        private void InitializeMobility()
        {
            _motionTimer.Interval = TimeSpan.FromMilliseconds(17);
            _motionTimer.IsEnabled = true;
            _motionTimer.Tick += _mainWindow.MotionLoop;
            _motionTimer.Start();
        }

        private void InitialAnimation()
        {
            _animationTimer.Interval = TimeSpan.FromMilliseconds(80);
            _animationTimer.IsEnabled = true;
            _animationTimer.Tick += _mainWindow.AnimationLoop;
            _animationTimer.Start();
        }

        private void InitialCoinAdd()
        {
            _coinAddTimer.Interval = new(0, 0, 5);
            _coinAddTimer.IsEnabled = true;
            _coinAddTimer.Tick += _mainWindow.CoinAddLoop;
        }
    }
}
