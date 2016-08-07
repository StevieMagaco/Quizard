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
    [Activity(Label = "AnswerPage")]
    public class AnswerPage : Activity
    {
        DataStruct data;
        private TextView Title;
        private TextView Answer;
        private ImageButton CorrectButton;
        private ImageButton IncorrectButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AnswerFlashCard);

            //Aquireing Objects from the QuestionFlashCard Layout
            Title = FindViewById<TextView>(Resource.Id.AnswerTitle);
            Answer = FindViewById<TextView>(Resource.Id.AnswerTextBox);
            CorrectButton = FindViewById<ImageButton>(Resource.Id.AnswerCorrect);
            IncorrectButton = FindViewById<ImageButton>(Resource.Id.AnswerIncorrect);

            //Option A getting data individually
            Title.Text = "Question: " + Intent.GetIntExtra("Question Number", 0).ToString();
            Answer.Text = Intent.GetStringExtra("Name Of Set") + " Answer!";
            //Option B getting an entire serialized Object
            data = JsonConvert.DeserializeObject<DataStruct>(Intent.GetStringExtra("data"));

            //Setting up a Click event for the Buttons
            IncorrectButton.Click += IncorrectButton_Click;
            CorrectButton.Click += CorrectButton_Click;

           
        }

        private void CorrectButton_Click(object sender, EventArgs e)
        {            
            //Add true to correct bool
            Transition_To_QuestionPage();
        }

        private void IncorrectButton_Click(object sender, EventArgs e)
        {
            //Add true to inccorrect bool
            Transition_To_QuestionPage();
        }

        private void Transition_To_QuestionPage()
        {
            //Increment Question
            data.Count++;

            //Sets Up an Intent For the Next Activity
            Intent intent = new Intent(this, typeof(QuestionPage));

            //Option A Serializing Individual basic type data sets
            intent.PutExtra("Name Of Set", data.NameOfSet);
            intent.PutExtra("Question Number", data.Count);
            //Option B Serializing an Object using Json
            intent.PutExtra("data", JsonConvert.SerializeObject(data));

            //Starts the Next Activity
            this.StartActivity(intent);
            //Closes this Activity
            this.Finish();
        }
    }
}