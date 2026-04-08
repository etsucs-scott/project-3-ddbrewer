using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Minesweeper.Core
{
    public class HighScoreService // Handles all high score actions: loading from, saving to, sorting and trimming the CSV
    {
        private const string Header = "size,seconds,moves,seed,timestamp";

        public List<HighScoreEntry> LoadHighScores(string filePath)
        {
            try
            {
                EnsureFileExists(filePath);

                List<HighScoreEntry> scores = new List<HighScoreEntry>();
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length == 0)
                {
                    File.WriteAllText(filePath, Header + Environment.NewLine);
                    return scores;
                }

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    if (i == 0 && line.Equals(Header, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    HighScoreEntry? entry = TryParseLine(line);

                    if (entry != null)
                    {
                        scores.Add(entry);
                    }
                }

                return scores;
            }
            
            catch (Exception ex)
            {
                throw new Exception("Could not load high scores file. " + ex.Message);
            }
        }

        public void SaveHighScores(string filePath, List<HighScoreEntry> scores)
        {
            try
            {
                EnsureDirectoryExists(filePath);

                List<string> lines = new List<string>();
                lines.Add(Header);

                for (int i = 0; i < scores.Count; i++)
                {
                    HighScoreEntry score = scores[i];

                    string line =
                        score.Size + "," +
                        score.Seconds + "," +
                        score.Moves + "," +
                        score.Seed + "," +
                        score.Timestamp.ToString("o");

                    lines.Add(line);
                }

                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not save high scores file." + ex.Message);
            }
        }

        public void AddHighScore(string filePath, HighScoreEntry newScore)
        {
            List<HighScoreEntry> scores = LoadHighScores(filePath);

            scores.Add(newScore);
            scores = SortAndTrimScores(scores);

            SaveHighScores(filePath, scores);
        }

        public List<HighScoreEntry> GetHighScoresForSize(string filePath, BoardSize size)
        {
            List<HighScoreEntry> allScores = LoadHighScores(filePath);
            List<HighScoreEntry> filteredScores = new List<HighScoreEntry>();

            for (int i = 0; i < allScores.Count; i++)
            {
                if (allScores[i].Size == size)
                {
                    filteredScores.Add(allScores[i]);
                }
            }

            filteredScores = SortScores(filteredScores);

            if (filteredScores.Count > 5)
            {
                filteredScores = filteredScores.GetRange(0, 5);
            }

            return filteredScores;
        }

        private List<HighScoreEntry> SortAndTrimScores(List<HighScoreEntry> scores)
        {
            List<HighScoreEntry> smallScores = new List<HighScoreEntry>();
            List<HighScoreEntry> mediumScores = new List<HighScoreEntry>();
            List<HighScoreEntry> largeScores = new List<HighScoreEntry>();

            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i].Size == BoardSize.Small)
                {
                    smallScores.Add(scores[i]);
                }
                else if (scores[i].Size == BoardSize.Medium)
                {
                    mediumScores.Add(scores[i]);
                }
                else if (scores[i].Size == BoardSize.Large)
                {
                    largeScores.Add(scores[i]);
                }
            }

            smallScores = SortScores(smallScores);
            mediumScores = SortScores(mediumScores);
            largeScores = SortScores(largeScores);

            if (smallScores.Count > 5)
            {
                smallScores = smallScores.GetRange(0, 5);
            }

            if (mediumScores.Count > 5)
            {
                mediumScores = mediumScores.GetRange(0, 5);
            }

            if (largeScores.Count > 5)
            {
                largeScores = largeScores.GetRange(0, 5);
            }

            List<HighScoreEntry> result = new List<HighScoreEntry>();

            result.AddRange(smallScores);
            result.AddRange(mediumScores);
            result.AddRange(largeScores);

            return result;
        }

        private List<HighScoreEntry> SortScores(List<HighScoreEntry> scores)
        {
            scores.Sort(CompareScores);
            return scores;
        }

        private int CompareScores(HighScoreEntry first, HighScoreEntry second)
        {
            if (first.Seconds < second.Seconds)
            {
                return -1;
            }

            if (first.Seconds > second.Seconds)
            {
                return 1;
            }

            if (first.Moves < second.Moves)
            {
                return -1;
            }

            if (first.Moves > second.Moves)
            {
                return 1;
            }

            return 0;
        }

        private HighScoreEntry? TryParseLine(string line)
        {
            string[] parts = line.Split(",");

            if (parts.Length != 5)
            {
                return null;
            }

            BoardSize size;
            int seconds;
            int moves;
            int seed;
            DateTime timestamp;

            bool sizeValid = Enum.TryParse(parts[0], out size);
            bool secondsValid = int.TryParse(parts[1], out seconds);
            bool movesValid = int.TryParse(parts[2], out moves);
            bool seedValid = int.TryParse(parts[3], out seed);
            bool timestampValid = DateTime.TryParse(parts[4], null, DateTimeStyles.RoundtripKind, out timestamp);

            if (!sizeValid || !secondsValid || !movesValid || !seedValid || !timestampValid)
            {
                return null;
            }

            if (seconds < 0 || moves < 0)
            {
                return null;
            }

            HighScoreEntry entry = new HighScoreEntry();
            entry.Size = size;
            entry.Seconds = seconds;
            entry.Moves = moves;
            entry.Seed = seed;
            entry.Timestamp = timestamp;

            return entry;
        }

        private void EnsureFileExists(string filePath)
        {
            EnsureDirectoryExists(filePath);

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, Header + Environment.NewLine);
            }
        }

        private void EnsureDirectoryExists(string filePath)
        {
            string? directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}