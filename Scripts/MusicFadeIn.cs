/*
 * By Jason Hein
 * 
 */


using UnityEngine;

namespace Music.Controller
{
    /// <summary>
    /// Wrapper for an audio clip to utilize the music player que and priority.
    /// When the music changes, the given transtition is used to fade out this music.
    /// </summary>
    public class MusicFadeIn : MusicController
    {
        [SerializeField] protected AudioClip m_Clip;
        [SerializeField] float m_TransitionTime = 0.5f;

        /// <summary>
        /// This controller has gained priority.
        /// </summary>
        public override void Activate()
        {
            MusicPlayer.instance.Play(m_Clip, m_TransitionTime);
        }

        /// <summary>
        /// This controller has lost priority.
        /// </summary>
        public override void Deactivate()
        {
            MusicPlayer.instance.fadeTimer = m_TransitionTime;
        }
    }
}
