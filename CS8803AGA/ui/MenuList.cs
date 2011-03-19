/*
 ***************************************************************************
 * Copyright notice removed by a creator for anonymity, please don't sue   *
 *                                                                         *
 * Licensed under the Apache License, Version 2.0 (the "License");         *
 * you may not use this file except in compliance with the License.        *
 * You may obtain a copy of the License at                                 *
 *                                                                         *
 * http://www.apache.org/licenses/LICENSE-2.0                              *
 *                                                                         *
 * Unless required by applicable law or agreed to in writing, software     *
 * distributed under the License is distributed on an "AS IS" BASIS,       *
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.*
 * See the License for the specific language governing permissions and     *
 * limitations under the License.                                          *
 ***************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS8803AGA.ui
{
    /// <summary>
    /// Creates a Menu made of different items, 
    /// holds a cursor position for item selection
    /// highlights a selected item
    /// </summary>
    public class MenuList
    {
        #region Default Values

        readonly Color DEFAULT_BASE_COLOR = Color.Green;
        readonly Color DEFAULT_SELECTED_COLOR = Color.White;
        const float DEFAULT_SPACING = 40.0f;
        const FontEnum DEFAULT_FONT = FontEnum.Kootenay14;
        const float DEFAULT_ROTATION = 0.0f;
        const float DEFAULT_SCALE = 1.0f;
        const float DEFAULT_DEPTH = Constants.DepthMainMenuText;
        const SpriteEffects DEFAULT_SPRITE_EFFECTS = SpriteEffects.None;
        const float DEFAULT_SPACE_AVAILABLE = 400.0f;

        #endregion

        #region Properties

        /// <summary>
        /// List of items to display in the MenuList, each on its own line.
        /// </summary>
        public List<string> StringList { get; private set; }

        /// <summary>
        /// Center-top position of the first menu item.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Color used to display unselected menu items.
        /// </summary>
        public Color BaseColor { get; set; }

        /// <summary>
        /// Color used to display the selected menu item.
        /// </summary>
        public Color SelectedColor { get; set; }

        /// <summary>
        /// Gets or sets the currently selected item index.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return m_selectedIndex;
            }
            set
            {
                while (value < m_selectedIndex)
                {
                    selectNextItem();
                }
                while (value > m_selectedIndex)
                {
                    selectPreviousItem();
                }
            }
        }
        protected int m_selectedIndex;

        /// <summary>
        /// Gets the string of the currently selected item.
        /// </summary>
        public string SelectedString
        {
            get { return StringList[m_selectedIndex]; }
        }

        /// <summary>
        /// Untested - Rotation of items
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Scale of displayed text with respect to original font; usually
        /// you should keep this the same or it might look wonky
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// SpriteEffects for the text
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; }

        /// <summary>
        /// Render depth of the menu text
        /// </summary>
        public float DrawDepth { get; set; }

        /// <summary>
        /// Space between each item in the menu
        /// </summary>
        public float ItemSpacing { get; set; }

        /// <summary>
        /// Height of area available on the screen for the menu; items outside
        /// of this height are hidden until a closer item is selected.
        /// </summary>
        public float SpaceAvailable { get; set; }

        /// <summary>
        /// Font used to display menu items
        /// </summary>
        public FontEnum Font
        {
            set
            {
                m_font = FontMap.getInstance().getFont(value);
            }
        }
        protected GameFont m_font;

        #endregion

        #region Helper Fields and Functions

        protected int m_visibleBase { get; set; }

        protected void setDefaults()
        {
            this.m_selectedIndex = 0;
            this.BaseColor = DEFAULT_BASE_COLOR;
            this.SelectedColor = DEFAULT_SELECTED_COLOR;
            this.Scale = DEFAULT_SCALE;
            this.ItemSpacing = DEFAULT_SPACING;
            this.SpriteEffects = DEFAULT_SPRITE_EFFECTS;
            this.DrawDepth = DEFAULT_DEPTH;
            this.Font = DEFAULT_FONT;
            this.SpaceAvailable = DEFAULT_SPACE_AVAILABLE;
        }

        #endregion

        /// <summary>
        /// Create a menu list with the provided strings.
        /// </summary>
        /// <param name="stringList">One string for each menu item.</param>
        /// <param name="position">Point on screen which will be attached to center of top menu item.</param>
        public MenuList(List<string> stringList, Vector2 position)
        {
            setDefaults();
            this.StringList = stringList;
            this.Position = position;
        }

        /// <summary>
        /// Draws each item in the menu ItemSpacing pixels apart from each other
        /// Currently center justifies each item in the menu
        /// </summary>
        public void draw()
        {
            int listLength = StringList.Count;
            Vector2 curPos = Position;
            Color myColor;
            for (int i = 0; i < listLength; i++)
            {
                if (i == m_selectedIndex)
                {
                    myColor = SelectedColor;
                }
                else
                {
                    myColor = BaseColor;
                }
                int itemsVisible = (int)SpaceAvailable / (int)ItemSpacing;
                if (itemsVisible >= StringList.Count)
                { itemsVisible = StringList.Count + 1; }

                if (i >= m_visibleBase && ((i < m_visibleBase + itemsVisible)))
                {
                    string curString = StringList[i];
                    m_font.drawStringCentered(curString,
                                          curPos,
                                          myColor,
                                          Rotation,
                                          Scale,
                                          SpriteEffects,
                                          DrawDepth);
                    curPos.Y = curPos.Y + ItemSpacing;
                }

            }
        }

        /// <summary>
        /// Increments the selected item, with wraparound
        /// </summary>
        public void selectNextItem()
        {
            int itemsVisible = (int)SpaceAvailable / (int)ItemSpacing;
            //int itemsVisible = 2;
            if (itemsVisible >= StringList.Count)
            { itemsVisible = StringList.Count + 1; }
            if (m_selectedIndex < StringList.Count - 1)
            {
                m_selectedIndex++;
                if (((m_selectedIndex + m_visibleBase) + 1 >= itemsVisible))
                {
                    m_visibleBase++;
                }

            }
            else
            {
                m_selectedIndex = 0;

                m_visibleBase = 0;
            }
        }

        /// <summary>
        /// Decrements the selected item, with wraparound
        /// </summary>
        public void selectPreviousItem()
        {
            if (m_selectedIndex != 0)
            {
                if (m_visibleBase != 0)
                {
                    m_visibleBase--;
                }
                m_selectedIndex--;
            }
            else
            {
                m_selectedIndex = StringList.Count - 1;
                int itemsVisible = (int)SpaceAvailable / (int)ItemSpacing;
                //int itemsVisible = 3;
                if (itemsVisible >= StringList.Count)
                {
                }
                else
                {
                    m_visibleBase = m_selectedIndex - itemsVisible + 1;
                }
            }
        }

        /// <summary>
        /// Changes a string in the menu list
        /// </summary>
        /// <param name="index">Index of string to replace</param>
        /// <param name="replacement">New string</param>
        public void setString(int index, string replacement)
        {
            StringList[index] = replacement;
        }

    }
}
