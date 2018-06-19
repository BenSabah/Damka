
using DamkaGameEngine;
using System;
using System.Collections.Generic;

namespace Damka
{
    class AsciiGame
    {
        public static void Main()
        {
            // get game parameters from user.
            string userName1 = getValidUserName();
            string userName2 = "PC";
            int boardSize = getValidBoardSize();
            int numberOfPlayers = getValidNumberOfPlayers();
            if (numberOfPlayers == 2)
            {
                userName2 = getValidUserName();
            }

            // create a new damka board engine.
            GameEngine GE = new GameEngine(boardSize, userName1, userName2);

            // loop the game.
            while (GE.IsGameOver)
            {
                Player lastPlayer = GE.CurrentPlayer;
                Console.WriteLine(GE);
                Console.WriteLine("{0}'s {1}",
                    lastPlayer.Name,
                    lastPlayer.LastMove);
                string userInput = Console.ReadLine();


                if (userInput.Equals("Q"))
                {
                    GE.IsGameOver = true;
                    //GE.setWinner(player);
                    break;
                }

                // NEEDS TO CHECK INPUT VALIDITY HERE!!!
                //GE.move(Player, move);
                //Ex02.ConsoleUtils.Screen.Clear();

            }

            // return the last game 
            Console.WriteLine(GE);
            Console.WriteLine("game over, {0} won!", arg0: GE.Winner);

            // pause before exit.
            Console.ReadLine();
        }

        private static string getValidUserName()
        {
            bool IsValidInput = false;
            String userInput = null;

            // get board size from the user.
            while (!IsValidInput)
            {
                Console.Write("please enter your name (up to 20 charecters): ");
                userInput = Console.ReadLine();
                IsValidInput = userInput.Length < 20;
                //Ex02.ConsoleUtils.Screen.Clear();
            }

            return userInput;
        }

        private static int getValidBoardSize()
        {
            bool IsValidInput = false;
            int parsedInput = 0;
            String userInput;

            // get board size from the user.
            while (!IsValidInput)
            {
                Console.Write("please select a valid board size (6,8 or 10): ");
                userInput = Console.ReadLine();
                IsValidInput = int.TryParse(userInput, out parsedInput);
                IsValidInput &= (parsedInput == 6 || parsedInput == 8 || parsedInput == 10);
                //Ex02.ConsoleUtils.Screen.Clear();
            }

            return parsedInput;
        }

        private static int getValidNumberOfPlayers()
        {
            bool IsValidInput = false;
            String userInput = null;
            int parsedInput = 0;

            // get board size from the user.
            while (!IsValidInput)
            {
                Console.Write("please enter how many players (1 or 2): ");
                userInput = Console.ReadLine();
                IsValidInput = int.TryParse(userInput, out parsedInput);
                IsValidInput |= (parsedInput == 1 || parsedInput == 2);
                //Ex02.ConsoleUtils.Screen.Clear();
            }

            return parsedInput;
        }


    }
}
