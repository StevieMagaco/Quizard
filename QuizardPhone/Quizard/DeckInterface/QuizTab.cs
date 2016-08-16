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
public class Question
{
        string mQuestion, mCorrectAnswer;
        List<string> mChoices;
        int mCorrectAnswerIndex;
        
        public Question(string question, string answer)
        {
            mQuestion = question;
            mCorrectAnswer = answer;
        }
        public string GetQuestion()
        {
            return mQuestion;
        }
        public string GetCorrectAnswer()
        {
            return mCorrectAnswer;
        }
        public List<string> GetChoices()
        {
            return mChoices;
        }
        public int GetCorrectIndex()
        {
            return mCorrectAnswerIndex;
        }
        public void SetQuestion(string _question)
        {
            mQuestion = _question;
        }
        public void SetCorrectAnswer(string _answer)
        {
            mCorrectAnswer = _answer;
        }
        void SetCorrectAnswerIndex(int index)
        {
            mCorrectAnswerIndex = index;
        }
        // Takes in full list of answers, and pulls out 4 for question
        // then sets correct answer index, and sets mChocies to tChoices
        public void CreateChoices(List<string> allAnswers)
        {
            // clear any bad information from mChoices
            if (mChoices != null)
                mChoices.Clear();
            else
                mChoices = new List<string>();


            int tIndex = 0;
            Random tRand = new Random();
            List<string> tChoices = new List<string>();
            // pull out 4 answers
            for (int i = 0; i < 4; i++)
            {
                tChoices.Add(allAnswers[tRand.Next() % allAnswers.Count]);
            }
            // check for correct answer in list, add if necessary  assign index
            bool correctAdded = false;
            for (int i = 0; i < tChoices.Count;i++)
            {
                if (tChoices[i] == mCorrectAnswer)
                {
                    correctAdded = true;
                    mCorrectAnswerIndex = i;
                }
            }

            if (!correctAdded)
            {
                int temp = tRand.Next();
                mCorrectAnswerIndex = temp % 4;
                tChoices[mCorrectAnswerIndex] = GetCorrectAnswer();
            }

            // set mChoices equal to tChoices
            mChoices = tChoices;
        }

}

public class DeckQuizTabFragment : Fragment
{
    List<string> mQuestionList, mAnswerList; // list for questions and answers
    int mCurrPosition = 0; // current position index
    TextView questionTextView; // text view for question
    ListView answerListView; // list view for answers
    Context mContext;

        ArrayAdapter mAdapterQ;
        string mUsername, mSetName;

        int rightAnswers = 0;

    Button nextButton;

    List<Question> mQuiz;
        public DeckQuizTabFragment(List<string> _questionList, List<string> _answerList, Context _Context, string[] _UserSetName)
        {
            mQuestionList = _questionList;
            mAnswerList = _answerList;
            mContext = _Context;
            mUsername = _UserSetName[0];
            mSetName = _UserSetName[1];
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
            RetrieveCards(mUsername, mSetName);

            // set question text view to a question
            questionTextView = view.FindViewById<TextView>(Resource.Id.quizTabQuestionTextView);
         questionTextView.Text = mQuestionList[mCurrPosition];

         // set answer list view to display answerList.
         answerListView = view.FindViewById<ListView>(Resource.Id.quizTabAnswerListView);
         // temp code, eventually answerList will hold all possible answers and I will 
         // add the list for the listview when i get the actual answers
         if (mAnswerList.Count == 0)
         {
             mAnswerList.Add("answe1r"); mAnswerList.Add("an2swer"); mAnswerList.Add("an3swer"); mAnswerList.Add("an4swer");
             mAnswerList.Add("an5swer"); mAnswerList.Add("ans6wer"); mAnswerList.Add("an7swer");
         }



        InitQuiz(mQuestionList, mAnswerList);
        ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mQuiz[0].GetChoices());
        answerListView.Adapter = ListAdapter;

        nextButton = view.FindViewById<Button>(Resource.Id.quizTabNextButtonID);
        nextButton.Visibility = ViewStates.Gone;

        // answer list view click event. Changes quiz to next question. 
        // TODO: Needs to check for actual correct answer
        answerListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
        {
            int temp = mQuiz[mCurrPosition].GetCorrectIndex();
            if (e.Position == mQuiz[mCurrPosition].GetCorrectIndex())
            {
                rightAnswers++;
            }
            if (mCurrPosition + 1 < mQuestionList.Count)
            {
                mCurrPosition++;
                
                // todo:fix nextButton
                //nextButton.Visibility = ViewStates.Visible;

                // remove when nextButton is fixed
                questionTextView.Text = mQuiz[mCurrPosition].GetQuestion();
                ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mQuiz[mCurrPosition].GetChoices());
                answerListView.Adapter = ListAdapter;
                nextButton.Visibility = ViewStates.Gone;
            }
            else // TODO: Insert actual score here
            {
                int percentage = (rightAnswers / mQuiz.Count) * 100;
                questionTextView.Text = "You finished with a " + percentage + "%";
                mCurrPosition = 0;
                answerListView.Visibility = ViewStates.Gone;
                
            }
        };

        nextButton.Click += NextButton_Click;
        return view;
    }

        private void NextButton_Click(object sender, EventArgs e)
        {
            questionTextView.Text = mQuiz[mCurrPosition].GetQuestion();
            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mQuiz[mCurrPosition].GetChoices());
            answerListView.Adapter = ListAdapter;
            nextButton.Visibility = ViewStates.Gone;
        }
        // 
        void InitQuiz(List<string> questions, List<string> answers)
        {
            mQuiz = new List<Question>();
            // 
            for (int i =0; i < mQuestionList.Count;i++)
            {
                Question tQuestion = new Question(questions[i], answers[i]);
                tQuestion.CreateChoices(answers);
                mQuiz.Add(tQuestion);
            }
             
        }
        private void RetrieveCards(string _Username, string _SetName)
        {
            try
            {
                DataBase.DBAdapter db = new DataBase.DBAdapter(mContext);
                db.openDB();

                ICursor CardsInfo = db.GetCards(_Username, _SetName);
                mAnswerList.Clear();
                mQuestionList.Clear();
                while (CardsInfo.MoveToNext())
                {
                    string Question = CardsInfo.GetString(2);
                    string Answer = CardsInfo.GetString(3);
                    mAnswerList.Add(Answer);
                    mQuestionList.Add(Question);
                }
                db.CloseDB();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(mContext, "Failed to retrieve Cards", ToastLength.Short).Show();
            }

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