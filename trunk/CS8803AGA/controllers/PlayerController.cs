using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CSharpQuadTree;
using CS8803AGA.engine;
using CS8803AGA.devices;
using CS8803AGA.collision;
using CS8803AGAGameLibrary;
using CS8803AGA.world.mission;
using Microsoft.Xna.Framework.Graphics;
using CS8803AGAGameLibrary.player;
using CS8803AGA.controllers.projectiles;
using CS8803AGA.world;

namespace CS8803AGA.controllers
{
    /// <summary>
    /// CharacterController for a player-controlled Character.
    /// Reads from inputs to update movement and animations.
    /// </summary>
    public class PlayerController : CharacterController
    {

        #region Constants

        protected const int c_defaultMaxMissile = 20;
        protected const int c_defaultMaxHealth = 1499;

        #endregion

        #region Non-Animation Properties and Fields

        public Inventory Inventory { get; set; }

        public int MissileCount { get; set; }
        public int MaxMissileCount { get; set; }
        public override int MaxHealth { get { return m_maxHealth; } }
        protected int m_maxHealth = c_defaultMaxHealth;

        public ProjectileType SelectedWeapon { get; set; }

        #endregion

        public PlayerModel model = new PlayerModel();

        public List<Point> roomsVisited = new List<Point>();

        #region Animation & State Fields

        public enum State
        {
            Idle,
            Moving,
            Turning,
            Takeoff,
            Airborne,
            Spin,
            Landing
        }
        public enum Aiming
        {
            Up, Down, None
        }
        public enum Facing
        {
            Left, Right, None
        }
        public enum Height
        {
            Stand, Crouch, Ball
        }

        private State m_previousState;
        private Aiming m_previousAiming;
        private Facing m_previousFacing;
        private Height m_previousHeight;

        private int m_baseCollisionBoxHeight;

        #endregion

        #region Core Methods

        public PlayerController(
            CharacterInfo ci,
            Vector2 startPosition)
            : base(startPosition, ci.speed, ci.collisionBox, ColliderType.Samus, ci.scale, ci.animationDataPath, ci.animationTexturePath)
        {
            this.Inventory = new Inventory();

            this.Health = this.MaxHealth;
            this.MaxMissileCount = c_defaultMaxMissile;
            this.MissileCount = this.MaxMissileCount;
            m_baseCollisionBoxHeight = ci.collisionBox.Height;

            this.model.setStat("shots", 0);
            this.model.setStat("damageTaken", 0);
            this.model.setStat("roomsVisited", 0);
            this.model.setStat("damageDone", 0);
        }

