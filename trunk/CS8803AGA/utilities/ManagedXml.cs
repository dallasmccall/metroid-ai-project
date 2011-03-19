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
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace CS8803AGA.utilties
{
    /// <summary>
    /// ManagedXml creates a new ContentManager so that Xml data is not cached
    /// in the main ContentManager.  This allows the GC to cleanup this data after
    /// it is no longer needed, because ManagedXml will unload the data.
    /// </summary>
    public class ManagedXml : IDisposable
    {
        protected ContentManager content_;

        public ManagedXml(Engine engine)
            : base()
        {
            content_ = new ContentManager(engine.Services);
            content_.RootDirectory = "Content";
        }

        public XmlDocument load(string asset)
        {
            return content_.Load<XmlDocument>(asset);
        }

        public XmlDocument loadFromFile(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);
            return doc;
        }

        public void Dispose()
        {
            content_.Unload();
            content_.Dispose();
            content_ = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
