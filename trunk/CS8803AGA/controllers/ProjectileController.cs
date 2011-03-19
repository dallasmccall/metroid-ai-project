using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MetroidAI.engine;

namespace MetroidAI.controllers
{
    public enum ProjectileType
    {
        Bullet, Missile, Spike, Bomb
    }

    public class ProjectileController : ACollidable
    {
        public IGameObject Owner { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Color Tint { get; protected set; }

        public int Damage { get; protected set; }

        public bool Freezes { get; protected set; }

        protected bool m_isAlive = true;

        protected GameTexture m_texture;

        protected ProjectileType m_type;

        public ProjectileController(IGameObject owner, Vector2 position, Vector2 velocity, ProjectileType type, int damage, String texturePath)
        {
            Rectangle bounds = new Rectangle((int)position.X - 1, (int)position.Y - 1, 3, 3);
            m_collider = new Collider(this, bounds, ColliderType.Projectile);

            Owner = owner;
            Velocity = velocity;
            Damage = damage;
            Tint = Color.White;
            Freezes = false;

            m_position = position;
            m_type = type;

            m_texture = new GameTexture(texturePath);
        }

        public virtual void handleSceneryHit()
        {
            m_isAlive = false;

            switch (m_type)
            {
                case ProjectileType.Bullet:
                    GameplayManager.ActiveZone.add(new BulletExplosion(m_position));
                    return;
                case ProjectileType.Missile:
                    GameplayManager.ActiveZone.add(new BulletExplosion(m_position, 6.0f));
                    return;
                case ProjectileType.Spike:
                    GameplayManager.ActiveZone.add(new BulletExplosion(m_position));
                    return;
                default:
                    throw new Exception("Unknown projectile type.");
            }
        }

        public virtual void handleProjectileHit(ProjectileController projectile)
        {
            // nch, for now
        }

        protected virtual void internalUpdate()
        {
            // nch
        }

        #region ACollidable Members

        public override bool isAlive()
        {
            return m_isAlive;
        }

        public override void update()
        {
            if (!m_isAlive)
            {
                return;
            }

            internalUpdate();
            m_collider.handleMovement(Velocity);
        }

        public override void draw()
        {
            float rotation = CommonFunctions.getAngle(Velocity);

            DrawCommand dc = DrawBuffer.getInstance().DrawCommands.pushGet();
            dc.set(m_texture, 0, m_position, CoordinateTypeEnum.RELATIVE, Constants.DepthMaxGameplay, true, Tint, rotation, 2.5f);
        }

        #endregion
    }
}
