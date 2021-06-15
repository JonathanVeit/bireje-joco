using System;

namespace BiReJeJoCo.Character
{
    [Flags]
    public enum InputBlockState 
    {
        // flags 
        Loading                 = Movement | Look | Interact | Shoot,
        Menu                    = Movement | Look | Interact | Shoot,
        Transformation          = Movement,

        // states 
        Free            = 1 << 0,
        Movement        = 1 << 1,
        Look            = 1 << 2,
        Interact        = 1 << 3,
        Shoot           = 1 << 4,
    }
}
