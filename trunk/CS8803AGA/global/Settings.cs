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
using System.Xml;
using System.IO;
using CS8803AGA.utilties;
using CS8803AGA.audio;

namespace CS8803AGA
{
    /// <summary>
    /// Singleton for control and game settings.
    /// </summary>
    public class Settings
    {
        protected const string DEFAULT_SETTINGS = @"XML\defaultsettings";
        protected const string SETTINGS_FOLDER = "settings";
        protected const string SETTINGS_FILE = "settings.xml";

        protected static Settings s_instance;

        protected MovementType m_movementType;

        internal PlayerIndex CurrentPlayer { get; set; }

        internal bool IsUsingMouse { get; set; }

        internal bool IsInDebugMode { get; set; }

        internal bool IsCameraFreeform { get; set; }

        internal bool IsGamerServicesAllowed { get; set; }

        internal bool IsExplorer { get; set; }

        protected bool m_isSoundAllowed;
        internal bool IsSoundAllowed
        {
            get
            {
                return m_isSoundAllowed;
            }

            set
            {
                m_isSoundAllowed = value;
                if (value)
                    SoundEngine.getInstance().changeAllVolume(1.0f);
                else
                    SoundEngine.getInstance().changeAllVolume(0.0f);
            }
        }

        internal Engine Engine { get; set; }

        internal protected Resolution m_resolution;
        internal Resolution Resolution
        {
            get
            {
                return m_resolution;
            }

            set
            {
                m_resolution = value;
                switch (value)
                {
                    case Resolution.auto:
                        Engine.initializeScreen();
                        break;
                    case Resolution.s640x480:
                        Engine.setScreenSize(640, 480);
                        break;
                    case Resolution.s800x600:
                        Engine.setScreenSize(800, 600);
                        break;
                    case Resolution.s1024x768:
                        Engine.setScreenSize(1024, 768);
                        break;
                    case Resolution.s1152x864:
                        Engine.setScreenSize(1152, 864);
                        break;
                    case Resolution.h1280x720:
                        Engine.setScreenSize(1280, 720);
                        break;
                }
            }
        }

        private StorageDevice m_storageDevice;
        internal StorageDevice StorageDevice
        {
            get
            {
                return m_storageDevice;
            }
            set
            {
                ContainerManager.cleanupContainer();
                m_storageDevice = value;
            }
        }

        internal static void initialize(Engine engine)
        {
            s_instance = new Settings();
            s_instance.Engine = engine;
            s_instance.m_movementType = MovementType.ABSOLUTE;
            s_instance.IsInDebugMode = false;
            s_instance.IsCameraFreeform = false;
            s_instance.IsSoundAllowed = true;
            s_instance.Resolution = Resolution.auto;
        }

        private Settings()
        {
            // Singleton
        }

        public static Settings getInstance()
        {
            return s_instance;
        }

        /// <summary>
        /// Change movement type between relative and absolute.
        /// </summary>
        public void swapMovementType()
        {
            if (m_movementType == MovementType.ABSOLUTE)
            {
                m_movementType = MovementType.RELATIVE;
            }
            else
            {
                m_movementType = MovementType.ABSOLUTE;
            }
        }

        /// <summary>
        /// Get whether movement should be relative to character
        /// direction or absolute.
        /// </summary>
        /// <returns>Type of movement</returns>
        public MovementType getMovementType()
        {
            return m_movementType;
        }

        /// <summary>
        /// Pulls settings from an XmlDocument
        /// </summary>
        /// <param name="doc">XmlDocument containing the appropriate commando-settings tag</param>
        protected void pullSettings(XmlDocument doc)
        {
            XmlNode root = doc.ChildNodes[1]; // index 0 is XML declaration
            if (root.Name != "commando-settings")
            {
                throw new XmlException("commando-settings missing from settings file");
            }

            XmlNodeList settings = root.ChildNodes;
            for (int i = 0; i < settings.Count; i++)
            {
                XmlNode cur = settings[i];
                switch (cur.Name)
                {
                    case "resolution":
                        Resolution = (Resolution)Convert.ToInt32(cur.InnerText);
                        break;
                    case "movement":
                        m_movementType = (MovementType)Convert.ToInt32(cur.InnerText);
                        break;
                    case "sound":
                        IsSoundAllowed = Convert.ToBoolean(cur.InnerText);
                        break;
                    case "debug":
                        IsInDebugMode = Convert.ToBoolean(cur.InnerText);
                        break;
                }
            }
        }

