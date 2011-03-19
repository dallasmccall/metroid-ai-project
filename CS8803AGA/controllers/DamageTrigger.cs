using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidAI.collision;

namespace MetroidAI.controllers
{
    /// <summary>
    /// A Trigger which causes damage to those in it.
    /// </summary>
    class DamageTrigger : ATrigger
    {
        protected object m_damageSource;
        protected int m_damageAmt;

        public DamageTrigger(Rectangle bounds, object damageSource, int damageAmt)
            : base(bounds)
        {
            m_damageSource = damageSource;
            m_damageAmt = damageAmt;
        }

        #region ITrigger Members

        public override void handleImpact(MetroidAI.collision.Collider mover)
        {
            // don't actually care about things moving through it, just what it's touching while it's alive
        }

        #endregion

        #region IGameObject Members

        public override bool isAlive()
        {
            return false; // only last one frame
        }

        public override void update()
        {
            List<Collider> collisions = m_collider.queryDetector(m_collider.Bounds);

            foreach (Collider collider in collisions)
            {
                CharacterController cc = collider.m_owner as CharacterController;
                {
                    if (cc != null && cc != m_damageSource)
                    {
                        cc.Health -= m_damageAmt;
                    }
                }
            }
        }

        public override void draw()
        {
            // nch
        }

        #endregion
    }
}
