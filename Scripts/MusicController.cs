/*
 * By Jason Hein
 * 
 */


using UnityEngine;


namespace Music.Controller
{
    /// <summary>
    /// An abstract class that all music controllers must inherit from to use the music player's priority que, voluem, and fade out properties.
    /// </summary>
    public abstract class MusicController : MonoBehaviour
    {
        // How much this controller should be prioritized in the priority music que.
        [SerializeField] byte m_Importance = 0;

        /// <summary>
        /// How much this controller should be prioritized in the priority music que.
        /// The higher the value, the higher priority this controller is.
        /// </summary>
        public byte importance
        {
            get
            {
                return m_Importance;
            }
        }

        /// <summary>
        /// Called when this controller becomes the highest priority among controllers in the Music Player list.
        /// </summary>
        public virtual void Activate () { }

        /// <summary>
        /// Called when this controller stops being the highest priority among controllers in the Music Player list.
        /// Typically does not turn off the current music, let MusicPlayer.Play() handle the transition
        /// </summary>
        public virtual void Deactivate () { }

        /// <summary>
        /// Add the controller to the priority que.
        /// </summary>
        public virtual void Add ()
        {
            MusicPlayer.instance.Add(this);
        }

        /// <summary>
        /// Removes the controller from the priority que.
        /// </summary>
        public virtual void Remove()
        {
            MusicPlayer.instance.Remove(this);
        }
    }
}