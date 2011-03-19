using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CS8803AGAGameLibrary;
using System.Collections.Generic;
using CSharpQuadTree;
using System;
using CS8803AGA.collision;
using CS8803AGA.devices;
using CS8803AGA.controllers;
using CS8803AGA.world;
using CS8803AGA.controllers.enemies;
using CS8803AGA.audio;
using CS8803AGA.world.space;
using CS8803AGA.controllers.mission;
using System.Threading;
using CS8803AGA.world.space.expanders;
using CS8803AGA.world.mission;
using System.IO;
using CS8803AGAGameLibrary.player;

namespace CS8803AGA.engine
{
    /// <summary>
    /// Engine state for the main gameplay processes of the game.
    /// </summary>
    public class EngineStateGameplay : AEngineState
    {
        private Thread m_zoneInitializationThread;
        private bool m_failedToCreate = false;

        private MiniKraidController m_winCondition;

        private Dictionary<Vector2, Color> m_octantSampleTest = new Dictionary<Vector2, Color>(); // TODO remove me

        /// <summary>
        /// Creates an EngineStateGameplay and registers it with the GameplayManager.
        /// Also creates and initializes a PlayerController and starting Area.
        /// </summary>
        /// <param name="engine">Engine instance for the game.</param>
        public EngineStateGameplay(Engine engine) : base(engine)
        {
            SoundEngine.getInstance().playMusic("Exploration");

            if (GameplayManager.GameplayState != null)
                throw new Exception("Only one EngineStateGameplay allowed at once!");

            CharacterInfo ci = GlobalHelper.loadContent<CharacterInfo>(@"Characters/SamusRed");

            PlayerController player =
                (PlayerController)CharacterController.construct(ci, new Vector2(600, 400), true);

            // Test mission construction

            int seed = RandomManager.get().Next(3000);
            if (Settings.getInstance().IsExplorer)
            {
                seed = 1282;
            }
            else
            {
                seed = 1019;
            }
            System.Console.WriteLine(String.Format("SEED: {0}", seed));
            RandomManager.Seed(seed);

            FileStream grammarStream = new FileStream(
               m_engine.Content.RootDirectory + @"/../../../../Content/Mission/NewGrammar.xml",
               FileMode.Open);
            IMissionGrammar grammar = new MissionGrammarImpl(grammarStream);
            IMission mission = grammar.WalkGrammar(new PlayerModel());
            IMissionQueue missionQueue = mission.MissionQueue;
            grammarStream.Close();

            // End test mission construction

            // Test space construction

            /*
            MissionQueueDummy mqd = new MissionQueueDummy();
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Right, Direction.Right, 3));
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Right, Direction.Down, 2));
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Right, Direction.Right, 1));
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Up, Direction.Right, 2));
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Up, Direction.Left, 2));
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Left, Direction.Up, 1));
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Left, Direction.Left, 3));
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Down, Direction.Left, 4));
            mqd.ReadyTerminals.Add(new MissionTerminalExpanderDummy(Direction.Down, Direction.Down, 1));
            */
            
            IMissionQueue missionQueueToUse = missionQueue;

            //ISpaceCreationAlgorithm algo = new AStarSpaceCreationAlgorithm(AStarSpaceCreationAlgorithm.Suboptimal);
            ISpaceCreationAlgorithm algo = new DFSSpaceCreationAlgorithm();
            algo.Mission = missionQueueToUse;

            SpaceImpl spaceImpl = (SpaceImpl) algo.ConstructSpace();
            if (spaceImpl == null)
            {
                m_failedToCreate = true;
                return;
            }
            Dictionary<Point, ScreenConstructionParameters> world = spaceImpl.makeWorld();

            ZoneBuilder.BuildWorld(world);

            // End test space construction

            // Test items and mission stuff

            /*Zone z = WorldManager.GetZone(new Point(-1, 0));
            if (z != null)
            {
                z.add(new RefillEnergy(new Vector2(420, 440)));
                z.add(new RefillMissile(new Vector2(660, 440)));
            }
            z = WorldManager.GetZone(new Point(1, 0));
            if (z != null)
            {
                z.add(new RipperController(new Vector2(420, 440)));
                z.add(new RipperController(new Vector2(420, 1040)));
                z.add(new RipperController(new Vector2(420, 1640)));
            }
            z = WorldManager.GetZone(new Point(2, 0));
            if (z != null)
            {
                z.add(new MiniKraidController(new Vector2(620, 440)));
            }*/
            foreach (Zone zone in WorldManager.GetZones())
            {
                if (zone.ScreenConstructionInfos[zone.TopLeftPosition].Parameters.Label.Equals("BossMiniTerminal"))
                {
                    zone.add(new MiniKraidController(new Vector2(620, 440)));
                }
                else if (zone.ScreenConstructionInfos[zone.TopLeftPosition].Parameters.Label.Equals("BattleTerminal"))
                {
                    zone.add(new SidehopperController(new Vector2(620, 440)));
                    zone.add(new SidehopperController(new Vector2(420, 440)));
                    zone.add(new SidehopperController(new Vector2(300, 440)));
                }
                else if (zone.ScreenConstructionInfos[zone.TopLeftPosition].Parameters.Label.Equals("TestSecretTerminal"))
                {
                    zone.add(new SidehopperController(new Vector2(420, 440)));
                    zone.add(new RipperController(new Vector2(420, 440)));
                }
                else if (zone.ScreenConstructionInfos[zone.TopLeftPosition].Parameters.Label.Equals("ItemQuestTerminal"))
                {
                    zone.add(new RefillEnergy(new Vector2(420, 440)));
                }

                else if (zone.ScreenConstructionInfos[zone.TopLeftPosition].Parameters.Label.Equals("BossFinalTerminal"))
                {
                    zone.add(new MiniKraidController(new Vector2(620, 440)));
                    zone.add(new MiniKraidController(new Vector2(300, 440)));
                    zone.add(new MiniKraidController(new Vector2(820, 440)));
                }
                else if (zone.ScreenConstructionInfos[zone.TopLeftPosition].Parameters.Label.Equals("ItemBonusTerminal"))
                {
                    zone.add(new RefillMissile(new Vector2(660, 440)));
                }
            }

            // End test items and mission stuff

            // Test octant sampling
            /*
            Queue<Color> colors = new Queue<Color>();
            colors.Enqueue(Color.Yellow);
            colors.Enqueue(Color.Blue);
            colors.Enqueue(Color.Red);
            colors.Enqueue(Color.Green);
            colors.Enqueue(Color.Pink);
            colors.Enqueue(Color.Purple);
            colors.Enqueue(Color.Brown);
            colors.Enqueue(Color.Orange);

            foreach (Direction primary in Direction.All)
            {
                Color c = colors.Dequeue();
                for (int i = 0; i < 200; ++i)
                {
                    m_octantSampleTest.Add(Zone.SampleOctant(primary, primary.RotationCW), c);
                }

                c = colors.Dequeue();
                for (int i = 0; i < 200; ++i)
                {
                    m_octantSampleTest.Add(Zone.SampleOctant(primary, primary.RotationCCW), c);
                }
            }
            */
            // end test octant sampling

            Zone startZone = WorldManager.GetZone(new Point(0, 0));
            startZone.Initialize();
            GameplayManager.initialize(this, player, startZone);

            m_zoneInitializationThread = new Thread(this.zoneInitializationThread);
            m_zoneInitializationThread.Start();

            m_winCondition = new MiniKraidController(new Vector2(800, 450));
            /*
            WorldManager.GetZone(new Point(0, 3)).add(m_winCondition);

            WorldManager.GetZone(new Point(0,0)).add(new SidehopperController(new Vector2(800,400)));
            WorldManager.GetZone(new Point(2, 0)).add(new SidehopperController(new Vector2(800, 400)), new Point(2, 0));
            WorldManager.GetZone(new Point(1, -2)).add(new SidehopperController(new Vector2(800, 400)), new Point(0, 0));
            WorldManager.GetZone(new Point(0, 1)).add(new SidehopperController(new Vector2(800, 400)), new Point(0, 1));
            WorldManager.GetZone(new Point(-1, 0)).add(new SidehopperController(new Vector2(800, 400)), new Point(0, 0));

            //WorldManager.GetZone(new Point(0, 0)).add(new RipperController(new Vector2(800, 250)));
            WorldManager.GetZone(new Point(2, 0)).add(new RipperController(new Vector2(800, 300)), new Point(1, 0));
            WorldManager.GetZone(new Point(1, -2)).add(new RipperController(new Vector2(800, 399)), new Point(1, 0));
            WorldManager.GetZone(new Point(0, 1)).add(new RipperController(new Vector2(800, 399)), new Point(-1, 1));
            WorldManager.GetZone(new Point(-1, 0)).add(new RipperController(new Vector2(800, 399)), new Point(1, 2));
            */

            GlobalHelper gh = GlobalHelper.getInstance();
            gh.Camera =
                new Camera(GlobalHelper.getInstance().Engine.Graphics.GraphicsDevice.Viewport.Width,
                           GlobalHelper.getInstance().Engine.Graphics.GraphicsDevice.Viewport.Height);
            gh.Camera.Position = Vector2.Zero;
                //new Vector2(GameplayManager.Samus.DrawPosition.X - gh.Engine.GraphicsDevice.Viewport.Width / 2,
                //            GameplayManager.Samus.DrawPosition.Y - gh.Engine.GraphicsDevice.Viewport.Height / 2);
        }

