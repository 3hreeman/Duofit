using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;

namespace Duofit.Pages
{
    public partial class TimerPage : ContentPage
    {
        IDispatcherTimer _timer;
        
        // phases: prep -> workout -> end (in seconds)
        int _prep = 10;
        int _workout = 60;
        int _end = 10;

        List<int> _phaseDurations;
        int _currentPhaseIndex = 0; // 0:prep,1:workout,2:end
        int _phaseRemaining = 0;
        bool _isRunning = false;

        float _progress = 0f; // 0..1 across current phase

        public TimerPage()
        {
            InitializeComponent();

            // prepare picker values (seconds 5..600 step 5)
            var options = Enumerable.Range(1, 120).Select(i => i * 5).ToList();
            foreach (var v in options)
            {
                PrepPicker.Items.Add(FormatSeconds(v));
                WorkoutPicker.Items.Add(FormatSeconds(v));
                EndPicker.Items.Add(FormatSeconds(v));
            }

            // defaults: 10s / 60s / 10s -> pick index 2,12,2 (since options are multiples of 5)
            PrepPicker.SelectedIndex = options.IndexOf(_prep);
            WorkoutPicker.SelectedIndex = options.IndexOf(_workout);
            EndPicker.SelectedIndex = options.IndexOf(_end);

            PrepPicker.SelectedIndexChanged += OnPickerChanged;
            WorkoutPicker.SelectedIndexChanged += OnPickerChanged;
            EndPicker.SelectedIndexChanged += OnPickerChanged;
 
            // Now treat 'end' as a trailing cooldown that overlaps the end of the workout.
            // _phaseDurations only contains prep and workout. The end duration (_end) will
            // be used as a trailing timer during the workout phase when remaining <= _end.
            _phaseDurations = new List<int> { _prep, _workout };
            _currentPhaseIndex = 0;
            _phaseRemaining = _phaseDurations[_currentPhaseIndex];
            UpdateLabels();

            // timer 1s tick
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTick;

            // setup GraphicsView draw handler
            // pass a phase resolver that returns 2 when we're in overlapped end/cooldown
            CircleView.Drawable = new CircleDrawable(() => _progress, () =>
            {
                if (_currentPhaseIndex == 1 && _phaseRemaining <= Math.Min(_end, _phaseDurations[1]))
                    return 2; // 종료(overlay)
                return _currentPhaseIndex;
            });
        }

        void OnPickerChanged(object sender, EventArgs e)
        {
            if (sender == PrepPicker && PrepPicker.SelectedIndex >= 0)
                _prep = (PrepPicker.SelectedIndex + 1) * 5;
            if (sender == WorkoutPicker && WorkoutPicker.SelectedIndex >= 0)
                _workout = (WorkoutPicker.SelectedIndex + 1) * 5;
            if (sender == EndPicker && EndPicker.SelectedIndex >= 0)
                _end = (EndPicker.SelectedIndex + 1) * 5;

            _phaseDurations = new List<int> { _prep, _workout };
            // reset if not running
            if (!_isRunning)
            {
                _currentPhaseIndex = 0;
                _phaseRemaining = _phaseDurations[_currentPhaseIndex];
                UpdateLabels();
                CircleView.Invalidate();
            }
        }

        void OnStartPauseClicked(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                _isRunning = true;
                StartPauseButton.Text = "PAUSE";
                _timer.Start();
            }
            else
            {
                _isRunning = false;
                StartPauseButton.Text = "START";
                _timer.Stop();
            }
        }

        void OnResetClicked(object sender, EventArgs e)
        {
            _timer.Stop();
            _isRunning = false;
            StartPauseButton.Text = "START";
            _currentPhaseIndex = 0;
            _phaseDurations = new List<int> { _prep, _workout };
            _phaseRemaining = _phaseDurations[_currentPhaseIndex];
            _progress = 0f;
            UpdateLabels();
            CircleView.Invalidate();
        }

