using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.controllers;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world.space.expanders
{
    class LockSwitch : AExpanderWrapper
    {
        protected static readonly ParametersTable s_defaultParams = new ParametersTable()
        {
            {"Item", "MorphingBall"},
        };

        public LockSwitch(
            String terminalName,
            String missionNodeID,
            ParametersTable parameters)
            : base(terminalName, missionNodeID, s_defaultParams, parameters)
        {
            // nch
        }

        protected override void InitializeAndCreateRealExpander()
        {
            Item item = Parameters.ParseEnum<Item>("Item");
            switch (item)
            {
                case Item.MorphingBall:
                    m_realExpander =
                        new LineCapsOnly(TerminalName, MissionNodeID, Parameters);
                    m_realExpander.AddParameter("Length", "2");
                    m_realExpander.AddParameter("Direction", "HRandom");
                    m_realExpander.AddParameter("PostProcessor[0]", "BallLock");
                    m_realExpander.AddParameter("PostProcessor[1]", "BallLock");
                    break;
                case Item.IceBeam:
                    const int iceBeamTrialLength = 3;
                    m_realExpander =
                        new LineCapsOnly(TerminalName, MissionNodeID, Parameters);
                    m_realExpander.AddParameter("FractalCreator", "FractalCreatorStraight");
                    m_realExpander.AddParameter("Length", iceBeamTrialLength.ToString());
                    m_realExpander.AddParameter("Direction", "Up");
                    for (int i = 0; i < iceBeamTrialLength; ++i)
                    {
                        m_realExpander.AddParameter(
                            String.Format("{0}[{1}]", "ObjectPopulator", i),
                            "SingleRipper");
                    }
                    break;
            }
        }
    }
}
