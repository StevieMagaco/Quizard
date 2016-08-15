using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace QuizardWatch
{
    [Activity(Label = "ToMenuSplashScreen", MainLauncher = true, Icon = "@drawable/icon")]
    public class ToMenuSplashScreen : Activity
    {
        private int coutDown = 0;
        private Timer timer;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.SplashScreen);

            coutDown = 3;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += OnTimedEvent;
            timer.Start();

            
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            coutDown--;

            if (coutDown <= 0)
            {
                Intent intent = new Intent(this, typeof(SetPage));

                this.StartActivity(intent);

                timer.Stop();
            }
        }
    }
}