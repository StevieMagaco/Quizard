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
using Android.Widget;

namespace Quizard
{
    public class OnCreateAnAccountEventArgs : EventArgs
    {
        private string eventNewUsername, eventNewPassword, eventNewConfirmPassword;

        public string newUsername
        {
            get { return eventNewUsername; }
            set { eventNewUsername = value; }
        }

        public string newPassword
        {
            get { return eventNewPassword; }
            set { eventNewPassword = value; }
        }

        public string newConfirmPassword
        {
            get { return eventNewConfirmPassword; }
            set { eventNewConfirmPassword = value; }
        }

        public OnCreateAnAccountEventArgs(string eventNewUsername, string eventNewPassword, string eventNewConfirmPassword)
        {
            newUsername = eventNewUsername;
            newPassword = eventNewPassword;
            newConfirmPassword = eventNewConfirmPassword;
        }
    }

    [Activity(Label = "Create an Account", MainLauncher = false /*Keep the MainLauncher = false unless this dialog fragment needs to be tested*/)]
    public class CreateAnAccountDialogFragment : DialogFragment
    {
        private EditText newUserUsername, newUserPassword, newUserConfirmPassword;
        private Button createNewAccount;
        public event EventHandler<OnCreateAnAccountEventArgs> onCreateAnAccountIsClicked;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.CreateAnAccountDialogLayout, container, false);

            newUserUsername = view.FindViewById<EditText>(Resource.Id.newUserUsernameEditTextID);
            newUserPassword = view.FindViewById<EditText>(Resource.Id.newUserPasswordEditTextID);
            newUserConfirmPassword = view.FindViewById<EditText>(Resource.Id.newUserConfirmPasswordEditTextID);
            createNewAccount = view.FindViewById<Button>(Resource.Id.createNewAccountButtonID);

            createNewAccount.Click += (object sender, EventArgs e) =>
            {
                onCreateAnAccountIsClicked.Invoke(this, new OnCreateAnAccountEventArgs(newUserUsername.Text, newUserPassword.Text, newUserConfirmPassword.Text));
                this.Dismiss();
            };

            return view;
        }
    }
}