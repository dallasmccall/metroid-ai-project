using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.controllers;
using CS8803AGA.world;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace CS8803AGA.engine
{
    class EngineStateZoneTransition : AEngineState
    {
        DoorController m_departingDoorController;
        DoorController m_arrivingDoorController;
        Zone m_arrivingZone;
        Vector2 m_newSamusPosition;
        float m_cameraDistanceToTravel;
        float m_cameraSpeed;

        bool m_newZoneLoaded = false;
        GameTexture m_overlay = new GameTexture(@"Sprites/blank");
        int m_fadeTime = 0;
        const int m_maxFadeTime = 15;

        Thread m_zoneInitializationThread;

        public EngineStateZoneTransition(
            Engine engine,
            DoorController departingDoor,
            DoorController arrivingDoor,
            Zone arrivingZone,
            Vector2 newSamusPosition
        )
            : base(engine)
        {
            m_departingDoorController = departingDoor;
            m_arrivingDoorController = arrivingDoor;
            m_arrivingZone = arrivingZone;
            m_newSamusPosition = newSamusPosition;

            if (m_departingDoorController.Side.IsHorizontal)
            {
                m_cameraDistanceToTravel = Zone.SCREEN_WIDTH_IN_PIXELS;
            }
            else
            {
                m_cameraDistanceToTravel = Zone.SCREEN_HEIGHT_IN_PIXELS;
            }
            m_cameraSpeed = m_cameraDistanceToTravel / (Engine.FRAMERATE * 1.0f);

            startTransition();
        }

        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (m_fadeTime < m_maxFadeTime && m_cameraDistanceToTravel > 0) // initial fade out
            {
                m_fadeTime++;
            }
            else if (m_fadeTime == m_maxFadeTime && // fully faded out, so move camera
                     m_cameraDistanceToTravel > 0) 
            {
                float distance = Math.Min(m_cameraSpeed, m_cameraDistanceToTravel);

                Camera cam = GlobalHelper.getInstance().Camera;
                cam.Position = m_departingDoorController.Side.Move(cam.Position, distance);

                m_cameraDistanceToTravel = Math.Max(0, m_cameraDistanceToTravel - distance);
            }
            else if (m_cameraDistanceToTravel == 0 &&   // camera moved, so fade back in
                     m_fadeTime > 0)
            {
                // must wait for next zone to be inited
                if (!m_zoneInitializationThread.IsAlive)
                {
                    if (m_fadeTime == m_maxFadeTime) // first frame of this state
                    {
                        finishTransition();
                    }
                    m_fadeTime--;
                }
            }
            else if (m_cameraDistanceToTravel == 0 && m_fadeTime <= 0) // fully faded - done!
            {
                EngineManager.popState();
            }
        }

        public override void draw()
        {
            Rectangle screen =
                new Rectangle(
                    0,
                    0,
                    m_engine.GraphicsDevice.Viewport.Width,
                    m_engine.GraphicsDevice.Viewport.Height);
            Color color = new Color(0.0f, 0.0f, 0.0f, (float)m_fadeTime / m_maxFadeTime);

            DrawCommand dc = DrawBuffer.getInstance().DrawCommands.pushGet();
            dc.set(m_overlay, 0, Vector2.Zero, CoordinateTypeEnum.ABSOLUTE, Constants.DepthGameplayOverlay, false, color, 0.0f, 1.0f);
            dc.Destination = screen;
            dc.UseDestination = true;

            EngineManager.peekBelowState(this).draw();

            if (!m_newZoneLoaded)
            {
                m_departingDoorController.draw(Constants.DepthGameplayDoors);
            }
            else
            {
                m_arrivingDoorController.draw(Constants.DepthGameplayDoors);
            }
        }

        protected void startTransition()
        {
            m_zoneInitializationThread =
                new Thread(new ThreadStart(m_arrivingZone.Initialize));
            m_zoneInitializationThread.Start();
        }

        protected void finishTransition()
        {
            m_newZoneLoaded = true;

            PlayerController samus = GameplayManager.Samus;
            Zone departingZone = GameplayManager.ActiveZone;

            departingZone.remove(samus);
            samus.getCollider().move(-samus.DrawPosition); // move Samus to 0,0
            samus.getCollider().move(m_newSamusPosition); // then to target position
            samus.PreviousPosition = samus.DrawPosition;

            m_arrivingZone.HasBeenVisited = true;
            m_arrivingZone.add(GameplayManager.Samus);

            GameplayManager.ActiveZone = m_arrivingZone;

            GlobalHelper.getInstance().Camera.Position =
                m_arrivingZone.getLocalScreenOffsetInPixelVector(
                    m_arrivingZone.getLocalScreenCoordFromPosition(GameplayManager.Samus.DrawPosition));

            m_departingDoorController.close();
        }
    }
}
