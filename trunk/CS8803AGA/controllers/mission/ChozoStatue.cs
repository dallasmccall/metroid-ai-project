using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.world;
using MetroidAI.collision;
using Microsoft.Xna.Framework;
using MetroidAI.engine;

namespace MetroidAI.controllers.mission
{
    class ChozoStatue : ACollidable, IShootable, ITrigger
    {
        protected enum State { ItemHidden, ItemRevealed, ItemTaken };

        protected Item m_item;
        protected AnimationController m_statueAnimation;
        protected AnimationController m_itemAnimation;
        protected Direction m_facing;
        protected State m_state = State.ItemHidden;

        public string StatueAnimation { get { return "Chozo" + m_facing.ToString(); } }
        public string ItemAnimation
        {
            get
            {
                switch (m_state)
                {
                    case State.ItemHidden:
                        return "ItemBox";
                    case State.ItemRevealed:
                        return m_item.ToString();
                    case State.ItemTaken:
                    default:
                        return null;
                }
            }
        }

        public ChozoStatue(Vector2 position, Item item, Direction facing)
        {
            if (facing != Direction.Left && facing != Direction.Right)
            {
                facing = Direction.Right;
            }

            m_position = position;
            m_item = item;
            m_facing = facing;
            m_statueAnimation = new AnimationController(
                "Animation/Data/MissionObjects",
                "Animation/Textures/MissionObjects",
                StatueAnimation);
            m_itemAnimation = new AnimationController(
                "Animation/Data/MissionObjects",
                "Animation/Textures/MissionObjects",
                ItemAnimation);

            Rectangle bounds;
            if (m_facing == Direction.Left)
            {
                bounds = new Rectangle(
                    (int)m_position.X - 60,
                    (int)m_position.Y - 120,
                    40,
                    40);
            }
            else
            {
                bounds = new Rectangle(
                    (int)m_position.X + 20,
                    (int)m_position.Y - 120,
                    40,
                    40);
            }
            m_collider = new Collider(this, bounds, ColliderType.Scenery);

        }

        public override bool isAlive()
        {
            return true;
        }

        public override void update()
        {
            m_statueAnimation.requestAnimation(StatueAnimation);
            m_statueAnimation.update();

            if (m_state != State.ItemTaken)
            {
                m_itemAnimation.requestAnimation(ItemAnimation);
                m_itemAnimation.update();
            }
        }

        public override void draw()
        {
            m_statueAnimation.draw(m_position, Constants.DepthGameplayDecorationsBehind);

            if (m_state != State.ItemTaken)
            {
                Vector2 itemPos = new Vector2(
                    (float)m_collider.Bounds.X + (float)m_collider.Bounds.Width / 2,
                    (float)m_collider.Bounds.Y + (float)m_collider.Bounds.Height);
               
                m_itemAnimation.draw(itemPos, Constants.DepthGameplayDecorationsBehind);
            }
        }

        #region IShootable Members

        public void handleProjectileHit(ProjectileController projectile)
        {
            m_state = State.ItemRevealed;
            m_collider.m_type = ColliderType.Trigger;
        }

        #endregion

        #region ITrigger Members

        public void handleImpact(Collider mover)
        {
            PlayerController samus = mover.m_owner as PlayerController;
            if (samus == null) return;

            samus.Inventory.AddItem(m_item);

            m_state = State.ItemTaken;
            m_collider.m_type = ColliderType.Effect;
        }

        #endregion
    }
}
