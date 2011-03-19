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
using Microsoft.Xna.Framework.Graphics;

namespace MetroidAI
{
    /// <summary>
    /// Helper class for drawing debugging lines to the screen.
    /// </summary>
    public static class LineDrawer
    {
        private static GameTexture s_blankTexture = null;

        static readonly Color LINE_COLOR = Color.LimeGreen;
        const float LINE_DEPTH = Constants.DepthDebugLines;

        public static void drawLine(Vector2 point1, Vector2 point2)
        {
            drawLine(point1, point2, LINE_COLOR);
        }

        public static void drawLine(Vector2 point1, Vector2 point2, Color color)
        {
            drawLine(point1, point2, color, CoordinateTypeEnum.RELATIVE);
        }
        internal static void drawLine(Vector2 point1, Vector2 point2, Color color, CoordinateTypeEnum coordType)
        {
            if (s_blankTexture == null)
            {
                initialize();
            }
            Vector2 center = point1;
            center.X += point2.X;
            center.Y += point2.Y;
            center.X /= 2.0f;
            center.Y /= 2.0f;
            Vector2 rotation = point2 - point1;

            DrawCommand drawer = DrawBuffer.getInstance().DrawCommands.pushGet();
            drawer.Texture = s_blankTexture;
            drawer.ImageIndex = 0;
            drawer.Direction = rotation;
            drawer.CoordinateType = coordType;
            drawer.UseDestination = true;
            drawer.Destination = new Rectangle((int)point1.X, (int)point1.Y, (int)rotation.Length(), 1);
            drawer.Depth = LINE_DEPTH;
            drawer.Centered = false;
            drawer.Color = color;
        }

        public static void drawCross(Vector2 point, Color color)
        {
            drawLine(new Vector2(point.X - 3, point.Y), new Vector2(point.X + 2, point.Y), color);
            drawLine(new Vector2(point.X, point.Y - 2), new Vector2(point.X, point.Y + 3), color);
        }

        private static void initialize()
        {
            Engine e = GlobalHelper.getInstance().Engine;
            s_blankTexture = new GameTexture(@"Sprites\blank");
        }
    }
}
