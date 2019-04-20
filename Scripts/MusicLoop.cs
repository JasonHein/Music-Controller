/*
 * By Jason Hein
 * 
 */


namespace Music.Controller
{
    /// <summary>
    /// Wrapper to play a music clip on loop, using the music player's priority system.
    /// </summary>
    public class MusicLoop : MusicSuddenPlay
    {
        /// <summary>
        /// This controller has gained priority.
        /// </summary>
        public override void Activate()
        {
            MusicPlayer.instance.loop = true;
            base.Activate();
        }

        /// <summary>
        /// This controller has lost priority.
        /// </summary>
        public override void Deactivate()
        {
            MusicPlayer.instance.loop = false;
            base.Deactivate();
        }
    }
}
