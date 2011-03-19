using Microsoft.Xna.Framework;
using System;
using CS8803AGAGameLibrary;
using CS8803AGA.collision;
using CS8803AGA.engine;
using CS8803AGA.actions;
using Microsoft.Xna.Framework.Graphics;
using CS8803AGA.controllers.enemies;

namespace CS8803AGA.controllers
{
    public abstract class CharacterController : ICollidable
    {
        public enum Gravity { Normal, Reverse, None, Double, DoubleReverse }

        public virtual int Health { get; set; }
        public abstract int MaxHealth { get; }

        public AnimationController AnimationController { get; protected set; }
        public Vector2 Velocity { get { return m_attemptedVelocity; } set { m_attemptedVelocity = value; } }
        public Vector2 PreviousPosition { get { return m_previousPosition; } set { m_previousPosition = value; } }
        public Gravity GravityEffect { get; set; }
        public bool IsAirborne { get; set; }

        protected Vector2 m_previousPosition; // position last frame before movement handled
        protected Vector2 m_position; // position last frame after movement handled
        protected Vector2 m_previousVelocity; // change in velocity from two frames ago
        protected Vector2 m_actualVelocity; // actual change in velocity from last frame
        protected Vector2 m_attemptedVelocity = Vector2.Zero; // what we will attempt to move this frame
        protected Vector2 m_acceleration;

        protected bool hitHeadLastFrame = false;
        protected bool hitFootLastFrame = false;

        protected Collider m_collider;
        protected int m_speedMax;

        protected int m_invincibilityCounter = 0;
        protected static readonly int s_invincibilityDuration = 30;

        protected float m_previousAngle;

        /// <summary>
        /// Factory method to create CharacterControllers
        /// </summary>
        /// <param name="ci">Information about character apperance and stats</param>
        /// <param name="startpos">Where in the Area the character should be placed</param>
        /// <param name="playerControlled">True if the character should be a PC, false if NPC</param>
        /// <returns>Constructed CharacterController</returns>
        public static CharacterController construct(CharacterInfo ci, Vector2 startpos, bool playerControlled)
        {
            CharacterController cc;
            ColliderType type;
            if (playerControlled)
            {
                cc = new PlayerController(ci, startpos);
                type = ColliderType.Samus;
            }
            else
            {
                throw new Exception("CharacterController:construct unimplemented code reached");
            }

            return cc;
        }

        /// <summary>
        /// Factory method to construct non-player character controllers
        /// See other construct() method for more details
        /// </summary>
        public static CharacterController construct(CharacterInfo ci, Vector2 startpos)
        {
            return construct(ci, startpos, false);
        }

        /// <summary>
        /// Protected ctor - use the construct() method
        /// </summary>
        protected CharacterController(
            Vector2 startPosition,
            int speed,
            Rectangle bounds,
            ColliderType type,
            float animationScale,
            string animationDataPath,
            string animationTexturePath)
        {
            this.m_previousPosition = startPosition;
            this.m_position = startPosition;
            this.m_speedMax = speed;

            bounds.Offset((int)m_position.X, (int)m_position.Y);
            this.m_collider = new Collider(this, bounds, type);

            this.AnimationController = new AnimationController(animationDataPath, animationTexturePath);
            this.AnimationController.ActionTriggered += new ActionTriggeredEventHandler(this.handleAction);
            this.AnimationController.Scale = animationScale;
            
            this.GravityEffect = Gravity.Normal;
            this.Health = this.MaxHealth;
            this.m_previousAngle = (float)Math.PI / 2;
        }

        public void update()
        {
            updateStart();

            updateInternal();

            updateEnd();
        }

        protected abstract void updateInternal();

