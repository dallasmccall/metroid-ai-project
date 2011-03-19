using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.world.space.expanders
{
    class LineCapsOnly : AMissionTerminalExpander
    {
        protected int m_length;
        protected List<Direction> m_directions;
        protected Direction m_bestEvaledDirection;

        protected List<IScreenPostProcessor> m_postProcessors;
        protected List<IObjectPopulator> m_objectPopulators;

        protected static readonly ParametersTable s_defaultParams = new ParametersTable()
        {
            {"Length", "2"},
            {"Direction", "Any"},
        };

        public LineCapsOnly(
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
            string directionStr = Parameters["Direction"];
            switch (directionStr)
            {
                case "Any":
                    m_directions = new List<Direction>(Direction.All);
                    break;
                case "Horizontal":
                    m_directions = new List<Direction>() { Direction.Left, Direction.Right };
                    break;
                case "Vertical":
                    m_directions = new List<Direction>() { Direction.Up, Direction.Down };
                    break;
                default:
                    m_directions = new List<Direction>();
                    m_directions.Add(Parameters.ParseDirection("Direction"));
                    break;
            }

            m_objectPopulators = Parameters.ParseList<IObjectPopulator>(
                "ObjectPopulator",
                Parameters.ParseObjectPopulator);
            m_postProcessors = Parameters.ParseList <IScreenPostProcessor>(
                "PostProcessor",
                Parameters.ParseScreenPostProcessor);
        }

        #region IMissionTerminalExpander Members

        public override float Evaluate(ISpace space, Point markedPoint, Point unmarkedPoint)
        {
            float bestCost = float.PositiveInfinity; // worst possible, best possible is 0

            foreach (Direction dir in m_directions)
            {
                // walk m_length tiles and verify they are all free
                Point cur = unmarkedPoint;
                int counter = 0;
                int blockedCounter = 0;
                for (int i = 0; i < m_length; ++i)
                {
                    if (!space.IsAreaFree(cur))
                    {
                        blockedCounter = Int32.MaxValue - 5;
                        break;
                    }

                    counter += 2;
                    if (!space.IsAreaFree(dir.RotationCW.Move(cur))) blockedCounter++;
                    if (!space.IsAreaFree(dir.RotationCCW.Move(cur))) blockedCounter++;

                    cur = dir.Move(cur);
                }

                counter++;
                if (!space.IsAreaFree(cur)) blockedCounter++; // one past the end

                float score = (float)blockedCounter / counter;

                if (score < bestCost)
                {
                    bestCost = score;
                    m_bestEvaledDirection = dir;
                }
            }

            return bestCost;
        }

        public override void Expand(ISpace space, Point markedPoint, Point unmarkedPoint)
        {
            Point prev = markedPoint;
            Point cur = unmarkedPoint;

            ScreenConstructionParameters scp = new ScreenConstructionParameters(cur, TerminalName, m_fractalCreator);

            space.AddScreen(scp, this.MissionNodeID);
            space.ConnectTwoWay(prev, cur, (Connection)space.GetConnection(prev, cur));

            if (m_objectPopulators.Count >= 1) scp.ObjectPopulator = m_objectPopulators[0];
            if (m_postProcessors.Count >= 1) scp.PostProcessor = m_postProcessors[0];

            for (int i = 1; i < m_length; ++i)
            {
                prev = cur;
                cur = DirectionUtils.Move(cur, m_bestEvaledDirection);

                scp = new ScreenConstructionParameters(cur, TerminalName, m_fractalCreator);

                space.AddScreen(scp, this.MissionNodeID);
                space.ConnectTwoWay(prev, cur, Connection.Open);

                if (m_objectPopulators.Count > i) scp.ObjectPopulator = m_objectPopulators[i];
                if (m_postProcessors.Count > i) scp.PostProcessor = m_postProcessors[i];
            }

            prev = cur;
            
            space.ConnectOneWay(prev, m_bestEvaledDirection.Move(prev), Connection.Door);
            space.ConnectOneWay(prev, m_bestEvaledDirection.RotationCW.Move(prev), Connection.Door);
            space.ConnectOneWay(prev, m_bestEvaledDirection.RotationCCW.Move(prev), Connection.Door);
            
        }

        #endregion
    }
}
