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
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    public class Camera
    {
        public float X { get; set; }
        public float Y { get; set; }

        public float ScreenWidth { get; set; }
        public float ScreenHeight { get; set; }

        public Vector2 Position
        {
            get
            {
                return new Vector2(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(X + ScreenWidth / 2, Y + ScreenHeight / 2);
            }
            set
            {
                X = value.X - ScreenWidth / 2;
                Y = value.Y - ScreenHeight / 2;
            }
        }

        public Camera()
            :this(0,0)
        {
        }

        public Camera(float screenWidth, float screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            X = 0;
            Y = 0;
        }

        public void setCenter(float x, float y)
        {
            X = x - (ScreenWidth / 2f);
            Y = y - (ScreenHeight / 2f);
        }

        public void move(float deltaX, float deltaY)
        {
            X += deltaX;
            Y += deltaY;
        }
    }
}
