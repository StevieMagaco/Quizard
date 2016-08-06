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

namespace Quizard.Resources.layout
{
    public class DeckQuizDialogFragment : DialogFragment
    {
        List<string> list;
        int position;

        Button nextButton;
        public DeckQuizDialogFragment(List<string> _list, int pos, Action<bool> nextResult)
        {
            list = new List<string>();
            list = _list;
            position = pos;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);



            var view = inflater.Inflate(Resource.Layout.DeckQuizDialogBox, container, false);
            var textView = view.FindViewById<TextView>(Resource.Id.quizRightWrongTextView);

            if (position == 2)
                textView.Text = "CORRECT!";
            else
                textView.Text = "WRONG! You chose " + list[position] + ", Answer was " + list[2];

            nextButton = view.FindViewById<Button>(Resource.Id.quizDialobNextButton);

            nextButton.Click += NextButton_Click;

            // Set up a handler to dismiss this DialogFragment when this button is clicked.
            view.FindViewById<Button>(Resource.Id.quizDialogRedoButton).Click += (sender, args) => Dismiss();
            return view;
        }
        private void NextButton_Click(object sender, EventArgs e)
        {
            position++;
            Dismiss();
        }
    }

}

