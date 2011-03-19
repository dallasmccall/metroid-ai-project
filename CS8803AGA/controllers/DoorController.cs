using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.collision;
using MetroidAI.world;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CSharpQuadTree;
using MetroidAI.engine;
using MetroidAI.controllers.enemies;

namespace MetroidAI.controllers
{
    class DoorController : ACollidable, IShootable
    {
        AnimationController AnimationController { get; set; }

        public string Color { get; set; }
        protected DoorState State { get; set; }

        protected enum DoorState { Open, Closed, Opening, Closing }

        const int LongEdge = 160;
        const int ShortEdge = 160;
        const int OpenLength = 40;

        const float Scale = 2.5f;

        public Direction Side { get; protected set; }

        protected Zone m_owningZone;
        protected Point m_zoneScreenCoord;

        public DoorController(Direction side, Zone zone, Point zoneScreenCoord)
        {
            AnimationController =
                new AnimationController(@"Animation/Data/Doors", @"Sprites/Doors");
            AnimationController.DrawBottomCenter = false;
            AnimationController.Scale = Scale;

            Color = "Metal";
            State = DoorState.Closed;
            Side = side;

            bool zoneWidthOdd = (Zone.SCREEN_WIDTH_IN_TILES % 2) == 1;
            bool zoneHeightOdd = (Zone.SCREEN_HEIGHT_IN_TILES % 2) == 1;

            // Initialize bounding box

            Point pos = Point.Zero;
            Rectangle bounds = Rectangle.Empty;
            if (side == Direction.Up)
            {
                pos =
                    zone.getPixelPositionFromScreenPosition(zoneScreenCoord,
                    new Point(Zone.SCREEN_WIDTH_IN_PIXELS / 2 + (zoneWidthOdd ? -Zone.TILE_WIDTH / 2 : 0),
                                  0));
                bounds = new Rectangle(
                    pos.X - LongEdge / 2,
                    pos.Y - ShortEdge / 2,
                    LongEdge,
                    ShortEdge);
            }
            if (side == Direction.Down)
            {
                pos =
                    zone.getPixelPositionFromScreenPosition(zoneScreenCoord,
                    new Point(Zone.SCREEN_WIDTH_IN_PIXELS / 2 + (zoneWidthOdd ? - Zone.TILE_WIDTH / 2 : 0),
                                    Zone.SCREEN_HEIGHT_IN_PIXELS));
                bounds = new Rectangle(
                    pos.X - LongEdge / 2,
                    pos.Y - ShortEdge / 2,
                    LongEdge,
                    ShortEdge);
            }
            if (side == Direction.Left)
            {
                pos =
                    zone.getPixelPositionFromScreenPosition(zoneScreenCoord,
                        new Point(0,
                                    Zone.SCREEN_HEIGHT_IN_PIXELS / 2 + (zoneHeightOdd ? -Zone.TILE_HEIGHT / 2 : 0)));
                bounds = new Rectangle(
                    pos.X - ShortEdge / 2,
                    pos.Y - LongEdge / 2,
                    ShortEdge,
                    LongEdge);
            }
            if (side == Direction.Right)
            {
                pos =
                    zone.getPixelPositionFromScreenPosition(zoneScreenCoord,
                        new Point(Zone.SCREEN_WIDTH_IN_PIXELS,
                                    Zone.SCREEN_HEIGHT_IN_PIXELS / 2 + (zoneHeightOdd ? -Zone.TILE_HEIGHT / 2 : 0)));
                bounds = new Rectangle(
                    pos.X - ShortEdge / 2,
                    pos.Y - LongEdge / 2,
                    ShortEdge,
                    LongEdge);
            }

            this.m_position = new Vector2(pos.X, pos.Y);
            this.m_collider = new Collider(this, bounds, ColliderType.Scenery);

            this.m_owningZone = zone;
            this.m_zoneScreenCoord = zoneScreenCoord;
        }

        protected void open()
        {
            if (Color == "Metal") return;
            State = DoorState.Opening;
        }

        public void close()
        {
            State = DoorState.Closing;
        }

        public override bool isAlive()
        {
            return true;
        }