        /// <summary>
        /// Pushes current settings to an XmlDocument
        /// </summary>
        /// <returns>An XmlDocument containing the current settings</returns>
        protected XmlDocument pushSettings()
        {
            XmlDocument doc = new XmlDocument();

            XmlNode declaration = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(declaration);

            XmlElement root = doc.CreateElement("commando-settings");
            doc.AppendChild(root);

            XmlElement res = doc.CreateElement("resolution");
            res.InnerText = Convert.ToString((int)m_resolution);
            XmlElement movement = doc.CreateElement("movement");
            movement.InnerText = Convert.ToString((int)m_movementType);
            XmlElement sound = doc.CreateElement("sound");
            sound.InnerText = Convert.ToString(IsSoundAllowed);
            XmlElement debug = doc.CreateElement("debug");
            debug.InnerText = Convert.ToString(IsInDebugMode);
            root.AppendChild(res);
            root.AppendChild(movement);
            root.AppendChild(sound);
            root.AppendChild(debug);

            return doc;
        }

        /// <summary>
        /// Copies the current settings into a local file
        /// </summary>
        internal void saveSettingsToFile()
        {
            XmlDocument doc = pushSettings();
        
            // No storage device, so we'll store/load the settings in a directory
            //  with the executable
            if (m_storageDevice == null)
            {
                const string folderpath = @".\" + SETTINGS_FOLDER;
                Directory.CreateDirectory(folderpath);
                const string filepath = folderpath + @"\" + SETTINGS_FILE;
                doc.Save(filepath);
            }

            // We have a storage device, so we'll store the settings in the associated
            //  container
            else
            {
                ContainerManager.cleanupContainer();
                StorageContainer sc = ContainerManager.getOpenContainer();
                string folderpath = Path.Combine(sc.Path, SETTINGS_FOLDER);
                Directory.CreateDirectory(folderpath);
                string filepath = Path.Combine(folderpath, SETTINGS_FILE);
                doc.Save(filepath);
            }
        
        }

        internal void loadSettingsFromFile()
        {
            try
            {
                // No storage device, so we'll store/load the settings in a directory
                //  with the executable
                if (m_storageDevice == null)
                {
                    const string filepath = @".\" + SETTINGS_FOLDER + @"\" + SETTINGS_FILE;
                    using (ManagedXml manager = new ManagedXml(Engine))
                    {
                        XmlDocument doc = manager.loadFromFile(filepath);
                        pullSettings(doc);
                    }
                }

                // We have a storage device, so we'll store the settings in the associated
                //  container
                else
                {
                    StorageContainer sc = ContainerManager.getOpenContainer();
                    string folderpath = Path.Combine(sc.Path, SETTINGS_FOLDER);
                    string filepath = Path.Combine(folderpath, SETTINGS_FILE);
                    using (ManagedXml manager = new ManagedXml(Engine))
                    {
                        XmlDocument doc = manager.loadFromFile(filepath);
                        pullSettings(doc);
                    }
                }
            }
            catch
            {
                using (ManagedXml manager = new ManagedXml(Engine))
                {
                    //XmlDocument doc = manager.load(DEFAULT_SETTINGS);
                    //pullSettings(doc);
                }
            }
        }
    }

    public enum MovementType
    {
        RELATIVE,
        ABSOLUTE
    }

    public enum Resolution
    {
        auto,
        s640x480,
        s800x600,
        s1024x768,
        s1152x864,
        h1280x720
    }
}
