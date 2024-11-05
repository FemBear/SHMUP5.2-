using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public float fadeDuration = 1.0f;

    private new void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SwitchMusic(scene.name);
    }

    public void PlaySound(AudioClip clip, AudioSource source)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null)
        {
            StartCoroutine(FadeInMusic(musicClip));
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic());
        }
    }

    public void SwitchMusic(string sceneName)
    {
        AudioClip newMusic = Resources.Load<AudioClip>($"Sound/Music/{sceneName}");
        if (newMusic != null)
        {
            StartCoroutine(SwitchMusicWithFade(newMusic));
        }
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }

    private IEnumerator FadeInMusic(AudioClip newMusic)
    {
        musicSource.clip = newMusic;
        musicSource.Play();
        musicSource.volume = 0f;

        while (musicSource.volume < 0.5f)
        {
            musicSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = 0.5f;
    }

    private IEnumerator SwitchMusicWithFade(AudioClip newMusic)
    {
        yield return FadeOutMusic();
        yield return FadeInMusic(newMusic);
    }

    public void Initialize()
    {
        GameObject audioSourceObject = new GameObject("AudioSources");
        audioSourceObject.transform.SetParent(transform);

        musicSource = CreateAudioSource(audioSourceObject.transform, "MusicSource", true, 0.5f);
        sfxSource = CreateAudioSource(audioSourceObject.transform, "SFXSource", false, 0.5f);

        SwitchMusic(SceneManager.GetActiveScene().name);
    }

    private AudioSource CreateAudioSource(Transform parent, string name, bool loop, float volume)
    {
        GameObject sourceObject = new GameObject(name);
        sourceObject.transform.SetParent(parent);
        AudioSource source = sourceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        source.volume = volume;
        return source;
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
        sfxSource.volume = volume;
    }

    public void Mute(bool isMuted)
    {
        musicSource.mute = isMuted;
        sfxSource.mute = isMuted;
    }

    public void PlaySFX(AudioClip clip)
    {
        PlaySound(clip, sfxSource);
    }
}
