using System;
using System.Collections;
using TreasureGame;
using UnityEngine;

/// <summary>
/// Dùng để đếm ngược thời gian.
/// </summary>
//public class Timer : MonoBehaviour
//{
//    private CountingTime _Start;
//    private CountingTime _Tick;
//    private CountingTime _Finish;
//    private CountingTime _Break;

//    //
//    // Các sự kiện:
//    // WhenStart: sự kiện khi timer bắt đầu đếm.
//    // Tick: sự kiện mỗi lần tick của timer.
//    // WhenFinish: sự kiện khi timer kết thúc đếm.
//    // WhenBreak: sự kiện khi timer bị ngắt dừng.
//    //
//    public event CountingTime WhenStart;
//    public event CountingTime Tick;
//    public event CountingTime WhenFinish;
//    public event CountingTime WhenBreak;

//    // 
//    // IsTimerRunning: dùng để ngăn chặn nhiều phương thức Counting (asynchronou) chạy.
//    //              => false: cho phép Counting chạy.
//    //              => true: ngăn chặn gọi lại hàm Counting.
//    //
//    // StopRunning: dùng để dừng timer.
//    //
//    public bool IsRunning { get; private set; }
//    private bool StopRunning;

//    //
//    // TimerRemaining: thời gian còn lại.
//    // Ban đầu TimerRemaining có giá trị lớn nhất là khoảng thời gian mà ta muốn timer đếm.
//    // Qua mỗi lần tick TimerRemaining sẽ bị trừ đi một khoảng là Step.
//    // Khi nào TimerRemaining bé hơn hoặc bằng 0 thì timer dừng đếm.
//    //
//    // Step: thời gian giữa mỗi tick.
//    //
//    // StartParameter: tham số của phương thức khi sự kiện WhenStart được gọi.
//    // TickParameter: tham số của phương thức khi sự kiện Tick được gọi.
//    // FinishParameter: tham số của phương thức khi sự kiện WhenFinish được gọi.
//    //
//    public float TimeRemaining;
//    public float Step;
//    public object StartParameter;
//    public object TickParameter;
//    public object FinishParameter;
//    public object BreakParameter;

//    // Start is called before the first frame update
//    void Start()
//    {
//        Step = 1f;
//    }

//    /// <summary>
//    /// Đặt parameter cho các phương thức khi sự kiện được gọi.
//    /// </summary>
//    public void SetParameter(object whenStart, object whenTick, object whenFinish, object whenBreak)
//    {
//        StartParameter = whenStart;
//        TickParameter = whenTick;
//        FinishParameter = whenFinish;
//        BreakParameter = whenBreak;
//    }

//    /// <summary>
//    /// Bắt đầu đếm của timer.
//    /// </summary>
//    /// <param name="interval">Khoảng thời gian mà timer đếm.</param>
//    public void Counting(float interval)
//    {
//        TimeRemaining = interval;
//        StopRunning = false;
//        if (!IsRunning)
//        {
//            IsRunning = true;
//            WhenStart?.Invoke(StartParameter);
//            StartCoroutine(Counting());
//        }
//    }
//    private IEnumerator Counting()
//    {
//        while (TimeRemaining > 0 && !StopRunning)
//        {
//            Tick?.Invoke(TickParameter);
//            TimeRemaining--;
//            yield return new WaitForSeconds(Step);
//        }
//        if (!StopRunning)
//        {
//            Tick?.Invoke(TickParameter);
//            WhenFinish?.Invoke(FinishParameter);
//        }
//        IsRunning = false;
//    }

//    public void Continue()
//    {
//        StopRunning = false;
//        if (!IsRunning)
//        {
//            IsRunning = true;
//            StartCoroutine(Counting());
//        }
//    }

//    public void Resume()
//    {
//        StopRunning = true;
//    }

//    /// <summary>
//    /// Dừng timer.
//    /// </summary>
//    public void Break()
//    {
//        StopRunning = true;
//        TimeRemaining = 0;
//        WhenBreak?.Invoke(BreakParameter);
//    }

//    public void Counting(float second, CountingTime start, CountingTime tick, CountingTime finish, CountingTime @break = null)
//    {
//        _Start = start;
//        _Tick = tick;
//        _Finish = finish;
//        _Break = @break;

//        WhenStart += start;
//        Tick += tick;
//        WhenFinish += finish;
//        WhenFinish += ClearAllEvent;
//        if (@break != null) WhenBreak += @break;

//        Counting(second);
//    }

//    public void ClearAllEvent(object obj)
//    {
//        WhenStart -= _Start;
//        Tick -= _Tick;
//        WhenFinish -= _Finish;
//        WhenBreak -= _Break;
//    }
//}


public class Timer : MonoBehaviour
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
    public float Delta = 0.1f;

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
            yield return new WaitForSeconds(Delta);
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
}

public class DelegateTool
{
    public static bool ExistInside(Delegate group, Delegate member)
    {
        if (group == null) return false;
        foreach (Delegate check in group.GetInvocationList())
            if (check.Method == member.Method) return true;
        return false;
    }
}