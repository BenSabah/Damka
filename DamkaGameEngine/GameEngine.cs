namespace DamkaGameEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1308:VariableNamesMustNotBePrefixed", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214:StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed. Suppression is OK here.")]

    public class GameEngine
    {

        // game variables
        private readonly int r_boardSize;

        private readonly int r_rowsOfGame;
        private readonly Player r_player1;
        private readonly Player r_player2;

        // getters & setters.
        public DamkaPieces.Type[,] Board { get; }

        public int BoardSize => this.r_boardSize;

        public Player CurrentPlayer { get; private set; }

        public Player Winner { get; set; }

        public bool IsGameOver { get; set; }

        public GameEngine(int i_Size, string i_Player1Name, string i_Player2Name)
        {
            this.r_boardSize = i_Size;
            this.Board = new DamkaPieces.Type[i_Size, i_Size];
            this.r_rowsOfGame = (i_Size - 2) / 2;
            this.r_player1 = new Player(i_Player1Name, DamkaPieces.Type.BlackSimple);
            this.r_player2 = new Player(i_Player2Name, DamkaPieces.Type.WhiteSimple);
            this.CurrentPlayer = this.r_player1;
            this.ResetBoard();
        }

        public void ResetBoard()
        {
            // fill with blanks.
            for (int y = 0; y < this.BoardSize; y++)
            {
                for (int x = 0; x < this.BoardSize; x++)
                {
                    this.Board[x, y] = DamkaPieces.Type.None;
                }
            }

            // place blacks.
            for (int y = 0; y < this.r_rowsOfGame; y++)
            {
                for (int x = 0; x < this.BoardSize; x++)
                {
                    if ((y % 2 == 0 && x % 2 == 1) || (y % 2 == 1 && x % 2 == 0))
                    {
                        this.Board[x, y] = DamkaPieces.Type.BlackSimple;
                    }
                }
            }

            // place whites.
            for (int y = this.BoardSize - this.r_rowsOfGame; y < this.BoardSize; y++)
            {
                for (int x = 0; x < this.BoardSize; x++)
                {
                    if ((y % 2 == 0 && x % 2 == 1) || (y % 2 == 1 && x % 2 == 0))
                    {
                        this.Board[x, y] = DamkaPieces.Type.WhiteSimple;
                    }
                }
            }
        }

        public void MovePiece(Point i_from, Point i_to)
        {
            if (this.GetPieceByIndex(i_from) != DamkaPieces.Type.None && this.GetPieceByIndex(i_from) != DamkaPieces.Type.None)
            {
                this.switchPieces(i_from, i_to);
                this.switchPlayers();
                this.checkIfEaten(i_from, i_to);
                this.checkIfKingged(i_to);
                this.checkIfGameOver();
                this.recordTheMove(i_from, i_to);
            }
        }

        private void switchPieces(Point i_index1, Point i_index2)
        {
            DamkaPieces.Type atIndex1 = this.GetPieceByIndex(i_index1);
            DamkaPieces.Type atIndex2 = this.GetPieceByIndex(i_index2);

            this.placePieceAt(i_index1, atIndex2);
            this.placePieceAt(i_index2, atIndex1);
        }

        private void switchPlayers()
        {
            this.CurrentPlayer = (this.CurrentPlayer == this.r_player1) ? this.r_player2 : this.r_player1;
        }

        private void checkIfEaten(Point i_index0, Point i_index1)
        {
            int howManySkipped = 0;

            int adder = (i_index1.X - i_index0.X > 0) ? -1 : 1;
            int deltaY = i_index0.Y - i_index1.Y;
            int deltaX = i_index0.X - i_index1.X;
            int m = deltaY / deltaX;

            int xLow = (i_index0.X < i_index1.X) ? i_index0.X : i_index1.X;
            int xHigh = (xLow == i_index1.X) ? i_index0.X : i_index1.X;
            int yOfLow = (xLow == i_index0.X) ? i_index0.Y : i_index1.Y;

            for (int x = i_index1.X + adder; x >= xLow && x <= xHigh; x = x + adder)
            {
                int y = m * (x - xLow) + yOfLow;
                DamkaPieces.Type tmp = this.GetPieceByIndex(new Point(x, y));
                if (DamkaPieces.IsEnemyType(tmp, this.GetPieceByIndex(i_index1)))
                {
                    if (this.isSkippableIndex(i_index0, i_index1))
                    {
                        howManySkipped++;
                    }
                }
            }

            if (howManySkipped == 1)
            {
                for (int x = i_index1.X + adder; x >= xLow && x <= xHigh; x = x + adder)
                {
                    int y = m * (x - xLow) + yOfLow;
                    Point p = new Point(x, y);
                    DamkaPieces.Type tmp = this.GetPieceByIndex(p);
                    if (DamkaPieces.IsEnemyType(tmp, this.GetPieceByIndex(i_index1)))
                    {
                        if (this.isSkippableIndex(i_index0, i_index1))
                        {
                            this.placePieceAt(p, DamkaPieces.Type.None);
                        }
                    }
                }
            }
        }

        private void checkIfKingged(Point i_index)
        {
            DamkaPieces.Type type = this.GetPieceByIndex(i_index);
            bool hasReachedOtherSide = this.isSimplePieceOnOtherSide(i_index, type);
            if (hasReachedOtherSide)
            {
                this.placePieceAt(i_index, DamkaPieces.GetKing(type));
            }
        }

        private void checkIfGameOver()
        {
            int sizeOfBlackPossibleMoves = 0;
            int sizeOfWhitePossibleMoves = 0;

            for (int y = 0; y < this.r_boardSize; y++)
            {
                for (int x = 0; x < this.r_boardSize; x++)
                {
                    Point p = new Point(x, y);
                    DamkaPieces.Type t = this.GetPieceByIndex(p);
                    int count = this.GetLandingOptions(p).Count;

                    if (DamkaPieces.GetKing(t) != DamkaPieces.Type.BlackKing)
                    {
                        sizeOfBlackPossibleMoves += count;
                    }
                    else if (DamkaPieces.GetKing(t) != DamkaPieces.Type.WhiteKing)
                    {
                        sizeOfWhitePossibleMoves += count;
                    }
                }
            }

            if (sizeOfBlackPossibleMoves == 0)
            {
                this.IsGameOver = true;
                this.Winner = this.r_player2;
            }
            else if (sizeOfWhitePossibleMoves == 0)
            {
                this.IsGameOver = true;
                this.Winner = this.r_player1;
            }
        }

        private void recordTheMove(Point i_from, Point i_to)
        {
            Player.PlayerMove m = new Player.PlayerMove(i_from, i_to, this.GetPieceByIndex(i_to));
            this.CurrentPlayer.AddPlayerMove(m);
        }

        private bool isSimplePieceOnOtherSide(Point i_index, DamkaPieces.Type i_type)
        {
            bool result = false;
            if (i_type == DamkaPieces.Type.BlackSimple && i_index.Y == this.r_boardSize - 1)
            {
                result = true;
            }
            else if (i_type == DamkaPieces.Type.WhiteSimple && i_index.Y == 0)
            {
                result = true;
            }

            return result;
        }



        public DamkaPieces.Type GetPieceByIndex(Point i_index)
        {
            try
            {
                return this.Board[i_index.X, i_index.Y];
            }
            catch (Exception)
            {
                MessageBox.Show("failed to place piece at:" + i_index.ToString(), " !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return DamkaPieces.Type.BlackKing;
        }

        private void placePieceAt(Point i_index, DamkaPieces.Type i_type)
        {
            this.Board[i_index.X, i_index.Y] = i_type;
        }


        public List<Point> GetLandingOptions(Point i_origin)
        {
            List<Point> landingOptions = this.createAllIndexOptions(i_origin);
            landingOptions = this.filterOptionsByBoardSize(landingOptions);
            landingOptions = this.filterOptionsByPieceType(landingOptions, i_origin);
            landingOptions = this.filterOptionsBySelf(landingOptions, i_origin);
            landingOptions = this.filterOptionsByEnemies(landingOptions, i_origin);
            return landingOptions;
        }

        private List<Point> createAllIndexOptions(Point i_origin)
        {
            List<Point> landingOptions = new List<Point>();
            for (int i = 1; i < this.r_boardSize; i++)
            {
                landingOptions.Add(new Point(i_origin.X + i, i_origin.Y + i));    // handling buttom-right corner.
                landingOptions.Add(new Point(i_origin.X + i, i_origin.Y - i));    // handling top-right corner.
                landingOptions.Add(new Point(i_origin.X - i, i_origin.Y + i));    // handling buttom-right corner.
                landingOptions.Add(new Point(i_origin.X - i, i_origin.Y - i));    // handling top-left corner.
            }
            return landingOptions;
        }

        private List<Point> filterOptionsByBoardSize(List<Point> i_landingOptions)
        {
            List<Point> filteredLandingOptions = new List<Point>();
            foreach (Point option in i_landingOptions)
            {
                if (this.isIndexOnBoard(option))
                {
                    filteredLandingOptions.Add(option);
                }
            }
            return filteredLandingOptions;
        }

        private List<Point> filterOptionsByPieceType(List<Point> i_landingOptions, Point i_origin)
        {
            List<Point> filteredLandingOptions = new List<Point>();
            foreach (Point option in i_landingOptions)
            {
                if (this.isDistanceFitType(option, i_origin))
                {
                    filteredLandingOptions.Add(option);
                }
            }
            return filteredLandingOptions;

        }

        private bool isIndexOnBoard(Point i_index)
        {
            return i_index.X >= 0 && i_index.Y >= 0 && i_index.X < this.r_boardSize && i_index.Y < this.r_boardSize;
        }

        private bool isDistanceFitType(Point i_destination, Point i_origin)
        {   
            bool isSkippable = (Math.Abs(i_destination.X - i_origin.X) == 1) && (Math.Abs(i_destination.Y - i_origin.Y) == 1);

            bool isKing = DamkaPieces.isKing(this.GetPieceByIndex(i_origin));

            return isKing || isSkippable;
        }

        private bool isSkippableIndex(Point i_destination, Point i_origin)
        {
            bool isDistTwo = (Math.Abs(i_destination.X - i_origin.X) == 2) && (Math.Abs(i_destination.Y - i_origin.Y) == 2);
            bool isOpen = this.GetPieceByIndex(i_destination) == DamkaPieces.Type.None;
            Point p = new Point((i_destination.X + i_origin.X) / 2, (i_destination.Y + i_origin.Y) / 2);
            bool isSkippable = DamkaPieces.IsEnemyType(this.GetPieceByIndex(p), this.GetPieceByIndex(i_origin));

            return isDistTwo && isOpen && isSkippable;
        }

        private List<Point> filterOptionsBySelf(List<Point> i_landingOptions, Point i_origin)
        {
            List<Point> filteredLandingOptions = new List<Point>();
            foreach (Point option in i_landingOptions)
            {
                if (!this.isIndexAbstractedBySelf(option, i_origin))
                {
                    filteredLandingOptions.Add(option);
                }
            }
            return filteredLandingOptions;
        }

        private bool isIndexAbstractedBySelf(Point i_destination, Point i_origin)
        {
            int adder = (i_origin.X - i_destination.X > 0) ? -1 : 1;
            int deltaY = i_destination.Y - i_origin.Y;
            int deltaX = i_destination.X - i_origin.X;
            int m = deltaY / deltaX;

            int xLow = (i_destination.X < i_origin.X) ? i_destination.X : i_origin.X;
            int xHigh = (xLow == i_origin.X) ? i_destination.X : i_origin.X;
            int yOfLow = (xLow == i_destination.X) ? i_destination.Y : i_origin.Y;

            for (int x = i_origin.X + adder; x >= xLow && x <= xHigh; x = x + adder)
            {
                int y = m * (x - xLow) + yOfLow;
                DamkaPieces.Type tmp = this.GetPieceByIndex(new Point(x, y));
                if (DamkaPieces.IsSameType(tmp, this.GetPieceByIndex(i_origin)))
                {
                    return true;
                }
            }
            return false;
        }

        private List<Point> filterOptionsByEnemies(List<Point> i_landingOptions, Point i_origin)
        {
            List<Point> filteredLandingOptions = new List<Point>();
            foreach (Point option in i_landingOptions)
            {
                if (!this.isIndexAbstractedByEnemy(option, i_origin))
                {
                    filteredLandingOptions.Add(option);
                }
            }
            return filteredLandingOptions;
        }

        private bool isIndexAbstractedByEnemy(Point i_destination, Point i_origin)
        {
            int adder = (i_origin.X - i_destination.X > 0) ? -1 : 1;
            int deltaY = i_destination.Y - i_origin.Y;
            int deltaX = i_destination.X - i_origin.X;
            int m = deltaY / deltaX;

            int xLow = (i_destination.X < i_origin.X) ? i_destination.X : i_origin.X;
            int xHigh = (xLow == i_origin.X) ? i_destination.X : i_origin.X;
            int yOfLow = (xLow == i_destination.X) ? i_destination.Y : i_origin.Y;

            int distance = 0;

            // count how many enemies between the points.
            int numberOfEnemiesInRange = 0;

            for (int x = i_origin.X + adder; x >= xLow && x <= xHigh; x = x + adder)
            {
                distance++;
                int y = m * (x - xLow) + yOfLow;

                DamkaPieces.Type tmp = this.GetPieceByIndex(new Point(x, y));
                if (DamkaPieces.IsEnemyType(tmp, this.GetPieceByIndex(i_origin)))
                {
                    numberOfEnemiesInRange++;
                }
            }

            bool isKing = DamkaPieces.isKing(this.GetPieceByIndex(i_origin));
            bool result = false;

            if (isKing && numberOfEnemiesInRange == 1)
            {
                result = false;
            }
            else if (distance == 1 && numberOfEnemiesInRange == 1)
            {
                result = true;
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // add top row of board.
            sb.Append("  ");
            for (int x = 0; x < this.BoardSize; x++)
            {
                sb.Append(" " + (char)(x + 'A') + "  ");
            }
            // add line of '='s,
            sb.Append("\n ");
            sb.Append('=', 4 * this.BoardSize + 1);
            sb.Append("\n");

            // add rest of board
            for (int y = 0; y < this.BoardSize; y++)
            {
                sb.Append((char)(y + 'a') + "|");

                for (int x = 0; x < this.BoardSize; x++)
                {
                    sb.Append(" ");
                    sb.Append(DamkaPieces.GetSimpleType(this.Board[x, y]));
                    sb.Append(" |");
                }

                // add line of '='s,
                sb.Append("\n ");
                sb.Append('=', 4 * this.BoardSize + 1);
                sb.Append("\n");
            }

            return sb.ToString();
        }

        private static string pointsListToString(List<Point> l)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in l)
            {
                sb.Append(item.ToString() + "\n");
            }
            return sb.ToString();
        }
    }
}
