using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace Quizard
{
    [Activity(Label = "Quizard", MainLauncher = true /* MainLauncher does NOT need to be changed unless another layout or diaglog fragment needs to be tested*/, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        private LinearLayout loginView;
        private EditText userLoginUsername, userLoginPassword;
        private Button login, createAnAccount;
        private CheckBox rememberMe;
        private ProgressBar loginProgressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.NoTitle);

            base.OnCreate(savedInstanceState);

            // Set our view from the "LoginLayout" layout resource
            SetContentView(Resource.Layout.LoginLayout);

            loginView = FindViewById<LinearLayout>(Resource.Id.loginViewLinearLayoutID);
            userLoginUsername = FindViewById<EditText>(Resource.Id.userLoginUsernameEditTextID);
            userLoginPassword = FindViewById<EditText>(Resource.Id.userLoginPasswordEditTextID);
            login = FindViewById<Button>(Resource.Id.loginButtonID);
            createAnAccount = FindViewById<Button>(Resource.Id.createAnAccountButtonID);
            rememberMe = FindViewById<CheckBox>(Resource.Id.rememberMeCheckBoxID);
            loginProgressBar = FindViewById<ProgressBar>(Resource.Id.loginProgressBarID);

            // If the "CreateAnAccount" dialog fragment is brought up by accident, the user may click the
            // layout around the dialog fragment to close it and bring them back to the main login layout
            loginView.Click += (object sender, EventArgs e) =>
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);
            };

            login.Click += (object sender, EventArgs e) =>
            {
                loginProgressBar.Visibility = ViewStates.Visible;

                Thread progressBarThread = new Thread(ServerRequest);
                progressBarThread.Start();

                // TODO: Implement the login error checking

                // Once the user has clicked the "Login" button, take them to the home screen
                Intent intent = new Intent(this, typeof(HomeActivity));
                this.StartActivity(intent);
            };

            createAnAccount.Click += (object sender, EventArgs e) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();

                // When the "New to Quizard?" button is clicked, bring up the assigned dialog fragment
                CreateAnAccountDialogFragment fragment = new CreateAnAccountDialogFragment();
                fragment.Show(transaction, "dialog fragment");

                fragment.onCreateAnAccountIsClicked += CreateAnAccount_onCreateAnAccountIsClicked;
            };
        }

        private void CreateAnAccount_onCreateAnAccountIsClicked(object sender, OnCreateAnAccountEventArgs e)
        {
            loginProgressBar.Visibility = ViewStates.Visible;

            Thread progressBarThread = new Thread(ServerRequest);
            progressBarThread.Start();

            // TODO: Implement the account creation error checking

            Intent intent = new Intent(this, typeof(HomeActivity));
            this.StartActivity(intent);
        }

        private void ServerRequest()
        {
            // This code is temporary until a server can be implemented
            Thread.Sleep(3000);

            RunOnUiThread(() =>
            {
                loginProgressBar.Visibility = ViewStates.Invisible;
            });
        }
    }
}