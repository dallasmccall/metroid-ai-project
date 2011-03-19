using Microsoft.Xna.Framework;
using CSharpQuadTree;
using System;
using System.Collections.Generic;
using MetroidAI.controllers;

namespace MetroidAI.collision
{
    public enum ColliderType
    {
        Samus,
        Enemy,
        FrozenEnemy,
        InvincibleEnemy,
        Scenery,
        Projectile,
        Effect,
        Trigger,
        Transition
    }

    /// <summary>
    /// Encapsulation of information about something which can be collided into.
    /// </summary>
    public class Collider : IQuadObject
    {
        /// <summary>
        /// CollisionDetector which is managing this collider.
        /// </summary>
        private CollisionDetector m_detector;

        /// <summary>
        /// Area the collider takes up.
        /// </summary>
        private DoubleRect m_bounds;

        /// <summary>
        /// Object whose area is represented by this collider.
        /// </summary>
        public ICollidable m_owner;

        /// <summary>
        /// Type of owning object.
        /// </summary>
        public ColliderType m_type;

        public Collider(ICollidable owner, Rectangle bounds, ColliderType type)
        {
            this.m_owner = owner;
            this.m_bounds = new DoubleRect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            this.m_type = type;
        }

        /// <summary>
        /// Find all Colliders which intersect the queried Area and are
        /// managed by this Collider's managing CollisionDetector.
        /// </summary>
        /// <param name="queryRect">Area in which to find Colliders.</param>
        /// <returns>All Colliders which intersect the queryRect.</returns>
        public List<Collider> queryDetector(Rectangle queryRect)
        {
            return queryDetector(new DoubleRect(m_bounds.X, m_bounds.Y, m_bounds.Width, m_bounds.Height));
        }

        /// <summary>
        /// Find all Colliders which intersect the queried Area and are
        /// managed by this Collider's managing CollisionDetector.
        /// </summary>
        /// <param name="queryRect">Area in which to find Colliders.</param>
        /// <returns>All Colliders which intersect the queryRect.</returns>
        public List<Collider> queryDetector(DoubleRect queryRect)
        {
            return m_detector.query(queryRect);
        }

        /// <summary>
        /// Forcible - ignores collisions.
        /// Move the Collider; informs its CollisionDetector via an event.
        /// Also moves the owning object's DrawPosition.
        /// </summary>
        /// <param name="dp">Delta of position</param>
        public void move(Vector2 dp)
        {
            m_bounds += dp;
            m_owner.DrawPosition += dp;

            RaiseBoundsChanged();
        }

        public DoubleRect Bounds
        {
            get { return m_bounds; }
            set { m_bounds = value; RaiseBoundsChanged(); }
        }

        /// <summary>
        /// Move the Collider according to what the CollisionHandler allows.
        /// </summary>
        /// <param name="dp">Delta position.</param>
        public void handleMovement(Vector2 dp)
        {
            m_detector.handleMovement(this, dp);
        }

        public void forCollisionDetectorUseOnly(CollisionDetector cd)
        {
            this.m_detector = cd;
        }

        public void unregister()
        {
            this.m_detector.remove(this);
        }

        private void RaiseBoundsChanged()
        {
            EventHandler handler = BoundsChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }

        public event System.EventHandler BoundsChanged;
    }
}