        protected override void updateInternal()
        {
            if (!roomsVisited.Contains(GameplayManager.ActiveZone.TopLeftPosition))
            {
                roomsVisited.Add(GameplayManager.ActiveZone.TopLeftPosition);
                this.model.setStat("roomsVisited", this.model.getStat("roomsVisited") + 1);
            }

            #region Gather Movement Inputs

            if (!IsAirborne && InputSet.getInstance().getButton(InputsEnum.BUTTON_1) &&
                (m_previousHeight != Height.Ball || Inventory.HasItem(Item.SpringBall)))
            {
                m_attemptedVelocity.Y -= 40;
            }

            // Debug movement, flies through stuff
            if (InputSet.getInstance().getButton(InputsEnum.BUTTON_4))
            {
                Vector2 debugDp = new Vector2();

                if (InputSet.getInstance().getLeftDirectionalX() > 0) debugDp.X = 10;
                if (InputSet.getInstance().getLeftDirectionalX() < 0) debugDp.X = -10;
                if (InputSet.getInstance().getLeftDirectionalY() > 0) debugDp.Y = -10;
                if (InputSet.getInstance().getLeftDirectionalY() < 0) debugDp.Y = 10;

                this.getCollider().move(debugDp);
                this.m_attemptedVelocity = Vector2.Zero;

                if (InputSet.getInstance().getButton(InputsEnum.BUTTON_3))
                {
                    Direction d = Direction.Up;
                    bool dirSelected = true;
                    if (InputSet.getInstance().getLeftDirectionalX() > 0) d = Direction.Right;
                    else if (InputSet.getInstance().getLeftDirectionalX() < 0) d = Direction.Left;
                    else if (InputSet.getInstance().getLeftDirectionalY() > 0) d = Direction.Up;
                    else if (InputSet.getInstance().getLeftDirectionalY() < 0) d = Direction.Down;
                    else dirSelected = false;

                    if (dirSelected)
                    {
                        InputSet.getInstance().setToggle(InputsEnum.LEFT_DIRECTIONAL);
                        GameplayManager.DebugZoneTransition(d);
                    }
                }

                return;
            }

            Facing facing = Facing.None;
            if (InputSet.getInstance().getLeftDirectionalX() > 0)
            {
                facing = Facing.Right;
            }
            else if (InputSet.getInstance().getLeftDirectionalX() < 0)
            {
                facing = Facing.Left;
            }
            Facing facingPressed = facing;

            Aiming aiming = Aiming.None;
            bool downPressed = false;
            bool upPressed = false;
            if (InputSet.getInstance().getLeftDirectionalY() > 0)
            {
                aiming = Aiming.Up;
                upPressed = true;
            }
            else if (InputSet.getInstance().getLeftDirectionalY() < 0)
            {
                aiming = Aiming.Down;
                downPressed = true;
            }

            if (InputSet.getInstance().getButton(InputsEnum.LEFT_TRIGGER) &&
                InputSet.getInstance().getButton(InputsEnum.RIGHT_TRIGGER))
            {
                aiming = Aiming.None;
            }
            else if (InputSet.getInstance().getButton(InputsEnum.LEFT_TRIGGER))
            {
                aiming = Aiming.Down;
            }
            else if (InputSet.getInstance().getButton(InputsEnum.RIGHT_TRIGGER))
            {
                aiming = Aiming.Up;
            }

            #endregion

            #region Determine States & Animation

            State nextState = m_previousState;
            Height nextHeight = m_previousHeight;

            switch (m_previousState)
            {
                case State.Idle:
                    if (m_attemptedVelocity.Y < 0 && aiming != Aiming.None)
                    {
                        nextState = State.Airborne;
                    }
                    else if (m_attemptedVelocity.Y < 0)
                    {
                        nextState = State.Takeoff;
                    }
                    else if (IsAirborne)
                    {
                        nextState = State.Airborne;
                    }
                    else if (facing != Facing.None && facing != m_previousFacing)
                    {
                        nextState = State.Turning;
                    }
                    else if (facing != Facing.None && facing == m_previousFacing && nextHeight != Height.Ball)
                    {
                        nextState = State.Moving;
                        nextHeight = Height.Stand;
                    }
                    else if (facing != Facing.None && facing == m_previousFacing && nextHeight == Height.Ball)
                    {
                        nextState = State.Moving;
                        nextHeight = Height.Ball;
                    }
                    else if (m_previousHeight == Height.Stand && downPressed)
                    {
                        nextState = State.Idle;
                        nextHeight = Height.Crouch;
                        InputSet.getInstance().setToggle(InputsEnum.LEFT_DIRECTIONAL);
                    }
                    else if (m_previousHeight == Height.Crouch && upPressed)
                    {
                        nextState = State.Idle;
                        nextHeight = Height.Stand;
                    }
                    else if (m_previousHeight == Height.Crouch && downPressed && Inventory.HasItem(Item.MorphingBall))
                    {
                        nextState = State.Idle;
                        nextHeight = Height.Ball;
                    }
                    else if (m_previousHeight == Height.Ball && upPressed)
                    {
                        nextState = State.Idle;
                        nextHeight = Height.Crouch;
                        InputSet.getInstance().setToggle(InputsEnum.LEFT_DIRECTIONAL);
                    }
                    else
                    {
                        nextState = State.Idle;
                    }
                    break;
                case State.Moving:
                    if (m_attemptedVelocity.Y < 0 && aiming == Aiming.None)
                    {
                        nextState = State.Spin;
                    }
                    else if (m_attemptedVelocity.Y < 0)
                    {
                        nextState = State.Takeoff;
                    }
                    else if (IsAirborne) // fell
                    {
                        nextState = State.Airborne;
                    }
                    else if (facing != Facing.None && facing != m_previousFacing)
                    {
                        nextState = State.Turning;
                    }
                    else if (facing != m_previousFacing)
                    {
                        nextState = State.Idle;
                    }
                    else
                    {
                        nextState = State.Moving;
                    }
                    break;
                case State.Turning:
                    nextState = State.Idle;
                    break;

                case State.Spin:

                    if (aiming != Aiming.None)
                    {
                        m_previousState = State.Airborne; // yes, it's a hack
                    }

                    // yes, this is indeed a goto - it's the C# way of supporting fall through.
                    // because it doesn't support real fall through.
                    // ...even though "break" is required at the end of each case.
                    goto case State.Airborne;

                case State.Airborne:

                    if (!InputSet.getInstance().getButton(InputsEnum.BUTTON_1))
                    {
                        m_attemptedVelocity.Y = Math.Max(0, m_attemptedVelocity.Y);
                    }

                    if (!IsAirborne && aiming == Aiming.None)
                    {
                        nextState = State.Landing;
                    }
                    else if (!IsAirborne)
                    {
                        nextState = State.Idle;
                    }
                    else
                    {
                        nextState = m_previousState; // see Spin hack
                    }
                    break;
                case State.Takeoff:
                    nextState = State.Airborne;
                    break;
                case State.Landing:
                    if (m_previousFacing == facing || facing == Facing.None)
                    {
                        nextState = State.Idle;
                        if (nextHeight != Height.Ball)
                            nextHeight = Height.Stand;
                    }
                    else
                    {
                        nextState = State.Turning;
                    }
                    break;
            }

            if (facing == Facing.None && nextState != State.Turning)
            {
                facing = m_previousFacing;
            }

            bool addHold =
                facingPressed == Facing.None &&
                ((nextState == State.Idle && upPressed) ||
                 (nextState == State.Airborne && (upPressed || downPressed)));

            string animName = String.Format("{0}{1}{2}{3}{4}",
                (nextHeight != Height.Ball) ? nextState.ToString() : "",
                (nextHeight != Height.Ball) ? facing.ToString() : "",
                (nextState != State.Takeoff && nextState != State.Landing 
                    && nextState != State.Spin && nextHeight != Height.Ball) ? aiming.ToString() : "",
                addHold ? "Hold" : "",
                (nextState == State.Idle || nextState == State.Turning || nextHeight == Height.Ball) ? nextHeight.ToString() : "" );

            try
            {
                AnimationController.requestAnimation(animName, AnimationController.AnimationCommand.Play);
                //System.Console.WriteLine(animName);
            }
            catch
            {
                System.Console.WriteLine(String.Format("Animation '{0}' not found", animName));
            }

            #endregion

            #region Handle Movement

            if (nextState == State.Moving)
            {
                if (facing == Facing.Left)
                {
                    m_attemptedVelocity.X -= m_speedMax;
                }
                else if (facing == Facing.Right)
                {
                    m_attemptedVelocity.X += m_speedMax;
                }
            }

            if ((nextState == State.Airborne || nextState == State.Spin) &&
                facingPressed != Facing.None)
            {
                if (facing == Facing.Left)
                {
                    m_attemptedVelocity.X -= m_speedMax * 2 / 3;
                }
                else if (facing == Facing.Right)
                {
                    m_attemptedVelocity.X += m_speedMax * 2 / 3;
                }
            }

            #endregion

            #region Handle Shooting
            //Fire a weapon
            if (InputSet.getInstance().getButton(InputsEnum.BUTTON_3)
                && nextState != State.Spin && nextHeight != Height.Ball)
            {
                model.setStat("shots", model.getStat("shots") + 1);

                InputSet.getInstance().setToggle(InputsEnum.BUTTON_3);

                if (this.SelectedWeapon == ProjectileType.Bullet)
                {
                    fire(animName, facing, aiming, ProjectileType.Bullet);
                }
                else if (this.SelectedWeapon == ProjectileType.Missile)
                {
                    fire(animName, facing, aiming, ProjectileType.Missile);
                }
            }

            if (InputSet.getInstance().getButton(InputsEnum.BUTTON_2))
            {
                InputSet.getInstance().setToggle(InputsEnum.BUTTON_2);
                if (this.SelectedWeapon == ProjectileType.Bullet)
                {
                    this.SelectedWeapon = ProjectileType.Missile;
                }
                else if (this.SelectedWeapon == ProjectileType.Missile)
                {
                    this.SelectedWeapon = ProjectileType.Bullet;
                }
                
            }

            #endregion

            m_previousFacing = facing;
            m_previousAiming = aiming;
            m_previousHeight = nextHeight;
            m_previousState = nextState;

            DoubleRect oldBounds = this.getCollider().Bounds;

            double heightDiff = oldBounds.Height - Math.Min(AnimationController.CurrentSprite.box.Height * AnimationController.Scale - 5, m_baseCollisionBoxHeight);

            DoubleRect newBounds = oldBounds;
            newBounds.Height -= heightDiff;
            newBounds.Y += heightDiff;

            if (this.getCollider().queryDetector(oldBounds).Count < this.getCollider().queryDetector(newBounds).Count)
            {
                newBounds = oldBounds;
            }

            this.getCollider().Bounds = newBounds;
        }

