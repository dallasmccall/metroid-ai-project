using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.ui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CS8803AGA.devices;

namespace CS8803AGA.engine
{
    class EngineStatePause : AEngineState
    {
        private const string c_ReturnToGame = "Return to Game";
        private const string c_Settings = "Settings";
        private const string c_Quit = "Quit Game";

        private GameTexture m_tPausePage = new GameTexture("Sprites/splash2");

        private MenuList m_menuList;

        public EngineStatePause(Engine engine)
            : base(engine)
        {
            List<string> menuOptions = new List<string>();
            menuOptions.Add(c_ReturnToGame);
            menuOptions.Add(c_Settings);
            menuOptions.Add(c_Quit);

            Point temp = m_engine.GraphicsDevice.Viewport.TitleSafeArea.Center;
            m_menuList = new MenuList(menuOptions, new Vector2(temp.X, temp.Y + 100));
            m_menuList.Font = FontEnum.Kootenay48;
            m_menuList.ItemSpacing = 70;
            m_menuList.SpaceAvailable = 400;
        }

        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (InputSet.getInstance().getButton(InputsEnum.CONFIRM_BUTTON) ||
                InputSet.getInstance().getButton(InputsEnum.BUTTON_1))
            {
                InputSet.getInstance().setAllToggles();

                switch (m_menuList.SelectedString)
                {
                    case c_ReturnToGame:
                        EngineManager.popState();
                        return;
                    case c_Settings:
                        EngineManager.pushState(new EngineStateSettings(m_engine));
                        return;
                    case c_Quit:
                        EngineManager.popState();
                        ((EngineStateGameplay)EngineManager.peekAtState()).cleanup();
                        EngineManager.popState();
                        EngineManager.pushState(new EngineStateMainMenu(m_engine));
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
