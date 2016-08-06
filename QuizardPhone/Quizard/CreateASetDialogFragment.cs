using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Quizard
{
    public class OnCreateASetEventArgs : EventArgs
    {
        private string eventSubject;

        public string newSetSubject
        {
            get { return eventSubject; }
            set { eventSubject = value; }
        }

        public OnCreateASetEventArgs(string eventSubject)
        {
            newSetSubject = eventSubject;
        }
    }

    [Activity(Label = "Create a Set", MainLauncher = false /*Keep the MainLauncher = false unless this dialog fragment needs to be tested*/)]
    public class CreateASetDialogFragment : DialogFragment
    {
        public EditText subject;
        private Button createSet;
        public event EventHandler<OnCreateASetEventArgs> onCreateASetIsClicked;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.CreateASetDialogLayout, container, false);

            subject = view.FindViewById<EditText>(Resource.Id.setSubjectEditTextID);
            createSet = view.FindViewById<Button>(Resource.Id.createSetButtonID);

            createSet.Click += (object sender, EventArgs e) =>
            {
                onCreateASetIsClicked.Invoke(this, new OnCreateASetEventArgs(subject.Text));
                this.Dismiss();
            };

            return view;
        }
    }
}