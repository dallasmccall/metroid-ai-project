using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CS8803AGA.controllers;
using CS8803AGA.world;

namespace CS8803AGA.engine
{
    /// <summary>
    /// Static class for quickly accessing active gameplay information.
    /// Probably pure evil, but very handy.
    /// </summary>
    public class GameplayManager
    {
        public static EngineStateGameplay GameplayState { get; set; }

        public static PlayerController Samus { get { return playerController; } }
        private static PlayerController playerController = null;

        public static Zone ActiveZone { get; set; }
        private static GameFont HUDFont = FontMap.getInstance().getFont(FontEnum.Consolas16);
        private static GameTexture FullEnergyTank = new GameTexture("Sprites/EnergyFull");
        private static GameTexture EmptyEnergyTank = new GameTexture("Sprites/EnergyEmpty");
        private static GameTexture MissileSelected = new GameTexture("Sprites/MissileSelected");
        private static GameTexture MissileDeselected = new GameTexture("Sprites/MissileDeselected");

        public static void initialize(EngineStateGameplay esg, PlayerController pc, Zone startZone)
        {
            GameplayState = esg;
            playerController = pc;
            ActiveZone = startZone;

            ActiveZone.add(pc);
        }

        public static void drawHUD()
        {
            drawHUDWeapons();
            drawHUDTanks();
            string SamusHealth = String.Format("ENERGY      {0:00}     {1:000}", (Samus.Health % 100), Samus.MissileCount);
            //Draw Health & Ammunition Strings
            HUDFont.drawString(SamusHealth, new Vector2(10, 50), Color.White);

            string modelStuff = String.Format(  "Shots Fired     {0} Damage Taken   {1} Damage Done    {2}", Samus.model.getStat("shots"), Samus.model.getStat("damageTaken"), Samus.model.getStat("damageDone"));
            HUDFont.drawString(modelStuff, new Vector2(10, 80), Color.White);
            string roomsVisited = String.Format("Rooms Visited   {0}", Samus.model.getStat("roomsVisited"));
            HUDFont.drawString(roomsVisited, new Vector2(10, 110), Color.White);
        }

        private static void drawHUDWeapons()
        {
            //This needs to be a little fancier once we add in super missiles, power bombs, etc.
            Vector2 iconPosition = new Vector2(255, 30);
            DrawCommand dc = DrawBuffer.getInstance().DrawCommands.pushGet();

            if (Samus.SelectedWeapon == ProjectileType.Missile)
            {
                dc.set(MissileSelected, 0, iconPosition, CoordinateTypeEnum.ABSOLUTE,
                       Constants.DepthHUD, true, Color.White, 0, 2.5f);
            }
            else
            {
                dc.set(MissileDeselected, 0, iconPosition, CoordinateTypeEnum.ABSOLUTE,
                       Constants.DepthHUD, true, Color.White, 0, 2.5f);
            }
        }

        private static void drawHUDTanks()
        {
            int health = Samus.Health;
            Vector2 tankPosition = new Vector2(20, 42);
            // Each tank has a max of 100 units to fill, so number of tanks to draw is maxhealth/100
            for (int i = 0; i < (Samus.MaxHealth / 100); i++)
            {
                DrawCommand dc = DrawBuffer.getInstance().DrawCommands.pushGet();
                // If you have at least 100 units of health left, you can fill another tank
                if (health > 99)
                {
                    dc.set(FullEnergyTank, 0, tankPosition, CoordinateTypeEnum.ABSOLUTE,
                       Constants.DepthHUD, true, Color.White, 0, 1.5f);
                    health -= 100;
                }
                // Otherwise, just fill in with an empty tank
                else if (health <= 99)
                {
                    dc.set(EmptyEnergyTank, 0, tankPosition, CoordinateTypeEnum.ABSOLUTE,
                       Constants.DepthHUD, true, Color.White, 0, 1.5f);
                    
                }
                tankPosition.X += 24;
                if (i == 6)
                {
                    tankPosition.X = 20;
                    tankPosition.Y = 20;
                }
            }
        }

        internal static void DebugZoneTransition(Direction dir)
        {
            Point globalScreenCoord = ActiveZone.getGlobalScreenFromPosition(Samus.DrawPosition);
            Point newGlobalScreen = dir.Move(globalScreenCoord);

            if (ActiveZone.PositionsOwned.Contains(newGlobalScreen))
            {
                Vector2 newPos = ActiveZone.getLocalScreenOffsetInPixelVector(
                    ActiveZone.getLocalScreenFromGlobalScreen(newGlobalScreen)) +
                    new Vector2(Zone.SCREEN_WIDTH_IN_PIXELS / 2, Zone.SCREEN_HEIGHT_IN_PIXELS / 2);
                Samus.getCollider().move(newPos - Samus.DrawPosition);
            }
            else
            {
                Zone departingZone = GameplayManager.ActiveZone;
                Zone arrivingZone = WorldManager.GetZone(newGlobalScreen);

                if (arrivingZone == null) return;

                arrivingZone.Initialize();

                Vector2 newPos = arrivingZone.getLocalScreenOffsetInPixelVector(
                    arrivingZone.getLocalScreenFromGlobalScreen(newGlobalScreen)) +
                    new Vector2(Zone.SCREEN_WIDTH_IN_PIXELS / 2, Zone.SCREEN_HEIGHT_IN_PIXELS / 2);

                departingZone.remove(Samus);
                Samus.getCollider().move(-Samus.DrawPosition); // move Samus to 0,0
                Samus.getCollider().move(newPos); // then to target position
                Samus.PreviousPosition = Samus.DrawPosition;

                arrivingZone.HasBeenVisited = true;
                arrivingZone.add(GameplayManager.Samus);

                GameplayManager.ActiveZone = arrivingZone;

                GlobalHelper.getInstance().Camera.Position =
                    arrivingZone.getLocalScreenOffsetInPixelVector(
                        arrivingZone.getLocalScreenCoordFromPosition(GameplayManager.Samus.DrawPosition));
            }
        }
    }
}
