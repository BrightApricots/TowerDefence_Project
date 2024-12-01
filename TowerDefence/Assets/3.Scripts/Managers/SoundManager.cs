using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    // SoundManager.Instance.Play("BGM_Name", SoundManager.Sound.Bgm); 사용예시
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount // sound Enmum의 종류에 따라 가변적으로 늘어남
    }

    private static SoundManager instance;
    public static SoundManager Instance {
        get 
        {
            //SoundManager 첫 호출시 프로퍼티 접근
            if (instance == null)
            {
                //instance 생성 및 @Sound 오브젝트에 SoundManger 할당 후 DontDestroyOnLoad처리
                GameObject go = GameObject.Find("@Sound");
                if (go == null)
                {
                    go = new GameObject { name = "@Sound" };
                    go.AddComponent<SoundManager>(); // 첫 호출시 이 시점에서 Awake 실행됨
                }
                else
                {
                    instance = go.GetComponent<SoundManager>();
                }
                DontDestroyOnLoad(go);
            }
            return instance; 
        } 
    }

    private AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount]; // 사운드 데이터 출력하는곳 ex) bgm, effect..
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>(); //여기에 사운드 데이터 저장됨

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        string[] soundNames = System.Enum.GetNames(typeof(Sound));
        for (int i = 0; i < soundNames.Length - 1; i++)
        {
            GameObject go = new GameObject { name = soundNames[i] };
            _audioSources[i] = go.AddComponent<AudioSource>();
            go.transform.parent = transform;
        }

        _audioSources[(int)Sound.Bgm].loop = true; 
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.volume = 0.7f; // BGM 소리크기 수정하는곳
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    private AudioClip GetOrAddAudioClip(string path, Sound type = Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
        {
            path = $"Sounds/{path}";
        }

        AudioClip audioClip = null;

        if (type == Sound.Bgm)
        {
            audioClip = Resources.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                if (audioClip != null)
                {
                    _audioClips.Add(path, audioClip);
                }
                else
                {
                    Debug.LogError($"AudioClip Missing! Path: {path}");
                    // 사용 가능한 모든 클립 출력
                    AudioClip[] allClips = Resources.LoadAll<AudioClip>("Sounds");
                    Debug.Log("Available clips in Sounds folder:");
                    foreach (var clip in allClips)
                    {
                        Debug.Log($"- {clip.name}");
                    }
                }
            }
        }

        return audioClip;
    }

    public void Stop(Sound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.Stop();
    }

    public void SetVolume(Sound type, float volume)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.volume = volume;
    }

    public float GetVolume(Sound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        return audioSource.volume;
    }
}   
