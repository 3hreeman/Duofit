using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;

namespace Duofit.Pages
{
    enum TimerPhase { Prep, Workout, End }

    public partial class TimerPage : ContentPage
    {
        IDispatcherTimer _timer;
        DateTime _tickStartTime;

        double _prep = 10;
        double _workout = 60;
        double _end = 10;

        List<double> _phaseDurations;
        TimerPhase _currentPhase = TimerPhase.Prep;
        double _phaseRemaining = 0;
        bool _isRunning = false;

        float _progress = 1f;
        
        public TimerPage()
        {
            InitializeComponent();

            PrepEntry.Text = FormatSeconds((int)_prep);
            WorkoutEntry.Text = FormatSeconds((int)_workout);
            EndEntry.Text = FormatSeconds((int)_end);

            PrepEntry.Focused += OnTimeEntryFocused;
            WorkoutEntry.Focused += OnTimeEntryFocused;
            EndEntry.Focused += OnTimeEntryFocused;

            PrepEntry.Unfocused += OnTimeEntryUnfocused;
            WorkoutEntry.Unfocused += OnTimeEntryUnfocused;
            EndEntry.Unfocused += OnTimeEntryUnfocused;

            _phaseDurations = new List<double> { _prep, _workout };
            _currentPhase = TimerPhase.Prep;
            _phaseRemaining = _phaseDurations[(int)_currentPhase];
            UpdateLabels();

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(16);
            _timer.Tick += OnTick;

            CircleView.Drawable = new CircleDrawable(
                () => _progress,
                () => {
                    if (_currentPhase == TimerPhase.Workout && _phaseRemaining <= Math.Min(_end, _phaseDurations[1]))
                        return TimerPhase.End;
                    return _currentPhase;
                },
                () => _phaseRemaining,
                () => _phaseDurations
            );
        }

