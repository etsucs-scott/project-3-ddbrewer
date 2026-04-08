using System;
using Minesweeper.Core;

// To test behavior: 
// dotnet build
// dotnet run --project src/Minesweeper.Console

namespace Minesweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            BoardSize size = GetBoardSizeFromUser(); // Collects the user's set up choices before creating the game board
            int seed = GetSeedFromUser();

            MinesweeperGame game = new MinesweeperGame(size, seed);

            Console.WriteLine();
            Console.WriteLine("Game started.");
            Console.WriteLine("Seed used: " + seed);
            Console.WriteLine();

            bool playerQuit = false;

            while (!game.IsGameOver && !playerQuit) // Main gameplay loop, keeps refreshing the board and accepting commands until game is won/lost/quit
            {
                PrintBoard(game);
                Console.WriteLine();
                Console.WriteLine("Enter command: r row col, f row col, or q");
                Console.Write("> ");

                string input = Console.ReadLine();

                playerQuit = ProcessCommand(game, input);

                Console.WriteLine();
            }

            PrintBoard(game);

            if (playerQuit)
            {
                Console.WriteLine("You quit the game.");
            }
            else if (game.IsWin)
            {
                Console.WriteLine("You win!");
            }
            else
            {
                Console.WriteLine("You hit a mine! Game over.");
            }

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        static BoardSize GetBoardSizeFromUser()
        {
            while (true)
            {
                Console.WriteLine("Choose board size: ");
                Console.WriteLine("1 - Small (8x8, 10 mines)");
                Console.WriteLine("2 - Medium (12x12, 25 mines)");
                Console.WriteLine("3 - Large (16x16, 40 mines)");
                Console.WriteLine("Enter your choice: ");

                string boardChoice = Console.ReadLine();

                if (boardChoice == "1")
                {
                    return BoardSize.Small;
                }
                
                if (boardChoice == "2")
                {
                    return BoardSize.Medium;
                }

                if (boardChoice == "3")
                {
                    return BoardSize.Large;
                }

                Console.WriteLine("Invalid choice.");
                Console.WriteLine();
            }
        }

        static int GetSeedFromUser()
        {
            while (true)
            {
                Console.Write("Enter a seed, or press Enter to generate a random one: ");
                string userSeed = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userSeed)) // Blank input generates a seed from the current time
                {
                    int generatedSeed = DateTime.Now.Ticks.GetHashCode();
                    return generatedSeed;
                }

                int seed;
                bool isValid = int.TryParse(userSeed, out seed);

                if (isValid)
                {
                    return seed;
                }

                Console.WriteLine("Invalid seed. Please enter a whole number.");
                Console.WriteLine();
            }
        }

        static bool ProcessCommand(MinesweeperGame game, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid command.");
                return false;
            }

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                if (parts[0].ToLower() == "q")
                {
                    return true;
                }

                Console.WriteLine("Invalid command.");
                return false;
            }

            if (parts.Length != 3)
            {
                Console.WriteLine("Invalid command format.");
                return false;
            }

            string command = parts[0].ToLower();

            int row;
            int column;

            bool rowValid = int.TryParse(parts[1], out row);
            bool columnValid = int.TryParse(parts[2], out column);

            if (!rowValid || !columnValid)
            {
                Console.WriteLine("Row and column must be integers.");
                return false;
            }

            try
            {
                if (command == "r")
                {
                    bool changed = game.RevealTile(row, column);

                    if (!changed) // RevealTile returns false for invalid move input
                    {
                        Console.WriteLine("That tile is flagged and cannot be revealed. Please unflag it before revealing.");
                    }
                }
                else if (command == "f")
                {
                    bool changed = game.ToggleFlag(row, column);

                    if (!changed)
                    {
                        Console.WriteLine("That tile cannot be flagged/unflagged.");
                    }
                }
                else
                {
                    Console.WriteLine("Command unknown.");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Row or column is outside the board.");
            }

            return false;
        }

        static void PrintBoard(MinesweeperGame game)
        {
            Console.Write("   ");

            for (int column = 0; column < game.Columns; column++)
            {
                Console.Write(column + " ");
            }

            Console.WriteLine();

            for (int row = 0; row < game.Rows; row++)
            {
                if (row < 10)
                {
                    Console.Write(row + "  ");
                }
                else
                {
                    Console.Write(row + " ");
                }

                for (int column = 0; column < game.Columns; column++)
                {
                    Tile tile = game.Board[row, column];

                    if (game.IsGameOver && tile.HasMine)
                    {
                        Console.Write("b ");
                    }
                    else if (tile.IsFlagged)
                    {
                        Console.Write("f ");
                    }
                    else if (!tile.IsRevealed)
                    {
                        Console.Write("# ");
                    }
                    else if (tile.AdjacentMineCount == 0)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write(tile.AdjacentMineCount + " ");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}