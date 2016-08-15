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
using Newtonsoft.Json;

namespace QuizardWatch
{
    [Activity(Label = "SetPage"/*, MainLauncher = false, Icon = "@drawable/icon"*/)]
    public class SetPage : Activity
    {
        private ListView QuizList;
        //Temporary List of Dummy data
        private List<string> tempList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.CardSets);
            tempList = new List<string>();
            tempList.Add("History");
            tempList.Add("Mathematics");
            tempList.Add("Literature");
            tempList.Add("Chemistry");
            tempList.Add("French");

            QuizList = FindViewById<ListView>(Resource.Id.QuizList);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, tempList);

            QuizList.Adapter = adapter;

            QuizList.ItemClick += QuizList_ItemClick;
            
        }

        private async void QuizList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Test for getting data of list
            String SelectedSet = tempList[e.Position];
            //Sets Up an Intent For the Next Activity
            Intent intent = new Intent(this, typeof(QuestionPage));

            //Setting up data to be transfered between pages, this will be based on the SelectedSet
            DataStruct data = new DataStruct()
            {
                Answers = new List<string>(),//List Of Answers Obtained from the phone
                Questions = new List<string>(),//List Of Questios Obtained from the phone
                NameOfSet = SelectedSet,
                Count = 1,
                Correnct = 0,
                Incorrect = 0,
            };
            //Option A Serializing Individual basic type data sets
            intent.PutExtra("Name Of Set", SelectedSet);
            intent.PutExtra("Question Number", 1);
            //Option B Serializing an Object using Json
            intent.PutExtra("data", JsonConvert.SerializeObject(data));
            //Starts the Next Activity
            this.StartActivity(intent);
        }
    }
}