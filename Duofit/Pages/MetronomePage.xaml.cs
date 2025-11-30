using System.Diagnostics;

namespace Duofit.Pages;

public partial class MetronomePage : ContentPage
{
    int _bpm = 120;
    IDispatcherTimer _beatTimer;
    Stopwatch _tapStopwatch = new Stopwatch();
    List<long> _tapIntervals = new List<long>();
    int _beatCount = 0;

    public MetronomePage()
    {
        InitializeComponent();

        BpmSlider.Value = _bpm;
        BpmLabel.Text = _bpm.ToString();

        _beatTimer = Dispatcher.CreateTimer();
        _beatTimer.Interval = TimeSpan.FromMilliseconds(60000 / _bpm);
        _beatTimer.Tick += OnBeatTick;
    }

    void OnBpmSliderChanged(object sender, ValueChangedEventArgs e)
    {
        _bpm = (int)Math.Round(e.NewValue);
        BpmLabel.Text = _bpm.ToString();
        UpdateBeatTimer();
    }

    void UpdateBeatTimer()
    {
        if (_beatTimer != null)
        {
            _beatTimer.Stop();
            _beatTimer.Interval = TimeSpan.FromMilliseconds(60000.0 / Math.Max(1, _bpm));
            // if running, restart to apply new interval
            if (StartStopButton.Text == "STOP")
                _beatTimer.Start();
        }
    }

    void OnStartStopClicked(object sender, EventArgs e)
    {
        if (StartStopButton.Text == "START")
        {
            StartStopButton.Text = "STOP";
            StartStopButton.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#FFD600");
            _beatCount = 0;
            _beatTimer.Start();
        }
        else
        {
            StartStopButton.Text = "START";
            StartStopButton.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#FF9800");
            _beatTimer.Stop();
            ResetPulseVisual();
        }
    }

    void OnTapTempoClicked(object sender, EventArgs e)
    {
        var now = _tapStopwatch.ElapsedMilliseconds;
        if (!_tapStopwatch.IsRunning)
        {
            _tapStopwatch.Restart();
            _tapIntervals.Clear();
            return;
        }

        long last = now;
        var elapsed = _tapStopwatch.ElapsedMilliseconds;
        if (_tapIntervals.Count == 0)
        {
            _tapIntervals.Add(elapsed);
            _tapStopwatch.Restart();
            return;
        }

        // accumulate intervals
        _tapIntervals.Add(elapsed);
        _tapStopwatch.Restart();

        // compute average
        if (_tapIntervals.Count >= 2)
        {
            double avgMs = 0;
            for (int i = 1; i < _tapIntervals.Count; i++)
            {
                avgMs += (_tapIntervals[i]);
            }
            avgMs /= Math.Max(1, _tapIntervals.Count - 1);
            var bpm = (int)Math.Round(60000.0 / avgMs);
            bpm = Math.Max(30, Math.Min(240, bpm));
            _bpm = bpm;
            BpmSlider.Value = _bpm;
            BpmLabel.Text = _bpm.ToString();
            UpdateBeatTimer();
        }
    }

    void OnBeatTick(object sender, EventArgs e)
    {
        // Visual pulse: quick scale/opacity animation
        _beatCount++;
        PulseOnce();
        // TODO: play click sound or vibrate
    }

    void PulseOnce()
    {
        // Simple animation: toggle opacity/scale
        PulseBox.ScaleTo(1.08, 80, Easing.CubicOut);
        PulseBox.FadeTo(0.18, 80);
        // replace obsolete Device.StartTimer with Dispatcher
        PulseBox.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(120), () =>
        {
            PulseBox.ScaleTo(1.0, 100, Easing.CubicIn);
            PulseBox.FadeTo(0.08, 100);
            return false;
        });
    }

    void ResetPulseVisual()
    {
        PulseBox.Scale = 1.0;
        PulseBox.Opacity = 0.08;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_beatTimer != null)
            _beatTimer.Stop();
    }
}
