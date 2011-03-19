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
    /// Implementation of ControllerInputInterface for a regular PC user
    /// with keyboard and mouse.
    /// </summary>
    public class PCControllerInput : ControllerInputInterface
    {
        protected Engine engine_;
        protected InputSet inputs_;

        // Key mapping
        // ------------------
        // Currently the Left and Right triggers are hardcoded to the mouse
        // buttons, and the right directional is hardcoded to mouse movement.
        protected Keys LEFT_DIR_UP = Keys.W;
        protected Keys LEFT_DIR_DOWN = Keys.S;
        protected Keys LEFT_DIR_LEFT = Keys.A;
        protected Keys LEFT_DIR_RIGHT = Keys.D;
        protected Keys CONFIRM = Keys.Enter;
        protected Keys CANCEL = Keys.Escape;
        protected Keys BUTTON_1 = Keys.Space;
        protected Keys BUTTON_2 = Keys.LeftShift;
        protected Keys BUTTON_3 = Keys.X;
        protected Keys BUTTON_4 = Keys.C;
        protected Keys LEFT_BUMPER = Keys.Q;
        protected Keys RIGHT_BUMPER = Keys.E;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="engine">The engine using the controller.</param>
        public PCControllerInput(Engine engine)
        {
            engine_ = engine;
            inputs_ = InputSet.getInstance();
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

            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            float leftX = 0;
            float leftY = 0;

            if (ks.IsKeyDown(LEFT_DIR_UP) || ks.IsKeyDown(Keys.Up))
            {
                leftY += 1.0f;
            }
            if (ks.IsKeyDown(LEFT_DIR_DOWN) || ks.IsKeyDown(Keys.Down))
            {
                leftY += -1.0f;
            }
            
            if (ks.IsKeyDown(LEFT_DIR_RIGHT) || ks.IsKeyDown(Keys.Right))
            {
                leftX += 1.0f;
            }
            if (ks.IsKeyDown(LEFT_DIR_LEFT) || ks.IsKeyDown(Keys.Left))
            {
                leftX += -1.0f;
            }
            Vector2 leftDir = new Vector2(leftX, leftY);
            if (leftDir.Length() > 1.0f)
                leftDir.Normalize();

            inputs_.setDirectional(InputsEnum.LEFT_DIRECTIONAL, leftDir.X, leftDir.Y);

            /*
            if (PlayerHelper.Player_ != null)
            {
                //Vector2 playerCenter = PlayerHelper.Player_.getPosition();
                float centerX = GlobalHelper.getInstance().getCurrentCamera().getScreenWidth() / 2f;
                float centerY = GlobalHelper.getInstance().getCurrentCamera().getScreenHeight() / 2f;
                Vector2 rightDirectional =
                    new Vector2(ms.X - centerX, ms.Y - centerY);
                rightDirectional.Normalize();
                inputs_.setDirectional(InputsEnum.RIGHT_DIRECTIONAL,
                                    rightDirectional.X, rightDirectional.Y);
            }
            */

            inputs_.clearToggle(InputsEnum.RIGHT_DIRECTIONAL);
            inputs_.setDirectional(InputsEnum.RIGHT_DIRECTIONAL, ms.X, ms.Y);

            inputs_.setButton(InputsEnum.CONFIRM_BUTTON,
                                ks.IsKeyDown(CONFIRM));
            inputs_.setButton(InputsEnum.CANCEL_BUTTON,
                                ks.IsKeyDown(CANCEL));

            inputs_.setButton(InputsEnum.BUTTON_1, ks.IsKeyDown(BUTTON_1));
            inputs_.setButton(InputsEnum.BUTTON_2, ks.IsKeyDown(BUTTON_2));
            inputs_.setButton(InputsEnum.BUTTON_3, ks.IsKeyDown(BUTTON_3));
            inputs_.setButton(InputsEnum.BUTTON_4, ks.IsKeyDown(BUTTON_4));

            inputs_.setButton(InputsEnum.LEFT_TRIGGER,
                                ms.RightButton == ButtonState.Pressed);
            inputs_.setButton(InputsEnum.RIGHT_TRIGGER,
                                ms.LeftButton == ButtonState.Pressed);

            inputs_.setButton(InputsEnum.LEFT_BUMPER,
                                ks.IsKeyDown(LEFT_BUMPER));
            inputs_.setButton(InputsEnum.RIGHT_BUMPER,
                                ks.IsKeyDown(RIGHT_BUMPER));
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
                return LEFT_DIR_UP.ToString() +
                        LEFT_DIR_LEFT.ToString() +
                        LEFT_DIR_DOWN.ToString() +
                        LEFT_DIR_RIGHT.ToString();
            case InputsEnum.RIGHT_DIRECTIONAL:
                return "Mouse";
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
            case InputsEnum.LEFT_TRIGGER: // backwards on mouse!
                return "Right Click";
            case InputsEnum.RIGHT_TRIGGER:
                return "Left Click";
            default:
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
