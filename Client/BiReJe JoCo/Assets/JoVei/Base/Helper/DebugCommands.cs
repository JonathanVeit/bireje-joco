using System;

namespace JoVei.Base.Helper
{
    public class BaseDebugCommand
    {
        public string Id { get; private set; }
        public string Description { get; private set; }
        public string Format { get; private set; }

        public BaseDebugCommand(string Id, string description, string format)
        {
            this.Id = Id;
            this.Description = description;
            this.Format = format;
        }
    }

    public class DebugCommand : BaseDebugCommand
    {
        private Action command;

        public DebugCommand(string Id, string description, string format, Action command) : base(Id, description, format)
        {
            this.command = command;
        }

        public void Invoke()
        {
            command?.Invoke();
        }
    }

    public class DebugCommand<TValue> : BaseDebugCommand
    {
        private Action<TValue> command;

        public DebugCommand(string Id, string description, string format, Action<TValue> command) : base(Id, description, format)
        {
            this.command = command;
        }

        public void Invoke(TValue value)
        {
            command?.Invoke(value);
        }
    }

    public class DebugCommand<TValue1, TValue2> : BaseDebugCommand
    {
        private Action<TValue1, TValue2> command;

        public DebugCommand(string Id, string description, string format, Action<TValue1, TValue2> command) : base(Id, description, format)
        {
            this.command = command;
        }

        public void Invoke(TValue1 value1, TValue2 value2)
        {
            command?.Invoke(value1, value2);
        }
    }
}