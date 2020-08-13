using Facebook.CoreKit;
using Foundation;
using UIKit;

namespace App4.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Settings.AppId = "Insert_Your_App_Id";
            Settings.DisplayName = "Insert_Your_App_Name";

            // This is false by default,
            // If you set true, you can handle the user profile info once is logged into FB with the Profile.Notifications.ObserveDidChange notification,
            // If you set false, you need to get the user Profile info by hand with a GraphRequest
            Profile.EnableUpdatesOnAccessTokenChange(false);

            return ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);
        }
        
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }

        public override void OnActivated(UIApplication application) => AppEvents.ActivateApp();
    }
}