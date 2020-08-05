using System.Threading.Tasks;
using App4.iOS;
using CoreGraphics;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using LoginManager = Facebook.LoginKit.LoginManager;

[assembly: Xamarin.Forms.Dependency(typeof(IosAccessToken))]
namespace App4.iOS
{
    public class IosAccessToken : IAccessToken
    {
        readonly LoginManager _loginManager = new LoginManager();
        readonly string[] _permissions = { @"public_profile", @"email" };

        LoginResult _loginResult;
        TaskCompletionSource<LoginResult> _completionSource;

        public Task<LoginResult> Login()
        {
            _completionSource = new TaskCompletionSource<LoginResult>();
            _loginManager.LogIn(_permissions, GetCurrentViewController(), LoginManagerLoginHandler);
            return _completionSource.Task;
        }

        public void Logout()
        {
            _loginManager.LogOut();
        }

        void LoginManagerLoginHandler(LoginManagerLoginResult result, NSError error)
        {
            if (result.IsCancelled)
                _completionSource.TrySetResult(new LoginResult {LoginState = LoginState.Canceled});
            else if (error != null)
                _completionSource.TrySetResult(new LoginResult { LoginState = LoginState.Failed, ErrorString = error.LocalizedDescription });
            else
            {
                _loginResult = new LoginResult
                {
                    Token = result.Token.TokenString,
                    UserId = result.Token.UserID,
                    ExpireAt = result.Token.ExpirationDate.ToDateTime()
                };
            }
        }

        static UIViewController GetCurrentViewController()
        {
            var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (viewController.PresentedViewController != null)
                viewController = viewController.PresentedViewController;
            return viewController;
        }
        
        public void Get()
        {
            var login = Login();
            var tok = Facebook.CoreKit.AccessToken.CurrentAccessToken;
        }
    }
}