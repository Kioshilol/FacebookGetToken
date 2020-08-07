using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Color = Android.Graphics.Color;
using Debug = System.Diagnostics.Debug;

namespace App4.Android
{
	[Activity(Label = "App4", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity//FacebookActivity//
    {
        const string publishAct = "publish_actions";

        static readonly string[] PERMISSIONS = new[] { publishAct };

        private ICallbackManager callbackManager;

        private TextView OutputText { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //ToDo: Pavel - insert here your credentials - I still don't know how avoid that shit
            FacebookSdk.ApplicationName = Application.PackageName;
            FacebookSdk.ApplicationId = Application.PackageName;

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MainLayout);

            var btn = FindViewById<Button>(Resource.Id.btn);
            OutputText = FindViewById<TextView>(Resource.Id.txt);
            btn.Click += Btn_Click;

            FacebookSdk.AutoInitEnabled = true; //da emu pohui na samom dele
            FacebookSdk.SdkInitialize(this); //prosto zabei
            FacebookSdk.FullyInitialize();

            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = (res) => RunOnUiThread(() => OutputText.Text = $"TOKEN>> {AccessToken.CurrentAccessToken}"),
                HandleError = (ex) => Debug.WriteLine("Error on FB login detected>> " + ex),
                HandleCancel = () =>
                {
                    OutputText.Text = "CANCELED from Login";
                    OutputText.SetTextColor(Color.MediumVioletRed);
                }
            };
            callbackManager = CallbackManagerFactory.Create(); //absolutely inusable thing - I suppose

            LoginManager.Instance.SetDefaultAudience(DefaultAudience.Everyone);
            LoginManager.Instance.SetLoginBehavior(LoginBehavior.WebViewOnly);
            LoginManager.Instance.RegisterCallback(callbackManager, loginCallback);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            LoginManager.Instance.LogOut();
            LoginManager.Instance.LogIn(this, PERMISSIONS);

            Task.Run(() =>
            {
                var likesRequest = "Insert_Requred_Request_To_Get_Likes_Info";
                var response = GraphRequest.ExecuteAndWait(new GraphRequest(AccessToken.CurrentAccessToken, likesRequest));
                //ToDo: process response, get FB likes
                if (response != null && response.JSONObject is Org.Json.JSONObject jSON)
                {
                    var likes = jSON.GetInt("Here_Goes_The_Name_Of_Parameter");
                }
            });
        }

        private bool HasPublishPermission()
        {
            var accessToken = AccessToken.CurrentAccessToken;
            return accessToken != null && accessToken.Permissions.Contains(publishAct);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }
    }

    public class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback
        where TResult : Java.Lang.Object
    {
        public Action HandleCancel { get; set; }
        public Action<FacebookException> HandleError { get; set; }
        public Action<TResult> HandleSuccess { get; set; }

        public void OnCancel() => HandleCancel?.Invoke();
        public void OnError(FacebookException error) => HandleError?.Invoke(error);
        public void OnSuccess(Java.Lang.Object result) => HandleSuccess?.Invoke(result.JavaCast<TResult>());
    }
}