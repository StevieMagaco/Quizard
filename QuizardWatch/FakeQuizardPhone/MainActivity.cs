using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace QuizardWatch
{
    [Activity(Label = "FakeQuizardPhone", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            EditText textToSend = FindViewById<EditText>(Resource.Id.textToSend);
            Button myButton = FindViewById<Button>(Resource.Id.MyButton);

            myButton.Click += (object sender, EventArgs e) =>
            {
                //send the text from textToSend to the watch
            };



        }
    }
}

