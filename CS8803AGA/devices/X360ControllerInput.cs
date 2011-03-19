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
using Microsoft.Xna.Framework.Input;
using System;

namespace CS8803AGA.devices
{
    /// <summary>
    /// Implementation of a ControllerInputInterface for a user with an
    /// Xbox 360 Gamepad, either connected to a PC or a console.
    /// </summary>
    class X360ControllerInput : ControllerInputInterface
    {
        protected Engine engine_;
        protected PlayerIndex player_;
        protected InputSet inputs_;

        // Key mapping
        // ------------------
        // Currently both directionals are hardcoded to the thumbsticks.
        protected Buttons CONFIRM = Buttons.Start;
        protected Buttons CANCEL = Buttons.Back;
        protected Buttons BUTTON_1 = Buttons.A;
        protected Buttons BUTTON_2 = Buttons.B;
        protected Buttons BUTTON_3 = Buttons.X;
        protected Buttons BUTTON_4 = Buttons.Y;
        protected Buttons LEFT_TRIGGER = Buttons.LeftTrigger;
        protected Buttons RIGHT_TRIGGER = Buttons.RightTrigger;
        protected Buttons LEFT_BUMPER = Buttons.LeftShoulder;
        protected Buttons RIGHT_BUMPER = Buttons.RightShoulder;

        /// <summary>
        /// Default constructor; assigns itself to Player One's input device.
        /// </summary>
        /// <param name="engine">The engine using the controller.</param>
        public X360ControllerInput(Engine engine)
        {
            engine_ = engine;
            player_ = PlayerIndex.One;
            inputs_ = InputSet.getInstance();
        }

        /// <summary>
        /// Constructor which allows specification of a player.
        /// </summary>
        /// <param name="engine">The engine using the controller.</param>
        /// <param name="player">The player whose input should be read.</param>
        public X360ControllerInput(Engine engine, PlayerIndex player)
        {
            engine_ = engine;
            player_ = player;
            inputs_ = InputSet.getInstance(player);
        }

        #region ControllerInputInterface Members

        /// <summary>
        /// Returns the InputSet which the device is populating.
        /// </summary>
        /// <returns>An InputSet with the current frame's input.</returns>
        public InputSet getInputSet()
        {
            return inputs_;
        }

        /// <summary>
        /// Should be called EXACTLY once per frame to read inputs from
        /// the device and populate the InputSet accordingly.
        /// </summary>
        public void updateInputSet()
        {
            inputs_.update(this);

            GamePadState gps = GamePad.GetState(player_, GamePadDeadZone.Circular);


            Vector2 left = gps.ThumbSticks.Left;
            if (left.LengthSquared() == 0)
            {
                if (gps.IsButtonDown(Buttons.DPadLeft))
                    left.X -= 1;
                if (gps.IsButtonDown(Buttons.DPadRight))
                    left.X += 1;
                if (gps.IsButtonDown(Buttons.DPadUp))
                    left.Y += 1;
                if (gps.IsButtonDown(Buttons.DPadDown))
                    left.Y -= 1;
                if (left.LengthSquared() != 0)
                    left.Normalize();
            }
            inputs_.setDirectional(InputsEnum.LEFT_DIRECTIONAL,
                                        left.X,
                                        left.Y);

            Vector2 right = gps.ThumbSticks.Right;
            inputs_.setDirectional(InputsEnum.RIGHT_DIRECTIONAL,
                                        right.X,
                                        -right.Y);

            inputs_.setButton(InputsEnum.CONFIRM_BUTTON, gps.IsButtonDown(CONFIRM));
            inputs_.setButton(InputsEnum.CANCEL_BUTTON, gps.IsButtonDown(CANCEL));

            inputs_.setButton(InputsEnum.BUTTON_1, gps.IsButtonDown(BUTTON_1));
            inputs_.setButton(InputsEnum.BUTTON_2, gps.IsButtonDown(BUTTON_2));
            inputs_.setButton(InputsEnum.BUTTON_3, gps.IsButtonDown(BUTTON_3));
            inputs_.setButton(InputsEnum.BUTTON_4, gps.IsButtonDown(BUTTON_4));

            inputs_.setButton(InputsEnum.LEFT_TRIGGER, gps.IsButtonDown(LEFT_TRIGGER));
            inputs_.setButton(InputsEnum.RIGHT_TRIGGER, gps.IsButtonDown(RIGHT_TRIGGER));
            inputs_.setButton(InputsEnum.LEFT_BUMPER, gps.IsButtonDown(LEFT_BUMPER));
            inputs_.setButton(InputsEnum.RIGHT_BUMPER, gps.IsButtonDown(RIGHT_BUMPER));

        }

        /// <summary>
        /// Fetches the name of a particular button or directional
        /// </summary>
        /// <param name="device">Device whose name is desired</param>
        /// <returns>Name of the device</returns>
        public string getControlName(InputsEnum device)
        {
            switch (device)
            {
            case InputsEnum.LEFT_DIRECTIONAL:
                return "Left Thumbstick";
            case InputsEnum.RIGHT_DIRECTIONAL:
                return "Right Thumbstick";
            case InputsEnum.CONFIRM_BUTTON:
                return CONFIRM.ToString();
            case InputsEnum.CANCEL_BUTTON:
                return CANCEL.ToString();
            case InputsEnum.BUTTON_1:
                return BUTTON_1.ToString();
            case InputsEnum.BUTTON_2:
                return BUTTON_2.ToString();
            case InputsEnum.BUTTON_3:
                return BUTTON_3.ToString();
            case InputsEnum.BUTTON_4:
                return BUTTON_4.ToString();
            case InputsEnum.LEFT_BUMPER:
                return LEFT_BUMPER.ToString();
            case InputsEnum.RIGHT_BUMPER:
                return RIGHT_BUMPER.ToString();
            case InputsEnum.LEFT_TRIGGER:
                return LEFT_TRIGGER.ToString();
            case InputsEnum.RIGHT_TRIGGER:
                return RIGHT_TRIGGER.ToString();
            default:
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
