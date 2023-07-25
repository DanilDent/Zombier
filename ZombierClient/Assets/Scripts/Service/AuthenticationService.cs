using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;

namespace Prototype.Service
{
    public class AuthenticationService
    {

        public AuthenticationService()
        {
            _auth = FirebaseAuth.DefaultInstance;
        }

        public FirebaseUser CurrentUser => _auth.CurrentUser;

        public async Task SingInAnonymouslyAsync()
        {

            await _auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });
        }

        private FirebaseAuth _auth;
    }
}
