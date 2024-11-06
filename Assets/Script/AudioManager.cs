using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public float fadeDuration = 1.0f;
    private static bool isInitialized = false;

    new void Awake()
    {
        base.Awake();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
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
        Debug.Log("Scene loaded: " + scene.name);
        if (!isInitialized)
        {
            Initialize();
        }
        else
        {
            SwitchMusic(scene.name);
        }
    }

    public void PlaySound(AudioClip clip, AudioSource source)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        PlaySound(clip, sfxSource);
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
            Debug.Log("Switching music to " + newMusic.name);
            StartCoroutine(SwitchMusicWithFade(newMusic));
        }
        else
        {
            Debug.LogError("Music clip not found for scene: " + sceneName);
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
        Debug.Log("Starting music switch fade-out.");
        yield return FadeOutMusic();

        Debug.Log("Starting music switch fade-in.");
        yield return FadeInMusic(newMusic);
    }

    public void Initialize()
    {
        if (isInitialized) return;

        GameObject audioSourceObject = new GameObject("AudioSources");
        audioSourceObject.transform.SetParent(transform);

        musicSource = CreateAudioSource(audioSourceObject.transform, "MusicSource", true, 0.5f);
        sfxSource = CreateAudioSource(audioSourceObject.transform, "SFXSource", false, 0.5f);

        Debug.Log("AudioManager initialized. Current scene: " + SceneManager.GetActiveScene().name);
        SwitchMusic(SceneManager.GetActiveScene().name);

        isInitialized = true;
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
}
