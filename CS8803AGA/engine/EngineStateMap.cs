using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidAI.devices;

namespace MetroidAI.engine
{
    public class EngineStateMap : AEngineState
    {
        private Vector2 m_displayOffset;

        public EngineStateMap()
            : base(EngineManager.Engine)
        {
            m_displayOffset = new Vector2(m_engine.GraphicsDevice.Viewport.Width / 2, m_engine.GraphicsDevice.Viewport.Height / 2);
        }

        public override void update(GameTime gameTime)
        {
            m_displayOffset.X += InputSet.getInstance().getLeftDirectionalX() * -30;
            m_displayOffset.Y += InputSet.getInstance().getLeftDirectionalY() * 30;

            if (InputSet.getInstance().getButton(InputsEnum.BUTTON_1) ||
                InputSet.getInstance().getButton(InputsEnum.BUTTON_2) ||
                InputSet.getInstance().getButton(InputsEnum.CONFIRM_BUTTON) ||
                InputSet.getInstance().getButton(InputsEnum.CANCEL_BUTTON))
            {
                EngineManager.popState();
                InputSet.getInstance().setAllToggles();
                return;
            }
        }

        public override void draw()
        {
            /*
            TextureDrawer td = DrawBuffer.getInstance().getUpdateStack().getNext();
            Point p = engine_.GraphicsDevice.Viewport.TitleSafeArea.Center;
            Vector2 v = new Vector2(p.X, p.Y);
            td.set(baseImage_, 0, v, CoordinateTypeEnum.ABSOLUTE, Constants.DEPTH_DIALOGUE_PAGE,
                true, Color.White, 0f, 1f);
            DrawBuffer.getInstance().getUpdateStack().push();
            */

            WorldManager.DrawMap(m_displayOffset, 600, 500, Constants.DepthDialogueText);
        }
    }
}
