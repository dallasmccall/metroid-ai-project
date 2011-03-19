using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.collision;
using Microsoft.Xna.Framework;
using CS8803AGA.engine;

namespace CS8803AGA.controllers.mission
{
    abstract class ARefillController : ACollidable
    {
        protected AnimationController m_animationController;

        public string AnimationName { get { return m_refillType + "Refill" + (IsActive ? "Active" : "Inactive"); } }
        public bool IsActive { get; protected set; }

        protected static readonly Rectangle s_refillArea = new Rectangle(-39, -74, 78, 22);

        protected string m_refillType;
        protected Rectangle m_refillArea;

        public ARefillController(Vector2 position, string refillType)
        {
            this.IsActive = true;

            m_position = position;
            m_refillType = refillType;

            m_refillArea = new Rectangle(s_refillArea.X, s_refillArea.Y, s_refillArea.Width, s_refillArea.Height);
            m_refillArea.Offset((int)m_position.X, (int)m_position.Y);

            Rectangle bounds = new Rectangle(-40, -120, 80, 120);
            m_collider = new Collider(this, bounds, ColliderType.Scenery);

            m_animationController =
                new AnimationController(
                    "Animation/Data/MissionObjects",
                    "Animation/Textures/MissionObjects",
                    AnimationName);
        }

        public override bool isAlive()
        {
            return true;
        }

        public override void update()
        {
            if (IsActive)
            {
                Vector2 gunpoint = GameplayManager.Samus.getGunpoint();
                if (gunpoint.X >= m_refillArea.Left && gunpoint.X <= m_refillArea.Right &&
                    gunpoint.Y >= m_refillArea.Top && gunpoint.Y <= m_refillArea.Bottom)
                {
                    this.IsActive = !refill(GameplayManager.Samus);
                }
            }

            m_animationController.requestAnimation(AnimationName);
            m_animationController.update();
        }

        public override void draw()
        {
            m_animationController.draw(m_position, Constants.DepthGameplayDecorationsBehind);
        }

        /// <summary>
        /// Refills a particular resource if it isn't full.
        /// </summary>
        /// <param name="samus">Samus' controller.</param>
        /// <returns>True if it refilled the resource, false if it didn't need to.</returns>
        protected abstract bool refill(PlayerController samus);
    }
}
