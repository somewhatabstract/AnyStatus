namespace AnyStatus
{
    public class State : Enumeration<State, int>
    {
        public static readonly State None = new State(0, 0, "None", "Silver", "Blank.png");
        public static readonly State Unknown = new State(1, 1, "Unknown", "Silver", "StatusHelp_gray_16x.png");
        public static readonly State Disabled = new State(2, 2, "Disabled", "Silver", "StatusPause_grey_16x.png");
        public static readonly State Canceled = new State(3, 3, "Canceled", "Gray", "StatusStop_grey_16x.png");
        public static readonly State Ok = new State(4, 4, "Ok", "Green", "StatusOK_grey_16x.png");
        public static readonly State Open = new State(5, 5, "Open", "Green", "StatusInformation_grey_16x.png");
        public static readonly State Closed = new State(6, 6, "Closed", "Red", "StatusOK_grey_16x.png");
        public static readonly State PartiallySucceeded = new State(7, 7, "Partially Succeeded", "Orange", "StatusInvalid_grey_16x");
        public static readonly State Failed = new State(8, 8, "Failed", "Red", "StatusCriticalError_grey_16x.png");
        public static readonly State Invalid = new State(9, 9, "Invalid", "DarkRed", "StatusWarning_grey_16x.png");
        public static readonly State Error = new State(10, 10, "Error", "DarkRed", "StatusWarning_grey_16x.png");
        public static readonly State Running = new State(11, 11, "Running", "DodgerBlue", "StatusRun_grey_16x.png");

        public int Priority { get; private set; }

        public string Color { get; private set; }

        public string IconName { get; private set; }

        private State(int value, int priority, string displayName, string color, string iconName) : base(value, displayName)
        {
            Priority = priority;
            Color = color;
            IconName = iconName;
        }
    }
}
