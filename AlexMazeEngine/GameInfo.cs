using System;
using System.Xml.Linq;

namespace AlexMazeEngine
{
    public class GameInfo
    {
        public GameInfo()
        {
            PlayerName = "Player1";
            TimeInMaze = new TimeSpan(0, 0, 0);
            GameDate = DateTime.Now;
            Score = 0;
            CauseOfFinish = string.Empty;
        }

        public string PlayerName { set; get; }
        public TimeSpan TimeInMaze { set; get; }
        public DateTime GameDate { set; get; }
        public string CauseOfFinish { set; get; }
        public int Score { set; get; }


        public void SetCauseOfFinish(bool playerIsDead)
        {
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
