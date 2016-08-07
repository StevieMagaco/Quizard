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
    [Activity(Label = "Quizard", MainLauncher = false /* MainLauncher does NOT need to be changed unless another layout or diaglog fragment needs to be tested*/, Icon = "@drawable/icon")]
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
                        ICursor LoginInfo = db.GetUser(userLoginUsername.Text, userLoginPassword.Text);
                        LoginInfo.MoveToNext();
                        if (userLoginUsername.Text == LoginInfo.GetString(0) && userLoginPassword.Text == LoginInfo.GetString(1))
                        {
                            userLoginPassword.Text = "";
                            userLoginUsername.Text = "";
                            Toast.MakeText(this, "Welcome", ToastLength.Short).Show();
                            Intent intent = new Intent(this, typeof(HomeActivity));
                            this.StartActivity(intent);
                        }
                        else
                            throw new System.ArgumentException("Username or Password Didn't Match", "UserName/Password");

                        db.CloseDB();
                    }
                    else
                        throw new System.ArgumentException("Username or Password size 0", "Username/Password");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
                    ICursor UserInfo = db.GetUser(NewUsername, "");
                    if (UserInfo.Count == 0)
                    {
                        DataBase.DBFunction DataFunc = new DataBase.DBFunction();
                        if (DataFunc.SaveUser(NewUsername, NewPassword, this))
                        {
                            Toast.MakeText(this, "Welcome", ToastLength.Short).Show();
                            Intent intent = new Intent(this, typeof(HomeActivity));
                            this.StartActivity(intent);
                        }
                        else
                            throw new System.ArgumentException("Failed to Save USer", "SaveUser");
                    }
                    else
                        throw new System.ArgumentException("UserInfo Size 0", "UserInfo");
                }
                else
                    throw new System.ArgumentException("Username or Password size 0", "UserName / Password");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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