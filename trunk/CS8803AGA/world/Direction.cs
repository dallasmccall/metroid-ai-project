using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world
{
    struct Direction
    {
        #region Constants

        internal static readonly Direction Left = new Direction(DirectionEnum.Left);
        internal static readonly Direction Right = new Direction(DirectionEnum.Right);
        internal static readonly Direction Up = new Direction(DirectionEnum.Up);
        internal static readonly Direction Down = new Direction(DirectionEnum.Down);

        internal static readonly Direction[] All = new Direction[] {
            Up, Right, Left, Down
        };

        #endregion

        #region Properties and Fields

        private DirectionEnum m_direction;

        internal DirectionEnum EnumValue { get { return m_direction; } }

        #endregion

        #region Construction

        internal Direction(DirectionEnum direction)
        {
            m_direction = direction;
        }

        internal static Direction ParseString(string directionString)
        {
            return new Direction((DirectionEnum)Enum.Parse(typeof(DirectionEnum), directionString));
        }

        #endregion

        #region Utility Methods

        internal Point Move(Point point)
        {
            return Move(point, 1);
        }

        internal Point Move(Point point, int distance)
        {
            switch (m_direction)
            {
                case DirectionEnum.Up:
                    return new Point(point.X, point.Y - distance);
                case DirectionEnum.Down:
                    return new Point(point.X, point.Y + distance);
                case DirectionEnum.Left:
                    return new Point(point.X - distance, point.Y);
                case DirectionEnum.Right:
                    return new Point(point.X + distance, point.Y);
            }
            throw new Exception("DirectionUtils:Move: Unknown direction for move request");
        }

        internal Vector2 Move(Vector2 vector)
        {
            return Move(vector, 1.0f);
        }

        internal Vector2 Move(Vector2 vector, float distance)
        {
            switch (m_direction)
            {
                case DirectionEnum.Up:
                    return new Vector2(vector.X, vector.Y - distance);
                case DirectionEnum.Down:
                    return new Vector2(vector.X, vector.Y + distance);
                case DirectionEnum.Left:
                    return new Vector2(vector.X - distance, vector.Y);
                case DirectionEnum.Right:
                    return new Vector2(vector.X + distance, vector.Y);
            }
            throw new Exception("DirectionUtils:Move: Unknown direction for move request");
        }

        internal bool IsCWOf(Direction d)
        {
            return d.RotationCW.EnumValue == this.EnumValue;
        }

        internal bool IsCCWOf(Direction d)
        {
            return d.RotationCCW.EnumValue == this.EnumValue;
        }

        internal bool IsOppositeOf(Direction d)
        {
            return d.Opposite.EnumValue == this.EnumValue;
        }

        #endregion

        #region Utility Properties

        internal Direction Opposite
        {
            get
            {
                switch (m_direction)
                {
                    case DirectionEnum.Up:
                        return Direction.Down;
                    case DirectionEnum.Down:
                        return Direction.Up;
                    case DirectionEnum.Left:
                        return Direction.Right;
                    case DirectionEnum.Right:
                        return Direction.Left;
                }
                throw new Exception("Direction:Opposite: Unknown direction for opposite request");
            }
        }

        internal Direction RotationCW
        {
            get
            {
                switch (m_direction)
                {
                    case DirectionEnum.Up:
                        return Direction.Right;
                    case DirectionEnum.Down:
                        return Direction.Left;
                    case DirectionEnum.Left:
                        return Direction.Up;
                    case DirectionEnum.Right:
                        return Direction.Down;
                }
                throw new Exception("DirectionUtils:RotateClockwise: Unknown direction for rotate request");
            }
        }

        internal Direction RotationCCW
        {
            get
            {
                return RotationCW.Opposite;
            }
        }

        internal Direction[] Sides
        {
            get
            {
                return new Direction[2]{ RotationCW, RotationCCW };
            }
        }

        internal bool IsHorizontal
        {
            get { return m_direction == DirectionEnum.Left || m_direction == DirectionEnum.Right; }
        }

        internal bool IsVertical
        {
            get { return !IsHorizontal; }
        }

        #endregion

        #region Operators and Required Functionality

        public override int GetHashCode()
        {
 	         return m_direction.GetHashCode();
        }

        public static bool operator ==(Direction d1, Direction d2)
        {
            return d1.EnumValue == d2.EnumValue;
        }

        public static bool operator !=(Direction d1, Direction d2)
        {
            return !(d1 == d2);
        }

        public override string ToString()
        {
            return EnumValue.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Direction)
            {
                return EnumValue == ((Direction)obj).EnumValue;
            }
            return false;
        }

        #endregion

    }

    enum DirectionEnum
    {
        Left, Right, Up, Down
    }

    static class DirectionUtils
    {
        internal static Point Move(Point p, Direction d)
        {
            return Move(p, d, 1);
        }

        internal static Point Move(Point p, Direction d, int distance)
        {
            switch (d.EnumValue)
            {
                case DirectionEnum.Up:
                    return new Point(p.X, p.Y - distance);
                case DirectionEnum.Down:
                    return new Point(p.X, p.Y + distance);
                case DirectionEnum.Left:
                    return new Point(p.X - distance, p.Y);
                case DirectionEnum.Right:
                    return new Point(p.X + distance, p.Y);
            }
            throw new Exception("DirectionUtils:Move: Unknown direction for move request");
        }

        internal static Direction GetDirection(Point source, Point destination)
        {
            if (source == destination)
            {
                throw new Exception("GetDirection: source cannot equal destination");
            }
            if (source.X == destination.X)
            {
                if (source.Y + 1 == destination.Y)
                {
                    return Direction.Down;
                }
                else if (source.Y - 1 == destination.Y)
                {
                    return Direction.Up;
                }
            }
            else if (source.Y == destination.Y)
            {
                if (source.X + 1 == destination.X)
                {
                    return Direction.Right;
                }
                else if (source.X - 1 == destination.X)
                {
                    return Direction.Left;
                }
            }
            throw new Exception(String.Format("DirectionUtils:GetDirection: Unknown direction {0} to {1}", source, destination));
        }
    }
}
