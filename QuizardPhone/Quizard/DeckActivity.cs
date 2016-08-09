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
            mQuestions.Add("question 1");
            mQuestions.Add("question 2");
            mQuestions.Add("question 3");
            mQuestions.Add("question 4");
            mQuestions.Add("question 5");

            mAnswers = new List<string>();
            mAnswers.Add("answer 1");
            mAnswers.Add("answer 2");
            mAnswers.Add("answer 3");
            mAnswers.Add("answer 4");
            mAnswers.Add("answer 5");


            mQuizAnswers = new List<string>();
            mQuizAnswers.Add("1) answer");
            mQuizAnswers.Add("2) answer");
            mQuizAnswers.Add("3) answer");
            mQuizAnswers.Add("4) answer");

            // Set our view from the "DeckLayout" layout resource
            SetContentView(Resource.Layout.DeckLayout);
            string[] UserSetname_Buffer = Intent.GetStringArrayExtra("Username/SetName");
            this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            UserInformation.GetUser().SetUsername(UserSetname_Buffer[0]);
            string mSetName = UserSetname_Buffer[1];
            AddTab("CARDS", Resource.Drawable.cardIconSmall, new DeckCardTabFragment(mQuestions, mAnswers, UserSetname_Buffer));
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
        int mPosition;
        Button mViewAnswerButton, mEditButton;
        TextView mTextView;

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
            mTextView = view.FindViewById<TextView>(Resource.Id.cardDialogTextView);
            mTextView.Text = mQuestions[mPosition];

            // Set up a handler to dismiss this DialogFragment when this button is clicked.
            view.FindViewById<Button>(Resource.Id.cardDialogExitButton).Click += (sender, args) => Dismiss();

            mViewAnswerButton = view.FindViewById<Button>(Resource.Id.cardDialogAnswerButton);
            mViewAnswerButton.Click += ViewAnswerButton_Click;

            mEditButton = view.FindViewById<Button>(Resource.Id.cardDialogEditButton);
            mEditButton.Click += EditButton_Click;

            return view;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            DeckCardTabEditFragment aDifferentDetailsFrag = new DeckCardTabEditFragment(mQuestions[mPosition], mAnswers[mPosition]);
            aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");
        }

        private void ViewAnswerButton_Click(object sender, EventArgs e)
        {
            mTextView.Text = mAnswers[mPosition];
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
        ListView mCardTabListView;
        ImageButton mPlayButton, mAddButton, mHomeButton;
        string mUsername;
        public DeckCardTabFragment(List<string> _questions, List<string> _answers, string[] UserSetName)
        {

            mQuestions = new List<string>();
            mAnswers = new List<string>();
            mUsername = UserSetName[0];
            mQuestions = _questions;
            mAnswers = _answers;
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

            var view = inflater.Inflate(Resource.Layout.DeckCardTab, container, false);

            mCardTabListView = view.FindViewById<ListView>(Resource.Id.cardTabListView);

            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mQuestions);

            mCardTabListView.Adapter = ListAdapter;

            mCardTabListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                string selectedFromList = (string)mCardTabListView.GetItemAtPosition(e.Position);
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                DeckCardDialogFragment aDifferentDetailsFrag = new DeckCardDialogFragment(mQuestions, mAnswers, e.Position);
                aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");
            };

            mPlayButton = view.FindViewById<ImageButton>(Resource.Id.cardTabPlayButton);
            mPlayButton.Click += PlayButton_Click;

            mAddButton = view.FindViewById<ImageButton>(Resource.Id.cardTabAddButton);
            mAddButton.Click += AddButton_Click;

            mHomeButton = view.FindViewById<ImageButton>(Resource.Id.cardTabHomeButton);
            mHomeButton.Click += HomeButton_Click;

            return view;
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(HomeActivity));
            intent.PutExtra("UserName", mUsername);
            StartActivity(intent);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            mQuestions.Add("New question");
            mAnswers.Add("New question answer");

            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, mQuestions);

            mCardTabListView.Adapter = ListAdapter;

            // TODO: Add code for adding it into actual set 
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            deckPlayFragment fragment = new deckPlayFragment(mQuestions, mAnswers);
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragment.Show(fragmentTx, "dialog_fragment");
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