        /// <summary>
        /// Main game loop, checks for UI-related inputs and tells game objects to update.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (m_failedToCreate)
            {
                EngineManager.replaceCurrentState(new EngineStateMainMenu(m_engine));
                return;
            }

            if (InputSet.getInstance().getButton(InputsEnum.CONFIRM_BUTTON))
            {
                InputSet.getInstance().setAllToggles();
                EngineManager.pushState(new EngineStatePause(m_engine));
                return;
            }

            if (InputSet.getInstance().getButton(InputsEnum.CANCEL_BUTTON))
            {
                InputSet.getInstance().setAllToggles();
                EngineManager.pushState(new EngineStateMap());
                return;
            }

            Zone zone = GameplayManager.ActiveZone;
            zone.GameObjects.ForEach(i => i.update());
            zone.GameObjects.ForEach(i => { if (!i.isAlive() && i is ICollidable) ((ICollidable)i).getCollider().unregister(); });
            zone.GameObjects.RemoveAll(i => !i.isAlive());

            adjustCamera();

            if (m_winCondition.Health <= 0)
            {
                InputSet.getInstance().setAllToggles();
                EngineManager.pushState(new EngineStateWin(m_engine));
            }

        }

        enum CameraMovement
        {
            ScreenCenter, FreeForm, TrackHorizontal, TrackVertical
        }

