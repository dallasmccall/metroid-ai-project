using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CS8803AGA.engine;

namespace CS8803AGA.controllers.enemies
{
    public class MiniKraidController : EnemyController
    {
        private enum State
        {
            Idle, Left, Right, Charge, Open
        }

        public override int MaxHealth
        {
            get { return 10; }
        }

        #region Static and Constant

        const float c_scale = 3.5f;

        const int c_speed = 10;
        const int c_chargeSpeed = 15;
        const int c_jump = 15;
        const int c_spikeSpeed = 15;
        const int c_spikeLoadTime = 10;
        const int c_spikeReloadTimeMin = 40;
        const int c_spikeReloadTimeMax = 60;

        const String c_SpikeTexturePath = "Sprites/MiniKraidProjectile";
        
        static readonly Rectangle s_bounds =
            new Rectangle((int)(-23 * c_scale), (int)(-60 * c_scale), (int)(29 * c_scale), (int)(60 * c_scale));
        static readonly Vector2 c_Spike1Offset = new Vector2(-2, 7) * c_scale;
        static readonly Vector2 c_Spike2Offset = new Vector2(-8, 21) * c_scale;

        static GameTexture s_spikeTexture;

        static MiniKraidController()
        {
            s_spikeTexture = new GameTexture(c_SpikeTexturePath);
        }

        #endregion

        #region Data Members

        private State m_state = State.Idle;

        // decrease over time
        // at 0, fire spike, at -c_spikeReloadTime, load a new spike in
        private int[] m_spikeCounter = new int[3];
        private int[] m_spikeReloadTime = new int[3];

        private int m_stateCounter = 0;

        private bool m_facingRight = false;

        #endregion

        public MiniKraidController(Vector2 startPos)
            : base(startPos, c_speed, s_bounds, ColliderType.Enemy, c_scale, "Animation/Data/MiniKraid", "Animation/Textures/MiniKraid")
        {
            for (int i = 0; i < 3; ++i)
            {
                m_spikeReloadTime[i] = RandomManager.get().Next(c_spikeReloadTimeMin, c_spikeReloadTimeMax);
            }
        }

        protected override void handleSamusProjectileHit(ProjectileController projectile)
        {
            this.takeDamage(projectile.Damage);

            if (this.Health % 3 == 1)
            {
                changeState(State.Open);
            }
        }

        protected override void updateEnemyInternal()
        {
            if (m_state != State.Charge)
            {
                if (base.calculateVectorToSamus().X < 0)
                {
                    m_facingRight = false;
                }
                else
                {
                    m_facingRight = true;
                }
            }

            for (int i = 0; i < m_spikeCounter.Length; ++i)
            {
                m_spikeCounter[i]--;

                if (m_spikeCounter[i] == 0)
                {
                    fireSpike(i);
                }
                else if (m_spikeCounter[i] == -m_spikeReloadTime[i])
                {
                    loadSpike(i);
                }
            }

            string animName = "Idle";
            switch (m_state)
            {
                case State.Idle:
                    animName = "Idle";
                    if (m_stateCounter > 45) changeState(State.Left);
                    break;
                case State.Left:
                    animName = "Move";
                    m_attemptedVelocity.X -= c_speed;
                    if (m_stateCounter > 1 && m_previousVelocity.X == 0 && m_previousVelocity.Y >= 0) m_attemptedVelocity.Y -= c_jump;
                    if (m_stateCounter > 20) changeState(State.Right);
                    break;
                case State.Right:
                    animName = "Move";
                    m_attemptedVelocity.X += c_speed;
                    if (m_stateCounter > 1 && m_previousVelocity.X == 0 && m_previousVelocity.Y >= 0) m_attemptedVelocity.Y -= c_jump;
                    if (m_stateCounter > 20) changeState(State.Idle);
                    break;
                case State.Open:
                    animName = "Open";
                    if (m_stateCounter > 15) changeState(State.Charge);
                    break;
                case State.Charge:
                    animName = "Open";
                    m_attemptedVelocity.X += m_facingRight ? c_chargeSpeed : -c_chargeSpeed;
                    if (m_stateCounter > 2 && m_previousVelocity.X == 0 && m_previousVelocity.Y >= 0) m_attemptedVelocity.Y -= c_jump;
                    if (m_stateCounter > 20) changeState(State.Idle);
                    break;
            }

            m_stateCounter++;

            string animDir = m_facingRight ? "Right" : "Left";
            AnimationController.requestAnimation(animDir + animName);
        }

        private void changeState(State newState)
        {
            m_state = newState;
            m_stateCounter = 0;

            if (m_state == State.Charge)
            {
                this.m_speedMax = c_chargeSpeed;
            }
            else
            {
                this.m_speedMax = c_speed;
            }
        }

        private void loadSpike(int spikeIdx)
        {
            m_spikeCounter[spikeIdx] = c_spikeLoadTime;

            m_spikeReloadTime[spikeIdx] =
                RandomManager.get().Next(c_spikeReloadTimeMin, c_spikeReloadTimeMax);
        }

        private void fireSpike(int spikeIdx)
        {
            Vector2 shootPoint = AnimationController.getShootPoint(this.DrawPosition);

            shootPoint += getSpikeOffset(spikeIdx);

            ProjectileController spike = new ProjectileController(
                this,
                shootPoint,
                new Vector2(m_facingRight ? c_spikeSpeed : -c_spikeSpeed, 0),
                ProjectileType.Spike,
                30,
                c_SpikeTexturePath);

            GameplayManager.ActiveZone.add(spike);
        }

        public Vector2 getSpikeOffset(int spikeIdx)
        {
            switch (spikeIdx)
            {
                case 0:
                    return Vector2.Zero;
                case 1:
                    if (m_facingRight)
                    {
                        return new Vector2(-c_Spike1Offset.X, c_Spike1Offset.Y);
                    }
                    else
                    {
                        return c_Spike1Offset;
                    }
                case 2:
                    if (m_facingRight)
                    {
                        return new Vector2(-c_Spike2Offset.X, c_Spike2Offset.Y);
                    }
                    else
                    {
                        return c_Spike2Offset;
                    }
            }
            return Vector2.Zero; // make compiler happy
        }

        public override void draw()
        {
            base.draw();

            for (int i = 0; i < m_spikeCounter.Length; ++i)
            {
                if (m_spikeCounter[i] > 0)
                {
                    Vector2 shootPoint = AnimationController.getShootPoint(this.DrawPosition);


                    shootPoint += getSpikeOffset(i);

                    DrawCommand dc = DrawBuffer.getInstance().DrawCommands.pushGet();
                    dc.set(s_spikeTexture,
                        0,
                        shootPoint,
                        CoordinateTypeEnum.RELATIVE,
                        Constants.DepthBehindCharacters,
                        true,
                        Color.White,
                        m_facingRight ? 0 : (float)Math.PI,
                        c_scale);
                }
            }
        }
    }
}
