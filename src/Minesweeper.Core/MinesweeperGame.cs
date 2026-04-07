using System;
using System.Collections.Generic;

namespace Minesweeper.Core
{
    public class MinesweeperGame
    {
        public Tile[,] Board { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public int MineCount { get; private set; }
        public int Seed { get; private set; }

        public bool IsGameOver { get; private set; }
        public bool IsWin { get; private set; }

        public MinesweeperGame(BoardSize size, int seed)
        {
            Seed = seed;
            SetBoardSettings(size);
            CreateEmptyBoard();
            PlaceMines();
            CalculateAdjacentMineCounts();
        }

        private void SetBoardSettings(BoardSize size)
        {
            if (size == BoardSize.Small)
            {
                Rows = 8;
                Columns = 8;
                MineCount = 10;
            }
            else if (size == BoardSize.Medium)
            {
                Rows = 12;
                Columns = 12;
                MineCount = 25;
            }
            else
            {
                Rows = 16;
                Columns = 16;
                MineCount = 40;
            }
        }

        private void CreateEmptyBoard()
        {
            Board = new Tile[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Board[row, column] = new Tile();
                }
            }
        }

        private void PlaceMines()
        {
            Random random = new Random(Seed);
            int minesPlaced = 0;

            while (minesPlaced < MineCount)
            {
                int row = random.Next(0, Rows);
                int column = random.Next(0, Columns);

                if (!Board[row, column].HasMine)
                {
                    Board[row, column].HasMine = true;
                    minesPlaced++;
                }
            }
        }

        private void CalculateAdjacentMineCounts()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (Board[row, column].HasMine)
                    {
                        Board[row, column].AdjacentMineCount = -1;
                    }
                    else
                    {
                        Board[row, column].AdjacentMineCount = CountAdjacentMines(row, column);
                    }
                }
            }
        }

        private int CountAdjacentMines(int row, int column)
        {
            int count = 0;

            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
                {
                    if (rowOffset == 0 && columnOffset == 0)
                    {
                        continue;
                    }

                    int newRow = row + rowOffset;
                    int newColumn = column + columnOffset;

                    if (IsInsideBoard(newRow, newColumn))
                    {
                        if (Board[newRow, newColumn].HasMine)
                        {
                            count++;
                        }
                    }
                }
            }
            
            return count;
        }

        public bool RevealTile(int row, int column)
        {
            ValidateCoordinates(row, column);

            if (IsGameOver)
            {
                return false;
            }

            Tile tile = Board[row, column];

            if (tile.IsRevealed)
            {
                return false;
            }

            if (tile.IsFlagged)
            {
                return false;
            }

            tile.IsRevealed = true;

            if (tile.HasMine)
            {
                IsGameOver = true;
                IsWin = false;
                return true;
            }

            if (tile.AdjacentMineCount == 0)
            {
                RevealEmptyArea(row, column);
            }

            CheckWinCondition();
            return true;
        }

        private void RevealEmptyArea(int startRow, int startColumn)
        {
            Queue<(int row, int column)> queue = new Queue<(int row, int column)>();
            queue.Enqueue((startRow, startColumn));

            while (queue.Count > 0)
            {
                (int currentRow, int currentColumn) = queue.Dequeue();

                for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
                {
                    for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
                    {
                        int newRow = currentRow + rowOffset;
                        int newColumn = currentColumn + columnOffset;

                        if (!IsInsideBoard(newRow, newColumn))
                        {
                            continue;
                        }

                        Tile neighbor = Board[newRow, newColumn];

                        if (neighbor.IsRevealed)
                        {
                            continue;
                        }

                        if (neighbor.IsFlagged)
                        {
                            continue;
                        }

                        if (neighbor.HasMine)
                        {
                            continue;
                        }

                        neighbor.IsRevealed = true;

                        if (neighbor.AdjacentMineCount == 0)
                        {
                            queue.Enqueue((newRow, newColumn));
                        }
                    }
                }
            }
        }

        public bool ToggleFlag(int row, int column)
        {
            ValidateCoordinates(row, column);

            if (IsGameOver)
            {
                return false;
            }

            Tile tile = Board[row, column];

            if (tile.IsRevealed)
            {
                return false;
            }

            tile.IsFlagged = !tile.IsFlagged;
            return true;
        }

        public void CheckWinCondition()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Tile tile = Board[row, column];

                    if (!tile.HasMine && !tile.IsRevealed)
                    {
                        return;
                    }
                }
            }

            IsGameOver = true;
            IsWin = true;
        }

        private bool IsInsideBoard(int row, int column)
        {
            if (row < 0 || row >= Rows)
            {
                return false;
            }

            if (column < 0 || column >= Columns)
            {
                return false;
            }

            return true;
        }

        private void ValidateCoordinates(int row, int column)
        {
            if (!IsInsideBoard(row, column))
            {
                throw new ArgumentOutOfRangeException("Row or column is outside the board.");
            }
        }
    }
}