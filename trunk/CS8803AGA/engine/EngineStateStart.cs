/*
***************************************************************************
* Copyright notice removed by a creator for anonymity, please don't sue   *
*                                                                         *
* Licensed under the Apache License, Version 2.0 (the "License");         *
* you may not use this file except in compliance with the License.        *
* You may obtain a copy of the License at                                 *
*                                                                         *
* http://www.apache.org/licenses/LICENSE-2.0                              *
*                                                                         *
* Unless required by applicable law or agreed to in writing, software     *
* distributed under the License is distributed on an "AS IS" BASIS,       *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.*
* See the License for the specific language governing permissions and     *
* limitations under the License.                                          *
***************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Xml;
using CS8803AGA.devices;

namespace CS8803AGA.engine
{
    /// <summary>
    /// EngineStateStart's purpose is to determine the active controller and
    /// prompt storage device selection.  On PC, the logic runs automatically
    /// (since PC supports only one player) and the screen never needs to be
    /// presented.
    /// </summary>
    class EngineStateStart : AEngineState
    {
        protected const FontEnum TEXT_FONT = FontEnum.Kootenay14;
        protected readonly Color TEXT_COLOR = Color.White;
        protected const float TEXT_ROTATION = 0.0f;
        protected const float TEXT_DEPTH = Constants.DepthMainMenuText;
        protected Vector2 TEXT_POSITION
        {
            get
            {
                Rectangle r = m_engine.GraphicsDevice.Viewport.TitleSafeArea;
                float topEmptySpace = m_logoImage.ImageDimensions[0].Height;
                float y = r.Y + (r.Height - topEmptySpace) / 2 + topEmptySpace;
                return new Vector2(r.X + r.Width / 2.0f, y);
            }
        }

        protected Vector2 LOGO_POSITION
        {
            get
            {
                Rectangle r = m_engine.GraphicsDevice.Viewport.TitleSafeArea;
                return new Vector2(r.X + (r.Width - m_logoImage.ImageDimensions[0].Width) / 2, r.Y);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        protected const float LOGO_DEPTH = Constants.DepthMainMenuText;

        protected string TEXT_MESSAGE = "Press START to Continue";

        protected GameTexture m_logoImage;
        protected bool m_returnFlag;

        /// <summary>
        /// Constructor determines whether PC or Xbox 360 and initializes
        /// variables accordingly.
        /// 
        /// For PC, this means creating the controller and trying to get a
        /// storage device.
        /// 
        /// For the Xbox 360, we don't need to do much as we rely on update()
        /// logic to determine active controller and storage device.
        /// </summary>
        /// <param name="engine"></param>
        public EngineStateStart(Engine engine) : base(engine)
        {
            m_engine = engine;
            m_logoImage = new GameTexture(@"Sprites\TitleScreen");

#if !XBOX
            {
                Settings settings = Settings.getInstance();
                if (Settings.getInstance().IsUsingMouse)
                {
                    m_engine.Controls = new PCControllerInput(m_engine);
                    settings.CurrentPlayer = PlayerIndex.One;
                    TEXT_MESSAGE = "Press " + m_engine.Controls.getControlName(InputsEnum.CONFIRM_BUTTON).ToUpper() + " to Continue";
                }
                else
                {
                    m_engine.Controls = new X360ControllerInput(m_engine, PlayerIndex.One);
                    settings.CurrentPlayer = PlayerIndex.One;
                    TEXT_MESSAGE = "Press " + m_engine.Controls.getControlName(InputsEnum.CONFIRM_BUTTON).ToUpper() + " to Continue";
                }
                prepareStorageDevice();
                m_returnFlag = true;
            }
#else
            {
                m_engine.Controls = null; // reset controls if they are coming back to this
                m_returnFlag = false;
            }
#endif

        }

        /// <summary>
        /// Waits for input from a controller to determine the active controller.
        /// Also monitors flow of controller setup, profile setup, storage setup. 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void update(GameTime gameTime)
        {
            if (m_returnFlag)
            {
                InputSet.getInstance().setAllToggles();
                EngineManager.replaceCurrentState(new EngineStateMainMenu(m_engine));
                return;
            }

            if (m_engine.Controls == null && !m_returnFlag)
            {
                prepareControls();
            }

            // TODO
            // Load profile information here?

            if (m_engine.Controls != null && !m_returnFlag)
            {
                prepareStorageDevice();
            }
        }

        public override void draw()
        {
            //engine_.GraphicsDevice.Clear(Color.Black);
            DrawBuffer.getInstance().DrawCommands.ScreenClearColor = Color.Black;

            //logo_.drawImageAbsolute(0, LOGO_POSITION, LOGO_DEPTH);
            DrawStack stack = DrawBuffer.getInstance().DrawCommands;
            DrawCommand td = stack.pushGet();
            td.set(m_logoImage,
                    0,
                    LOGO_POSITION,
                    CoordinateTypeEnum.ABSOLUTE,
                    LOGO_DEPTH,
                    false,
                    Color.White,
                    0.0f,
                    1.0f);

            GameFont myFont = FontMap.getInstance().getFont(TEXT_FONT);
            myFont.drawStringCentered(TEXT_MESSAGE,
                                        TEXT_POSITION,
                                        TEXT_COLOR,
                                        TEXT_ROTATION,
                                        TEXT_DEPTH);
        }

        /// <summary>
        /// Waits for any one of four controllers to press Start or A, then sets that
        /// controller as the active player.
        /// </summary>
        private void prepareControls()
        {
            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
            {
                GamePadState gps = GamePad.GetState(index);
                if (gps.IsButtonDown(Buttons.Start) || gps.IsButtonDown(Buttons.A))
                {
                    Settings.getInstance().CurrentPlayer = index;
                    m_engine.Controls = new X360ControllerInput(m_engine, index);
                    break;
                }
            }
        }

        /// <summary>
        /// If GamerServices is available, opens up the Guide so the user can select
        /// a storage device.  Otherwise, it sets a flag to quit and the storage
        /// will be determined later (PC).
        /// </summary>
        private void prepareStorageDevice()
        {
            Settings settings = Settings.getInstance();
            if (settings.IsGamerServicesAllowed && !Guide.IsVisible)
            {
                try
                {
                    IAsyncResult result =
                        Guide.BeginShowStorageDeviceSelector(fetchStorageDevice, "selectStorage");
                }
                catch (GuideAlreadyVisibleException)
                {
                    // No code here, but this catch block is needed
                    // because !Guide.IsVisible doesn't always work
                }
            }
            else
            {
                Settings.getInstance().loadSettingsFromFile();
                settings.StorageDevice = null;
                m_returnFlag = true;
            }
        }

        /// <summary>
        /// Asynchronous call to run once the user selects a storage device from the
        /// Guide menu.  Determines next state based on user's decision.
        /// </summary>
        /// <param name="result"></param>
        private void fetchStorageDevice(IAsyncResult result)
        {
            StorageDevice storageDevice = Guide.EndShowStorageDeviceSelector(result);

            // User selected a device, so we set it and get ready to quit
            if (storageDevice != null)
            {
                Settings.getInstance().StorageDevice = storageDevice;
                m_returnFlag = true;
            }

            // User cancelled, so we remove active controller and wait again
            else
            {
                m_engine.Controls = null;
                m_returnFlag = false;
            }

            Settings.getInstance().loadSettingsFromFile();
        }

    }
}
