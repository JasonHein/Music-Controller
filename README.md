# Music-Controller
A collection of unity scripts that allow you to have a priority queue of music to play that you can change at-runtime, fade-in/out songs, shuffle, and use "music-zones" in scenes to switch the music based on the player's location.

# Basics

The MusicPlayer must be on a game object in the scene.

The MusicPlayer plays the music, controls the music volume, and handles the priority queue for what to play next.

There are multiple types of MusicControllers that can be in the priority queue. Simply add one to a game object and give it a clip(s). Then drag the MusicControlelr into the MusicPlayer's Controller list. The music controller with the highest priority will play when the scene loads.

You can change the priority of a music controller through the unity editor's inspector window.

The music will continue to play between scenes. If you enter a new scene and attempt to play the same music as was already playing, nothing happens and the music will just continue.



# Types of music controllers.

MusicSuddenPlay - Plays a music clip.
MusicFadeIn - Plays a music clip with a short fade-in time and fades out the last clip played.
MusicLoop - Plays a music clip that plays itself again when it finishes.
MusicShuffle - Shuffles between a list of music clips.
MusicZone2D - Adds itself to the priority queue when the zone is entered. Then removes itself when the zone is exited.

And many controllers that are combinations of these basics.


# For Developers

The MusicPlayer script is a singleton.

MusicController is an abstract class. The Activate and Deactivate functions must be implimented by inheriting classes. These get called when that controller becomes the highest priority, or is no longer the highest priority. This effectively lets the controller know that it is in control of the music. Typically in the Activate function you should call MusicPlayer.instance.Play(Audioclip clip).

MusicPlayer will handle fading in or out of music as long as you call MusicPlayer.instance.Play(AudioClip clip, float transitionTime).