        protected virtual void updateStart()
        {
            this.AnimationController.Color = Color.White;

            m_invincibilityCounter--;
            if (m_invincibilityCounter < 0)
            {
                m_invincibilityCounter = 0;
            }
            if (m_invincibilityCounter > 0 && m_invincibilityCounter % 2 == 1)
            {
                this.AnimationController.Color = new Color(0, 0, 0, 0);
            }

            // actual velocity is change in position between this frame and last frame
            m_actualVelocity = m_position - m_previousPosition;

            // acceleration is how much we've *actually* changed in velocity over past two frames
            m_acceleration = m_actualVelocity - m_previousVelocity;

            // an object in motion stays in motion, so we'll try to apply same velocity this frame
            m_attemptedVelocity = m_actualVelocity;

            hitHeadLastFrame = (m_previousVelocity.Y < 0 && m_attemptedVelocity.Y >= 0);
            hitFootLastFrame = (m_previousVelocity.Y > 0 && m_attemptedVelocity.Y <= 0);

            IsAirborne = (Math.Abs(m_acceleration.Y) > 0.0001f);
            //IsAirborne = (!IsAirborne && Math.Abs(m_acceleration.Y) > 0.0001f) ||
            //              (IsAirborne && !(Math.Abs(m_acceleration.Y) > 0.0001f));

            const float GravityDV = 1.0f;
            switch (GravityEffect)
            {
                case Gravity.Normal:
                    m_attemptedVelocity.Y = this.Velocity.Y + GravityDV; break;
                case Gravity.Reverse:
                    m_attemptedVelocity.Y = this.Velocity.Y + -GravityDV; break;
                case Gravity.Double:
                    m_attemptedVelocity.Y = this.Velocity.Y + 2 * GravityDV; break;
                case Gravity.DoubleReverse:
                    m_attemptedVelocity.Y = this.Velocity.Y + 2 * -GravityDV; break;
                case Gravity.None:
                    break;
            }

            const float Friction = 10.0f;
            if (this.Velocity.X > 0)
            {
                m_attemptedVelocity.X = m_attemptedVelocity.X - Math.Min(m_attemptedVelocity.X, Friction);
                m_attemptedVelocity.X = Math.Min(m_attemptedVelocity.X, m_speedMax);
            }
            else if (this.Velocity.X < 0)
            {
                m_attemptedVelocity.X = m_attemptedVelocity.X + Math.Max(m_attemptedVelocity.X, Friction);
                m_attemptedVelocity.X = Math.Max(m_attemptedVelocity.X, -m_speedMax);
            }
        }

        protected virtual void updateEnd()
        {
            AnimationController.update();

            // yes, do this *before* handling movement
            // if gravity doesn't have effect, we want to be able to catch this
            //  as the fact that we're on ground
            m_previousPosition = this.DrawPosition;

            m_previousVelocity = m_actualVelocity;
            //m_collider.handleMovement(this.Velocity);
            m_collider.handleMovement(new Vector2(this.Velocity.X, 0));
            m_collider.handleMovement(new Vector2(0, this.Velocity.Y));
        }

        protected virtual void takeDamage(int amount)
        {
            if (m_invincibilityCounter > 0) return;
            if (this is PlayerController)
            {
                ((PlayerController)this).model.setStat("damageTaken", ((PlayerController)this).model.getStat("damageTaken") + amount);
            }
            
            this.Health -= amount;
            this.AnimationController.Color = Color.Red;
            this.m_invincibilityCounter = s_invincibilityDuration;
        }

        public virtual void draw()
        {
            AnimationController.draw(m_position, Constants.DepthGameplayCharacters);
        }

        /// <summary>
        /// Converts an angle in radians to a direction left,right,up,down
        /// </summary>
        /// <param name="angle">Angle in radians, where 0 = right</param>
        /// <returns>Left, right, up, or down</returns>
        protected virtual string angleTo4WayAnimation(float angle)
        {
            angle += MathHelper.PiOver4;
            angle += MathHelper.Pi;
            if (angle > MathHelper.TwoPi) angle -= MathHelper.TwoPi;
            angle *= 4.00f / MathHelper.TwoPi;
            angle -= 0.001f;
            int angleInt = (int)angle;
            switch (angleInt)
            {
                case 0: return "left";
                case 1: return "down";
                case 2: return "right";
                case 3: return "up";
                default: throw new Exception("Math is wrong");
            }
        }

        /// <summary>
        /// Converts an angle in radians to an 8-way direction
        /// </summary>
        /// <param name="angle">Angle in radians, where 0 = right</param>
        /// <returns>Left, right, up, down, upleft, upright, downleft, or downright</returns>
        protected virtual string angleTo8WayAnimation(float angle)
        {
            // complicated.. essentially takes angle and maps to 8 directions
            angle += MathHelper.PiOver4 / 2; // adjust so ranges don't wrap around -pi
            angle += MathHelper.Pi; // shift ranges to 0-TwoPi
            if (angle > MathHelper.TwoPi) angle -= MathHelper.TwoPi; // fix edge case
            angle *= 8.00f / MathHelper.TwoPi;
            angle -= 0.001f;
            int angleInt = (int)angle;
            switch (angleInt)
            {
                case 0: return "left";
                case 1: return "downleft";
                case 2: return "down";
                case 3: return "downright";
                case 4: return "right";
                case 5: return "upright";
                case 6: return "up";
                case 7: return "upleft";
                default: throw new Exception("Math is wrong");
            }
        }

        /// <summary>
        /// Handles actions passed to it from its Animation Controller
        /// </summary>
        /// <param name="sender">Object sending the action, probably AnimationController</param>
        /// <param name="action">The action itself</param>
        protected void handleAction(object sender, IAction action)
        {
            action.execute(this);
        }

        #region Collider Members

        public Collider getCollider()
        {
            return m_collider;
        }

        public Vector2 DrawPosition
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        #endregion

        #region IGameObject Members

        public bool isAlive()
        {
            return Health > 0;
        }

        #endregion
    }
}