using System.ComponentModel;

namespace FinTrackForWindows.Enums
{
    public enum FeedbackType
    {
        [Description("Bug Report")]
        BugReport,

        [Description("Feature Request")]
        FeatureRequest,

        [Description("General Feedback")]
        GeneralFeedback,

        [Description("Other")]
        Other
    }
}
