using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.controllers
{
    class BulletExplosion : IGameObject
    {
        const int c_lifespan = 10;

        private Vector2 m_position;
        private int m_lifeCounter = 0;

        private AnimationController m_animationController =
            new AnimationController("Animation/Data/BulletExplosion", "Animation/Textures/BulletExplosion");

        public BulletExplosion(Vector2 centerPoint) : this(centerPoint, 2.5f)
        {
            // nch
        }

        public BulletExplosion(Vector2 centerPoint, float scale)
        {
            m_position = centerPoint;
            m_animationController.DrawBottomCenter = false;
            m_animationController.Scale = scale;
        }

        #region IGameObject Members

        public bool isAlive()
        {
            return m_lifeCounter < c_lifespan;
        }

        public void update()
        {
            m_animationController.requestAnimation("Explode");

            m_animationController.update();

            m_lifeCounter++;

        }

        public void draw()
        {
            m_animationController.draw(m_position, Constants.DepthGameplayExplosions);
        }

        #endregion
    }
}
