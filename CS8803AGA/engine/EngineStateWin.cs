using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.audio;
using Microsoft.Xna.Framework;
using MetroidAI.devices;
using Microsoft.Xna.Framework.Graphics;

namespace MetroidAI.engine
{
    public class EngineStateWin : AEngineState
    {
        private int m_tick = 0;
        private GameTexture m_splash = new GameTexture("Sprites/OriginalMetroidEnding");

        public EngineStateWin(Engine engine)
            : base(engine)
        {
            SoundEngine.getInstance().playMusic("Win");
        }

        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (InputSet.getInstance().getButton(InputsEnum.CONFIRM_BUTTON) ||
                InputSet.getInstance().getButton(InputsEnum.BUTTON_1) ||
                m_tick > 30 * 27) // 27 is length of song
            {
                InputSet.getInstance().setAllToggles();

                EngineManager.popState();
                ((EngineStateGameplay)EngineManager.peekAtState()).cleanup();
                EngineManager.popState();
                EngineManager.pushState(new EngineStateMainMenu(m_engine));
                return;
            }

            m_tick++;
        }

        public override void draw()
        {
            DrawCommand td = DrawBuffer.getInstance().DrawCommands.pushGet();
            Point p = m_engine.GraphicsDevice.Viewport.TitleSafeArea.Center;
            Vector2 v = new Vector2(p.X, p.Y);
            td.set(m_splash, 0, v, CoordinateTypeEnum.ABSOLUTE, Constants.DepthDebugLines,
                false, Color.White, 0f, 1f);
            td.UseDestination = true;
            td.Destination = m_engine.GraphicsDevice.Viewport.TitleSafeArea;
        }
    }
}
