using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioSource AudioClip;
    public ParticleSystem ParticleSystem;
    public Timer Timer;

    public float Duration { get => ParticleSystem.main.duration; }

    private void Awake()
    {
        Timer = gameObject.AddComponent<Timer>();
        Timer.StartListening((obj) => Play());
        Timer.FinishListening((obj) => Stop());
        Stop();
    }

    public void Play(float second, Vector3 at)
    {
        transform.position = at;
        Play(second);
    }

    public void Play(float second)
    {
        if (gameObject.activeSelf) Timer.Play(second);
    }

    public void Play()
    {
        ParticleSystem.Play();
        AudioClip.Play();
    }

    public void Stop()
    {
        ParticleSystem.Stop();
        AudioClip.Stop();
    }
}
