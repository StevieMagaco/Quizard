using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Database.Sqlite;
using Android.Database;

namespace Quizard
{
    [Activity(Label = "DeckActivity", MainLauncher = false /*Keep the MainLauncher = false unless this dialog fragment needs to be tested*/)]
    public class DeckActivity : Activity
    {
        List<string> mQuestions, mAnswers, mQuizAnswers;
        DataBase.UserInfo UserInformation = new DataBase.UserInfo();

        //public DeckActivity(List<string> _questions, List<string> _answers)
        //{
        //    questions = new List<string>();
        //    questions = _questions;
        //    answers = new List<string>();
        //    answers = _answers;
        //}

        // Initializes list, and creates action bar
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            mQuestions = new List<string>();

            mAnswers = new List<string>();


            mQuizAnswers = new List<string>();

            // Set our view from the "DeckLayout" layout resource
            SetContentView(Resource.Layout.DeckLayout);


            string[] UserSetname_Buffer = Intent.GetStringArrayExtra("Username/SetName");
            this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            UserInformation.GetUser().SetUsername(UserSetname_Buffer[0]);
            string mSetName = UserSetname_Buffer[1];


            AddTab("CARDS", Resource.Drawable.cardIconSmall, new DeckCardTabFragment(mQuestions, mAnswers, UserSetname_Buffer, this));
            AddTab("QUIZ", Resource.Drawable.quizIcon, new DeckQuizTabFragment(mQuestions, mQuizAnswers));


            if (bundle != null)
                this.ActionBar.SelectTab(this.ActionBar.GetTabAt(bundle.GetInt("tab")));
        }

        // Adds tab to action bar
        void AddTab(string tabText, int iconResourceId, Fragment view)
        {
            var tab = this.ActionBar.NewTab();

            tab.SetText(tabText);
            tab.SetIcon(iconResourceId);

            // must set event handler before adding tab
            tab.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
            {
                var fragment = this.FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);

                if (fragment != null)
                    e.FragmentTransaction.Remove(fragment);

                e.FragmentTransaction.Add(Resource.Id.fragmentContainer, view);
            };

            tab.TabUnselected += delegate (object sender, ActionBar.TabEventArgs e)
            {
                e.FragmentTransaction.Remove(view);
            };