        private void OnTimeEntryFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Dispatcher.Dispatch(() =>
                {
                    entry.CursorPosition = 0;
                    entry.SelectionLength = entry.Text?.Length ?? 0;
                });
            }
        }

        private void OnTimeEntryUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                var timeInSeconds = ParseTime(entry.Text);
                entry.Text = FormatSeconds((int)timeInSeconds);
            }
        }

        void OnTimeEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry) return;

            var text = e.NewTextValue ?? "";
            var oldText = e.OldTextValue ?? "";

            var cleanText = new string(text.Where(c => char.IsDigit(c) || c == ':').ToArray());

            if (cleanText.Length == 2 && oldText.Length == 1 && !cleanText.Contains(":"))
            {
                entry.Text = cleanText + ":";
                entry.CursorPosition = entry.Text.Length;
            }
            else if (cleanText != text)
            {
                entry.Text = cleanText;
            }

            UpdateDurationsFromEntries();
            if (!_isRunning)
            {
                _currentPhase = TimerPhase.Prep;
                _phaseRemaining = _phaseDurations[(int)_currentPhase];
                _progress = 1f;
                UpdateLabels();
                CircleView.Invalidate();
            }
        }

        private void UpdateDurationsFromEntries()
        {
            _prep = ParseTime(PrepEntry.Text);
            _workout = ParseTime(WorkoutEntry.Text);
            _end = ParseTime(EndEntry.Text);
            _phaseDurations = new List<double> { _prep, _workout };
        }

        private double ParseTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time)) return 0;
            var parts = time.Split(':');
            if (parts.Length == 2)
            {
                int.TryParse(parts[0], out int minutes);
                int.TryParse(parts[1], out int seconds);
                return minutes * 60 + seconds;
            }
            if (parts.Length == 1)
            {
                int.TryParse(parts[0], out int seconds);
                return seconds;
            }
            return 0;
        }

        void OnStartPauseClicked(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                UpdateDurationsFromEntries();
                _isRunning = true;
                StartPauseButton.Text = "PAUSE";
                _tickStartTime = DateTime.Now;
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
            
            _prep = 10;
            _workout = 60;
            _end = 10;

            PrepEntry.Text = FormatSeconds((int)_prep);
            WorkoutEntry.Text = FormatSeconds((int)_workout);
            EndEntry.Text = FormatSeconds((int)_end);

            _phaseDurations = new List<double> { _prep, _workout };
            _currentPhase = TimerPhase.Prep;
            _phaseRemaining = _phaseDurations[(int)_currentPhase];
            _progress = 1f;
            UpdateLabels();
            CircleView.Invalidate();
        }

        void OnTick(object sender, EventArgs e)
        {
            if (!_isRunning) return;

            var elapsed = (DateTime.Now - _tickStartTime).TotalSeconds;
            _tickStartTime = DateTime.Now;

            _phaseRemaining -= elapsed;

            if (_phaseRemaining <= 0)
            {
                if (_currentPhase == TimerPhase.Prep)
                {
                    _currentPhase = TimerPhase.Workout;
                    _phaseRemaining += _phaseDurations[(int)_currentPhase];
                    PlayBeep(big: true);
                }
                else
                {
                    _phaseRemaining = 0;
                    _timer.Stop();
                    _isRunning = false;
                    StartPauseButton.Text = "START";
                    PlayBeep(big: true);
                }
            }
            
            UpdateProgress();
            UpdateLabels();
            CircleView.Invalidate();
        }

        void UpdateProgress()
        {
            double currentPhaseTotal = _phaseDurations[(int)_currentPhase];
            
            if (_currentPhase == TimerPhase.Workout) // 운동 또는 종료 단계
            {
                _progress = (float)(_phaseRemaining / currentPhaseTotal);
            }
            else // 준비 단계
            {
                _progress = (float)(_phaseRemaining / currentPhaseTotal);
            }
        }

        void PlayBeep(bool big)
        {
            var displayPhase = _currentPhase;
            if (displayPhase == TimerPhase.Workout && _phaseRemaining <= Math.Min(_end, _phaseDurations[1]))
                displayPhase = TimerPhase.End;

            var phaseName = displayPhase switch
            {
                TimerPhase.Prep => "준비",
                TimerPhase.Workout => "운동",
                TimerPhase.End => "종료",
                _ => ""
            };

            if (big)
            {
                Debug.WriteLine($"[BEEP-BIG] Phase={phaseName} reached 0");
            }
            else
            {
                Debug.WriteLine($"[beep] Phase={phaseName} remaining={_phaseRemaining}s");
            }
        }

        void UpdateLabels()
        {
            if (_currentPhase == TimerPhase.Workout && _phaseRemaining <= Math.Min(_end, _phaseDurations[1]))
            {
                TimeLabel.Text = FormatSeconds((int)Math.Ceiling(_phaseRemaining));
                PhaseLabel.Text = "종료";
            }
            else
            {
                TimeLabel.Text = FormatSeconds((int)Math.Ceiling(_phaseRemaining));
                PhaseLabel.Text = _currentPhase == TimerPhase.Prep ? "준비" : "운동";
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

    class CircleDrawable : IDrawable
    {
        Func<float> _getProgress;
        Func<TimerPhase> _getPhase;
        private readonly Func<double> _getRemaining;
        private readonly Func<List<double>> _getPhaseDurations;
        
        static float _startAngle = 90f; // 12시 방향
        static Color _prepPhaseColor = Colors.YellowGreen;
        static Color _workoutPhaseColor = Colors.Green;
        static Color _endPhaseColor = Colors.Orange;
        public CircleDrawable(Func<float> getProgress, Func<TimerPhase> getPhase, Func<double> getRemaining, Func<List<double>> getPhaseDurations)
        {
            _getProgress = getProgress;
            _getPhase = getPhase;
            _getRemaining = getRemaining;
            _getPhaseDurations = getPhaseDurations;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();
            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;
            var radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - 8;

            // 기본 링
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 10;
            canvas.DrawCircle(cx, cy, radius);

            float progress = Math.Clamp(_getProgress(), 0f, 1f);
            TimerPhase phase = _getPhase();
            Color color;

            if (phase == TimerPhase.End) // 종료 단계
            {
                var phaseDurations = _getPhaseDurations();
                if (phaseDurations.Count > 1)
                {
                    // 운동 단계의 진행률을 유지
                    float workoutProgress = (float)(_getRemaining() / phaseDurations[1]);
                    progress = Math.Clamp(workoutProgress, 0f, 1f);
                }
                color = _endPhaseColor;
            }
            else // 준비 또는 운동 단계
            {
                color = phase == TimerPhase.Prep ? _prepPhaseColor : _workoutPhaseColor;
            }
            
            canvas.StrokeColor = color;
            canvas.StrokeSize = 10;
            canvas.StrokeLineCap = LineCap.Round;

            // 12시 방향에서 시작하여 시계 방향으로 채워지는 호
            var endAngle = _startAngle + (360 * progress);
            
            canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, _startAngle, endAngle, false, false);

            canvas.RestoreState();
        }
    }
}
