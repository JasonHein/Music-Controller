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
    public class MusicZoneShuffle2D : MusicShuffleFade
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Add();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Remove();
        }
    }
}