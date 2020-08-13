using System.Collections.Generic;
using CoreGraphics;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;

namespace App4.iOS
{
	public partial class MainViewController : UIViewController
	{
		// This permission is set by default, even if you don't add it, but FB recommends to add it anyway
		private List<string> readPermissions = new List<string> { "public_profile" };

		private LoginButton loginButton;
		private ProfilePictureView pictureView;
		private UILabel nameLabel;

		private IUIAlertViewDelegate AlertViewDelegate { get; } = new MainAlertViewDelegate();

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// If was send true to Profile.EnableUpdatesOnAccessTokenChange method
			// this notification will be called after the user is logged in and
			// after the AccessToken is gotten
			Profile.Notifications.ObserveDidChange((sender, e) =>
			{
				if (e.NewProfile == null)
				{
					return;
				}
				nameLabel.Text = e.NewProfile.Name;
			});

			// Set the Read and Publish permissions you want to get
			loginButton = new LoginButton(new CGRect(80, 20, 220, 46))
			{
				Permissions = readPermissions.ToArray()
			};
			// Handle actions once the user is logged in
			loginButton.Completed += (sender, e) =>
			{
				if (e.Error != null)
				{
					// Handle if there was an error
					new UIAlertView("Login", e.Error.Description, AlertViewDelegate, "Ok", null).Show();
					return;
				}

				if (e.Result.IsCancelled)
				{
					// Handle if the user cancelled the login request
					new UIAlertView("Login", "The user cancelled the login", AlertViewDelegate, "Ok", null).Show();
					return;
				}

				// Handle your successful login
				new UIAlertView("Login", "Success!!", AlertViewDelegate, "Ok", null).Show();
			};

			// Handle actions once the user is logged out
			loginButton.LoggedOut += (sender, e) =>
			{
				// Handle your logout
				nameLabel.Text = "";
			};
			// The user image profile is set automatically once is logged in
			pictureView = new ProfilePictureView(new CGRect(80, 100, 220, 220));
			// Create the label that will hold user's facebook name
			nameLabel = new UILabel(new CGRect(80, 340, 220, 21))
			{
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};
			// If you have been logged into the app before, ask for the your profile name
			if (AccessToken.CurrentAccessToken != null)
			{
				var request = new GraphRequest("/me?fields=name", null, AccessToken.CurrentAccessToken.TokenString, null, "GET");
				request.Start((connection, result, error) =>
				{
					// Handle if something went wrong with the request
					if (error != null)
					{
						new UIAlertView("Error...", error.Description, AlertViewDelegate, "Ok", null).Show();
						return;
					}
					// Get your profile name
					var userInfo = result as NSDictionary;
					nameLabel.Text = userInfo["name"].ToString();
				});
			}

			// Add views to main view
			View.AddSubview(loginButton);
			View.AddSubview(pictureView);
			View.AddSubview(nameLabel);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}

	public class MainAlertViewDelegate : UIAlertViewDelegate
	{
		public override void WillPresent(UIAlertView alertView)
		{
			base.WillPresent(alertView);
		}
	}
}

