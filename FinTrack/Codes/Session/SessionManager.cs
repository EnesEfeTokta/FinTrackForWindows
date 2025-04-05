using FinTrack.Codes.DataAccess;

namespace FinTrack.Codes.Session
{
    public class SessionManager
    {
        private static SessionManager instance;
        public static SessionManager Instance => instance ??= new SessionManager();
        private SessionManager() { }

        private users currentUser { get; set; } = new users();

        public users CurrentUser
        {
            get 
            {
                if (currentUser == null)
                {
                    currentUser = new users();
                }
                return currentUser;
            }
        }

        public void SetCurrentUser(users user) => currentUser = user;

        public void ClearCurrentUser() => currentUser = new users();

        public bool IsUserLoggedIn() => currentUser != null;

        public bool IsUserLoggedOut() => CurrentUser == null;

        public void UpdateUser(users user) => currentUser = user;

        public void Logout() => currentUser = new users();

        public void Login(users user) => currentUser = user;
    }
}
