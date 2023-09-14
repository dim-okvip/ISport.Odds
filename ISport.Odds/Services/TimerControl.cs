namespace ISport.Odds.Services
{
    public class TimerControl
    {
        private Timer? _timer;
        private AutoResetEvent? _autoResetEvent;
        private Action? _action;
        public DateTime TimerStarted { get; set; }
        public bool IsTimerStarted { get; set; }

        public void ScheduleTimer(Action action, int paramTime)
        {
            _action = action;
            _autoResetEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoResetEvent, 1000, paramTime);
            TimerStarted = DateTime.Now;
            IsTimerStarted = true;
        }

        public void Execute(object? stateInfo)
        {
            Console.WriteLine($"{DateTime.Now} Executing synchronization from MongoDb to FE...");
            _action();
            if ((DateTime.Now - TimerStarted).TotalSeconds > 60)
            {
                IsTimerStarted = false;
                _timer.Dispose();
            }
        }
    }
}
