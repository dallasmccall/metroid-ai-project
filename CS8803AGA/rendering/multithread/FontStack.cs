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
using Microsoft.Xna.Framework.Graphics;

namespace CS8803AGA
{
    /// <summary>
    /// A stack of FontDrawCommands which gets filled during the Update Thread
    /// and then rendered by the Draw Thread.
    /// </summary>
    public class FontStack
    {
        private FontDrawCommand[] m_stack;

        protected int m_top;

        protected int m_size;

        protected SpriteBatch m_spriteBatch;

        public FontStack(int size, SpriteBatch spriteBatch)
        {
            m_stack = new FontDrawCommand[size];
            m_size = size;
            m_top = -1;
            m_spriteBatch = spriteBatch;
            initializeStack();
        }

        /// <summary>
        /// Pops the top FontDrawCommand off the stack and returns it
        /// </summary>
        /// <returns>A reference to the FontDrawCommand popped off</returns>
        internal FontDrawCommand pop()
        {
            if (m_top >= 0)
            {
                m_top--;
                return m_stack[m_top + 1];
            }
            throw new Exception("Tried to pop off an empty stack!");
        }

        /// <summary>
        /// Get a reference to the top of the stack
        /// </summary>
        /// <returns>reference to the FontDrawCommand at the top of the stack</returns>
        internal FontDrawCommand top()
        {
            return m_stack[m_top];
        }

        /// <summary>
        /// Pushes a FontDrawCommand onto the FontStack to be drawn.
        /// If possible, it is better to use pushGet() for garbage reasons.
        /// </summary>
        /// <param name="drawCommand">Font draw command to be drawn.</param>
        internal void push(FontDrawCommand drawCommand)
        {
            if (m_top == m_size - 1)
            {
                resize(m_size + 1);
            }
            m_top++;
            m_stack[m_top] = drawCommand;
        }

        /// <summary>
        /// Pushes a FontDrawCommand onto the FontStack, and returns that
        /// command for editing.  This method is preferred over push() because
        /// then the FontStack can manage the creation of FontDrawCommand objects.
        /// </summary>
        /// <returns>Encapsulation of the command - edit the parameters</returns>
        internal FontDrawCommand pushGet()
        {
            if (m_top == m_size - 1)
            {
                resize(m_size + 1);
            }
            m_top++;
            m_stack[m_top].clear();
            return m_stack[m_top];
        }

        public bool hasMoreItems()
        {
            return m_top >= 0;
        }

        /// <summary>
        /// Resize the stack in a non-destructive way
        /// </summary>
        /// <param name="newSize">The new size of the stack</param>
        public void resize(int newSize)
        {
            if (newSize > m_size)
            {
                int nextSize = (newSize > m_size * 2) ? newSize : m_size * 2;
                FontDrawCommand[] tempStack = new FontDrawCommand[nextSize];
                for (int i = 0; i <= m_top; i++)
                {
                    tempStack[i] = m_stack[i];
                }
                m_stack = tempStack;
                tempStack = null;
                m_size = nextSize;
                initializeStack();
            }
        }

        /// <summary>
        /// Resize and reset the stack.  Destroys all elements and resets m_top to zero
        /// </summary>
        /// <param name="newSize">The new size of stack</param>
        public void resizeDestructively(int newSize)
        {
            m_stack = new FontDrawCommand[newSize];
            m_size = newSize;
            m_top = -1;
            initializeStack();
        }

        protected void initializeStack()
        {
            for (int i = m_top + 1; i < m_size; i++)
            {
                m_stack[i] = new FontDrawCommand(m_spriteBatch);
            }
        }
    }
}
