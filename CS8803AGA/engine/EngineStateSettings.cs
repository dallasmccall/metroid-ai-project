using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.ui;
using Microsoft.Xna.Framework;
using CS8803AGA.devices;
using Microsoft.Xna.Framework.Graphics;

namespace CS8803AGA.engine
{
    class EngineStateSettings : AEngineState
    {
        private const string c_Back = "Back";
        private const string c_SoundOn = "Sound: On";
        private const string c_SoundOff = "Sound: Off";
        private const string c_DebugOn = "Debug: On";
        private const string c_DebugOff = "Debug: Off";
        private const string c_CameraSmart = "Camera: Smart";
        private const string c_CameraFreeForm = "Camera: FreeForm";
        private const string c_PlayerTypeExplorer = "Player Type: Explorer";
        private const string c_PlayerTypeKiller = "Player Type: Killer";

        private GameTexture m_tPausePage = new GameTexture("Sprites/splash2");

        private MenuList m_menuList;

        public EngineStateSettings(Engine engine)
            : base(engine)
        {
            List<string> menuOptions = new List<string>();
            if (Settings.getInstance().IsSoundAllowed)
            {
                menuOptions.Add(c_SoundOn);
            }
            else
            {
                menuOptions.Add(c_SoundOff);
            }
            if (Settings.getInstance().IsInDebugMode)
            {
                menuOptions.Add(c_DebugOn);
            }
            else
            {
                menuOptions.Add(c_DebugOff);
            }
            if (Settings.getInstance().IsCameraFreeform)
            {
                menuOptions.Add(c_CameraFreeForm);
            }
            else
            {
                menuOptions.Add(c_CameraSmart);
            }
            if (Settings.getInstance().IsExplorer)
            {
                menuOptions.Add(c_PlayerTypeExplorer);
            }
            else
            {
                menuOptions.Add(c_PlayerTypeKiller);
            }
            menuOptions.Add(c_Back);

            Point temp = m_engine.GraphicsDevice.Viewport.TitleSafeArea.Center;
            m_menuList = new MenuList(menuOptions, new Vector2(temp.X, temp.Y + 50));
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
                    case c_Back:
                        EngineManager.popState();
                        return;
                    case c_SoundOn:
                        Settings.getInstance().IsSoundAllowed = false;
                        m_menuList.setString(m_menuList.SelectedIndex, c_SoundOff);
                        break;
                    case c_SoundOff:
                        Settings.getInstance().IsSoundAllowed = true;
                        m_menuList.setString(m_menuList.SelectedIndex, c_SoundOn);
                        break;
                    case c_DebugOn:
                        Settings.getInstance().IsInDebugMode = false;
                        m_menuList.setString(m_menuList.SelectedIndex, c_DebugOff);
                        break;
                    case c_DebugOff:
                        Settings.getInstance().IsInDebugMode = true;
                        m_menuList.setString(m_menuList.SelectedIndex, c_DebugOn);
                        break;
                    case c_CameraFreeForm:
                        Settings.getInstance().IsCameraFreeform = false;
                        m_menuList.setString(m_menuList.SelectedIndex, c_CameraSmart);
                        break;
                    case c_CameraSmart:
                        Settings.getInstance().IsCameraFreeform = true;
                        m_menuList.setString(m_menuList.SelectedIndex, c_CameraFreeForm);
                        break;
                    case c_PlayerTypeExplorer:
                        Settings.getInstance().IsExplorer = false;
                        m_menuList.setString(m_menuList.SelectedIndex, c_PlayerTypeKiller);
                        break;
                    case c_PlayerTypeKiller:
                        Settings.getInstance().IsExplorer = true;
                        m_menuList.setString(m_menuList.SelectedIndex, c_PlayerTypeExplorer);
                        break;
                    default:
                        break;
                }
            }

            if (InputSet.getInstance().getButton(InputsEnum.CANCEL_BUTTON) ||
                InputSet.getInstance().getButton(InputsEnum.BUTTON_2))
            {
                InputSet.getInstance().setAllToggles();
                EngineManager.popState();
                return;
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