        void OnTick(object sender, EventArgs e)
        {
            if (_phaseRemaining > 0)
            {
                _phaseRemaining--;
                // Determine progress and beep behavior.
                if (_currentPhaseIndex == 0)
                {
                    // Prep phase: behave as before
                    var total = _phaseDurations[_currentPhaseIndex];
                    _progress = (float)(total - _phaseRemaining) / Math.Max(1, total);
                    if (_phaseRemaining == 0)
                        PlayBeep(big: true);
                    else
                        PlayBeep(big: false);
                }
                else // workout phase (index == 1)
                {
                    // If remaining time is within the trailing end/cooldown, switch to '종료' mode
                    var endDuration = Math.Min(_end, _phaseDurations[1]);
                    if (_phaseRemaining <= endDuration)
                    {
                        // progress relative to end duration (0..1)
                        _progress = (float)(endDuration - _phaseRemaining) / Math.Max(1, endDuration);
                        // beep each tick in cooldown, big beep when reaching zero
                        if (_phaseRemaining == 0)
                            PlayBeep(big: true);
                        else
                            PlayBeep(big: false);
                    }
                    else
                    {
                        // normal workout progress
                        var total = _phaseDurations[_currentPhaseIndex];
                        _progress = (float)(total - _phaseRemaining) / Math.Max(1, total);
                    }
                }

                UpdateLabels();
                CircleView.Invalidate();

                if (_phaseRemaining == 0)
                {
                    // If we were in prep, move to workout. If we were in workout, the whole timer finishes
                    if (_currentPhaseIndex < _phaseDurations.Count - 1)
                    {
                        _currentPhaseIndex++;
                        _phaseRemaining = _phaseDurations[_currentPhaseIndex];
                        UpdateLabels();
                    }
                    else
                    {
                        // workout finished (end/cooldown is overlapped and also finished) -> stop
                        _timer.Stop();
                        _isRunning = false;
                        StartPauseButton.Text = "START";
                    }
                }
            }
        }

        // PlayBeep: currently logs small/large beep events. Replace the Debug.WriteLine blocks
        // with actual audio playback once you add audio resources (e.g. Resources/Raw/beep_small.mp3, beep_big.mp3)
        void PlayBeep(bool big)
        {
            // Determine displayed phase name including overlapped end mode
            var phaseIndex = _currentPhaseIndex;
            if (phaseIndex == 1 && _phaseRemaining <= Math.Min(_end, _phaseDurations[1]))
                phaseIndex = 2; // show 종료 during workout's trailing end

            var phaseName = phaseIndex == 0 ? "준비" : phaseIndex == 1 ? "운동" : "종료";
            if (big)
            {
                Debug.WriteLine($"[BEEP-BIG] Phase={phaseName} reached 0");
                // TODO: play big beep audio resource. Example (using a cross-platform audio player):
                // var player = CrossSimpleAudioPlayer.Current;
                // player.Load("beep_big.mp3", typeof(TimerPage).Assembly);
                // player.Play();
            }
            else
            {
                Debug.WriteLine($"[beep] Phase={phaseName} remaining={_phaseRemaining}s");
                // TODO: play small beep audio resource. Example:
                // var player = CrossSimpleAudioPlayer.Current;
                // player.Load("beep_small.mp3", typeof(TimerPage).Assembly);
                // player.Play();
            }
        }

        void UpdateLabels()
        {
            // If we're in workout and within the trailing end duration, show '종료'
            if (_currentPhaseIndex == 1 && _phaseRemaining <= Math.Min(_end, _phaseDurations[1]))
            {
                TimeLabel.Text = FormatSeconds(_phaseRemaining);
                PhaseLabel.Text = "종료";
            }
            else
            {
                TimeLabel.Text = FormatSeconds(_phaseRemaining);
                PhaseLabel.Text = _currentPhaseIndex == 0 ? "준비" : "운동";
            }
        }

        string FormatSeconds(int s)
        {
            var ts = TimeSpan.FromSeconds(Math.Max(0, s));
            return $"{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_timer != null)
                _timer.Stop();
        }
    }

    // drawable for circular progress
    class CircleDrawable : IDrawable
    {
        Func<float> _getProgress;
        Func<int> _getPhase;
        public CircleDrawable(Func<float> getProgress, Func<int> getPhase)
        {
            _getProgress = getProgress;
            _getPhase = getPhase;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();
            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;
            var radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - 8;

            // background circle
            canvas.FillColor = Colors.Transparent;
            canvas.FillCircle(cx, cy, radius + 8);

            // base ring
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 10;
            canvas.DrawCircle(cx, cy, radius);

            // progress arc            
            float progress = Math.Clamp(_getProgress(), 0f, 1f);
            int phase = _getPhase();
            Color color = phase == 0 ? Colors.Orange : phase == 1 ? Colors.Green : Colors.Red;
            canvas.StrokeColor = color;
            canvas.StrokeSize = 10;

            // draw arc by drawing many short lines along angle (simple approximation)
            var startAngle = -90f; // top
            var sweep = progress * 360f;
            // Use DrawArc with bounding rectangle
            var rect = new RectF(cx - radius, cy - radius, radius * 2, radius * 2);
            // DrawArc(RectF, float startAngle, float sweep, bool includeCenter, bool clockwise)
            canvas.DrawArc(rect, startAngle, sweep, false, false);

            canvas.RestoreState();
        }
    }
}
