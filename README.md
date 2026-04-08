# CSCI 1260 — Project #3

---

# Minesweeper Console Application (C#)

## Project Overview:
This project is a C#-based implementation of the popular game Minesweeper built using object-oriented programming design.
The program supports seed-based board generation, differing levels of difficulty, and saving and loading of high scores.

## How to Build and Run (Command Line):
1. Open the terminal
2. Build the project: dotnet build
3. Run the program: dotnet run --project src/Minesweeper.Console

---

## How the Minesweeper Game Works:

- The player is prompted to choose a board size from Small (8x8, 10 mines), Medium (12x12, 25 mines), and Large (16x16, 40 mines).
- The player is then prompted to enter a seed (if no seed is selected the game generates a seed based off of the current system time).
- The board is then generated and the player enters commands in the format of: r row column (to reveal a space), f row column (to flag a space), or q (to quit the game prematurely).

## Win / Loss Conditions:

- The game is considered won when the player successfully reveals all hidden spaces that do not have a mine placed on them.
- The game is considered lost when the player reveals a hidden space with a mine on it.
- The game is also considered lost when the player issues the "q" command to quit the game.

---

## Known Limitations:

- The classes for the program are unfortunately not split down into neat SOLID principle design. (The game kind of uses a few "monster classes".) Still working on this and going to try and make it a major focus of the final project.

---

## Submission Note:

This project was completed as part of the CSCI 1260 course and is included in the classroom GitHub repo as required.