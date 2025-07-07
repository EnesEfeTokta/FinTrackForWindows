using System.ComponentModel;

namespace FinTrack.Enums
{
    public enum FeedbackTypes
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
