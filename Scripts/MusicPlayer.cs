/*
 * By Jason Hein
 * 
 */


using UnityEngine;
using Music.Controller;
using System.Collections.Generic;

/// <summary>
/// Singleton music player.
/// </summary>
public class MusicPlayer : MonoBehaviour
{

    #region Variables

    // References
    [HideInInspector] public static MusicPlayer instance;
    AudioSource m_MusicSource;
    AudioSource m_TransitionSource;

    // Controller priority que
    [SerializeField] List<MusicController> m_Controllers;

    /// <summary>
    /// If the main music source is playing.
    /// </summary>
    public bool isPlaying
    {
        get
        {
            return m_MusicSource.isPlaying;
        }
    }

    /// <summary>
    /// The music volume.
    /// </summary>
    float m_Volume = 0.5f;
    public float volume
    {
        get
        {
            return m_Volume;
        }
        set
        {
            m_Volume = Mathf.Clamp01(value);

            //If the volume is 0, stop the music
            if (value <= 0f)
            {
                m_MusicSource.Stop();
                m_TransitionSource.Stop();
            }
            // If there is music to play, start playing it.
            else if (!m_MusicSource.isPlaying && m_MusicSource.clip)
            {
                m_MusicSource.Play();
            }
            // If the music isn't mid transition, simply set the volume
            else if (!enabled)
            {
                m_MusicSource.volume = value;
            }
            // If the music is fading out, cap the music volume of both audio sources
            else if (m_Volume < m_MusicSource.volume)
            {
                m_MusicSource.volume = value;
                if (m_Volume < m_TransitionSource.volume)
                {
                    m_TransitionSource.volume = value;
                }
            }
            // If the music is fading in, set the transition volume to go to.
            else if (m_Volume < m_TransitionSource.volume)
            {
                m_TransitionSource.volume = value;
            }
        }
    }

    /// <summary>
    /// If the music will loop instead of ending
    /// </summary>
    public bool loop
    {
        get
        {
            return m_MusicSource.loop;
        }
        set
        {
            m_MusicSource.loop = value;
        }
    }

