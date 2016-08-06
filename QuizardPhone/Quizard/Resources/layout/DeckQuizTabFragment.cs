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
    public class DeckQuizTabFragment : Fragment
    {
        List<string> questionList;
        List<string> answerList;
        int currPosition = 0;
        Action<bool> nextButtonDialogResult;

        public DeckQuizTabFragment(List<string> _questionList, List<string> _answerList)
        {
            questionList = _questionList;
            answerList = _answerList;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            FragmentManager.PopBackStack();

            base.OnCreateView(inflater, container, savedInstanceState);


            var view = inflater.Inflate(Resource.Layout.DeckQuizTab, container, false);

            var sampleTextView = view.FindViewById<TextView>(Resource.Id.quizTabQuestionTextView);
            sampleTextView.Text = questionList[currPosition];

            var sampleListView = view.FindViewById<ListView>(Resource.Id.quizTabAnswerListView);

            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, answerList);

            sampleListView.Adapter = ListAdapter;

            sampleListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                DeckQuizDialogFragment aDifferentDetailsFrag = new DeckQuizDialogFragment(answerList, e.Position, nextButtonDialogResult);
                aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");
            };
            return view;
        }
    }
}