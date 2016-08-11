using System;
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
    [Activity(Label = "SetPage")]
    public class SetPage : Activity
    {
        private ListView QuizList;
        //Temporary List of Dummy data
        private List<string> tempList = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
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