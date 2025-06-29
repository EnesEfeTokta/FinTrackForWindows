namespace FinTrack.Core
{
    public interface ISecureTokenStorage
    {
        void SaveToken(string token);
        string? GetToken();
        void ClearToken();
    }
}