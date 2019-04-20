/*
 * By Jason Hein
 * 
 */


using UnityEngine;

namespace Music.Controller
{
    /// <summary>
    /// Wrapper for an array of audio clips to shuffle between each other and utilize the music player priority que.
    /// When this music controller drops priority, the music will fade out before switching.
    /// </summary>
    public class MusicShuffleFade : MusicShuffle
    {
        [SerializeField] float m_TransitionTime = 0.5f;
        bool m_InitialSwitch = false;

        /// <summary>
        /// This controller has gained priority.
        /// </summary>
        public override void Activate()
        {
            m_InitialSwitch = true;
            base.Activate();
        }

        /// <summary>
        /// This controller has lost  priority.
        /// </summary>
        public override void Deactivate()
        {
            MusicPlayer.instance.fadeTimer = m_TransitionTime;
        }

        protected override void Play()
        {
            if (m_InitialSwitch)
            {
                m_InitialSwitch = false;
                MusicPlayer.instance.Play(m_Clips[m_ClipsIndex], m_TransitionTime);
                return;
            }
            base.Play();
        }
    }
}
