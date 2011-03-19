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

using Microsoft.Xna.Framework;
using System;

// TODO
//
// Refactor this to have a single set function for values
//

namespace MetroidAI.devices
{
    /// <summary>
    /// Pseudo-singleton which contains a data structure of values of input
    /// from a user device during a single frame.  Also contains some
    /// functionality for controlling how often this input can change.
    /// </summary>
    public class InputSet
    {

        /// <summary>
        /// An array of instances of InputSets, one for each possible player.
        /// </summary>
        static protected InputSet[] s_instances = null;

        /// <summary>
        /// Control device which has most recently used this InputSet.
        /// </summary>
        protected ControllerInputInterface m_controller;

        /// <summary>
        /// An array of ints for each input device in InputsEnum;
        ///  each int represents the number of frames left for which that
        ///  particular device will pretend to be released regardless of its
        ///  actual state.
        /// </summary>
        protected int[] m_stickStates = new int[(int)InputsEnum.LENGTH];

        /// <summary>
        /// An array of booleans for each input device in InputsEnum; each
        /// bool represents whether that device must be released before it can
        /// register normally again.
        /// </summary>
        protected bool[] m_toggleStates = new bool[(int)InputsEnum.LENGTH];

        private const int ARRAY_OFFSET = 2;
        protected Vector2[] m_directionals = new Vector2[ARRAY_OFFSET];
        protected bool[] m_buttons = new bool[(int)InputsEnum.LENGTH - ARRAY_OFFSET];

        /// <summary>
        /// Statically creates an InputSet for four different players.
        /// </summary>
        static InputSet()
        {
            s_instances = new InputSet[4];

            // Not expensive to create 4 InputSets, so we aren't
            // worried about lazy instantiation.  If we instantiate
            // them now, and later we use multithreading, we won't
            // have to use sync locks - just pull the InputSet and
            // read from it.
            for (int i = 0; i < 4; i++)
            {
                s_instances[i] = new InputSet();
            }
        }

        /// <summary>
        /// Private constructor as per Singleton pattern.
        /// Initializes empty arrays for Sticks and Toggles.
        /// </summary>
        private InputSet()
        {
            m_stickStates = new int[(int)InputsEnum.LENGTH];
            m_toggleStates = new bool[(int)InputsEnum.LENGTH];
        }

        /// <summary>
        /// Returns Player One's InputSet as per the Singleton pattern.
        /// </summary>
        /// <returns>The InputSet for Player One.</returns>
        static public InputSet getInstance()
        {
            return s_instances[(int)Settings.getInstance().CurrentPlayer];
        }

        /// <summary>
        /// Returns the InputSet of specific player.
        /// </summary>
        /// <param name="index">The player whose inputs are desired.</param>
        /// <returns>The InputSet for the specified player.</returns>
        static public InputSet getInstance(PlayerIndex index)
        {
            int i = (int)index;
            return s_instances[i];
        }

        /// <summary>
        /// Reduces all sticks by one.
        /// </summary>
        /// <param name="cii">Controller updating this InputSet</param>
        public void update(ControllerInputInterface cii)
        {
            m_controller = cii;
            for (int i = 0; i < (int)InputsEnum.LENGTH; i++)
            {
                if (m_stickStates[i] > 0)
                {
                    m_stickStates[i]--;
                }
            }
        }

        public float getLeftDirectionalX()
        {
            return m_directionals[0].X;
        }

        public float getLeftDirectionalY()
        {
            return m_directionals[0].Y;
        }

        public float getRightDirectionalX()
        {
            return m_directionals[1].X;
        }

        public float getRightDirectionalY()
        {
            return m_directionals[1].Y;
        }

        /// <summary>
        /// Retrieves the value of a button-type device.
        /// </summary>
        /// <param name="button">Button value to retrieve</param>
        /// <returns>Returns true if pressed, false if depressed</returns>
        public bool getButton(InputsEnum button)
        {
            int index = (int)button;
            if (index < ARRAY_OFFSET || index >= (int)InputsEnum.LENGTH)
            {
                throw new InvalidInputSetException();
            }
            index -= ARRAY_OFFSET; // normalize to the buttons_ array
            return m_buttons[index];
        }

        /// <summary>
        /// Sets a directional-type device to a specific value.
        /// </summary>
        /// <param name="device">Directional device to be set</param>
        /// <param name="x">Cartesian X</param>
        /// <param name="y">Cartesian Y</param>
        public void setDirectional(InputsEnum device, float x, float y)
        {
            int index = (int)device;
            if (index < 0 || index > ARRAY_OFFSET)
            {
                throw new InvalidInputSetException();
            }
            if (m_stickStates[index] > 0 ||
                m_toggleStates[index])
            {
                if (x == 0 && y == 0)
                {
                    m_toggleStates[index] = false;
                }
                m_directionals[index].X = 0;
                m_directionals[index].Y = 0;
                return;
            }
            m_directionals[index].X = x;
            m_directionals[index].Y = y;
        }

