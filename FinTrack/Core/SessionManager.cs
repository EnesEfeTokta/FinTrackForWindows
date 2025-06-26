namespace FinTrack.Core
{
    public static class SessionManager
    {
        public static string CurrentToken { get; private set; } 

        public static void SetToken(string token)
        {
            CurrentToken = token;
        }

        public static void ClearToken()
        {
            CurrentToken = null;
        }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(CurrentToken);
    }
}
