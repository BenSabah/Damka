namespace DamkaGameEngine
{
    using System.Collections.Generic;
    using System.Drawing;

    public class Player
    {
        private readonly LinkedList<PlayerMove> m_movesHistory;

        // getters
        public string Name { get; }

        public PlayerMove LastMove => this.m_movesHistory.Last.Value;

        public DamkaPieces.Type PlayerType { get; }

        public Player(string i_playerName, DamkaPieces.Type i_playerType)
        {
            this.Name = i_playerName;
            this.PlayerType = i_playerType;
            this.m_movesHistory = new LinkedList<PlayerMove>();
        }

        public object AddPlayerMove(PlayerMove move)
        {
            return this.m_movesHistory.AddLast(move);
        }

        public class PlayerMove
        {
            public DamkaPieces.Type Type { get; set; }

            public Point From { get; set; }

            public Point To { get; set; }

            public PlayerMove(Point p0, Point p1, DamkaPieces.Type type)
            {
                this.From = p0;
                this.To = p1;
                this.Type = type;
            }
        }
    }
}
