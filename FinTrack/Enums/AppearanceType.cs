using System.ComponentModel;

namespace FinTrack.Enums
{
    public enum AppearanceType
    {
        [Description("Light Mode")]
        Light,

        [Description("Dark Mode")]
        Dark,

        [Description("System Default")]
        SystemDefault
    }
}
