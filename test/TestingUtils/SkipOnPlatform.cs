using System;

namespace TestingUtils
{
    [Flags]
    public enum SkipOnPlatform
    {
        Linux = 0b0001,
        Mac = 0b0010,
        Windows = 0b0100,
        All = 0b0111
    }
}
