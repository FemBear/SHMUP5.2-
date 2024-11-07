using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class AudioManager : Singleton<AudioManager>
{
    #region Variables
    public AudioSource m_MusicSource;
    public AudioSource m_SfxSource;
    public float m_FadeDuration = 0.5f;
    private static bool m_IsInitialized = false;
    #endregion

    #region Basics
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
        Time.timeScale = 1;
        if (!m_IsInitialized)
        {
            Initialize();
        }
        else
        {
            SwitchMusic(scene.name);
        }
    }
    #endregion

    #region SwitchMusic
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

    private IEnumerator SwitchMusicWithFade(AudioClip newMusic)
    {
        Debug.Log("Starting music switch fade-out.");
        float startVolume = m_MusicSource.volume;

        while (m_MusicSource.volume > 0)
        {
            m_MusicSource.volume -= Time.deltaTime / m_FadeDuration;
            yield return null;
        }

        m_MusicSource.Stop();
        m_MusicSource.volume = startVolume;

        yield return StartCoroutine(FadeInMusic(newMusic));
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = m_MusicSource.volume;

        while (m_MusicSource.volume > 0)
        {
            m_MusicSource.volume -= startVolume * Time.deltaTime / m_FadeDuration;
            yield return new WaitForEndOfFrame();
        }

        m_MusicSource.Stop();
        m_MusicSource.volume = startVolume;
    }

    private IEnumerator FadeInMusic(AudioClip newMusic)
    {
        m_MusicSource.clip = newMusic;
        m_MusicSource.Play();
        m_MusicSource.volume = 0f;

        while (m_MusicSource.volume < 0.5f)
        {
            m_MusicSource.volume += Time.deltaTime / m_FadeDuration;
            yield return null;
        }

        m_MusicSource.volume = 0.5f;
    }
    #endregion

    #region Utility
    public void PlaySound(AudioClip clip, AudioSource source)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        PlaySound(clip, m_SfxSource);
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
        if (m_MusicSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic());
        }
    }

    public void SetVolume(float volume)
    {
        m_MusicSource.volume = volume;
        m_SfxSource.volume = volume;
    }

    public void Mute(bool isMuted)
    {
        m_MusicSource.mute = isMuted;
        m_SfxSource.mute = isMuted;
    }
    #endregion

    #region Setup
    private void Initialize()
    {
        if (m_IsInitialized) return;

        GameObject audioSourceObject = new GameObject("AudioSources");
        audioSourceObject.transform.SetParent(transform);

        m_MusicSource = CreateAudioSource(audioSourceObject.transform, "MusicSource", true, 0.5f);
        m_SfxSource = CreateAudioSource(audioSourceObject.transform, "SFXSource", false, 0.5f);

        Debug.Log("AudioManager initialized. Current scene: " + SceneManager.GetActiveScene().name);
        SwitchMusic(SceneManager.GetActiveScene().name);

        m_IsInitialized = true;
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
    #endregion
}
