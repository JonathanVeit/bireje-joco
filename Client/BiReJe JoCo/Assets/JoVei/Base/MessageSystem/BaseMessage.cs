namespace JoVei.Base.MessageSystem
{
    public abstract class BaseMessage : IMessage
    {
        public string Name { get { return GetType().Name; } }
    }

    public abstract class Message<TParam1> : BaseMessage
    {
        public TParam1 Param1 { get; protected set; }

        public Message(TParam1 param1)
        {
            Param1 = param1;
        }
    }

    public abstract class Message<TParam1, TParam2> : Message <TParam1>
    {
        public TParam2 Param2 { get; protected set; }

        public Message(TParam1 param1, TParam2 param2) : base (param1)
        {
            Param2 = param2;
        }
    }

    public abstract class Message<TParam1, TParam2, TParam3> : Message <TParam1, TParam2>
    {
        public TParam3 Param3 { get; protected set; }

        public Message(TParam1 param1, TParam2 param2, TParam3 param3) : base(param1, param2)
        {
            Param3 = param3;
        }
    }

    public abstract class Message<TParam1, TParam2, TParam3, TParam4> : Message<TParam1, TParam2, TParam3>
    {
        public TParam4 Param4 { get; protected set; }

        public Message(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4) : base(param1, param2, param3)
        {
            Param4 = param4;
        }
    }

    public abstract class Message<TParam1, TParam2, TParam3, TParam4, TParam5> : Message<TParam1, TParam2, TParam3, TParam4>
    {
        public TParam5 Param5 { get; protected set; }

        public Message(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5) : base(param1, param2, param3, param4)
        {
            Param5 = param5;
        }
    }

    public abstract class Message<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : Message<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        public TParam6 Param6 { get; protected set; }

        public Message(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6) : base(param1, param2, param3, param4, param5)
        {
            Param6 = param6;
        }
    }
}
