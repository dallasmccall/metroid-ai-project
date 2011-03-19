using CS8803AGA.controllers;
using Microsoft.Xna.Framework;

namespace CS8803AGA.collision
{
    /// <summary>
    /// Interface for all objects which should register themselves with the
    /// CollisionDetector.
    /// </summary>
    public interface ICollidable : IGameObject
    {
        /// <summary>
        /// Get the object's collider.
        /// </summary>
        /// <returns></returns>
        Collider getCollider();

        /// <summary>
        /// Where the object is drawing to the screen (usu. center).
        /// </summary>
        Vector2 DrawPosition
        {
            get;
            set;
        }
    }

    public abstract class ACollidable : ICollidable
    {
        protected Vector2 m_position;
        protected Collider m_collider;

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
                m_position = value;
            }
        }

        #endregion

        #region IGameObject Members

        public abstract bool isAlive();

        public abstract void update();

        public abstract void draw();

        #endregion
    }
}