        /// <summary>
        /// Sets a button-type device to a specific value.
        /// </summary>
        /// <param name="device">Button to set</param>
        /// <param name="value">True = pressed, False = depressed</param>
        public void setButton(InputsEnum device, bool value)
        {
            int index = (int)device;
            if (index < ARRAY_OFFSET || index >= (int)InputsEnum.LENGTH)
            {
                throw new InvalidInputSetException();
            }
            if (m_stickStates[index] > 0 ||
                m_toggleStates[index])
            {
                if (!value)
                {
                    m_toggleStates[index] = false;
                }
                index -= ARRAY_OFFSET; // normalize to the buttons_ array
                m_buttons[index] = false;
                return;
            }
            index -= ARRAY_OFFSET; // normalize to the buttons_ array
            m_buttons[index] = value;
        }

        /// <summary>
        /// Causes an input unit to act released for a set number of frames.
        /// </summary>
        /// <param name="toStick">The input unit which will act released.</param>
        /// <param name="numFrames">Duration (in frames) before acting normal.</param>
        public void setStick(InputsEnum toStick, int numFrames)
        {
            if (numFrames >= 0)
            {
                m_stickStates[(int)toStick] = numFrames;
            }
            else
            {
                m_stickStates[(int)toStick] = 0;
            }
        }

        /// <summary>
        /// Causes an input unit to act released until being physically
        /// released and then pressed/moved/clicked/etc again.
        /// </summary>
        /// <param name="toToggle">The input unit which will act released.</param>
        public void setToggle(InputsEnum toToggle)
        {
            m_toggleStates[(int)toToggle] = true;
        }

        /// <summary>
        /// Clears the toggle requirement for an input unit.  See setToggle().
        /// </summary>
        /// <param name="toToggle">The input unit to no longer wait for a toggle.</param>
        public void clearToggle(InputsEnum toToggle)
        {
            m_toggleStates[(int)toToggle] = false;
        }

        /// <summary>
        /// Performs a setStick() on all input units.
        /// </summary>
        /// <param name="numFrames">Duration (in frames) before acting normal.</param>
        public void setAllSticks(int numFrames)
        {
            for (int i = 0; i < (int)InputsEnum.LENGTH; i++)
            {
                m_stickStates[i] = numFrames;
            }
        }

        /// <summary>
        /// Performs a setToggle() on all input units.
        /// </summary>
        public void setAllToggles()
        {
            for (int i = 0; i < (int)InputsEnum.LENGTH; i++)
            {
                m_toggleStates[i] = true;
            }
        }

        /// <summary>
        /// Returns normal functionality to input units which were put into
        /// a Stick state by setStick() or setAllSticks().  Does not affect
        /// Toggle states.
        /// </summary>
        public void clearSticks()
        {
            for (int i = 0; i < (int)InputsEnum.LENGTH; i++)
            {
                m_stickStates[i] = 0;
            }
        }

        /// <summary>
        /// Returns normal functionality to input units which were put into
        /// a Toggle state by setToggle() or setAllToggles().  Does not affect
        /// Stick states.
        /// </summary>
        public void clearToggles()
        {
            for (int i = 0; i < (int)InputsEnum.LENGTH; i++)
            {
                m_toggleStates[i] = false;
            }
        }

        /// <summary>
        /// Sets all buttons to the released state.
        /// </summary>
        public void clearInputs()
        {
            for (int i = 0; i < ARRAY_OFFSET; i++)
            {
                m_directionals[i].X = 0;
                m_directionals[i].Y = 0;
            }
            for (int i = 0; i < (int)InputsEnum.LENGTH - ARRAY_OFFSET; i++)
            {
                m_buttons[i] = false;
            }
        }

        /// <summary>
        /// Completely reinitializes an InputSet
        /// </summary>
        public void reset()
        {
            clearSticks();
            clearToggles();
            clearInputs();
        }

        /// <summary>
        /// Fetches the name of a particular button or directional
        /// </summary>
        /// <param name="device">Device whose name is desired</param>
        /// <returns>Name of the device</returns>
        public string getControlName(InputsEnum device)
        {
            if (m_controller == null)
                return "Error";
            return m_controller.getControlName(device);
        }

    }

    /// <summary>
    /// This enumeration is used when setting and clearing toggles and sticks
    /// The last item, LENGTH, should never be used in these conditions, but
    ///  is a replacement since C# enumerations do not have a .length attrib,
    ///  and is useful for iterating over arrays which contain one element
    ///  for each item in the enumeration.  This item MUST remain last.
    /// </summary>
    public enum InputsEnum
    {
        LEFT_DIRECTIONAL,
        RIGHT_DIRECTIONAL,
        CONFIRM_BUTTON,
        CANCEL_BUTTON,
        BUTTON_1,
        BUTTON_2,
        BUTTON_3,
        BUTTON_4,
        LEFT_TRIGGER,
        RIGHT_TRIGGER,
        LEFT_BUMPER,
        RIGHT_BUMPER,
        
        LENGTH
    }

    /// <summary>
    /// Represents an error because a caller tried to set a particular
    /// device with the wrong type of inputs for that device.
    /// </summary>
    internal class InvalidInputSetException : Exception
    {

    }
}
