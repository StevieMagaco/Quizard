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
using Android;

namespace Quizard.Resources.layout
{
    public class DeckCardDialogFragment : DialogFragment
    {
        List<string> questions;
        List<string> answers;
        int position;

        Button viewAnswerButton;

        TextView textView;
        public DeckCardDialogFragment(List<string> _questions, List<string> _answers, int pos)
        {
            questions = new List<string>();
            questions = _questions;
            position = pos;
            answers = new List<string>();
            answers = _answers;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            var view = inflater.Inflate(Resource.Layout.DeckCardDialogBox, container, false);
            textView = view.FindViewById<TextView>(Resource.Id.cardDialogTextView);

            textView.Text = questions[position];


            // Set up a handler to dismiss this DialogFragment when this button is clicked.
            view.FindViewById<Button>(Resource.Id.cardDialogExitButton).Click += (sender, args) => Dismiss();


            viewAnswerButton = view.FindViewById<Button>(Resource.Id.cardDialogAnswerButton);
            viewAnswerButton.Click += ViewAnswerButton_Click;
            return view;
        }

        private void ViewAnswerButton_Click(object sender, EventArgs e)
        {
            textView.Text = answers[position];

        }
    }
}