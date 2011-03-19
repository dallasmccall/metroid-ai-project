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
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;

namespace CS8803AGA.utilties
{
    /// <summary>
    /// Manages access to Containers to prevent opening the same container
    /// more than once; one Container per player allowed.
    /// </summary>
    internal static class ContainerManager
    {

#if !XBOX
        internal const string CONTAINER_NAME = "Commando";
#else
        internal const string CONTAINER_NAME = "CommandoXbox";
#endif

        private static StorageContainer[] s_containers
            = new StorageContainer[4];

        /// <summary>
        /// Opens and returns the container for the current player.
        /// </summary>
        /// <returns>An opened container where data can be saved.</returns>
        internal static StorageContainer getOpenContainer()
        {
            return getOpenContainer(Settings.getInstance().CurrentPlayer);
        }

        /// <summary>
        /// Opens and returns the container for the specified player.
        /// </summary>
        /// <param name="player">Player whose data will be saved.</param>
        /// <returns>An opened container where data can be saved.</returns>
        internal static StorageContainer getOpenContainer(PlayerIndex player)
        {
            int index = (int)player;

            if (s_containers[index] == null || s_containers[index].IsDisposed)
            {
                StorageDevice device = Settings.getInstance().StorageDevice;
                if (device == null)
                {
                    throw new NotImplementedException();
                }
                s_containers[index] = device.OpenContainer(CONTAINER_NAME);
                return s_containers[index];
            }

            return s_containers[index];
        }

        /// <summary>
        /// Disposes of the container owned by the current player.
        /// </summary>
        internal static void cleanupContainer()
        {
            cleanupContainer(Settings.getInstance().CurrentPlayer);
        }

        /// <summary>
        /// Disposes of the container owned by the specified player.
        /// </summary>
        /// <param name="player">Player whose container should be disposed.</param>
        internal static void cleanupContainer(PlayerIndex player)
        {
            int index = (int)player;
            if (s_containers[index] == null)
            {
                return;
            }
            s_containers[index].Dispose();
            s_containers[index] = null;
        }
    }
}
