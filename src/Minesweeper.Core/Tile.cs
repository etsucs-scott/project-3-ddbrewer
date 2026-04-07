namespace Minesweeper.Core
{
    public class Tile
    {
        public bool HasMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public int AdjacentMineCount { get; set; }
    }
}