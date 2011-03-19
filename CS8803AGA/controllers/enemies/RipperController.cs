using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CS8803AGA.collision;

namespace CS8803AGA.controllers.enemies
{
    public class RipperController : EnemyController1
    {
        private bool m_facingRight = true;

        const int c_velX = 10;

        static readonly Rectangle s_bounds = new Rectangle(-30, -34, 60, 34);

        public override int MaxHealth
        {
            get { return 2; }
        }

        public RipperController(Vector2 startPos)
            : base(startPos, c_velX, s_bounds, 2.5f)
        {
            this.GravityEffect = Gravity.None;
        }

        protected override void handleSamusProjectileHit(ProjectileController projectile)
        {
            projectile.handleSceneryHit();

            if (projectile.Freezes)
            {
                this.m_frozenCounter = s_frozenDuration;
                this.m_collider.m_type = ColliderType.FrozenEnemy;

                this.m_invincibilityCounter = s_invincibilityDuration;
            }
        }

        protected override void updateEnemyInternal()
        {
            if (this.m_actualVelocity.X == 0)
            {
                m_facingRight = !m_facingRight;
            }

            string dir = m_facingRight ? "Right" : "Left";
            string anim = "Ripper" + dir;

            AnimationController.requestAnimation(anim);

            m_attemptedVelocity.X += m_facingRight ? c_velX : -c_velX;
        }
    }
}
