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

namespace Quizard
{
    [Activity(Label = "SplashScreen")]
    public class SplashScreen : Activity
    {
        private int countDown = 0;
        private Timer timer;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.SplashScreen);

            countDown = 3;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += OnTimedEvent;
            timer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            countDown--;

            if (countDown <= 0)
            {
                timer.Stop();

                this.FinishAfterTransition();
            }
        }
    }
 }
