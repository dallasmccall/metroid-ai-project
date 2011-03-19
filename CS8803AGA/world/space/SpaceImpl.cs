using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world.space
{
    class SpaceImpl : ISpace
    {
        int m_hashCode = 0;

        Dictionary<Point, ScreenConstructionParameters> m_screenParameters = new Dictionary<Point, ScreenConstructionParameters>();
        Dictionary<Point, HashSet<String>> m_qualifiers = new Dictionary<Point, HashSet<String>>();
        Dictionary<CoordPair, Connection> m_connections = new Dictionary<CoordPair, Connection>();
        Dictionary<CoordPair, Connection> m_openConnections = new Dictionary<CoordPair, Connection>();

        protected SpaceImpl()
        {
            // nch, only used for copy
        }

        public SpaceImpl(Point startPoint)
        {
            Point origin = startPoint;
            this.AddScreen(new ScreenConstructionParameters(startPoint, "Start", new FractalCreatorJagged()), "-1");
            this.ConnectOneWay(origin, DirectionUtils.Move(origin, Direction.Left), Connection.Door);
            this.ConnectOneWay(origin, DirectionUtils.Move(origin, Direction.Right), Connection.Door);
            this.ConnectOneWay(origin, DirectionUtils.Move(origin, Direction.Up), Connection.Door);
            this.ConnectOneWay(origin, DirectionUtils.Move(origin, Direction.Down), Connection.Door);
        }

        #region ISpace Members

        public bool IsAreaFree(Point coord)
        {
            return !m_qualifiers.ContainsKey(coord);
        }

        public ScreenConstructionParameters GetParameters(Point coord)
        {
            ScreenConstructionParameters sp;
            if (m_screenParameters.TryGetValue(coord, out sp))
            {
                return sp;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<String> GetQualifiers(Point coord)
        {
            HashSet<String> qualifiers;
            if (m_qualifiers.TryGetValue(coord, out qualifiers))
            {
                return qualifiers;
            }
            else
            {
                return null;
            }
        }

        public Connection? GetConnection(Point sourceCoord, Point destCoord)
        {
            CoordPair cp = new CoordPair(sourceCoord, destCoord);
            Connection conn;
            if (m_connections.TryGetValue(cp, out conn))
            {
                return conn;
            }
            return null;
        }

        public Dictionary<CoordPair, Connection> OpenConnections
        {
            get { return m_openConnections; }
        }

        public IOrderedEnumerable<KeyValuePair<CoordPair, Connection>> OpenConnectionsRandom
        {
            get
            {
                Random r = RandomManager.get();
                return m_openConnections.OrderBy(i => r.Next());
            }
        }

        public void AddScreen(ScreenConstructionParameters scp, string missionNodeID)
        {
            Point location = scp.Location;
            if (m_screenParameters.ContainsKey(location))
            {
                throw new InvalidOperationException("SpaceImpl already has that screen");
            }
            m_screenParameters.Add(location, scp);
            m_qualifiers.Add(location, new HashSet<String>());
            m_qualifiers[location].Add(missionNodeID);

            m_hashCode ^= getAreaHashCode(location);

            Point north = DirectionUtils.Move(location, Direction.Up);
            Point south = DirectionUtils.Move(location, Direction.Down);
            Point west = DirectionUtils.Move(location, Direction.Left);
            Point east = DirectionUtils.Move(location, Direction.Right);

            CoordPair fromNorth = new CoordPair(north, location);
            CoordPair fromSouth = new CoordPair(south, location);
            CoordPair fromWest = new CoordPair(west, location);
            CoordPair fromEast = new CoordPair(east, location);

            // checks for non-wall connections into this area.  those need to
            // be dealt with, preferably by: seeing if the old connection would cause a short
            // circuit, if so delete it, otherwise add it to this one.
            CleanupExistingConnection(fromNorth);
            CleanupExistingConnection(fromSouth);
            CleanupExistingConnection(fromWest);
            CleanupExistingConnection(fromEast);

            // TODO - figure out whether to add Wall connections adjacent to screen or not
            //throw new NotImplementedException();
        }

        public void AddQualifier(Point coord, string qualifier)
        {
            HashSet<String> qualifiers;
            if (m_qualifiers.TryGetValue(coord, out qualifiers))
            {
                qualifiers.Add(qualifier);
                //m_hashCode ^= getQualifierHashCode(coord, qualifier);
            }
            else
            {
                throw new InvalidOperationException("Cannot add qualifiers to an unknown screen");
            }
        }

        public void ConnectOneWay(Point sourceCoord, Point destCoord, Connection connection)
        {
            if (!m_qualifiers.ContainsKey(sourceCoord))
            {
                throw new InvalidOperationException("Cannot connect an unknown screen to something else");
            }

            CoordPair cp = new CoordPair(sourceCoord, destCoord);
            CoordPair reverse = cp.Reverse;

            // undo hash from connection being overwritten
            Connection oldValue;
            if (m_connections.TryGetValue(cp, out oldValue))
            {
                m_hashCode ^= getConnectionHashCode(cp.SourceCoord, cp.DestCoord, oldValue);
            }

            m_connections[cp] = connection;
            m_hashCode ^= getConnectionHashCode(cp.SourceCoord, cp.DestCoord, oldValue);

            if (m_openConnections.TryGetValue(reverse, out oldValue))
            {
                //if (oldValue == connection)
                //{
                    // this could be circumventing the ordering.
                    // to be safe, remove both directions of the connection.
                    m_openConnections.Remove(reverse);
                    m_connections.Remove(reverse);
                    m_hashCode ^= getConnectionHashCode(reverse.SourceCoord, reverse.DestCoord, oldValue);

                    m_connections.Remove(cp);
                    m_hashCode ^= getConnectionHashCode(cp.SourceCoord, cp.DestCoord, oldValue);
                //}
                //else
                //{
                //    throw new InvalidOperationException("Not currently supported to have parallel connections of different types");
                //}
            }
            else if (connection != Connection.None && !m_connections.ContainsKey(reverse))
            {
                m_openConnections.Add(cp, connection);
            }
        }

        public void ConnectTwoWay(Point sourceCoord, Point destCoord, Connection connection)
        {
            if (!m_qualifiers.ContainsKey(sourceCoord) ||
                !m_qualifiers.ContainsKey(destCoord))
            {
                throw new InvalidOperationException("Cannot connect an unknown screen to something else");
            }

            if (connection != Connection.None)
            {
                m_qualifiers[destCoord].UnionWith(m_qualifiers[sourceCoord]);
            }

            CoordPair forward = new CoordPair(sourceCoord, destCoord);
            CoordPair reverse = forward.Reverse;

            // undo hash from connections being overwritten
            Connection oldValue;
            if (m_connections.TryGetValue(forward, out oldValue))
            {
                m_hashCode ^= getConnectionHashCode(forward.SourceCoord, forward.DestCoord, oldValue);
            }
            if (m_connections.TryGetValue(reverse, out oldValue))
            {
                m_hashCode ^= getConnectionHashCode(reverse.SourceCoord, reverse.DestCoord, oldValue);
            }

            m_connections[forward] = connection;
            m_connections[reverse] = connection;
            m_hashCode ^= getConnectionHashCode(forward.SourceCoord, forward.DestCoord, connection);
            m_hashCode ^= getConnectionHashCode(reverse.SourceCoord, reverse.DestCoord, connection);

            m_openConnections.Remove(forward);
            m_openConnections.Remove(reverse);
        }

        public ISpace DeepCopy()
        {
            SpaceImpl copy = new SpaceImpl();

            foreach (KeyValuePair<Point, ScreenConstructionParameters> kvp in m_screenParameters)
            {
                copy.m_screenParameters.Add(kvp.Key, kvp.Value.DeepCopy());
            }

            foreach (KeyValuePair<Point, HashSet<String>> kvp in m_qualifiers)
            {
                HashSet<String> qualifiers = new HashSet<String>(kvp.Value);
                copy.m_qualifiers.Add(kvp.Key, qualifiers);
            }

            copy.m_connections = new Dictionary<CoordPair, Connection>(this.m_connections);
            copy.m_openConnections = new Dictionary<CoordPair, Connection>(this.m_openConnections);

            copy.m_hashCode = this.m_hashCode;

            return copy;
        }

        #endregion

        private void CleanupExistingConnection(CoordPair cp)
        {
            Connection forwardConn;
            Connection reverseConn;
            if (m_connections.TryGetValue(cp, out forwardConn))
            {
                if (forwardConn != Connection.None &&
                    m_connections.TryGetValue(cp.Reverse, out reverseConn))
                {
                    if (forwardConn != reverseConn)
                    {
                        HashSet<String> srcQualifiers = m_qualifiers[cp.SourceCoord];
                        HashSet<String> dstQualifiers = m_qualifiers[cp.DestCoord];

                        // testing to see if both sets of qualifiers are the same
                        int targetSize = Math.Max(srcQualifiers.Count, dstQualifiers.Count);
                        int count = 0;
                        IEnumerable<String> intersect = srcQualifiers.Intersect(dstQualifiers);
                        foreach (String s in intersect)
                        {
                            ++count;
                        }

                        if (targetSize == count)
                        {
                            // qualifiers are the same, so no short circuit - connect them
                            this.ConnectOneWay(cp.DestCoord, cp.SourceCoord, forwardConn);
                        }
                        else
                        {
                            // would cause a short circuit, delete the connection
                            this.ConnectOneWay(cp.SourceCoord, cp.DestCoord, Connection.None);
                        }
                    }
                }
            }
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }

        private int getAreaHashCode(Point area)
        {
            return area.X.GetHashCode() ^ area.Y.GetHashCode();
        }

        private int getConnectionHashCode(Point source, Point dest, Connection connection)
        {
            int srcHash = getAreaHashCode(source);
            int dstHash = getAreaHashCode(dest);
            return
                wraparoundShift(srcHash, (dstHash & 0x0000FF00) >> 8) ^
                wraparoundShift(dstHash, (srcHash & 0x00FF0000) >> 16) ^
                wraparoundShift(connection.GetHashCode(), srcHash & 0x000000FF);
        }

        private int getQualifierHashCode(Point source, string qualifier)
        {
            return wraparoundShift(qualifier.GetHashCode(), (getAreaHashCode(source) & 0x000000FF) >> 24);
        }

        private int wraparoundShift(int code, int shiftAmt)
        {
            int wrappedBitsMask = (1 << shiftAmt) - 1; // get a mask for bits about to be lost
            int lostBits = wrappedBitsMask & code; // grab the bits about to be lost
            code >>= shiftAmt; // perform the shift, losing bits
            int lostBitsMoved = (lostBits << (32 - shiftAmt)); // move the bits from right to left
            code ^= lostBitsMoved; // and tack them back on (could probably use OR, but XOR safer because not sure how >> handles negative)
            return code;
        }

        public Dictionary<Point, ScreenConstructionParameters> makeWorld()
        {
            Dictionary<Point, ScreenConstructionParameters> screens =
                new Dictionary<Point, ScreenConstructionParameters>();

            foreach(KeyValuePair<Point,ScreenConstructionParameters> kvp in m_screenParameters)
            {
                screens.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<CoordPair, Connection> kvp in m_openConnections)
            {
                m_connections.Remove(kvp.Key);
            }

            foreach (KeyValuePair<CoordPair,Connection> kvp in m_connections)
            {
                Direction d = DirectionUtils.GetDirection(kvp.Key.SourceCoord, kvp.Key.DestCoord);
                screens[kvp.Key.SourceCoord].Connections[d] = kvp.Value;
            }

            return screens;
        }

        public static bool TestAreEqual(SpaceImpl a, SpaceImpl b)
        {
            foreach (KeyValuePair<CoordPair, Connection> kvp in a.m_connections)
            {
                if (b.m_connections[kvp.Key] != kvp.Value)
                {
                    return false;
                }
            }

            foreach (KeyValuePair<CoordPair, Connection> kvp in b.m_connections)
            {
                if (a.m_connections[kvp.Key] != kvp.Value)
                {
                    return false;
                }
            }

            foreach (KeyValuePair<Point, HashSet<String>> kvp in a.m_qualifiers)
            {
                foreach (String s in kvp.Value)
                {
                    if (!b.m_qualifiers[kvp.Key].Contains(s))
                    {
                        return false;
                    }
                }
            }

            foreach (KeyValuePair<Point, HashSet<String>> kvp in b.m_qualifiers)
            {
                foreach (String s in kvp.Value)
                {
                    if (!a.m_qualifiers[kvp.Key].Contains(s))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
