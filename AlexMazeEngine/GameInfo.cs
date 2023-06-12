using AlexMazeEngine.Humanoids;
using System;
using System.Collections.Generic;
using System.IO;
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
            PlayerName = (name == string.Empty) ? "Player" : name;
            GameDate = DateTime.Now;
            Score = 0;
            CauseOfFinish = string.Empty;
        }

        public string PlayerName { set; get; }
        public string TimeInMaze { set; get; }
        public DateTime GameDate { set; get; }
        public string CauseOfFinish { set; get; }
        public int Score { set; get; }

        public static void CreateResultsIfNeed()
        {
            if (!File.Exists(ResultFile))
            {
                XDocument doc = new();
                XElement results = new("results");
                doc.Add(results);
                doc.Save(ResultFile);
            }
        }

        public void SetLastInfo(int score, string timeInMaze, PlayerState state)
        {
            Score = score;
            TimeInMaze = timeInMaze;
            CauseOfFinish = (state == PlayerState.Dead) ? "Died heroically!" : "Deserted from the battlefield";
        }

        public void Serialize()
        {
            XDocument doc = XDocument.Load(ResultFile);
            doc.Element("results").AddFirst(
                new XElement("GameInfo",
                new XElement("PlayerName", PlayerName),
                new XElement("Score", Score),
                new XElement("TimeInMaze", TimeInMaze.ToString()),
                new XElement("GameDate", GameDate.ToString()),
                new XElement("CauseOfFinish", CauseOfFinish)));
            doc.Save(ResultFile);
        }

        public static DataGrid GetStatistic(DataGrid dataGridResult)
        {
            XDocument doc = XDocument.Load(ResultFile);
            List<XElement> list = doc.Root.Elements().ToList();
            if (list.Count == 0)
            {
                return dataGridResult;
            } 

            dataGridResult.ItemsSource = list;
            dataGridResult.CanUserAddRows = false;
            dataGridResult.CanUserDeleteRows = false;
            dataGridResult.DataContext = doc;            
            return dataGridResult;
        }

        public static string GetLastPlayerName()
        {
            XDocument doc = XDocument.Load(ResultFile);
            List<XElement> list = doc.Root.Elements().ToList();
            return (list.Count == 0)? "Player" : list[0].Element("PlayerName").Value;
        }
    }
}
