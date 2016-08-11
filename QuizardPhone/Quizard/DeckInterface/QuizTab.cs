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
using Android.Database;

namespace Quizard.DeckInterface
{

public class DeckQuizTabFragment : Fragment
{
    List<string> mQuestionList, mAnswerList; // list for questions and answers
    int mCurrPosition = 0; // current position index
    TextView questionTextView; // text view for question
    ListView answerListView; // list view for answers
    Context mContext;

    public DeckQuizTabFragment(List<string> _questionList, List<string> _answerList)
    {
        mQuestionList = _questionList;
        mAnswerList = _answerList;
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

        // TODO: Get cards from database to fill questionList and answerList


        // set question text view to a question
        questionTextView = view.FindViewById<TextView>(Resource.Id.quizTabQuestionTextView);
        questionTextView.Text = mQuestionList[mCurrPosition];

        // set answer list view to display answerList.
        answerListView = view.FindViewById<ListView>(Resource.Id.quizTabAnswerListView);
        ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mAnswerList);
        answerListView.Adapter = ListAdapter;

        // answer list view click event. Changes quiz to next question. 
        // TODO: Needs to check for actual correct answer
        answerListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
        {
            if (mCurrPosition + 1 < mQuestionList.Count)
            {
                mCurrPosition++;
                questionTextView.Text = mQuestionList[mCurrPosition];
            }
            else // TODO: Insert actual score here
            {
                questionTextView.Text = "You finished!";
                mCurrPosition = 0;
            }
        };
        return view;
    }

}
//public class DeckQuizDialogFragment : DialogFragment
//{
//    List<string> mAnswers, mQuestions;
//    int mPosition, correctPosition;
//    Button mNextButton;
//    View mView;
//    public DeckQuizDialogFragment(List<string> _list, int pos)
//    {
//        mAnswers = new List<string>();
//        mAnswers = _list;
//        mPosition = pos;
//    }
//
//    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//    {
//        base.OnCreate(savedInstanceState);
//
//        mView = inflater.Inflate(Resource.Layout.DeckQuizDialogBox, container, false);
//        var textView = mView.FindViewById<TextView>(Resource.Id.quizRightWrongTextView);
//
//        if (mPosition == 2)
//            textView.Text = "CORRECT!";
//        else
//            textView.Text = "WRONG! You chose " + mAnswers[mPosition] + ", Answer was " + mAnswers[2];
//
//        mNextButton = mView.FindViewById<Button>(Resource.Id.quizDialobNextButton);
//
//        mNextButton.Click += NextButton_Click;
//
//            // Set up a handler to dismiss this DialogFragment when this button is clicked.
//            mView.FindViewById<Button>(Resource.Id.quizDialogRedoButton).Click += (sender, args) => Dismiss();
//
//        return mView;
//    }
//
//    private void NextButton_Click(object sender, EventArgs e)
//    {
//        mPosition++;
//        Dismiss();
//    }
//}


}