using UnityEngine;
using System.Collections;

public class AmbiencePlayer : MonoBehaviour
{
    [field: SerializeField] public AudioClip[] AmbientMusic { get; set; }
    [field: SerializeField] public AudioClip AmbientSound { get; set; }
    [field: SerializeField] public float CrossFadeDuration { get; set; } = 2f;
    [field: SerializeField] public float TargetVolume { get; set; } = 0.8f;

    private AudioSource _soundSource;
    private AudioSource _musicSourceA;
    private AudioSource _musicSourceB;
    private AudioSource _activeMusicSource;

    private void Awake()
    {
        _soundSource = gameObject.AddComponent<AudioSource>();
        _musicSourceA = gameObject.AddComponent<AudioSource>();
        _musicSourceB = gameObject.AddComponent<AudioSource>();

        _soundSource.clip = AmbientSound;
        _soundSource.loop = true;
        _soundSource.playOnAwake = false;

        _musicSourceA.loop = false;
        _musicSourceB.loop = false;

        _activeMusicSource = _musicSourceA;
    }

    private void Start()
    {
        if (AmbientSound != null)
            _soundSource.Play();

        PlayRandomMusic();
    }

    private void Update()
    {
        if (!_activeMusicSource.isPlaying)
        {
            PlayRandomMusic();
        }
    }

    private void PlayRandomMusic()
    {
        if (AmbientMusic == null || AmbientMusic.Length == 0)
            return;

        var randomClip = AmbientMusic[Random.Range(0, AmbientMusic.Length)];
        var nextSource = (_activeMusicSource == _musicSourceA) ? _musicSourceB : _musicSourceA;

        nextSource.clip = randomClip;
        nextSource.volume = 0f;
        nextSource.Play();
        
        var normalizedVolume = NormalizeClipVolume(randomClip, TargetVolume);
        StartCoroutine(CrossFade(_activeMusicSource, nextSource, normalizedVolume));

        _activeMusicSource = nextSource;
    }

    private IEnumerator CrossFade(AudioSource from, AudioSource to, float targetVol)
    {
        var timer = 0f;
        while (timer < CrossFadeDuration)
        {
            timer += Time.deltaTime;
            var t = timer / CrossFadeDuration;

            from.volume = Mathf.Lerp(targetVol, 0f, t);
            to.volume = Mathf.Lerp(0f, targetVol, t);

            yield return null;
        }

        from.Stop();
        to.volume = targetVol;
    }

    private static float NormalizeClipVolume(AudioClip clip, float targetVol)
    {
        if (clip == null) return targetVol;

        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        var sum = 0f;

        foreach (var sample in samples)
        {
            sum += Mathf.Abs(sample);
        }

        var avg = sum / samples.Length;
        if (avg <= 0.0001f) return targetVol;
        
        var gain = targetVol / avg;
        return Mathf.Clamp(gain * targetVol, 0f, 1f);
    }
}
