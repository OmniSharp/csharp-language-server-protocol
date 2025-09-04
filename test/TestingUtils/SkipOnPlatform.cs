namespace TestingUtils
{
    [Flags]
    public enum SkipOnPlatform
    {
        None = 0b0000,
        Linux = 0b0001,
        Mac = 0b0010,
        Windows = 0b0100,
        All = Linux | Mac | Windows
    }
}
