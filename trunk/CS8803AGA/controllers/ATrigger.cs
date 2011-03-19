using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.collision;
using Microsoft.Xna.Framework;

namespace CS8803AGA.controllers
{
    /// <summary>
    /// A Trigger is a Collidable which can be moved through but causes
    /// something to happen.  It's up to the Trigger whether it polls the
    /// CollisionDetector each frame or works via the CollisionHandler.
    /// </summary>
    public abstract class ATrigger : ITrigger, ICollidable
    {
        public ATrigger(Rectangle bounds)
        {
            m_collider = new Collider(this, bounds, ColliderType.Trigger);
            m_position = new Vector2(m_collider.Bounds.Center().X, m_collider.Bounds.Center().Y);
        }

        protected Collider m_collider;
        protected Vector2 m_position;

        #region ICollidable Members

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
                this.m_position = value;
            }
        }

        #endregion

        #region ITrigger Members

        public abstract void handleImpact(Collider mover);

        #endregion

        #region IGameObject Members

        public abstract bool isAlive();

        public abstract void update();

        public abstract void draw();

        #endregion
    }
}
