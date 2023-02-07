using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource soundEffectSource;  // Drag a reference to the audio source which will play the sound effects.
    public static AudioManager i = null; // Allows other scripts to call functions from SoundManager. 
    [SerializeField] AudioClip[] soundEffectClips; // Array of sound effect clips to play.

     float lowPitchRange = .95f;              // The lowest a sound effect will be randomly pitched.
     float highPitchRange = 1.05f;            // The highest a sound effect will be randomly pitched.

    void Awake()
    {
        // Check if there is already an instance of SoundManager
        if (i == null)
            // if not, set it to this.
            i = this;
        // If instance already exists:
        else if (i != this)
            // Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        // Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    // Play a single clip through the sound effect source.
    public void PlaySoundEffect(AudioClip clip)
    {
        soundEffectSource.clip = clip;
        soundEffectSource.Play();
    }

    // Play a random clip from the array of sound effect clips.
    public void PlayRandomSoundEffect(AudioClip[] audioClips)
    {
        // Choose a random index from the array of clips.
        int randomIndex = Random.Range(0, audioClips.Length);

        // Play the clip at the chosen index.
        PlaySoundEffect(audioClips[randomIndex]);
    }

    // Play a sound effect based on the file name in the Resource Folder.
    public void PlaySoundEffectByName(string name)
    {
        // Load the audio clip with the specified name from the "Sound Effects" folder in the assets directory.
        AudioClip clip = Resources.Load<AudioClip>("Sound Effects/" + name);

        // If the clip was not found, log an error and return.
        if (clip == null)
        {
            Debug.LogError("AudioManager: Sound effect not found with name '" + name + "'");
            return;
        }

        // Play the audio clip.
        PlaySoundEffect(clip);
    }

    public void PlaySoundEffectByName(string name, bool randomPitch)
    {
        // Load the audio clip with the specified name from the "Sound Effects" folder in the assets directory.
        AudioClip clip = Resources.Load<AudioClip>("Sound Effects/" + name);

        if (randomPitch)
        {
            soundEffectSource.pitch = Random.Range(lowPitchRange, highPitchRange);
        }

        // If the clip was not found, log an error and return.
        if (clip == null)
        {
            Debug.LogError("AudioManager: Sound effect not found with name '" + name + "'");
            return;
        }

        // Play the audio clip.
        PlaySoundEffect(clip);
    }


    // RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(params AudioClip[] clips)
    {
        // Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        // Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        // Set the pitch of the audio source to the randomly chosen pitch.
        soundEffectSource.pitch = randomPitch;

        // Set the clip to the clip at our randomly chosen index.
        soundEffectSource.clip = clips[randomIndex];

        // Play the clip.
        soundEffectSource.Play();
    }
}
