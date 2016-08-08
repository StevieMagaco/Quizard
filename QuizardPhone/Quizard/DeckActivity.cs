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
    [Activity(Label = "DeckActivity", MainLauncher = true /*Keep the MainLauncher = false unless this dialog fragment needs to be tested*/)]
    public class DeckActivity : Activity
    {
        List<string> questions, answers, quizAnswers;

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

            questions = new List<string>();
            questions.Add("question 1");
            questions.Add("question 2");
            questions.Add("question 3");
            questions.Add("question 4");
            questions.Add("question 5");

            answers = new List<string>();
            answers.Add("answer 1");
            answers.Add("answer 2");
            answers.Add("answer 3");
            answers.Add("answer 4");
            answers.Add("answer 5");


            quizAnswers = new List<string>();
            quizAnswers.Add("1) answer");
            quizAnswers.Add("2) answer");
            quizAnswers.Add("3) answer");
            quizAnswers.Add("4) answer");

            // Set our view from the "DeckLayout" layout resource
            SetContentView(Resource.Layout.DeckLayout);

            this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            AddTab("CARDS", Resource.Drawable.cardIconSmall, new DeckCardTabFragment(questions, answers));
            AddTab("QUIZ", Resource.Drawable.quizIcon, new DeckQuizTabFragment(questions, quizAnswers));

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
        List<string> questions, answers;
        int position;
        Button viewAnswerButton, editButton;
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

            editButton = view.FindViewById<Button>(Resource.Id.cardDialogEditButton);
            editButton.Click += EditButton_Click;

            return view;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            DeckCardTabEditFragment aDifferentDetailsFrag = new DeckCardTabEditFragment(questions[position], answers[position]);
            aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");
        }

        private void ViewAnswerButton_Click(object sender, EventArgs e)
        {
            textView.Text = answers[position];
        }
    }

    public class DeckCardTabEditFragment : DialogFragment
    {
        EditText questionText, answerText;

        public DeckCardTabEditFragment(string question, string answer)
        {
            questionText = new EditText(this.Activity);
            answerText = new EditText(this.Activity);
            questionText.FindViewById<EditText>(Resource.Id.questionTextBox);
            answerText.FindViewById<EditText>(Resource.Id.answerTextBox);
            questionText.Text = question;
            answerText.Text = answer;
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
        List<string> questions, answers;
        ListView cardTabListView;
        ImageButton playButton, addButton, homeButton;

        public DeckCardTabFragment(List<string> _questions, List<string> _answers)
        {
            questions = new List<string>();
            answers = new List<string>();

            questions = _questions;
            answers = _answers;
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

            cardTabListView = view.FindViewById<ListView>(Resource.Id.cardTabListView);

            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, questions);

            cardTabListView.Adapter = ListAdapter;

            cardTabListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                string selectedFromList = (string)cardTabListView.GetItemAtPosition(e.Position);
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                DeckCardDialogFragment aDifferentDetailsFrag = new DeckCardDialogFragment(questions, answers, e.Position);
                aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");
            };

            playButton = view.FindViewById<ImageButton>(Resource.Id.cardTabPlayButton);
            playButton.Click += PlayButton_Click;

            addButton = view.FindViewById<ImageButton>(Resource.Id.cardTabAddButton);
            addButton.Click += AddButton_Click;

            homeButton = view.FindViewById<ImageButton>(Resource.Id.cardTabHomeButton);
            homeButton.Click += HomeButton_Click;

            return view;
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(HomeActivity));
            StartActivity(intent);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            questions.Add("New question");
            answers.Add("New question answer");

            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, questions);

            cardTabListView.Adapter = ListAdapter;

            // TODO: Add code for adding it into actual set 
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            deckPlayFragment fragment = new deckPlayFragment(questions, answers);
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragment.Show(fragmentTx, "dialog_fragment");
        }
    }
    public class deckPlayFragment : DialogFragment
    {
        List<string> questions, answers;
        int pos;
        Button nextButton, answerButton;

        TextView txtView;
        public deckPlayFragment(List<string> _questions, List<string> _answers)
        {
            questions = new List<string>();
            answers = new List<string>();

            questions = _questions;
            answers = _answers;
            pos = 0;
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

            nextButton = view.FindViewById<Button>(Resource.Id.nextCardButton);
            answerButton = view.FindViewById<Button>(Resource.Id.viewAnswerButton);

            txtView = view.FindViewById<TextView>(Resource.Id.deckTextView);

            txtView.Text = questions[pos];
            nextButton.Enabled = false;
            nextButton.Click += NextButton_Click;
            answerButton.Click += AnswerButton_Click;
            return view;
        }

        private void AnswerButton_Click(object sender, EventArgs e)
        {
            nextButton.Enabled = true;
            answerButton.Enabled = false;
            txtView.Text = answers[pos];
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (pos + 1 < questions.Count)
                pos++;
            else
                Dismiss();
            txtView.Text = questions[pos];

            nextButton.Enabled = false;
            answerButton.Enabled = true;
        }
    }
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
    public class deckPlayDialogFragment : DialogFragment
    {
        List<string> questions, answers;
        int position;
        Button answerButton;
        View view;
        ViewGroup mContainer;
        LayoutInflater mInflater;

        public deckPlayDialogFragment(List<string> _questions, List<string> _answers)
        {
            questions = new List<string>();
            answers = new List<string>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mInflater = inflater;
            mContainer = container;


            view = inflater.Inflate(Resource.Layout.PlaySetLayout_Front, container, false);


            // Set up a handler to dismiss this DialogFragment when this button is clicked.
            answerButton = view.FindViewById<Button>(Resource.Id.PlaySetFront_ViewAnswerButton);
            answerButton.Click += AnswerButton_Click;
            return view;
        }

        private void AnswerButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            deckPlayDialogFragment_Answer aDifferentDetailsFrag = new deckPlayDialogFragment_Answer(answers[position]);
            aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");
            
        }
    }
public class deckPlayDialogFragment_Answer : DialogFragment
{
        TextView answerTextView;
        string answer;
        public deckPlayDialogFragment_Answer(string _answer)
        {
            answer = _answer;
            
        }
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
            var view = inflater.Inflate(Resource.Layout.PlaySetLayout_Front, container, false);

            answerTextView = view.FindViewById<TextView>(Resource.Id.answerTextView);

            return view;
    }
}
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
                DeckQuizDialogFragment quizDialogFrag = new DeckQuizDialogFragment(answerList, e.Position, nextButtonDialogResult);
                quizDialogFrag.Show(fragmentTx, "dialog_fragment");
            };

            return view;
        }
    }
}