            this.ActionBar.AddTab(tab);
        }
    }

    public class DeckCardDialogFragment : DialogFragment
    {
        List<string> mQuestions, mAnswers;
        ArrayAdapter mAdapterQ, mAdapterA;
        int mPosition;
        Button mEditButton;
        TextView mQuestionTextView, mAnswerTextView;

        public DeckCardDialogFragment(List<string> _questions, List<string> _answers, int pos)
        {
            mQuestions = new List<string>();
            mQuestions = _questions;
            mPosition = pos;
            mAnswers = new List<string>();
            mAnswers = _answers;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DeckCardDialogBox, container, false);
            mQuestionTextView = view.FindViewById<TextView>(Resource.Id.cardDialogQuestionTextViewID);
            mQuestionTextView.Text = mQuestions[mPosition];

            mAnswerTextView = view.FindViewById<TextView>(Resource.Id.cardDialogAnswerTextViewID);
            mAnswerTextView.Text = mAnswers[mPosition];

            // Set up a handler to dismiss this DialogFragment when this button is clicked.
            view.FindViewById<Button>(Resource.Id.cardDialogExitButtonID).Click += (sender, args) => Dismiss();


            mEditButton = view.FindViewById<Button>(Resource.Id.cardDialogEditButtonID);
            mEditButton.Click += EditButton_Click;

            return view;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            DeckCardTabEditFragment aDifferentDetailsFrag = new DeckCardTabEditFragment(mQuestions[mPosition], mAnswers[mPosition]);
            aDifferentDetailsFrag.Show(fragmentTx, "Edit dialog fragment");
        }

    }

    public class DeckCardTabEditFragment : DialogFragment
    {
        EditText mQuestionText, mAnswerText;

        public DeckCardTabEditFragment(string question, string answer)
        {
            mQuestionText = new EditText(this.Activity);
            mAnswerText = new EditText(this.Activity);
            mQuestionText.FindViewById<EditText>(Resource.Id.questionTextBox);
            mAnswerText.FindViewById<EditText>(Resource.Id.answerTextBox);
            mQuestionText.Text = question;
            mAnswerText.Text = answer;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.DeckCardTab, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }





    public class DeckCardTabFragment : Fragment
    {
        List<string> mQuestions, mAnswers;
        ArrayAdapter mAdapterQ;
        ListView mCardTabListView;
        ImageButton mPlayButton, mAddButton, mHomeButton;
        Button mAddCardNextButton, mDoneButton;
        EditText mAddCardQuestionText, mAddCardAnswerText;
        string mUsername, mSetName;
        Context mContext;
        public DeckCardTabFragment(List<string> _questions, List<string> _answers, string[] UserSetName, Context _Context)
        {

            mQuestions = new List<string>(_questions);
            mAnswers = new List<string>(_answers);
            mUsername = UserSetName[0];
            mSetName = UserSetName[1];
            mQuestions = _questions;
            mContext = _Context;
        }
        //public override void OnAttach(Activity activity)
        //{
        //    base.OnAttach(activity);
        //    mContext = activity;
        //}
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            FragmentManager.PopBackStack();

            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DeckCardTab, container, false);

            mCardTabListView = view.FindViewById<ListView>(Resource.Id.cardTabListView);
            

            mCardTabListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                string selectedFromList = (string)mCardTabListView.GetItemAtPosition(e.Position);
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                DeckCardDialogFragment aDifferentDetailsFrag = new DeckCardDialogFragment(mQuestions, mAnswers, e.Position);
                aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");
            };

            mAddCardQuestionText = view.FindViewById<EditText>(Resource.Id.deckAddCardQuestionEditTextID);

            mAddCardAnswerText = view.FindViewById<EditText>(Resource.Id.deckAddCardAnswerEditTextID);

            mAddCardNextButton = view.FindViewById<Button>(Resource.Id.deckAddToDeckButtonID);
            mAddCardNextButton.Click += AddCardNextButton_Click;
            mDoneButton = view.FindViewById<Button>(Resource.Id.deckDoneAddCardButtonID);
            mDoneButton.Click += DoneButton_Click;

            mPlayButton = view.FindViewById<ImageButton>(Resource.Id.cardTabPlayButton);
            mPlayButton.Click += PlayButton_Click;

            mAddButton = view.FindViewById<ImageButton>(Resource.Id.cardTabAddButton);
            mAddButton.Click += AddButton_Click;

            mHomeButton = view.FindViewById<ImageButton>(Resource.Id.cardTabHomeButton);
            mHomeButton.Click += HomeButton_Click;
            mAdapterQ = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mQuestions);
            RetrieveCards(mUsername, mSetName);
            return view;
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {

            string tempSubject, tempAnswer;
            tempSubject = mAddCardQuestionText.Text;
            tempAnswer = mAddCardAnswerText.Text;

            mQuestions.Add(tempSubject);
            mAnswers.Add(tempAnswer);



            mAddCardQuestionText.Visibility = ViewStates.Invisible;
            mAddCardAnswerText.Visibility = ViewStates.Invisible;

            mDoneButton.Visibility = ViewStates.Invisible;
            mAddCardNextButton.Visibility = ViewStates.Invisible;

            // TODO
        }

        private void AddCardNextButton_Click(object sender, EventArgs e)
        {
            //string tempSubject, tempAnswer;
            //tempSubject = mAddCardQuestionText.Text;
            //tempAnswer = mAddCardAnswerText.Text;
            ////temp code til added to actual database
            //mQuestions.Add(tempSubject);
            //mAnswers.Add(tempAnswer);
             AddCard_db(mUsername, mSetName, mAddCardQuestionText.Text, mAddCardAnswerText.Text);
            
            mAddCardQuestionText.Text = "";
            mAddCardAnswerText.Text = "";
            // TODO: add to actual deck
            // ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mQuestions);


        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(HomeActivity));
            intent.PutExtra("UserName", mUsername);
            StartActivity(intent);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {

            mAddCardQuestionText.Visibility = ViewStates.Visible;
            mAddCardAnswerText.Visibility = ViewStates.Visible;

            mDoneButton.Visibility = ViewStates.Visible;
            mAddCardNextButton.Visibility = ViewStates.Visible;


            // TODO: Add code for adding it into actual set 

        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            deckPlayFragment fragment = new deckPlayFragment(mQuestions, mAnswers);
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragment.Show(fragmentTx, "dialog_fragment");
        }
        private bool AddCard_db(string _Username, string _SetName, string _Question, string _Answer)
        {
            try
            {
                DataBase.Cards CardBuffer = new DataBase.Cards(_Username, _SetName, _Question, _Answer, "", "");
                DataBase.DBAdapter db = new DataBase.DBAdapter(mContext);
                db.openDB();
                if (db.SetCard(CardBuffer))
                {
                    RetrieveCards(_Username, _SetName);

                    db.CloseDB();
                    return true;
                }
                else
                    throw new System.ArgumentException("Failed to save new Card", "CardSave");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(mContext, "Failed to add card", ToastLength.Short).Show();
                return false;
            }
        }
        private void RetrieveCards(string _Username, string _SetName)
        {
            try
            {
                DataBase.DBAdapter db = new DataBase.DBAdapter(mContext);
                db.openDB();
                
                ICursor CardsInfo = db.GetCards(_Username, _SetName);
                mAnswers.Clear();
                mQuestions.Clear();
                while (CardsInfo.MoveToNext())
                {
                    string Question = CardsInfo.GetString(2);
                    string Answer = CardsInfo.GetString(3);
                    mAnswers.Add(Answer);
                    mQuestions.Add(Question);
                }
                db.CloseDB();

                mCardTabListView.Adapter = mAdapterQ;

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(mContext, "Failed to retrieve Cards", ToastLength.Short).Show();
            }

        }
    }



    public class deckPlayFragment : DialogFragment
    {
        List<string> mQuestions, mAnswers;
        int mPosition;
        Button mNextButton, mAnswerButton;

        TextView mTxtView;
        public deckPlayFragment(List<string> _questions, List<string> _answers)
        {
            mQuestions = new List<string>();
            mAnswers = new List<string>();

            mQuestions = _questions;
            mAnswers = _answers;
            mPosition = 0;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.deckPlayLayout, container, false);

            mNextButton = view.FindViewById<Button>(Resource.Id.nextCardButton);
            mAnswerButton = view.FindViewById<Button>(Resource.Id.viewAnswerButton);

            mTxtView = view.FindViewById<TextView>(Resource.Id.deckTextView);

            mTxtView.Text = mQuestions[mPosition];
            mNextButton.Enabled = false;
            mNextButton.Click += NextButton_Click;
            mAnswerButton.Click += AnswerButton_Click;
            return view;
        }

        private void AnswerButton_Click(object sender, EventArgs e)
        {
            mNextButton.Enabled = true;
            mAnswerButton.Enabled = false;
            mTxtView.Text = mAnswers[mPosition];
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (mPosition + 1 < mQuestions.Count)
                mPosition++;
            else
                Dismiss();
            mTxtView.Text = mQuestions[mPosition];

            mNextButton.Enabled = false;
            mAnswerButton.Enabled = true;
        }
    }
    public class DeckQuizDialogFragment : DialogFragment
    {
        List<string> mAnswers, mQuestions;
        int mPosition, correctPosition;
        Button mNextButton;

        public DeckQuizDialogFragment(List<string> _list, int pos, Action<bool> nextResult)
        {
            mAnswers = new List<string>();
            mAnswers = _list;
            mPosition = pos;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DeckQuizDialogBox, container, false);
            var textView = view.FindViewById<TextView>(Resource.Id.quizRightWrongTextView);

            if (mPosition == 2)
                textView.Text = "CORRECT!";
            else
                textView.Text = "WRONG! You chose " + mAnswers[mPosition] + ", Answer was " + mAnswers[2];

            mNextButton = view.FindViewById<Button>(Resource.Id.quizDialobNextButton);

            mNextButton.Click += NextButton_Click;

            // Set up a handler to dismiss this DialogFragment when this button is clicked.
            view.FindViewById<Button>(Resource.Id.quizDialogRedoButton).Click += (sender, args) => Dismiss();

            return view;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            mPosition++;
            Dismiss();
        }
    }
    public class deckPlayDialogFragment : DialogFragment
    {
        List<string> mQuestions, mAnswers;
        int mPosition;
        Button mAnswerButton;
        View mView;
        ViewGroup mContainer;
        LayoutInflater mInflater;

        public deckPlayDialogFragment(List<string> _questions, List<string> _answers)
        {
            mQuestions = new List<string>();
            mAnswers = new List<string>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mInflater = inflater;
            mContainer = container;


            mView = inflater.Inflate(Resource.Layout.PlaySetLayout_Front, container, false);


            // Set up a handler to dismiss this DialogFragment when this button is clicked.
            mAnswerButton = mView.FindViewById<Button>(Resource.Id.PlaySetFront_ViewAnswerButton);
            mAnswerButton.Click += AnswerButton_Click;
            return mView;
        }

        private void AnswerButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            deckPlayDialogFragment_Answer aDifferentDetailsFrag = new deckPlayDialogFragment_Answer(mAnswers[mPosition]);
            aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");

        }
    }
    public class deckPlayDialogFragment_Answer : DialogFragment
    {
        TextView mAnswerTextView;
        string mAnswer;
        public deckPlayDialogFragment_Answer(string _answer)
        {
            mAnswer = _answer;

        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.PlaySetLayout_Front, container, false);

            mAnswerTextView = view.FindViewById<TextView>(Resource.Id.answerTextView);

            return view;
        }
    }
    public class DeckQuizTabFragment : Fragment
    {
        List<string> mQuestionList, mAnswerList;
        int mCurrPosition = 0;
        Action<bool> mNextButtonDialogResult;

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

            var sampleTextView = view.FindViewById<TextView>(Resource.Id.quizTabQuestionTextView);
            sampleTextView.Text = mQuestionList[mCurrPosition];

            var sampleListView = view.FindViewById<ListView>(Resource.Id.quizTabAnswerListView);

            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mAnswerList);

            sampleListView.Adapter = ListAdapter;

            sampleListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                DeckQuizDialogFragment quizDialogFrag = new DeckQuizDialogFragment(mAnswerList, e.Position, mNextButtonDialogResult);
                quizDialogFrag.Show(fragmentTx, "dialog_fragment");
            };

            return view;
        }
    }
}