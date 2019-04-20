/*
 * By Jason Hein
 * 
 * Use physics layer masks via the editor to setup only colliding with the the desired trigger object.
 * 
 */

using UnityEngine;


namespace Music.Controller
{
    /// <summary>
    /// When triggered, adds this music controller to the que.
    /// When existed, removes this music controller from the que.
    /// </summary>
    public class MusicZone2D : MusicLoopFade
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
