/*
 * By Jason Hein
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Music.Controller
{
    /// <summary>
    /// Wrapper for an array of audio clips to shuffle between each other and utilize the music player priority que.
    /// </summary>
    public class MusicShuffle : MusicController
    {
        [SerializeField] protected List<AudioClip> m_Clips;
        const float SHUFFLE_CHECK_TIME = 2.5f;
        protected int m_ClipsIndex = 99999;

        /// <summary>
        /// This controller has gained priority.
        /// </summary>
        public override void Activate()
        {
            if (m_Clips.Count > 0)
            {
                StartCoroutine(CheckMusicStopped());
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("No clips in music shuffle.");
            }
#endif
        }

        /// <summary>
        /// This controller has lost priority.
        /// </summary>
        public override void Deactivate()
        {
            StopCoroutine(CheckMusicStopped());
        }

        /// <summary>
        /// Randomizes the next clip to play from a given list.
        /// </summary>
        protected void Shuffle ()
        {
            if (m_Clips.Count > 0)
            {
                List<AudioClip> copy = new List<AudioClip>();
                copy.AddRange(m_Clips);
                m_Clips.Clear();

                // Add any clip first except for the last clip played.
                m_ClipsIndex = Random.Range(0, copy.Count - 1);
                m_Clips.Add(copy[m_ClipsIndex]);
                copy.RemoveAt(m_ClipsIndex);

                while (copy.Count > 0)
                {
                    m_ClipsIndex = Random.Range(0, copy.Count);
                    m_Clips.Add(copy[m_ClipsIndex]);
                    copy.RemoveAt(m_ClipsIndex);
                }
                m_ClipsIndex = 0;
            }
        }

        /// <summary>
        /// Plays the next clip in the list.
        /// If all the clips have played, then shuffle the list and play a random clip.
        /// </summary>
        void PlayNext ()
        {
            if (m_ClipsIndex++ >= m_Clips.Count - 1)
            {
                Shuffle();
            }
            Play();
        }

        /// <summary>
        /// Plays the current clip.
        /// </summary>
        protected virtual void Play()
        {
            MusicPlayer.instance.Play(m_Clips[m_ClipsIndex]);
        }

        /// <summary>
        /// Repeatedly checks if the music has stopped. If it has, play the next clip.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckMusicStopped ()
        {
            PlayNext();
            while (true)
            {
                yield return new WaitForSecondsRealtime(SHUFFLE_CHECK_TIME);
                if (!MusicPlayer.instance.isPlaying)
                {
                    PlayNext();
                }
            }
        }

#if UNITY_EDITOR

        private void Reset()
        {
            enabled = false;
        }

#endif
    }
}
