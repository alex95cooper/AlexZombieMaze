using AlexMazeEngine.Humanoids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace AlexMazeEngine
{
    public class GameInfo
    {
        private const string ResultFile = "Result.xml";

        public GameInfo(string name)
        {
            PlayerName = (name == string.Empty) ? "Player1" : name;
            GameDate = DateTime.Now;
            Score = 0;
            CauseOfFinish = string.Empty;
        }

        public string PlayerName { set; get; }
        public string TimeInMaze { set; get; }
        public DateTime GameDate { set; get; }
        public string CauseOfFinish { set; get; }
        public int Score { set; get; }

        public void SetLastInfo(int score, string timeInMaze, PlayerState state)
        {
            Score = score;
            TimeInMaze = timeInMaze;
            CauseOfFinish = (state == PlayerState.Dead) ? "Died heroically!" : "Deserted from the battlefield";
        }

        public void Serialize()
        {
            XDocument doc = XDocument.Load(ResultFile);
            doc.Element("Results").AddFirst(
                new XElement("GameInfo",
                new XElement("PlayerName", PlayerName),
                new XElement("Score", Score),
                new XElement("TimeInMaze", TimeInMaze.ToString()),
                new XElement("GameDate", GameDate.ToString()),
                new XElement("CauseOfFinish", CauseOfFinish)));
            doc.Save(ResultFile);
        }

        public static DataGrid GetStatistic()
        {
            DataGrid dataGridResult = new();
            XDocument doc = XDocument.Load(ResultFile);
            dataGridResult.ItemsSource = null;
            List<XElement> list = doc.Root.Elements().ToList();
            dataGridResult.ItemsSource = list;
            dataGridResult.CanUserAddRows = false;
            dataGridResult.CanUserDeleteRows = false;
            dataGridResult.DataContext = doc;
            dataGridResult.Visibility = Visibility.Visible;
            return dataGridResult;
        }

        public static string GetLastPlayerName()
        {
            XDocument doc = XDocument.Load(ResultFile);
            List<XElement> list = doc.Root.Elements().ToList();
            return list[0].Element("PlayerName").Value;
        }
    }
}