        private Vector2 m_cameraTarget = new Vector2();
        /*
        private Vector2 m_cameraVelocity = new Vector2();
        private Vector2 m_previousCameraTarget = new Vector2();
        private int m_stillFrames = 0;
        private Vector2 m_stillPosition = new Vector2();
         */

        private void adjustCamera()
        {
            GlobalHelper gh = GlobalHelper.getInstance();
            Camera cam = gh.Camera;

            Zone zone = GameplayManager.ActiveZone;

            Vector2 samusPos = GameplayManager.Samus.getCollider().Bounds.Center();

            if (Settings.getInstance().IsCameraFreeform)
            {
                cam.Center = samusPos;
                return;
            }

            Point screen = zone.getLocalScreenCoordFromPosition(samusPos);

            Vector2 samusPosInScreen = samusPos - zone.getLocalScreenOffsetInPixelVector(screen);

            Direction primary;
            Direction secondary;
            Zone.GetOctant(samusPosInScreen, out primary, out secondary);

            if (Settings.getInstance().IsInDebugMode)
            {
                GameFont gf = FontMap.getInstance().getFont(FontEnum.Kootenay14);
                gf.drawString(primary.ToString() + "," + secondary.ToString(), new Vector2(50, 600), Color.White);
            }

            CameraMovement movement;
            Point globalScreen = zone.getGlobalScreenFromPosition(samusPos);
            Point primaryDirScreen = DirectionUtils.Move(globalScreen, primary);
            Point secondaryDirScreen = DirectionUtils.Move(globalScreen, secondary);
            Point diagonalDirScreen = DirectionUtils.Move(primaryDirScreen, secondary);
            primaryDirScreen = zone.getGlobalScreenFromLocalScreen(primaryDirScreen);
            secondaryDirScreen = zone.getGlobalScreenFromLocalScreen(secondaryDirScreen);
            diagonalDirScreen = zone.getGlobalScreenFromLocalScreen(diagonalDirScreen);

            bool primaryDirScreenViewable =
                (zone.ScreenConstructionInfos[globalScreen].Parameters.Connections[primary] == Connection.Open);
            bool secondaryDirScreenViewable =
                (zone.ScreenConstructionInfos[globalScreen].Parameters.Connections[secondary] == Connection.Open);
            bool diagonalDirScreenViewable =
                (primaryDirScreenViewable && secondaryDirScreenViewable &&
                zone.ScreenConstructionInfos[primaryDirScreen].Parameters.Connections[secondary] == Connection.Open &&
                zone.ScreenConstructionInfos[secondaryDirScreen].Parameters.Connections[primary] == Connection.Open);
            /*bool primaryDirScreenViewable = zone.PositionsOwned.Contains(primaryDirScreen);
            bool secondaryDirScreenViewable = zone.PositionsOwned.Contains(secondaryDirScreen);
            bool diagonalDirScreenViewable = zone.PositionsOwned.Contains(diagonalDirScreen);*/

            if (primaryDirScreenViewable &&
                secondaryDirScreenViewable &&
                diagonalDirScreenViewable)
            {
                movement = CameraMovement.FreeForm;
                m_cameraTarget = samusPos;
            }
            else if (primaryDirScreenViewable)
            {
                if (primary == Direction.Left || primary == Direction.Right)
                {
                    movement = CameraMovement.TrackHorizontal;
                    m_cameraTarget = new Vector2(
                        samusPos.X,
                        zone.getPixelPositionFromScreenPosition(screen, new Point(5318008, Zone.SCREEN_HEIGHT_IN_PIXELS / 2)).Y);
                }
                else
                {
                    movement = CameraMovement.TrackVertical;
                    m_cameraTarget = new Vector2(
                        zone.getPixelPositionFromScreenPosition(screen, new Point(Zone.SCREEN_WIDTH_IN_PIXELS / 2, 5318008)).X,
                        samusPos.Y);
                }
            }
            else if (secondaryDirScreenViewable)
            {
                if (secondary == Direction.Left || secondary == Direction.Right)
                {
                    movement = CameraMovement.TrackHorizontal;
                    m_cameraTarget = new Vector2(
                        samusPos.X,
                        zone.getPixelPositionFromScreenPosition(screen, new Point(5318008, Zone.SCREEN_HEIGHT_IN_PIXELS / 2)).Y);
                }
                else
                {
                    movement = CameraMovement.TrackVertical;
                    m_cameraTarget = new Vector2(
                        zone.getPixelPositionFromScreenPosition(screen, new Point(Zone.SCREEN_WIDTH_IN_PIXELS / 2, 5318008)).X,
                        samusPos.Y);
                }
            }
            else
            {
                movement = CameraMovement.ScreenCenter;
                m_cameraTarget = new Vector2(
                        zone.getPixelPositionFromScreenPosition(screen, new Point(Zone.SCREEN_WIDTH_IN_PIXELS / 2, 5318008)).X,
                        zone.getPixelPositionFromScreenPosition(screen, new Point(5318008, Zone.SCREEN_HEIGHT_IN_PIXELS/2)).Y);
            }

            /* Attempt using an instantaneous camera...
             */
            cam.Center = m_cameraTarget;

            /*
             * Attempt using Lerp or Slerp...
            const int maxMovementTime = 10;

            if (m_cameraTarget == cam.Center)
            {
                m_stillPosition = cam.Center;
                m_stillFrames = 0;
            }
            else if (m_stillFrames == 0)
            {
                m_stillPosition = cam.Center;
                m_stillFrames++;
            }
            else if (m_cameraTarget == m_previousCameraTarget)
            {
                m_stillFrames++;
                m_stillFrames = Math.Min(m_stillFrames, maxMovementTime);
            }
            else
            {
                m_stillFrames++;
                m_stillFrames = Math.Min(m_stillFrames, maxMovementTime / 2);
            }

            m_previousCameraTarget = m_cameraTarget;

            float t = (float)m_stillFrames / maxMovementTime;

            const float omega = 0.5f;
            Vector2 leftTerm = (float)Math.Sin((1 - t) * omega) * m_stillPosition / (float)Math.Sin(omega);
            Vector2 rightTerm = (float)Math.Sin(t * omega) * m_cameraTarget / (float)Math.Sin(omega);
            //Vector2 leftTerm = (1 - t) * m_stillPosition;
            //Vector2 rightTerm = t * m_cameraTarget;
            cam.Center = leftTerm + rightTerm;
             */

            /* Attempt to use camera acceleration and velocity...
             * 
            const float maxAccel = 15;
            Vector2 acceleration = m_cameraTarget - cam.Center;
            float accelerationMagn = acceleration.Length();
            if (accelerationMagn > 0)
            {
                acceleration.Normalize();
                acceleration = Math.Min(acceleration.Length(), maxAccel) * acceleration;

                m_cameraVelocity =
                    m_cameraVelocity +
                    acceleration;
            }

            if (m_cameraTarget.X - (cam.Center.X + m_cameraVelocity.X) < 0 &&
                m_cameraTarget.X - cam.Center.X > 0
                    ||
                m_cameraTarget.X - (cam.Center.X + m_cameraVelocity.X) > 0 &&
                m_cameraTarget.X - cam.Center.X < 0)
            {
                m_cameraVelocity.X = m_cameraTarget.X - cam.Center.X;
            }
            if (m_cameraTarget.Y - (cam.Center.Y + m_cameraVelocity.Y) < 0 &&
                m_cameraTarget.Y - cam.Center.Y > 0
                    ||
                m_cameraTarget.Y - (cam.Center.Y + m_cameraVelocity.Y) > 0 &&
                m_cameraTarget.Y - cam.Center.Y < 0)
            {
                m_cameraVelocity.Y = m_cameraTarget.Y - cam.Center.Y;
            }

            cam.Center += m_cameraVelocity;

            if (cam.Center.X == m_cameraTarget.X) m_cameraVelocity.X = 0;
            if (cam.Center.Y == m_cameraTarget.Y) m_cameraVelocity.Y = 0;
             */
        }

