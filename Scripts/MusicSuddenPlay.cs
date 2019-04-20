/*
 * By Jason Hein
 * 
 */


using UnityEngine;

namespace Music.Controller
{
    /// <summary>
    /// Wrapper for an audio clip to utilize the music player que and priority.
    /// </summary>
    public class MusicSuddenPlay : MusicController
    {
        [SerializeField] protected AudioClip m_Clip;

        /// <summary>
        /// This controller has gained priority.
        /// </summary>
        public override void Activate()
        {
            MusicPlayer.instance.Play(m_Clip);
        }

        /// <summary>
        /// This controller has lost priority.
        /// </summary>
        public override void Deactivate()
        {
            MusicPlayer.instance.fadeTimer = 0f;
        }
    }
}
