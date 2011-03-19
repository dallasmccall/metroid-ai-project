using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.controllers;

namespace MetroidAI.world.space.expanders
{
    class KeySwitch : AExpanderWrapper
    {
        protected static readonly ParametersTable s_defaultParams = new ParametersTable()
        {
            {"Item", "MorphingBall"},
        };

        public KeySwitch(
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

            m_realExpander = new LineCapsOnly(TerminalName, MissionNodeID, Parameters);
            m_realExpander.AddParameter("Length", "1");
            m_realExpander.AddParameter("FractalCreator", "FractalCreatorStraight");
            m_realExpander.AddParameter("ObjectPopulator[0]", "ChozoWithItem");
        }
    }
}