        private static float DistanceToSamus(Camera camera)
        {
            return (camera.Center - GameplayManager.Samus.DrawPosition).Length();
        }

        private static bool IsCameraOffScreen(Camera camera)
        {
            Vector2 topRight = new Vector2(camera.Position.X + camera.ScreenWidth, camera.Position.Y);
            Vector2 bottomRight = new Vector2(camera.Position.X + camera.ScreenWidth, camera.Position.Y + camera.ScreenHeight);
            Vector2 bottomLeft = new Vector2(camera.Position.X, camera.Position.Y + camera.ScreenHeight);

            return (!GameplayManager.ActiveZone.getIsPositionInZone(camera.Position) ||
                !GameplayManager.ActiveZone.getIsPositionInZone(topRight) ||
                !GameplayManager.ActiveZone.getIsPositionInZone(bottomRight) ||
                !GameplayManager.ActiveZone.getIsPositionInZone(bottomLeft));
        }

        public override void draw()
        {
            if (m_failedToCreate)
            {
                return;
            }

            GameplayManager.ActiveZone.draw();
            GameplayManager.drawHUD();

            if (Settings.getInstance().IsInDebugMode)
            {
                GameplayManager.ActiveZone.CollisionDetector.draw();
            }

            foreach (KeyValuePair<Vector2, Color> kvp in m_octantSampleTest)
            {
                LineDrawer.drawCross(kvp.Key, kvp.Value);
            }
        }

        public void cleanup()
        {
            GameplayManager.GameplayState = null;
            WorldManager.reset();

            m_zoneInitializationThread.Abort();
        }

        private void zoneInitializationThread()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                Zone currentZone = GameplayManager.ActiveZone;
                Point samusScreen =
                    currentZone.getGlobalScreenFromPosition(GameplayManager.Samus.DrawPosition);

                Zone closestZone = null;
                int minDist = Int32.MaxValue;

                foreach (Zone z in WorldManager.GetZones())
                {
                    if (!z.IsInitialized)
                    {

                        foreach (Point globalScreenCoord in z.PositionsOwned)
                        {
                            int dist = Math.Abs(samusScreen.X - globalScreenCoord.X) +
                                        Math.Abs(samusScreen.Y - globalScreenCoord.Y);
                            if (dist < minDist ||
                                (dist == minDist && z.PositionsOwned.Count < closestZone.PositionsOwned.Count))
                            {
                                minDist = dist;
                                closestZone = z;
                            }
                        }
                    }
                }

                if (closestZone == null)
                {
                    keepGoing = false;
                }
                else
                {
                    closestZone.Initialize();
                }
            }
        }
    }
}