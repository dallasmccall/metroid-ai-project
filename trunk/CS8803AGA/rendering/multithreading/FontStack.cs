/*
***************************************************************************
* Copyright 2009 Eric Barnes, Ken Hartsook, Andrew Pitman, & Jared Segal  *
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

namespace QuestAdaptation
{
    public class FontStack
    {
        private FontDrawer[] stack_;

        protected int top_;

        protected int size_;

        protected SpriteBatch spriteBatch_;

        public FontStack(int size, SpriteBatch spriteBatch)
        {
            stack_ = new FontDrawer[size];
            size_ = size;
            top_ = -1;
            spriteBatch_ = spriteBatch;
            initializeStack();
        }

        /// <summary>
        /// Pops the top FontDrawer off the stack and returns it
        /// </summary>
        /// <returns>A reference to the FontDrawer popped off</returns>
        internal FontDrawer pop()
        {
            if (top_ >= 0)
            {
                top_--;
                return stack_[top_ + 1];
            }
            throw new Exception("Tried to pop off an empty stack!");
        }

        /// <summary>
        /// Get a reference to the top of the stack
        /// </summary>
        /// <returns>reference to the FontDrawer at the top of the stack</returns>
        internal FontDrawer top()
        {
            return stack_[top_];
        }

        /// <summary>
        /// Puts the next FontDrawer (whose values are set using getNext()) at the
        /// top of the stack
        /// </summary>
        internal void push()
        {
            if (top_ == size_ - 1)
            {
                resize(size_ + 1);
            }
            top_++;
        }

        /// <summary>
        /// Get a reference to the next FontDrawer in order to set its variables
        /// </summary>
        /// <returns>Next FontDrawer, which is pushed on to the stack with push()</returns>
        internal FontDrawer getNext()
        {
            if (top_ == size_ - 1)
            {
                resize(size_ + 1);
            }
            return stack_[top_ + 1];
        }

        public bool hasMoreItems()
        {
            return top_ >= 0;
        }

        /// <summary>
        /// Resize the stack in a non-destructive way
        /// </summary>
        /// <param name="newSize">The new size of the stack</param>
        public void resize(int newSize)
        {
            if (newSize > size_)
            {
                int nextSize = (newSize > size_ * 2) ? newSize : size_ * 2;
                FontDrawer[] tempStack = new FontDrawer[nextSize];
                for (int i = 0; i <= top_; i++)
                {
                    tempStack[i] = stack_[i];
                }
                stack_ = tempStack;
                tempStack = null;
                size_ = nextSize;
                initializeStack();
            }
        }

        /// <summary>
        /// Resize and reset the stack.  Destroys all elements and resets top_ to zero
        /// </summary>
        /// <param name="newSize">The new size of stack</param>
        public void resizeDestructively(int newSize)
        {
            stack_ = new FontDrawer[newSize];
            size_ = newSize;
            top_ = -1;
            initializeStack();
        }

        protected void initializeStack()
        {
            for (int i = top_ + 1; i < size_; i++)
            {
                stack_[i] = new FontDrawer(spriteBatch_);
            }
        }
    }
}
