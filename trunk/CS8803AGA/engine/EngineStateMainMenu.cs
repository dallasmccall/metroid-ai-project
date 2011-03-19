using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MetroidAI.devices;
using MetroidAI.ui;
using Microsoft.Xna.Framework.Graphics;
using MetroidAI.audio;

namespace MetroidAI.engine
{
    public class EngineStateMainMenu : AEngineState
    {
        private const string c_StartGame = "Start Game";
        private const string c_Settings = "Settings";
        private const string c_Credits = "Credits";
        private const string c_Quit = "Quit";

        private GameTexture m_tPausePage = new GameTexture("Sprites/splash2");

        private MenuList m_menuList;

        public EngineStateMainMenu(Engine engine) : base(engine)
        {
            List<string> menuOptions = new List<string>();
            menuOptions.Add(c_StartGame);
            menuOptions.Add(c_Settings);
            menuOptions.Add(c_Credits);
            menuOptions.Add(c_Quit);

            Point temp = m_engine.GraphicsDevice.Viewport.TitleSafeArea.Center;
            m_menuList = new MenuList(menuOptions, new Vector2(temp.X, temp.Y + 100));
            m_menuList.Font = FontEnum.Kootenay48;
            m_menuList.ItemSpacing = 70;
            m_menuList.SpaceAvailable = 400;

            SoundEngine.getInstance().playMusic("Menu");
        }

        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (InputSet.getInstance().getButton(InputsEnum.CONFIRM_BUTTON))
            {
                InputSet.getInstance().setAllToggles();

                switch (m_menuList.SelectedString)
                {
                    case c_StartGame:
                        EngineManager.replaceCurrentState(new EngineStateLoading(m_engine));
                        return;
                    case c_Settings:
                        EngineManager.pushState(new EngineStateSettings(m_engine));
                        return;
                    case c_Credits:
                        EngineManager.replaceCurrentState(new EngineStateCredits(m_engine));
                        return;
                    case c_Quit:
                        m_engine.Exit();
                        return;
                    default:
                        break;
                }
            }

            if (InputSet.getInstance().getLeftDirectionalY() < 0)
            {
                m_menuList.selectNextItem();
                InputSet.getInstance().setStick(InputsEnum.LEFT_DIRECTIONAL, 5);
            }

            if (InputSet.getInstance().getLeftDirectionalY() > 0)
            {
                m_menuList.selectPreviousItem();
                InputSet.getInstance().setStick(InputsEnum.LEFT_DIRECTIONAL, 5);
            }
        }

        public override void draw()
        {
            m_menuList.draw();

            DrawCommand td = DrawBuffer.getInstance().DrawCommands.pushGet();
            Point p = m_engine.GraphicsDevice.Viewport.TitleSafeArea.Center;
            Vector2 v = new Vector2(p.X, p.Y);
            td.set(m_tPausePage, 0, v, CoordinateTypeEnum.ABSOLUTE, Constants.DEPTH_LOW,
                true, Color.White, 0f, 1f);
        }
    }
}
