using System;

namespace JoVei.Base.StatmodSystem
{
    public interface IStatMod<TStatType, TOrigin, TCategory>
        where TStatType : struct, Enum
        where TOrigin : struct, Enum
        where TCategory : struct, Enum
    {
        TStatType Type { get; set; }
        TOrigin Origin { get; set; }
        TCategory Category { get; set; }
        StatModOperator Operator { get; set; }
        float Value { get; set; }
        string Id { get; set; }
    }
}