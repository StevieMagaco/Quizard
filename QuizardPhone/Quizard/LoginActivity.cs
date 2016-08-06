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
using Android.Database.Sqlite;
using Android.Database;
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
        private string NewUsername, NewPassword;
       private CreateAnAccountDialogFragment fragment = new CreateAnAccountDialogFragment();
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
                #region Database_login
                /*
                 * This try catch will check to make sure 
                 * Username and password are greater than 0
                 * That the user actually exists in the database 
                 */
                try
                {
                    if (userLoginUsername.Length() > 0 && userLoginPassword.Length() > 0)
                    {
                        DataBase.DBAdapter db = new DataBase.DBAdapter(this);
                        db.openDB();
                        ICursor c = db.GetUser(userLoginUsername.Text, userLoginPassword.Text);
                        c.MoveToNext();
                        String username = c.GetString(0), password = c.GetString(1);
                        if (userLoginUsername.Text == username && userLoginPassword.Text == password)
                        {
                            userLoginPassword.Text = "";
                            userLoginUsername.Text = "";
                            Toast.MakeText(this, "Welcome", ToastLength.Short).Show();
                            Intent intent = new Intent(this, typeof(HomeActivity));
                            this.StartActivity(intent);
                        }
                        else
                        {
                            Toast.MakeText(this, "Username or Password is Incorrect", ToastLength.Short).Show();
                        }
                        db.CloseDB();
                    }
                    else
                        Toast.MakeText(this, "Username or Password is Incorrect", ToastLength.Short).Show();
                }
                catch
                {
                    Toast.MakeText(this, "Username or Password is Incorrect", ToastLength.Short).Show();
                }
                #endregion
            };

            createAnAccount.Click += (object sender, EventArgs e) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                // When the "New to Quizard?" button is clicked, bring up the assigned dialog fragment
               // CreateAnAccountDialogFragment fragment = new CreateAnAccountDialogFragment();
                fragment.Show(transaction, "dialog fragment");
                fragment.onCreateAnAccountIsClicked += CreateAnAccount_onCreateAnAccountIsClicked;
                
            };
        }

        private void CreateAnAccount_onCreateAnAccountIsClicked(object sender, OnCreateAnAccountEventArgs e)
        {
            loginProgressBar.Visibility = ViewStates.Visible;

            Thread progressBarThread = new Thread(ServerRequest);
            progressBarThread.Start();
            #region Database_AddUser
            /*
             * once user clicks create account this will check to make sure 
             * Username and password are grater than 0
             * Username is does not already exist
             * attempt to add the user to the database
             */
            NewUsername = fragment.GetNewUserName();
            NewPassword = fragment.GetNewPassword();
            try
            {
                if (NewUsername.Length > 0 && NewPassword.Length > 0)
                {
                    DataBase.DBAdapter db = new DataBase.DBAdapter(this);
                    db.openDB();
                    ICursor c = db.GetUser(userLoginUsername.Text, "");
                    if (c.Count == 0)
                    {
                        DataBase.DBFunction DataFunc = new DataBase.DBFunction();
                        if (DataFunc.SaveUser(NewUsername, NewPassword, this))
                        {
                            Toast.MakeText(this, "Welcome", ToastLength.Short).Show();
                            Intent intent = new Intent(this, typeof(HomeActivity));
                            this.StartActivity(intent);
                        }
                        else
                            Toast.MakeText(this, "Unable to Create new User", ToastLength.Short).Show();
                    }
                    else
                        Toast.MakeText(this, "Unable to Create new User", ToastLength.Short).Show();
                }
                else
                    Toast.MakeText(this, "Unable to Create new User", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Unable to Create new User", ToastLength.Short).Show();
            }
            fragment.SetNewConfermPassword("");
            fragment.SetNewPassword("");
            fragment.SetNewUserName("");
            #endregion
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