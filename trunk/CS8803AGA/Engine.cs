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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using MetroidAI.devices;
using MetroidAI.engine;
using MetroidAI.audio;

namespace MetroidAI
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;
        protected ControllerInputInterface m_controls;

        public GraphicsDeviceManager Graphics
        {
            get { return m_graphics; }
        }
        public SpriteBatch SpriteBatch
        {
            get { return m_spriteBatch; }
        }
        public ControllerInputInterface Controls
        {
            get
            {
                return m_controls;
            }
            set
            {
                m_controls = value;
                if (UpdateThread != null)
                {
                    UpdateThread.Controls = m_controls;
                }
            }
        }
        public UpdateThread UpdateThread { get; protected set; }
        public RenderThread RenderThread { get; protected set; }
        public DrawBuffer DrawBuffer { get; protected set; }

        internal bool UpdateGraphicsFlag { get; set; }

        public const int FRAMERATE = 30;

        public const int SCREEN_MIN_WIDTH = 1360; // 1360
        public const int SCREEN_MIN_HEIGHT = 820; // 820

        public Engine()
        {
            
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (1000 / FRAMERATE));
        }

        public void initializeScreen()
        {
            /*setScreenSize(
                Math.Max(this.GraphicsDevice.DisplayMode.Width, SCREEN_MIN_WIDTH),
                Math.Max(this.GraphicsDevice.DisplayMode.Height, SCREEN_MIN_HEIGHT)
                );*/
            setScreenSize(SCREEN_MIN_WIDTH, SCREEN_MIN_HEIGHT);
        }

        public void setScreenSize(int x, int y)
        {
            Graphics.IsFullScreen = false;

#if !XBOX
            if (!Graphics.IsFullScreen)
            {
                y -= 100; // account for application bar
            }
#endif

            Graphics.PreferredBackBufferHeight = y;
            Graphics.PreferredBackBufferWidth = x;
            UpdateGraphicsFlag = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            initializeScreen();
            Settings.initialize(this);

            this.IsMouseVisible = true;

#if XBOX
            Settings.getInstance().IsUsingMouse = false;
#else
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                Settings.getInstance().IsUsingMouse = false;
            }
            else
            {
                Settings.getInstance().IsUsingMouse = true;
            }
#endif

            try
            {
                // Debugging - Uncomment this line to try PC version as if it
                //  were running with the Redistributable runtime in which
                //  GamerServices is not available
                // Note that this is not a truly accurate test, as there could
                //  be lurking calls to GamerServices outside of a block which
                //  tests Settings.IsGamerServicesAllowed prior to using
                throw new Exception();

                GamerServicesComponent gsc = new GamerServicesComponent(this);
                gsc.Initialize();
                this.Components.Add(gsc);
                Settings.getInstance().IsGamerServicesAllowed = true;
            }
            catch
            {
                Settings.getInstance().IsGamerServicesAllowed = false;
            }

            // creating EngineStateStart must come AFTER setting the
            //  IsGamerServicesAllowed member of Settings

            EngineManager.initialize(this);
            EngineManager.pushState(new EngineStateSplash(this));

            int tiles = (int)((GraphicsDevice.Viewport.Height / 15) * (GraphicsDevice.Viewport.Width / 15) * 1.2);
            tiles += 350;

            DrawBuffer.initialize(tiles, SpriteBatch);
            DrawBuffer = DrawBuffer.getInstance();
            UpdateThread = new UpdateThread(this);
            RenderThread = new RenderThread();
            UpdateThread.Controls = Controls;
            UpdateThread.startThread();

            GlobalHelper.getInstance().Engine = this;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            GameTexture.s_spriteBatch = SpriteBatch;
            GameTexture.s_content = Content;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (UpdateGraphicsFlag)
                Graphics.ApplyChanges();

            if (Controls != null)
                Controls.updateInputSet();

            base.Update(gameTime);

            SoundEngine.getInstance().update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);

            /*
             * Problems with bleeding can be resolved either by adding gutter pixels to all sprite sheets
             * or not using texture filtering.  To turn off texture filtering, include these lines of code
             * and change spriteBatch.Begin's SpriteSortMode to "Immediate".  Problem is that Immediate doesnt
             * take sprite depth into account when rendering... could sort them manually.
             * 
             * Note that normally 0.0 = near, 1.0 = far, but in Commando we had accidentally adopted the opposite
             * convention, which is why we used FrontToBack instead of BackToFront.  If we want stuff with
             * transparency, this is a problem as stuff behind the transparent object will get drawn after the
             * transparent object, negating (or at very least slowing down) the transparency.
             * 
             * Thus, my solution was to instead use BackToFront like we should if we want transparency, and
             * just invert depth (to 1.0f - depth) in the TextureDrawer and FontDrawer class.
             * 
             * More info here: http://www.gamedev.net/community/forums/topic.asp?topic_id=529591
             * And here: http://blogs.msdn.com/shawnhar/archive/2006/12/13/spritebatch-and-spritesortmode.aspx
             * 
             
            spriteBatch_.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.None;
            spriteBatch_.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.None;
            spriteBatch_.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.None;
             */

            DrawBuffer.globalStartFrame(gameTime);
            Graphics.GraphicsDevice.Clear(DrawBuffer.getRenderStack().ScreenClearColor);

            RenderThread.tick();

            base.Draw(gameTime);

            DrawBuffer.globalSynchronize();
            SpriteBatch.End();

        }

        /// <summary>
        /// Event handler for when the game tries to close.  Overridden to push
        /// settings to a file and clean up audio resources.
        /// </summary>
        /// <param name="sender">Object initiating the event.</param>
        /// <param name="args">Arguments.</param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            DrawBuffer.cleanUp();
            if (UpdateThread.RunningThread != null)
            {
                UpdateThread.RunningThread.Abort();
            }

            base.OnExiting(sender, args);
            SoundEngine.cleanup();
            Settings.getInstance().saveSettingsToFile();
        }

#if !XBOX
        /// <summary>
        /// Whether or not the mouse is outside the game window.
        /// </summary>
        /// <returns>True is the mouse is outside the game window, false otherwise</returns>
        internal protected bool mouseOutsideWindow()
        {
            MouseState ms = Mouse.GetState();
            if (ms.X < 0 || ms.Y < 0 ||
                ms.X > this.GraphicsDevice.Viewport.X + this.GraphicsDevice.Viewport.Width ||
                ms.Y > this.GraphicsDevice.Viewport.Y + this.GraphicsDevice.Viewport.Height)
            {
                return true;
            }
            return false;
        }
#endif

    }

}