        #endregion

        #region Combat and Collision Handling

        public void fire(string animName, Facing facing, Aiming aiming, ProjectileType projType)
        {
            Vector2 shootPoint = AnimationController.getShootPoint(this.DrawPosition);
            Vector2 projDirection = Vector2.Zero;
            if (!animName.Contains("Hold"))
            {
                if (facing == Facing.Left)
                {
                    projDirection.X -= 1;
                }
                else if (facing == Facing.Right)
                {
                    projDirection.X += 1;
                }
            }
            if (aiming == Aiming.Up)
            {
                projDirection.Y -= 1;
            }
            else if (aiming == Aiming.Down)
            {
                projDirection.Y += 1;
            }

            ProjectileController projectile;
            switch (projType)
            {
                case ProjectileType.Missile:
                    if (MissileCount < 1) return;
                    MissileCount -= 1;
                    projectile = new MissileController(
                        this,
                        shootPoint,
                        m_previousVelocity,
                        projDirection);
                    break;
                case ProjectileType.Bullet:
                default:
                    projectile = new BulletController(
                        this,
                        shootPoint,
                        m_previousVelocity,
                        projDirection);
                    break;
            }
            
            GameplayManager.ActiveZone.add(projectile);
        }

        public void receiveDamage(int amount)
        {
            // TODO add armor, etc. calculations here
            this.takeDamage(amount);
        }

        public void handleProjectileHit(ProjectileController projectile)
        {
            if (projectile.Owner == this)
            {
                return;
            }

            this.receiveDamage(1);
        }

        #endregion

        #region public Helpers

        public Vector2 getGunpoint()
        {
            return AnimationController.getShootPoint(m_position);
        }

        #endregion
    }
}
