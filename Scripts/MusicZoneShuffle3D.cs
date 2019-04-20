/*
 * By Jason Hein
 */


using UnityEngine;

namespace Music.Controller
{
    /// <summary>
    /// When triggered, adds this music controller to the que.
    /// When existed, removes this music controller from the que.
    /// </summary>
    public class MusicZoneShuffle3D : MusicShuffleFade
    {
        private void OnTriggerEnter(Collider collision)
        {
            Add();
        }

        private void OnTriggerExit(Collider collision)
        {
            Remove();
        }
    }
}