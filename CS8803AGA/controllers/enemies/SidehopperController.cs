using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.world;
using Microsoft.Xna.Framework;
using MetroidAI.engine;

namespace MetroidAI.controllers.enemies
{
    public class SidehopperController : EnemyController2
    {
        private enum State
        {
            Waiting, PreparingToJump, Jumping, Landing
        }

        public override int MaxHealth
        {
            get
            {
                return c_health;
            }
        }

        const int c_health = 2;
        const int c_jumpX = 20;
        const int c_jumpY = -20;

        private State m_state = State.Waiting;
        private int m_stateCounter = 0;
        private float m_movementX;

        static readonly Rectangle s_bounds = new Rectangle(-60, -80, 120, 80);

        public SidehopperController(Vector2 startPos)
            : base(startPos, c_jumpX, s_bounds, 2.5f)
        {
            this.GravityEffect = Gravity.Double;
        }

        protected override void updateEnemyInternal()
        {
            State previousState = m_state;

            string requestedAnimation = "";
            switch (m_state)
            {
                case State.Waiting:
                    requestedAnimation = "SidehopperIdle";
                    if (m_stateCounter > RandomManager.get().Next(20, 30))
                    {
                        m_state = State.PreparingToJump;
                    }
                    break;
                case State.PreparingToJump:
                    requestedAnimation = "SidehopperCrouch";
                    if (m_stateCounter > 10)
                    {
                        m_state = State.Jumping;

                        int jumpX = c_jumpX + (int)(RandomManager.get().NextDouble() * .2 * c_jumpX - .1 * c_jumpX);

                        Vector2 vectorToSamus = calculateVectorToSamus();
                        if (vectorToSamus.Length() < Zone.SCREEN_HEIGHT_IN_PIXELS)
                        {
                            m_attemptedVelocity.X = (vectorToSamus.X > 0) ? jumpX : -jumpX;
                            m_attemptedVelocity.Y = c_jumpY;

                            m_movementX = m_attemptedVelocity.X;
                        }
                        else
                        {
                            m_attemptedVelocity.X = (RandomManager.get().NextDouble() > 0.5) ? -jumpX : jumpX;
                            m_attemptedVelocity.Y = c_jumpY;

                            m_movementX = m_attemptedVelocity.X;
                        }
                    }
                    break;
                case State.Jumping:
                    requestedAnimation = "SidehopperJump";
                    if (!IsAirborne)
                    {
                        m_state = State.Landing;
                    }
                    else
                    {
                        m_attemptedVelocity.X = m_movementX;
                    }
                    break;
                case State.Landing:
                    requestedAnimation = "SidehopperCrouch";
                    if (m_stateCounter > 3)
                    {
                        m_state = State.Waiting;
                    }
                    break;
            }

            if (m_state == previousState)
            {
                m_stateCounter++;
            }
            else
            {
                m_stateCounter = 0;
            }

            AnimationController.requestAnimation(requestedAnimation);
        }
    }
}
