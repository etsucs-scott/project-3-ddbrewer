using Minesweeper.Core;
using Xunit;

namespace Minesweeper.Tests;

public class MinesweeperGameTests
{
    [Fact]
    public void SmallBoardDimensionsTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Small, 123);

        Assert.Equal(8, game.Rows);
        Assert.Equal(8, game.Columns);
    }

    [Fact]
    public void MediumBoardDimensionsTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Medium, 123);

        Assert.Equal(12, game.Rows);
        Assert.Equal(12, game.Columns);
    }

    [Fact]
    public void LargeBoardDimensionsTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Large, 123);

        Assert.Equal(16, game.Rows);
        Assert.Equal(16, game.Columns);
    }

    [Fact]
    public void SeedCreatesSameLayoutTest()
    {
        MinesweeperGame game1 = new MinesweeperGame(BoardSize.Small, 123);
        MinesweeperGame game2 = new MinesweeperGame(BoardSize.Small, 123);

        for (int row = 0; row < game1.Rows; row++)
        {
            for (int column = 0; column < game1.Columns; column++)
            {
                Assert.Equal(
                    game1.Board[row, column].HasMine,
                    game2.Board[row, column].HasMine);
            }
        }
    }

    [Fact]
    public void DifferentSeedsCreateDifferentLayoutsTest()
    {
        MinesweeperGame game1 = new MinesweeperGame(BoardSize.Small, 123);
        MinesweeperGame game2 = new MinesweeperGame(BoardSize.Small, 456);

        bool difference = false;

        for (int row = 0; row < game1.Rows; row++)
        {
            for (int column = 0; column < game1.Columns; column++)
            {
                if (game1.Board[row, column].HasMine != game2.Board[row, column].HasMine)
                {
                    difference = true;
                }
            }
        }

        Assert.True(difference);
    }

    [Fact]
    public void AdjacentMineCountTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Small, 123);

        for (int row = 0; row < game.Rows; row++)
        {
            for (int column = 0; column < game.Columns; column++)
            {
                if (!game.Board[row, column].HasMine)
                {
                    int expectedCount = CountAdjacentMinesManually(game, row, column);
                    int actualCount = game.Board[row, column].AdjacentMineCount;

                    Assert.Equal(expectedCount, actualCount);
                }
            }
        }
    }

    [Fact]
    public void CascadeRevealTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Small, 123);

        bool foundZeroTile = false;

        for (int row = 0; row < game.Rows; row++)
        {
            for (int column = 0; column < game.Columns; column++)
            {
                if (!game.Board[row, column].HasMine && game.Board[row, column].AdjacentMineCount == 0)
                {
                    game.RevealTile(row, column);
                    foundZeroTile = true;

                    Assert.True(game.Board[row, column].IsRevealed);

                    bool revealedMoreThanOneTile = CountRevealedTiles(game) > 1;
                    Assert.True(revealedMoreThanOneTile);

                    return;
                }
            }
        }

        Assert.True(foundZeroTile);
    }

    [Fact]
    public void RevealingMineCausesLossTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Small, 123);

        for (int row = 0; row < game.Rows; row++)
        {
            for (int column = 0; column < game.Columns; column++)
            {
                if (game.Board[row, column].HasMine)
                {
                    game.RevealTile(row, column);

                    Assert.True(game.IsGameOver);
                    Assert.False(game.IsWin);
                    
                    return;
                }
            }
        }
    }

    [Fact]
    public void RevealingAllNonMineTilesIsWinTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Small, 123);

        for (int row = 0; row < game.Rows; row++)
        {
            for (int column = 0; column < game.Columns; column++)
            {
                if (!game.Board[row, column].HasMine)
                {
                    game.RevealTile(row, column);
                }
            }
        }

        Assert.True(game.IsGameOver);
        Assert.True(game.IsWin);
    }

    [Fact]
    public void FlaggedTileCannotBeRevealedTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Small, 123);

        game.ToggleFlag(0, 0);
        bool result = game.RevealTile(0, 0);

        Assert.False(result);
        Assert.False(game.Board[0, 0].IsRevealed);
    }

    [Fact]
    public void RevealedTileCannotBeFlaggedTest()
    {
        MinesweeperGame game = new MinesweeperGame(BoardSize.Small, 123);

        for (int row = 0; row < game.Rows; row++)
        {
            for (int column = 0; column < game.Columns; column++)
            {
                if (!game.Board[row, column].HasMine)
                {
                    game.RevealTile(row, column);

                    bool result = game.ToggleFlag(row, column);

                    Assert.False(result);
                    Assert.False(game.Board[row, column].IsFlagged);

                    return;
                }
            }
        }
    }

    // Helper methods down here for 2 of my tests:

    private int CountAdjacentMinesManually(MinesweeperGame game, int row, int column)
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

                if (newRow >= 0 && newRow < game.Rows && newColumn >= 0 && newColumn < game.Columns)
                {
                    if (game.Board[newRow, newColumn].HasMine)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private int CountRevealedTiles(MinesweeperGame game)
    {
        int count = 0;

        for (int row = 0; row < game.Rows; row++)
        {
            for (int column = 0; column < game.Columns; column++)
            {
                if (game.Board[row, column].IsRevealed)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
