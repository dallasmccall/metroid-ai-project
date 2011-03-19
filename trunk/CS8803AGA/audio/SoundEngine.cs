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

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

// Important Notice:
// The techniques used in constructing this class are based on those found
// at http://www.ziggyware.com/readarticle.php?article_id=40.
//
// Thank you very much for your tutorial, 'zygote'!

namespace CS8803AGA.audio
{
    /// <summary>
    /// A simple class for pulling sounds from a compiled XACT project and
    /// playing them.  Employs the Singleton pattern.
    /// </summary>
    internal class SoundEngine
    {
        private static SoundEngine s_instance;

        private AudioEngine m_audio;
        private WaveBank m_waveBank;
        private SoundBank m_soundBank;

        private Cue m_music = null;

        /// <summary>
        /// Private constructor as per the Singleton pattern which reads the
        /// compiled sound files into memory.
        /// </summary>
        private SoundEngine()
        {
            // These files are automatically created in the output directory
            //  matching the relative path of wherever the .xap file is located
            m_audio = new AudioEngine(@"Content\Audio\sounds.xgs");
            //m_waveBank = new WaveBank(m_audio,@"Content\Audio\waves1.xwb");
            m_soundBank = new SoundBank(m_audio,@"Content\Audio\sounds1.xsb");

            AudioCategory music = m_audio.GetCategory("Music");
            music.SetVolume(1f);
            AudioCategory effects = m_audio.GetCategory("Effect");
            effects.SetVolume(1f);
        }

        /// <summary>
        /// Provides the instance of the class as per the Singleton pattern.
        /// </summary>
        /// <returns>The only instance of SoundEngine.</returns>
        public static SoundEngine getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new SoundEngine();
            }
            return s_instance;
        }

        public void update()
        {
            if (m_music != null && m_music.IsStopped)
            {
                m_music = m_soundBank.GetCue(m_music.Name);
                m_music.Play();
            }

            m_audio.Update();
        }

        /// <summary>
        /// Plays a sound based on a provided key.
        /// </summary>
        /// <param name="cueName">The cue key from the XACT project.</param>
        /// <returns>Returns a handle to the sound.</returns>
        public Cue playCue(string cueName)
        {
            Cue cue = m_soundBank.GetCue(cueName);
            cue.Play();
            return cue;
        }

        /// <summary>
        /// Stops a sound immediately.
        /// </summary>
        /// <param name="cue">The handle of the sound to stop.</param>
        public static void stopCue(Cue cue)
        {
            cue.Stop(AudioStopOptions.Immediate);
        }

        /// <summary>
        /// Cleans up the instance's resources.
        /// </summary>
        public static void cleanup()
        {
            if (s_instance != null)
            {
                s_instance.m_audio.Dispose();
                //s_instance.m_waveBank.Dispose();
                s_instance.m_soundBank.Dispose();
            }
            s_instance = null;
        }

        /// <summary>
        /// Adjusts the play volume of all sounds.
        /// </summary>
        /// <param name="amount">Percentage of max volume?</param>
        public void changeAllVolume(float amount)
        {
            
            AudioCategory music = m_audio.GetCategory("Music");
            music.SetVolume(amount * 1f);
            AudioCategory effects = m_audio.GetCategory("Effect");
            effects.SetVolume(amount * 0.4f);
            

            // We can use root category Global instead of the above
            //AudioCategory cat = audio_.GetCategory("Global");
            //cat.SetVolume(amount);
        }

        /// <summary>
        /// Temporary.
        /// Controls game music to one song at a time.
        /// </summary>
        /// <param name="cueName">Name of song to replace current.</param>
        public void playMusic(string cueName)
        {
            // if a different cue is playing, kill it
            if (m_music != null && m_music.Name != cueName)
            {
                m_music.Stop(AudioStopOptions.AsAuthored);
            }

            // play new cue only if it's not same as old cue
            if (m_music == null || m_music.Name != cueName)
            {
                //m_music = m_soundBank.GetCue(cueName);
                //m_music.Play();
            }
        }
    }
}
