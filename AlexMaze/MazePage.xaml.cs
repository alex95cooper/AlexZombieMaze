using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using AlexMazeEngine;
using System.Threading.Tasks;
using AlexMazeEngine.Generators;
using System.ComponentModel;

namespace AlexMaze
{
    public partial class MazePage : Page
    {
        public MazePage(string playerName)
        {
            InitializeComponent();
        }
    }
}
