using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world.space.expanders
{
    abstract class AExpanderWrapper : AMissionTerminalExpander
    {
        protected IMissionTerminalExpander m_realExpander;

        public AExpanderWrapper(
            String terminalName,
            String missionNodeID,
            ParametersTable subclassParameters,
            ParametersTable parameters)
            : base(terminalName, missionNodeID, subclassParameters, parameters)
        {
            // nch
        }

        protected abstract void InitializeAndCreateRealExpander();

        protected override void InitializeMyParams()
        {
            InitializeAndCreateRealExpander();
            m_realExpander.InitializeParams();
        }

        public override float Evaluate(ISpace space, Point markedPoint, Point unmarkedPoint)
        {
            return m_realExpander.Evaluate(space, markedPoint, unmarkedPoint);
        }

        public override void Expand(ISpace space, Point markedPoint, Point unmarkedPoint)
        {
            m_realExpander.Expand(space, markedPoint, unmarkedPoint);
        }
    }
}
