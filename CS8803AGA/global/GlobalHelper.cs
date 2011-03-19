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

namespace CS8803AGA
{
    /// <summary>
    /// A Singleton with access to some objects which can be useful everywhere.
    /// Should be configured during Engine initialization.
    /// </summary>
    public class GlobalHelper
    {
        protected static GlobalHelper s_instance = null;

        /// <summary>
        /// Singleton pattern, ctor is private.
        /// </summary>
        private GlobalHelper()
        {
            // nch
        }

        /// <summary>
        /// Get instance of Singleton.
        /// </summary>
        /// <returns>Singleton instance.</returns>
        public static GlobalHelper getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new GlobalHelper();
            }
            return s_instance;
        }

        /// <summary>
        /// Engine object used for the current game.
        /// </summary>
        public Engine Engine { get; set; }

        /// <summary>
        /// Camera object, should be the one used by the render thread.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// An easy way to load an asset from the Content directory.
        /// </summary>
        /// <typeparam name="T">Type of asset to load.</typeparam>
        /// <param name="assetPath">Path to asset, relative to Content folder.</param>
        /// <returns>Loaded asset.</returns>
        public static T loadContent<T>(string assetPath)
        {
            return s_instance.Engine.Content.Load<T>(assetPath);
        }
    }
}
