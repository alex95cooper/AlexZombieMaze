using System;
using System.Xml.Linq;

namespace AlexMazeEngine
{
    public class GameInfo
    {
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

        public void SetLastInfo(int score, string timeInMaze, bool playerIsDead)
        {
            Score = score;
            TimeInMaze = timeInMaze;
            CauseOfFinish = (playerIsDead) ? "Died heroically!" : "Deserted from the battlefield";
        }

        public void Serialize(string filename)
        {
            XDocument doc = XDocument.Load(filename);
            doc.Element("Results").AddFirst(
                new XElement("GameInfo",
                new XElement("PlayerName", PlayerName),
                new XElement("Score", Score),
                new XElement("TimeInMaze", TimeInMaze.ToString()),
                new XElement("GameDate", GameDate.ToString()),
                new XElement("CauseOfFinish", CauseOfFinish)));
            doc.Save(filename);
        }
    }
}