    /// <summary>
    /// How long the music will take to fade out.
    /// </summary>
    float m_FadeTimer = 0f;
    public float fadeTimer
    {
        get
        {
            return m_FadeTimer;
        }
        set
        {
            m_FadeTimer = value;
        }
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Setup singleton and give the initial high priority controller in the que control.
    /// </summary>
    void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
#if UNITY_EDITOR
        if (sources.Length < 2)
        {
            Debug.LogError("Music Player Needs 2 Audio Sources attached to the game object.");
            return;
        }
#endif
        m_MusicSource = sources[0];
        m_TransitionSource = sources[1];

        // Stup initial instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // If there is a controller, give it control of the music
            if (m_Controllers.Count > 0)
            {
                if (m_MusicSource.clip)
                {
                    m_MusicSource.clip = null;
                }
                if (m_TransitionSource.clip)
                {
                    m_TransitionSource.clip = null;
                }
                m_Controllers[0].Activate();
            }
        }
        // If this is a scene and a music player already exists.
        // Give it the values of this music player, then destroy this one.
        else if (instance != this)
        {
            instance.m_Controllers = m_Controllers;
            instance.loop = false;
            instance.fadeTimer = 0f;
            if (m_Controllers.Count > 0)
            {
                m_Controllers[0].Activate();
            }
            // There are no controllers, just play the music initially in this audio source.
            else if (m_MusicSource.clip)
            {
                if (m_MusicSource.clip != instance.m_MusicSource.clip)
                {
                    instance.Play(m_MusicSource.clip);
                }
            }
            // There are no controllers or music in this scene. Don't play anything.
            else
            {
                instance.Stop();
            }

            // Destroy this instance. One already exists.
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handle music fading in or out. Music Player is only enabled if there is a transition occuring.
    /// </summary>
    private void Update()
    {
        // Volume difference this frame
        float volumeChange = (m_Volume / m_FadeTimer) * Time.deltaTime;

        // Fade out the old music
        if (m_TransitionSource.clip)
        {
            m_TransitionSource.volume -= volumeChange;
            if (m_TransitionSource.volume <= 0f)
            {
                m_TransitionSource.Stop();
                m_TransitionSource.clip = null;
            }
        }

        // Fade in the new music
        if (m_MusicSource.clip && m_MusicSource.volume < m_Volume)
        {
            m_MusicSource.volume += volumeChange;
            if (m_MusicSource.volume >= m_Volume)
            {
                m_MusicSource.volume = m_Volume;
            }
        }

        // If the transition is done, turn off this function.
        if ((!m_MusicSource.clip || m_MusicSource.volume == m_Volume) && !m_TransitionSource.clip)
        {
            enabled = false;
        }
    }

    #endregion

    #region Public

    /// <summary>
    /// Plays a given clip with no fade in or out.
    /// </summary>
    public void Play(AudioClip clip)
    {
        // If asked to play nothing, stop the music
        if (clip == null)
        {
            Stop();
        }
        // If asked to play music already playing, do nothing
        else if (clip == m_MusicSource.clip)
        {
            return;
        }

        // Set the music clip to play
        m_FadeTimer = 0f;
        m_MusicSource.clip = clip;
        if (clip && m_Volume > 0)
        {
            m_MusicSource.volume = m_Volume;
            m_MusicSource.Play();
        }
        // If the volume is set to 0, don't start playing music
        else if (m_Volume <= 0f)
        {
            m_MusicSource.Stop();
        }

        // End any transitions taking place
        if (enabled)
        {
            m_TransitionSource.Stop();
            m_TransitionSource.volume = m_Volume;
            m_TransitionSource.clip = null;
            enabled = false;
        }
    }

    /// <summary>
    /// Plays a new clip and fades it in over time.
    /// </summary>
    public void Play(AudioClip clip, float transitionTime)
    {
        // If asked to play nothing, stop the music
        if (clip == null)
        {
            Stop();
        }
        // If asked to play music already playing, do nothing
        else if (clip == m_MusicSource.clip)
        {
            return;
        }

        // Set fade in time
        m_FadeTimer = transitionTime;
        if (transitionTime <= 0)
        {
            Play(clip);
            return;
        }

        // Set volume change and enable transition
        float volumeChange = (m_Volume / transitionTime) * Time.deltaTime;
        if (m_Volume > 0f)
        {
            enabled = true;

            //If there was already a transition in place and the last clip was dominant, transition from it.
            if (m_MusicSource.clip && (!m_TransitionSource.clip || m_MusicSource.volume >= m_TransitionSource.volume))
            {
                m_TransitionSource.clip = m_MusicSource.clip;
                m_TransitionSource.time = m_MusicSource.time;
                m_TransitionSource.volume = m_MusicSource.volume - volumeChange;
                m_TransitionSource.Play();
            }

            // Set the new music clip and play it
            m_MusicSource.clip = clip;
            if (clip)
            {
                m_MusicSource.volume = volumeChange;
                m_MusicSource.Play();
            }
        }
        // If the volume is 0, just set the music clip without playing it.
        else
        {
            m_MusicSource.clip = clip;
        }
    }

    /// <summary>
    /// Stops all music, sets the clips to null, and disables any transitions occuring.
    /// </summary>
    public void Stop()
    {
        m_MusicSource.Stop();
        m_TransitionSource.Stop();
        m_MusicSource.clip = null;
        m_TransitionSource.clip = null;
        enabled = false;
    }

    /// <summary>
    /// Adds a controller to the priority que.
    /// </summary>
    public void Add (MusicController controller)
    {
        // If there weren't any controllers, the new one becomes active.
        if (m_Controllers.Count == 0)
        {
            m_Controllers.Add(controller);
            controller.Activate();
            return;
        }
        // If the controller is already in the que, do not add it.
        else if (m_Controllers.Contains(controller))
        {
            return;
        }

        // Insert the controller into the priority que
        for (int i = 0; i < m_Controllers.Count; ++i)
        {
            if (controller.importance > m_Controllers[i].importance)
            {
                // If this is the highest priority controller, give it control.
                if (i == 0)
                {
                    if (m_Controllers[0])
                    {
                        m_Controllers[0].Deactivate();
                    }
                    m_Controllers.Insert(i, controller);
                    controller.Activate();
                    return;
                }

                // Otherwise just insert it into the que.
                m_Controllers.Insert(i, controller);
                return;
            }
        }

        // If this is the lowest priority que, just add it to the end.
        m_Controllers.Add(controller);
    }

    /// <summary>
    /// Removes a controller from the que.
    /// If the controller was active, deactivate it and activate the next controller.
    /// </summary>
    public void Remove(MusicController controller)
    {
        if (m_Controllers.Count == 0)
        {
            return;
        }

        // Find where the controller is in the que
        // If the controller is not in the que do nothing.
        short index = -1;
        for (short i = 0; i < m_Controllers.Count; ++i)
        {
            if (m_Controllers[i] == controller)
            {
                index = i;
            }
        }

        // If the controller was active, deactivate it and activate the next controller.
        if (index == 0)
        {
            m_Controllers[index].Deactivate();
            m_Controllers.RemoveAt(index);
            while (m_Controllers.Count > 0)
            {
                if (m_Controllers[0] != null)
                {
                    m_Controllers[0].Activate();
                    return;
                }
                else
                {
                    m_Controllers.RemoveAt(0);
                }
            }

            // If there now are no controllers, stop any transitions
            if (m_FadeTimer > 0f && m_Volume > 0f && m_MusicSource.clip)
            {
                enabled = true;
                if (!m_TransitionSource.clip || m_MusicSource.volume >= m_TransitionSource.volume)
                {
                    m_TransitionSource.clip = m_MusicSource.clip;
                    m_TransitionSource.time = m_MusicSource.time;
                    m_TransitionSource.volume = m_MusicSource.volume - (m_Volume / m_FadeTimer) * Time.deltaTime;
                    m_TransitionSource.Play();
                }
                m_MusicSource.clip = null;
                m_MusicSource.volume = 0f;
            }
            // If there are no transitions, just stop the music
            else
            {
                Stop();
            }
        }
        // If the controller wasn't active, just remove it.
        else if (index > 0)
        {
            m_Controllers.RemoveAt(index);
        }
    }

    #endregion

    #region Private

    /// <summary>
    /// When disabled the music timer is reset.
    /// </summary>
    private void OnDisable()
    {
        m_FadeTimer = 0f;
    }

    #endregion

    #region Editor

#if UNITY_EDITOR

    //Make sure that 2 audio sources are attached and the transition defaults to off.
    private void Reset()
    {
        enabled = false;

        AudioSource[] sources = GetComponents<AudioSource>();
        AudioSource addedSource;
        if (sources.Length < 2)
        {
            for (int i = 0; i < 2 - sources.Length; ++i)
            {
                addedSource = gameObject.AddComponent<AudioSource>();
                if (addedSource)
                {
                    addedSource.playOnAwake = false;
                }
                addedSource = null;
            }
        }
    }

#endif

    #endregion
}
