using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Explosion : MonoBehaviour
//{
//    public AudioSource AudioClip;
//    public ParticleSystem ParticleSystem;
//    public Timer Timer;

//    public float Duration { get => ParticleSystem.main.duration; }

//    private void Awake()
//    {
//        Timer = gameObject.AddComponent<Timer>();
//        Timer.WhenStart += StartTimer;
//        Timer.WhenFinish += FinishTimer;
//    }

//    private void Start()
//    {
//        Stop();
//    }

//    private void StartTimer(object obj) => Play();
//    private void FinishTimer(object obj) => Stop();

//    public void Play(Vector3 at)
//    {
//        transform.position = at;
//        Play();
//    }

//    public void Play()
//    {
//        ParticleSystem.Play();
//        AudioClip.Play();
//    }

//    public void Stop()
//    {
//        ParticleSystem.Stop();
//        AudioClip.Stop();
//    }

//    public void Play(float second) => Timer.Counting(second);
//}

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

    public void Play(float second) => Timer.Play(second);

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
