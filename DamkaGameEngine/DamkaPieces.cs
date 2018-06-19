namespace DamkaGameEngine
{
    public static class DamkaPieces
    {
        public enum Type
        {
            None, BlackSimple, BlackKing, WhiteSimple, WhiteKing
        }

        public static char GetSimpleType(Type t)
        {
            switch (t)
            {
                case Type.BlackSimple:
                    return 'O';
                case Type.BlackKing:
                    return 'Q';
                case Type.WhiteSimple:
                    return 'X';
                case Type.WhiteKing:
                    return 'K';

                case Type.None:
                default:
                    return ' ';
            }
        }

        public static string GetAdvancedType(Type t)
        {
            switch (t)
            {
                case Type.BlackSimple:
                    return "⛂";
                case Type.BlackKing:
                    return "♛";
                case Type.WhiteSimple:
                    return "⛀";
                case Type.WhiteKing:
                    return "♕";

                case Type.None:
                default:
                    return " ";
            }
        }

        public static bool IsSameType(Type t1, Type t2)
        {
            if (t1 == Type.BlackKing || t1 == Type.BlackSimple)
            {
                return t2 == Type.BlackKing || t2 == Type.BlackSimple;
            }
            else if (t1 == Type.WhiteKing || t1 == Type.WhiteSimple)
            {
                return t2 == Type.WhiteKing || t2 == Type.WhiteSimple;
            }
            return false;
        }

        public static bool IsEnemyType(Type t1, Type t2)
        {
            if (t1 == Type.None || t2 == Type.None)
            {
                return false;
            }
            return !IsSameType(t1, t2);
        }

        public static bool isKing(Type type)
        {
            return (type == Type.BlackKing) || (type == Type.WhiteKing);
        }

        public static Type GetKing(Type type)
        {
            if (type == Type.BlackSimple)
            {
                return Type.BlackKing;
            }
            else if (type == Type.WhiteSimple)
            {
                return Type.WhiteKing;
            }
            else
            {
                return type;
            }
        }
    }
}
