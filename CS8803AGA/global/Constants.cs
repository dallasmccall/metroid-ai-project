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
    public class Constants
    {
        // TODO: Also support loading constants from a file

        public const float DepthDebugLines = 1.0f;

        public const float DepthGameplayBackground = 0.19f;

        public const float DepthBaseGameplay = 0.2f;
        public const float DepthMaxGameplay = 0.3f;
        public const float DepthRangeGameplay = DepthMaxGameplay - DepthBaseGameplay;

        public const float DepthGameplayDecorationsBehind = DepthBaseGameplay + 0.01f;
        public const float DepthBehindCharacters = DepthBaseGameplay + 0.019f;
        public const float DepthGameplayCharacters = DepthBaseGameplay + 0.02f;
        public const float DepthGameplayTiles = DepthBaseGameplay + 0.03f;
        public const float DepthGameplayDecorationsFront = DepthBaseGameplay + 0.04f;
        public const float DepthGameplayExplosions = DepthBaseGameplay + 0.05f;

        public const float DepthGameplayOverlay = DepthMaxGameplay + 0.01f; // fade overlay for zone transitions
        public const float DepthGameplayDoors = DepthMaxGameplay + 0.02f; // so doors appear above fade overlay
        public const float DepthHUD = DepthMaxGameplay + 0.02f;

        public const float DepthMainMenuText = 0.5f;

        public const float DEPTH_LOW = 0.2f;

        public const float DepthDialoguePage = .95f;
        public const float DepthDialogueText = .951f;
    }
}
