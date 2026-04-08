using System;

namespace Minesweeper.Core
{
    public class HighScoreEntry // Represents a single high score file record stored in the csv file
    {
        public BoardSize Size { get; set; }
        public int Seconds { get; set; }
        public int Moves { get; set; }
        public int Seed { get; set; }
        public DateTime Timestamp {get; set; }
    }
}