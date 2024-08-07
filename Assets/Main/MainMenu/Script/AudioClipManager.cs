using UnityEngine;

public class AudioClipManager : MonoBehaviour {
    public float Volume = 1f;
    [SerializeField]
    private AudioClip[] audioClips = {};
    [SerializeField]
    private int defaultClip = 0;
    public AudioSource AudioSource { get; private set; }

    void Awake()
    {
        AudioSource = gameObject.AddComponent<AudioSource>();
        AudioSource.loop = true;
        AudioSource.volume = Volume;
        if (defaultClip >= 0){
            PlayAudio(audioClips[defaultClip].name, true);
        }
    }

    public void PlayAudio(string clipName, bool loop)
    {
        AudioClip clip = GetAudioClipByName(clipName);
        if (clip != null)
        {
            AudioSource.Stop();
            AudioSource.clip = clip;
            AudioSource.loop = loop;
            AudioSource.Play();
        }
        else
        {
            throw new System.Exception("Audio clip not found: " + clipName);
        }
    }

    private AudioClip GetAudioClipByName(string clipName)
    {
        foreach (var clip in audioClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }
}
