using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidAI.controllers
{
    public class Inventory
    {
        protected static readonly Item[] s_defaultItems =
            new Item[] { /*Item.MorphingBall, Item.Missile, Item.IceBeam */ };

        protected bool[] m_itemArray = new bool[Enum.GetValues(typeof(Item)).Length];

        public Inventory()
        {
            foreach (Item item in s_defaultItems)
            {
                AddItem(item);
            }
        }

        public bool HasItem(Item item)
        {
            return m_itemArray[(int)item];
        }

        public void AddItem(Item item)
        {
            m_itemArray[(int)item] = true;
        }
    }

    public enum Item
    {
        MorphingBall,
        SpringBall,
        Missile,
        SuperMissile,
        IceBeam
    }
}
