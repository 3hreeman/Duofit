using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Duofit.Pages;

public partial class StopwatchPage : ContentPage
{
    readonly Stopwatch _stopwatch = new Stopwatch();
    readonly ObservableCollection<LapItem> _laps = new ObservableCollection<LapItem>();
    IDispatcherTimer _uiTimer = null;
    TimeSpan _lastLapElapsed = TimeSpan.Zero;
    int _lapCounter = 0;

    public StopwatchPage()
    {
        InitializeComponent();

        LapCollectionView.ItemsSource = _laps;
        UpdateUi(TimeSpan.Zero);
        UpdateButtons();
    }

    void StartTimer()
    {
        if (_uiTimer == null)
        {
            _uiTimer = Dispatcher.CreateTimer();
            _uiTimer.Interval = TimeSpan.FromMilliseconds(50);
            _uiTimer.Tick += OnUiTimerTick;
        }
        _stopwatch.Start();
        _uiTimer.Start();
        UpdateButtons();
    }

    void PauseTimer()
    {
        _stopwatch.Stop();
        if (_uiTimer != null)
            _uiTimer.Stop();
        UpdateButtons();
    }

    void ResetTimer()
    {
        _stopwatch.Reset();
        _laps.Clear();
        _lapCounter = 0;
        _lastLapElapsed = TimeSpan.Zero;
        UpdateUi(TimeSpan.Zero);
        UpdateButtons();
    }

    void OnUiTimerTick(object sender, EventArgs e)
    {
        UpdateUi(_stopwatch.Elapsed);
    }

    void UpdateUi(TimeSpan elapsed)
    {
        UpdateCurrentLapTime(elapsed);
        UpdateLapTime(elapsed);
    }
    
    void UpdateCurrentLapTime(TimeSpan elapsed)
    {
        string mainText = $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}.{elapsed.Milliseconds / 10:D2}";
        MainTimeLabel.Text = mainText;
    }

    void UpdateLapTime(TimeSpan elapsed)
    {
        var lapSpan = elapsed - _lastLapElapsed;
        string lapText = $"Lap: {lapSpan.Hours:D2}:{lapSpan.Minutes:D2}:{lapSpan.Seconds:D2}.{lapSpan.Milliseconds / 10:D2}";
        LapTimeLabel.Text = lapText;
    }

    void UpdateButtons()
    {
        if (_stopwatch.IsRunning)
        {
            StartStopButton.Text = "PAUSE";
            StartStopButton.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#FFD600"); // vivid yellow
            LapButton.Text = "LAP";
        }
        else
        {
            StartStopButton.Text = "START";
            StartStopButton.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#FF9800");
            LapButton.Text = (_stopwatch.Elapsed.TotalMilliseconds > 0 || _laps.Count > 0) ? "RESET" : "LAP";
        }
    }

    void OnStartStopClicked(object sender, EventArgs e)
    {
        if (!_stopwatch.IsRunning)
        {
            // 시작
            StartTimer();
        }
        else
        {
            // 일시정지
            PauseTimer();
        }
    }

    void OnLapClicked(object sender, EventArgs e)
    {
        if (!_stopwatch.IsRunning)
        {
            // 스톱 상태에서 LAP(또는 RESET) : 리셋 동작
            ResetTimer();
            return;
        }

        AddLap();
    }

    void AddLap()
    {
        var elapsed = _stopwatch.Elapsed;
        var lapTime = elapsed - _lastLapElapsed;
        _lapCounter++;
        _lastLapElapsed = elapsed;

        _laps.Insert(0, new LapItem
        {
            Id = _lapCounter,
            LapTime = lapTime,
            ElapsedTime = elapsed
        });

        // Limit number of laps shown to reasonable amount (e.g., 200)
        if (_laps.Count > 200)
            _laps.RemoveAt(_laps.Count - 1);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // 타이머 정리
        if (_uiTimer != null)
        {
            _uiTimer.Stop();
            _uiTimer = null;
        }
        _stopwatch.Stop();
    }

    class LapItem
    {
        public int Id { get; set; }
        public TimeSpan LapTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}
