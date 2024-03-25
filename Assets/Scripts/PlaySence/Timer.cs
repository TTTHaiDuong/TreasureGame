using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Timer : MonoBehaviour, INetworkSerializable
{
    private TimerHandling _WhenStart;
    private TimerHandling _Tick;
    private TimerHandling _WhenFinish;

    public event TimerHandling WhenStart;
    public event TimerHandling Tick;
    public event TimerHandling WhenFinish;

    public delegate void TimerHandling(object obj);

    public object StartObj;
    public object TickObj;
    public object FinishObj;

    public bool IsRunning;
    public float Time;
    public float Delta = 1f;

    public void StartListening(TimerHandling start)
    {
        if (!DelegateTool.ExistInside(WhenStart, start)) WhenStart += start;
    }
    public void TickListening(TimerHandling tick)
    {
        if (!DelegateTool.ExistInside(Tick, tick)) Tick += tick;
    }
    public void FinishListening(TimerHandling finish)
    {
        if (!DelegateTool.ExistInside(WhenFinish, finish)) WhenFinish += finish;
    }
    public void StartRecall(TimerHandling start) => WhenStart -= start;  
    public void TickRecall(TimerHandling tick) => Tick -= tick;
    public void FinishRecall(TimerHandling finish) => WhenFinish -= finish;

    public void AssignAndRecall(TimerHandling whenStart, TimerHandling tick, TimerHandling whenFinish)
    {
        if (IsRunning) return;

        _WhenStart = whenStart;
        WhenStart += whenStart;

        _Tick = tick;
        Tick += tick;

        _WhenFinish = whenFinish;
        WhenFinish += whenFinish;

        WhenFinish += Recall_;
    }
    private void Recall_(object obj) => Recall();

    public void Recall()
    {
        WhenStart -= _WhenStart;
        Tick -= _Tick;
        WhenFinish -= _WhenFinish;

        WhenFinish -= Recall_;
    }

    public void Play(float second = -1)
    {
        if (!IsRunning)
        {
            if (second >= 0)
            {
                WhenStart?.Invoke(StartObj);
                Time = second;
            }
            IsRunning = true;
            StartCoroutine(Counting());
        }
    }
    private IEnumerator Counting()
    {
        
        while (Time > 0 && IsRunning)
        {
            Time -= Delta;
            Tick?.Invoke(TickObj);
            yield return new WaitForSecondsRealtime(Delta);
        }
        if (IsRunning)
        {
            Tick?.Invoke(TickObj);
            WhenFinish?.Invoke(FinishObj);
        }
        IsRunning = false;
    }

    public void Stop() => IsRunning = false;

    public void Break()
    {
        Stop();
        Time = 0;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref IsRunning);
        serializer.SerializeValue(ref Time);
        serializer.SerializeValue(ref Delta);
    }

    public void NetworkDeserialize(Timer timer)
    {
        timer.Delta = Delta;
        timer.Time = Time;
        if (timer.IsRunning) timer.Play(Time);
        else timer.Stop();
    }
}

public class DelegateTool
{
    public static bool ExistInside(Delegate group, Delegate member)
    {
        if (group == null || member == null) return false;
        foreach (Delegate check in group.GetInvocationList())
            if (check.Method == member.Method) return true;
        return false;
    }
}