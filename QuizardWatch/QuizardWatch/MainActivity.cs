using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Java.Interop;
using Android.Views.Animations;
using System.Collections.Generic;

namespace QuizardWatch
{
    [Activity(Label = "QuizardWatch", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ListView QuizList;
        //Temporary List of Dummy data
        private List<string> tempList = new List<string>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Set our view from the "main" layout resource
            SetContentView(Resource.Layout.CardSets);

            tempList.Add("History");
            tempList.Add("Mathematics");
            tempList.Add("Literature");

            QuizList = FindViewById<ListView>(Resource.Id.QuizList);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, tempList);

            QuizList.Adapter = adapter;

        }
    }
}


