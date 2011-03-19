using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS8803AGA.engine
{
    class EngineStateLoading : AEngineState
    {
        private bool m_hasUpdated = false;

        public EngineStateLoading(Engine engine) : base(engine)
        {
            // nch
        }

        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (m_hasUpdated)
            {
                EngineManager.replaceCurrentState(new EngineStateGameplay(m_engine));
            }

            m_hasUpdated = true;
        }

        public override void draw()
        {
            // nch
        }
    }
}
