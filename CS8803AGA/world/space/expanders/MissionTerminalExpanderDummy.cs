using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world.space.expanders
{
    class MissionTerminalExpanderDummy : AMissionTerminalExpander
    {
        protected int m_length;
        protected Direction m_direction;
        protected Direction m_exitDir;

        protected static readonly ParametersTable s_defaultParams = new ParametersTable()
        {
            {"Length", "3-5"},
            {"Direction", "HRandom"},
            {"ExitDirection", "Random"},
        };

        public MissionTerminalExpanderDummy(
            String terminalName,
            String missionNodeID,
            ParametersTable parameters)
            : base(terminalName, missionNodeID, s_defaultParams, parameters)
        {
            // nch
        }

        protected override void InitializeMyParams()
        {
            m_length = Parameters.ParseInt("Length");
            m_direction = Parameters.ParseDirection("Direction");
            m_exitDir = Parameters.ParseDirection("ExitDirection");

            if (m_direction == m_exitDir.Opposite)
            {
                m_exitDir = m_exitDir.RotationCW; ;
            }
        }

        #region IMissionTerminalExpander Members

        public override float Evaluate(ISpace space, Point markedPoint, Point unmarkedPoint)
        {
            // walk m_length tiles and verify they are all free
            Point cur = unmarkedPoint;
            for (int i = 0; i < m_length; ++i)
            {
                if (!space.IsAreaFree(cur)) return float.PositiveInfinity;
                cur = DirectionUtils.Move(cur, m_direction);
            }

            // went one too far, so back up to last tile in the dealio
            cur = DirectionUtils.Move(cur, m_direction.Opposite);

            int blockedCount = 0;
            if (!space.IsAreaFree(cur)) blockedCount++;
            if (!space.IsAreaFree(DirectionUtils.Move(cur, m_exitDir.RotationCW))) blockedCount++;
            if (!space.IsAreaFree(DirectionUtils.Move(cur, m_exitDir.RotationCCW))) blockedCount++;
            if (!space.IsAreaFree(DirectionUtils.Move(cur, m_exitDir))) blockedCount++;

            return blockedCount / 4.0f;
        }

        public override void Expand(ISpace space, Point markedPoint, Point unmarkedPoint)
        {
            Point prev = markedPoint;
            Point cur = unmarkedPoint;

            ScreenConstructionParameters scp = new ScreenConstructionParameters(cur, TerminalName, m_fractalCreator);

            space.AddScreen(scp, this.MissionNodeID);
            space.ConnectTwoWay(prev, cur, Connection.Door);

            for (int i = 1; i < m_length; ++i)
            {
                prev = cur;
                cur = DirectionUtils.Move(cur, m_direction);

                scp = new ScreenConstructionParameters(cur, TerminalName, m_fractalCreator);

                space.AddScreen(scp, this.MissionNodeID);
                space.ConnectTwoWay(prev, cur, Connection.Open);

                if (i != m_length - 1)
                {
                    Point side = DirectionUtils.Move(cur, m_direction.RotationCW);
                    space.ConnectOneWay(cur, side, Connection.Door);
                    side = DirectionUtils.Move(cur, m_direction.RotationCCW);
                    space.ConnectOneWay(cur, side, Connection.Door);
                }
            }

            prev = cur;
            //cur = m_direction.Move(cur, m_exitDir);
            space.ConnectOneWay(prev, m_direction.Move(prev), Connection.Door);
            space.ConnectOneWay(prev, m_direction.RotationCW.Move(prev), Connection.Door);
            space.ConnectOneWay(prev, m_direction.RotationCCW.Move(prev), Connection.Door);
            //space.ConnectOneWay(prev, cur, Connection.Door);
        }

        #endregion
    }
}
