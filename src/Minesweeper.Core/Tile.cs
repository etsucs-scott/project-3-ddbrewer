namespace Minesweeper.Core
{
    public class Tile // Represents a single tile of the minesweeper board, stores all states needed for gameplay / display
    {
        public bool HasMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public int AdjacentMineCount { get; set; }
    }
}