        public override void update()
        {
            // Update AnimationController *before* checking whether an
            // animation has gone idle.  Otherwise, state may have changed and
            // it might be reporting that the previous state's anim is idle.
            string animRequest = Color + State.ToString();
            AnimationController.requestAnimation(animRequest, AnimationController.AnimationCommand.Play);
            AnimationController.update();

            if (Color == "Metal")
            {
                if (m_owningZone.GameObjects.Count(i => { return i is EnemyController && !(i is RipperController); }) == 0)
                {
                    Color = "Blue";
                }
            }

            switch (State)
            {
                case DoorState.Closed:
                    break;
                case DoorState.Closing:
                    if (AnimationController.IsIdle)
                    {
                        State = DoorState.Closed;
                    }
                    break;
                case DoorState.Open:
                    break;
                case DoorState.Opening:
                    if (AnimationController.IsIdle)
                    {
                        State = DoorState.Open;
                        setCollisionModeOpen();
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Changes a blocking collision box over the whole door to a nonblocking
        /// collision box inside the door which triggers the zone transition.
        /// </summary>
        protected void setCollisionModeOpen()
        {
            if (Side == Direction.Left || Side == Direction.Right)
            {
                m_collider.Bounds = new DoubleRect(
                    m_collider.Bounds.X + (ShortEdge - OpenLength) / 2,
                    m_collider.Bounds.Y,
                    OpenLength,
                    LongEdge);
            }
            else
            {
                m_collider.Bounds = new DoubleRect(
                    m_collider.Bounds.X,
                    m_collider.Bounds.Y + (ShortEdge - OpenLength) / 2,
                    LongEdge,
                    OpenLength);
            }

            m_collider.m_type = ColliderType.Transition;
        }

        /// <summary>
        /// Changes a zone transition collision box to a blocking scenery
        /// collision box that covers the whole door.
        /// </summary>
        protected void setCollisionModeClosed()
        {
            if (Side == Direction.Left || Side == Direction.Right)
            {
                m_collider.Bounds = new DoubleRect(
                    m_collider.Bounds.X - (ShortEdge - OpenLength) / 2,
                    m_collider.Bounds.Y,
                    ShortEdge,
                    LongEdge);
            }
            else
            {
                m_collider.Bounds = new DoubleRect(
                    m_collider.Bounds.X,
                    m_collider.Bounds.Y - (ShortEdge - OpenLength) / 2,
                    LongEdge,
                    ShortEdge);
            }

            m_collider.m_type = ColliderType.Scenery;
        }

        /// <summary>
        /// Handle projectile hit - check whether projectile can open the door.
        /// </summary>
        /// <param name="projectile"></param>
        public void handleProjectileHit(ProjectileController projectile)
        {
            // TODO check type of projectile
            open();
        }

        /// <summary>
        /// Performs door-based operations to move Samus into the adjacent zone.
        /// </summary>
        /// <param name="samus"></param>
        public void handleZoneTransition(PlayerController samus)
        {
            Point globalCoord = m_owningZone.getGlobalScreenFromLocalScreen(m_zoneScreenCoord);

            // Find target door (door in target zone which aligns with this door)
            Zone targetZone = WorldManager.GetZone(DirectionUtils.Move(globalCoord, Side));
            Point targetScreenInZoneCoord =
                targetZone.getLocalScreenFromGlobalScreen(globalCoord);
            Rectangle targetScreenArea =
                targetZone.getScreenRectangle(targetScreenInZoneCoord);
            List<Collider> colliders = targetZone.CollisionDetector.query(new DoubleRect(ref targetScreenArea));
            Collider targetDoorCollider =
                colliders.Find(i => {return (i.m_owner is DoorController && ((DoorController)i.m_owner).Side == this.Side.Opposite); });
            DoorController targetDoor = (DoorController)targetDoorCollider.m_owner;

            // Trigger closing animation on target door
            targetDoor.close();

            // Calculate Samus' new position in target zone
            DoubleRect samusBounds = samus.getCollider().Bounds;
            Vector2 dropPos = new Vector2();
            switch (Side.Opposite.EnumValue)
            {
                case DirectionEnum.Left:
                    dropPos.X = (float)(targetDoorCollider.Bounds.X + targetDoorCollider.Bounds.Width + 1 + samusBounds.Width / 2);
                    dropPos.Y = (float)(targetDoorCollider.Bounds.Y + targetDoorCollider.Bounds.Height - 1);
                    break;
                case DirectionEnum.Right:
                    dropPos.X = (float)(targetDoorCollider.Bounds.X - 1 - samusBounds.Width / 2);
                    dropPos.Y = (float)(targetDoorCollider.Bounds.Y + targetDoorCollider.Bounds.Height - 1);
                    break;
                case DirectionEnum.Up:
                    dropPos.X = (float)(targetDoorCollider.Bounds.X + targetDoorCollider.Bounds.Width / 2);
                    dropPos.Y = (float)(targetDoorCollider.Bounds.Y + targetDoorCollider.Bounds.Height + 1 + samusBounds.Height);
                    break;
                case DirectionEnum.Down:
                    dropPos.X = (float)(targetDoorCollider.Bounds.X + targetDoorCollider.Bounds.Width / 2);
                    dropPos.Y = (float)(targetDoorCollider.Bounds.Y - 1 - samusBounds.Height);
                    break;
            }

            // And create a new zone transition state with this information
            EngineManager.pushState(new EngineStateZoneTransition(
                EngineManager.Engine,
                this,
                targetDoor,
                targetZone,
                dropPos));

            setCollisionModeClosed();
        }

        public override void draw()
        {
            draw(Constants.DepthGameplayTiles);
        }

        public void draw(float depth)
        {
            float rotation = 0.0f;
            if (Side == Direction.Up || Side == Direction.Down)
            {
                rotation = (float)Math.PI / 2;
            }

            AnimationController.Rotation = rotation;

            /*DrawCommand dc = DrawBuffer.getInstance().DrawCommands.pushGet();
            dc.set(m_texture, m_openState, m_position, CoordinateTypeEnum.RELATIVE,
                Constants.DepthGameplayTiles, true, Color.White, rotation, Scale);*/
            AnimationController.draw(m_position, depth);
        }

        public void drawMap(Vector2 offset, float scale, float depth)
        {
            float rotation = 0.0f;
            if (Side == Direction.Up || Side == Direction.Down)
            {
                rotation = (float)Math.PI / 2;
            }

            DrawCommand dc = DrawBuffer.getInstance().DrawCommands.pushGet();
            dc.set(
                AnimationController.Texture,
                AnimationController.ImageDimensionsIdx,
                m_position * scale + offset,
                CoordinateTypeEnum.ABSOLUTE,
                depth,
                true,
                Microsoft.Xna.Framework.Graphics.Color.White,
                rotation,
                Scale * scale);
        }
    }
}
