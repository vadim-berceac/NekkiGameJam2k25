using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [field: SerializeField] public float MaxSeconds { get; set; }
    [field: SerializeField] public float Penalty { get; set; }
    [field: SerializeField] public Text Text { get; set; }
    private float _currentSeconds;
    private const string TimeFormat = "{0:00}:{1:00}";
    public static Action OnTimerEnd;
    public static Action OnWin;
    private bool _isTimerEnd;

    private void Start()
    {
        _currentSeconds = MaxSeconds;
        PlayerMovement.OnWrongChose += OnPenalty;
        PlayerMovement.OnRightChose += OnVictory;
        OnTimerEnd += OnTimerEnded;
    }

    private void FixedUpdate()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (_isTimerEnd)
        {
            return;
        }
        _currentSeconds -= Time.deltaTime;

        var minutes = Mathf.FloorToInt(_currentSeconds / 60f);
        var seconds = Mathf.FloorToInt(_currentSeconds % 60f);

        Text.text = string.Format(TimeFormat, minutes, seconds);

        if (_currentSeconds <= 0)
        {
            OnTimerEnd?.Invoke();
            _isTimerEnd = true;
            Text.text = string.Format(TimeFormat, 0, 0);
        }
    }

    private void OnPenalty()
    {
      _currentSeconds -= Penalty;   
    }

    private void OnVictory()
    {
        OnWin?.Invoke();
        _isTimerEnd = true;
        Debug.Log("Victory!");
    }

    private void OnTimerEnded()
    {
        Debug.Log("Timer Ended!");
    }

    private void OnDisable()
    {
        PlayerMovement.OnWrongChose -= OnPenalty;
        PlayerMovement.OnRightChose -= OnVictory;
        OnTimerEnd -= OnTimerEnded;
    }
}
