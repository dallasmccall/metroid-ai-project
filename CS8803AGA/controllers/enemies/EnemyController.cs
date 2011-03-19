using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.engine;
using Microsoft.Xna.Framework;
using MetroidAI.collision;
using Microsoft.Xna.Framework.Graphics;

namespace MetroidAI.controllers.enemies
{
    public abstract class EnemyController : CharacterController
    {
        protected static readonly int s_frozenDuration = 120;
        protected int m_frozenCounter = 0;

        public EnemyController(
            Vector2 startPosition,
            int speed,
            Rectangle bounds,
            ColliderType type,
            float animationScale,
            string animationDataPath,
            string animationTexturePath
        )
            : base(startPosition, speed, bounds, ColliderType.Enemy, animationScale, animationDataPath, animationTexturePath)
        {
            // nch
        }

        public bool IsFrozen { get { return m_frozenCounter > 0; } }

        protected override void updateStart()
        {
            base.updateStart();

            if (m_invincibilityCounter <= 0 && !IsFrozen)
            {
                m_collider.m_type = ColliderType.Enemy;
            }

            if (IsFrozen)
            {
                m_frozenCounter--;
                if (!IsFrozen)
                {
                    m_collider.m_type = ColliderType.Enemy;
                }

                if (this.AnimationController.Color == Color.White)
                {
                    this.AnimationController.Color = Color.DeepSkyBlue;
                }
            }
        }

        protected override sealed void updateInternal()
        {
            if (!IsFrozen)
            {
                updateEnemyInternal();
            }
        }

        protected override void updateEnd()
        {
            base.updateEnd();
        }

        protected abstract void updateEnemyInternal();

        protected override void takeDamage(int amount)
        {
            base.takeDamage(amount);

            if (!IsFrozen)
            {
                m_collider.m_type = ColliderType.InvincibleEnemy;
            }
        }

        public virtual void handleSamusHit(PlayerController samus)
        {
            samus.receiveDamage(9);
        }

        public void handleProjectileHit(ProjectileController projectile)
        {
            if (projectile.Owner == GameplayManager.Samus && m_invincibilityCounter <= 0)
            {
                GameplayManager.Samus.model.setStat("damageDone", GameplayManager.Samus.model.getStat("damageDone") + projectile.Damage);
                handleSamusProjectileHit(projectile);
            }
        }

        protected virtual void handleSamusProjectileHit(ProjectileController projectile)
        {
            projectile.handleSceneryHit();

            if (projectile.Freezes && this.Health <= this.MaxHealth / 2 && !this.IsFrozen)
            {
                this.m_frozenCounter = s_frozenDuration;
                this.m_collider.m_type = ColliderType.FrozenEnemy;

                this.m_invincibilityCounter = s_invincibilityDuration;
            }
            else
            {
                this.takeDamage(projectile.Damage);

                if (this.Health <= 0)
                {
                    Vector2 explosion = new Vector2(
                        m_position.X,
                        m_position.Y - (float)m_collider.Bounds.Height / 2);
                    GameplayManager.ActiveZone.add(new BulletExplosion(explosion, 8.0f));
                }
            }
        }

        public Vector2 calculateVectorToSamus()
        {
            return (GameplayManager.Samus.getCollider().Bounds.Center() - m_collider.Bounds.Center());
        }

        public float calculateDistanceToSamus()
        {
            return calculateVectorToSamus().Length();
        }
    }

    public abstract class EnemyController1 : EnemyController
    {
        public EnemyController1(
            Vector2 startPosition,
            int speed,
            Rectangle bounds,
            float animationScale
        )
            : base(startPosition, speed, bounds, ColliderType.Enemy, animationScale, "Animation/Data/Enemies", "Animation/Textures/Enemies")
        {
            // nch
        }
    }

    public abstract class EnemyController2 : EnemyController
    {
        public EnemyController2(
            Vector2 startPosition,
            int speed,
            Rectangle bounds,
            float animationScale
        )
            : base(startPosition, speed, bounds, ColliderType.Enemy, animationScale, "Animation/Data/Enemies2", "Animation/Textures/Enemies2")
        {
            // nch
        }
    }
}
