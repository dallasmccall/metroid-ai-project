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


namespace CS8803AGA.devices
{
    /// <summary>
    /// Interface for a wrapper of a device which receives inputs such as
    /// button presses from the player.
    /// </summary>
    public interface ControllerInputInterface
    {

        /// <summary>
        /// Returns the InputSet which the device is populating.
        /// </summary>
        /// <returns>An InputSet with the current frame's input.</returns>
        InputSet getInputSet();

        /// <summary>
        /// Should be called EXACTLY once per frame to read inputs from
        /// the device and populate the InputSet accordingly.
        /// </summary>
        void updateInputSet();

        /// <summary>
        /// Fetches the name of a particular button or directional
        /// </summary>
        /// <param name="device">Device whose name is desired</param>
        /// <returns>Name of the device</returns>
        string getControlName(InputsEnum device);
    }
}
