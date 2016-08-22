using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

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
        string mQuestion, mCorrectAnswer; // holds string values for correct answer an the question
        List<string> mChoices; // holds the 4 possible answers
        int mCorrectAnswerIndex; // the index of the correct answer in mChoices
        // initialize variables
        public Question(string question, string answer)
        {
            mQuestion = question;
            mCorrectAnswer = answer;
        } 
        // returns question string
        public string GetQuestion()
        {
            return mQuestion;
        }
        // returns answer string
        public string GetCorrectAnswer()
        {
            return mCorrectAnswer;
        }
        // returns list of multiple choice answers
        public List<string> GetChoices()
        {
            return mChoices;
        }
        // returns correct answer index from mChoices
        public int GetCorrectIndex()
        {
            return mCorrectAnswerIndex;
        }
        // sets the question string
        public void SetQuestion(string _question)
        {
            mQuestion = _question;
        }
        // sets the answer string
        public void SetCorrectAnswer(string _answer)
        {
            mCorrectAnswer = _answer;
        }
        // set the index for the correct answer
        void SetCorrectAnswerIndex(int index)
        {
            mCorrectAnswerIndex = index;
        }
        //shuffles mChoices
         public List<string> Shuffle(List<string> array)
        {
            List<string> BufferList = new List<string>();
            Random rng = new Random();
            for (int loop = 0; loop < 4; loop++)
            {
               int Range = rng.Next(1, 5000 + loop);
                int index = Range % array.Count;
                BufferList.Add(array[index]);
                array.Remove(array[index]);
            }
            return BufferList;
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
            
            Random tRand = new Random();
            
            int tIndex = 0;
            List<string> bufferList = new List<string>();
            bufferList = allAnswers.ToList<string>();
            bufferList.Remove(GetCorrectAnswer()); // finds and removes correct answer

            List<string> tChoices = new List<string>();
            // pull out 4 answers
            for (int i = 0; i < 4; i++)
            {
                int range = tRand.Next(1,7000+i);
                int index = range % bufferList.Count;
                tChoices.Add(bufferList[index]);
                bufferList.RemoveAt(index);
            }
            // check for correct answer in list, add if necessary, and assign index
            bool correctAdded = false;
            for (int i = 0; i < tChoices.Count; i++)
            {
                if (tChoices[i] == mCorrectAnswer)
                {
                    correctAdded = true;
                    mCorrectAnswerIndex = i;
                }
            }

            if (!correctAdded)
            {
                mCorrectAnswerIndex = tRand.Next(0, 3);
                tChoices[mCorrectAnswerIndex] = GetCorrectAnswer();
            }

            tChoices = Shuffle(tChoices);
            
            for (int i = 0; i < tChoices.Count; i++)
            {
                if (tChoices[i] == mCorrectAnswer)
                    mCorrectAnswerIndex = i;
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

        ArrayAdapter mAdapterQ; // database adapter
        string mUsername, mSetName; // database strings
         
        int mRightAnswers = 0; // count of correct answers

        Button mNextButton; // next button
        ImageButton mHomeButton; // home button

        List<Question> mQuiz; // list containing all of the questions
        // initialize all variables
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
        // creates view, checks for a minimum of 4 cards && sets up all widgets
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            FragmentManager.PopBackStack();

            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DeckQuizTab, container, false);

            if (mQuestionList.Count >= 4)
            {
                
                // TODO: Get cards from database to fill questionList and answerList
                RetrieveCards(mUsername, mSetName);


                mHomeButton = view.FindViewById<ImageButton>(Resource.Id.QuizTabHomeButtonID);
                mHomeButton.Click += HomeButton_Click;

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

                mNextButton = view.FindViewById<Button>(Resource.Id.quizTabNextButtonID);
                mNextButton.Visibility = ViewStates.Gone;

                // answer list view click event. Changes quiz to next question. 
                answerListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
                {
                    int temp = mQuiz[mCurrPosition].GetCorrectIndex();
                    if (e.Position == mQuiz[mCurrPosition].GetCorrectIndex())
                    {
                        mRightAnswers++;
                        UpdateCardNumberBox(mUsername, mSetName, mQuestionList[mCurrPosition], mAnswerList[mCurrPosition], true);
                    }
                    else
                    {
                        UpdateCardNumberBox(mUsername, mSetName, mQuestionList[mCurrPosition], mAnswerList[mCurrPosition], false);
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
                        mNextButton.Visibility = ViewStates.Gone;
                    }
                    else 
                    {
                        int percentage = (int)Math.Round((double)(100 * mRightAnswers) / mAnswerList.Count); ;
                        questionTextView.Text = "You finished with a " + percentage + "%";
                        mRightAnswers = 0;
                        mCurrPosition = 0;
                        answerListView.Visibility = ViewStates.Gone;
                    }
                };
                mNextButton.Click += NextButton_Click;


                AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);

                alert.SetTitle("Quiz will now begin");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    return;
                });

                this.Activity.RunOnUiThread(() =>
                {
                    alert.Show();
                });
            }
            else
            {


                AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);

                alert.SetTitle("You need more cards");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    return;
                });

                this.Activity.RunOnUiThread(() =>
                {
                    alert.Show();
                });

            }
            return view;
        }
        // handles click event for homepage button
        private void HomeButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(HomeActivity));
            intent.PutExtra("UserName", mUsername);
            StartActivity(intent);
        }
        // handles next button for going through quiz
        // ** not implemented **
         private void NextButton_Click(object sender, EventArgs e)
        {
            questionTextView.Text = mQuiz[mCurrPosition].GetQuestion();
            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mQuiz[mCurrPosition].GetChoices());
            answerListView.Adapter = ListAdapter;
            mNextButton.Visibility = ViewStates.Gone;
        }
        // creates the list of questions and then sets mQuiz 
        void InitQuiz(List<string> questions, List<string> answers)
        {
            mQuiz = new List<Question>();
            List<string> tList = new List<string>();
            tList = answers;
            // 
            for (int i = 0; i < mQuestionList.Count; i++)
            {
                Question tQuestion = new Question(questions[i], answers[i]);
                tQuestion.CreateChoices(tList);
                mQuiz.Add(tQuestion);
            }

        }
        // database function for getting list of cards
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
        // database function for leitner system
        public void UpdateCardNumberBox(string _Username, string _Setname, string _Question, string _Answer, bool _Correct)
        {
            DataBase.DBAdapter db = new DataBase.DBAdapter(mContext);
            DataBase.Cards BufferCard = new DataBase.Cards(_Username, _Setname, _Question, _Answer, "", "");
            db.openDB();
            BufferCard = db.GetSpecificCard(BufferCard);
            if(_Correct)
            {
                int NuberBoxBuffer = Convert.ToInt32(BufferCard.GetNumberBox()) + 1;
                BufferCard.SetNumberBox(NuberBoxBuffer.ToString());
            }
            else
                BufferCard.SetNumberBox("0");

            db.UpdateCard(BufferCard, "", "");
            db.CloseDB();
        }
    }
}