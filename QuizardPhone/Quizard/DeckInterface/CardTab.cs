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
using Android.Views.InputMethods;

namespace Quizard.DeckInterface
{

    public class DeckCardTabFragment : Fragment
    {
        List<string> mQuestions, mAnswers, mShuffledQuestions, mShuffledAnswers; // Lists to hold set questions and answers
        ListView mCardTabListView; // List of all card questions
        ImageButton mPlayButton, mAddButton, mHomeButton; // buttons for toolbar
        Button mAddCardNextButton, mDoneButton; // Buttons for adding cards
        EditText mAddCardQuestionText, mAddCardAnswerText; // EditText fields for single cards
        string mUsername, mSetName; // Database information
        Context mContext; // Current activity
        ArrayAdapter<string> ListAdapter; // ListAdapter for changing ListView values
        View mView; // this fragments view

        // Initialize all variables. 
        public DeckCardTabFragment(List<string> _questions, List<string> _answers, string[] UserSetName, Context _Context)
        {

            mQuestions = new List<string>();
            mAnswers = new List<string>();
            mUsername = UserSetName[0];
            mSetName = UserSetName[1];
            mQuestions = _questions;
            mAnswers = _answers;
            mContext = _Context;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        // Setting all widgets to corresponding variables.
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Starting the fragment
            FragmentManager.PopBackStack();
            base.OnCreateView(inflater, container, savedInstanceState);
            mView = inflater.Inflate(Resource.Layout.DeckCardTab, container, false);


            // Assign variables to the EditText widgets for the Add Card feature
            mAddCardQuestionText = mView.FindViewById<EditText>(Resource.Id.deckAddCardQuestionEditTextID);
            mAddCardAnswerText = mView.FindViewById<EditText>(Resource.Id.deckAddCardAnswerEditTextID);

            // Assign variables to the buttons used on the AddCard feature
            mAddCardNextButton = mView.FindViewById<Button>(Resource.Id.deckAddToDeckButtonID);
            mAddCardNextButton.Click += AddCardNextButton_Click;
            mDoneButton = mView.FindViewById<Button>(Resource.Id.deckDoneAddCardButtonID);
            mDoneButton.Click += DoneButton_Click;

            // Set up all buttons on toolbar ( Play, Add, Home)
            mPlayButton = mView.FindViewById<ImageButton>(Resource.Id.cardTabPlayButton);
            mPlayButton.Click += PlayButton_Click;
            mAddButton = mView.FindViewById<ImageButton>(Resource.Id.cardTabAddButton);
            mAddButton.Click += AddButton_Click;
            mHomeButton = mView.FindViewById<ImageButton>(Resource.Id.cardTabHomeButton);
            mHomeButton.Click += HomeButton_Click;

            // Get cards from Database
            RetrieveCards(mUsername, mSetName);

            // Set ListView
            mCardTabListView = mView.FindViewById<ListView>(Resource.Id.cardTabListView);
            ListAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.DeckListView, mQuestions);
            mCardTabListView.Adapter = ListAdapter;

            // When an item on the list is clicked open up a DialogFragment with the individual card information.
            mCardTabListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                string selectedFromList = (string)mCardTabListView.GetItemAtPosition(e.Position);
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                DeckCardDialogFragment dialogFragment = new DeckCardDialogFragment(mQuestions, mAnswers, e.Position, this);
                dialogFragment.Show(fragmentTx, "Individual card dialog fragment");
            };

            // Set all AddCard widgets to Gone, so that they aren't visible til Add button is clicked. 
            mAddCardQuestionText.Visibility = ViewStates.Gone;
            mAddCardAnswerText.Visibility = ViewStates.Gone;
            mDoneButton.Visibility = ViewStates.Gone;
            mAddCardNextButton.Visibility = ViewStates.Gone;


            return mView;
        }
        // shuffles question and answer list to generate a random list for play 
        void PlayListShuffle()
        {
            List<string> bufferQuestions = new List<string>(mQuestions);
            List<string> bufferAnswers = new List<string>(mAnswers);

            List<string> tQuestions = new List<string>();
            List<string> tAnswers = new List<string>();
           



            Random rng = new Random();
            int count = mQuestions.Count;
            for (int i = 0; i < count; i++)
            {
                int Range = rng.Next(1, 5000 + i);
                int index = Range % bufferQuestions.Count;

                // add value at index to tQuiz, and remove it from the bufferlist
                tQuestions.Add(bufferQuestions[index]);
                tAnswers.Add(bufferAnswers[index]);
                bufferQuestions.Remove(bufferQuestions[index]);
                bufferAnswers.Remove(bufferAnswers[index]);
            }
            mShuffledQuestions = tQuestions;
            mShuffledAnswers = tAnswers;
        }
        // Function is called when Done button is clicked while adding cards. Takes in values from EditText widgets,
        // sets it to temp string variables (tempSubject, tempAnswer), and then adds the cards to list. 
        private void DoneButton_Click(object sender, EventArgs e)
        {

            string tempSubject, tempAnswer; // temp variables to hold subject and answer
            tempSubject = mAddCardQuestionText.Text; // gets info from question EditText widget
            tempAnswer = mAddCardAnswerText.Text; // gets info from answer EditText widget

            InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(Activity.CurrentFocus.WindowToken, HideSoftInputFlags.ImplicitOnly);

            // adds subject and answer to list and database
            if (tempSubject.Length > 0 && tempAnswer.Length > 0)
            {
                mQuestions.Add(tempSubject);
                mAnswers.Add(tempAnswer);
                AddCard_db(mUsername, mSetName, tempSubject, tempAnswer);
            }

            // Reset EdiText wdigets for new values
            mAddCardQuestionText.Text = "";
            mAddCardAnswerText.Text = "";


            // turn off all associated widgets
            mAddCardQuestionText.Visibility = ViewStates.Gone;
            mAddCardAnswerText.Visibility = ViewStates.Gone;
            mDoneButton.Visibility = ViewStates.Gone;
            mAddCardNextButton.Visibility = ViewStates.Gone;

            // Set new ListView
            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.DeckListView, mQuestions);
            mCardTabListView.Adapter = ListAdapter;
        }
        // takes in card position, and then removes that card from database
        public void Remove(int pos)
        {
            
            DeleteCard(mUsername, mSetName, mQuestions[pos], mAnswers[pos]);
            RetrieveCards(mUsername, mSetName);

            ListAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.DeckListView, mQuestions);
            mCardTabListView.Adapter = ListAdapter;

        }
        // Changes cards with updated strings question/answer, and sets the list to carry new cards
        public void AddCardToList(string question, string answer, int pos)
        {
            DataBase.Cards CardBuffer = new DataBase.Cards(mUsername, mSetName, mQuestions[pos], mAnswers[pos], "", "");
            UpdateCard(CardBuffer, question, answer);
            RetrieveCards(mUsername, mSetName);

            ListAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.DeckListView, mQuestions);
            mCardTabListView.Adapter = ListAdapter;
        }
        // Function is called when Add button is clicked while adding cards. Takes in values from EditText widgets,
        // sets it to temp string variables (tempSubject, tempAnswer), and then adds the cards to list and database.
        private void AddCardNextButton_Click(object sender, EventArgs e)
        {
            // get values from user
            string tempSubject, tempAnswer;
            tempSubject = mAddCardQuestionText.Text;
            tempAnswer = mAddCardAnswerText.Text;

            // Reset EdiText wdigets for new values
            mAddCardQuestionText.Text = "";
            mAddCardAnswerText.Text = "";

            // add to actual deck

            if (tempSubject.Length > 0 && tempAnswer.Length > 0)
            {
                AddCard_db(mUsername, mSetName, tempSubject, tempAnswer);
            }
            // Set new ListView
            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.DeckListView, mQuestions);
            mCardTabListView.Adapter = ListAdapter;

            // return to question text box
            mAddCardQuestionText.RequestFocus();
        }
        // Opens new intent, and sends user to homepage
        private void HomeButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this.Activity, typeof(HomeActivity));
            intent.PutExtra("UserName", mUsername);
            StartActivity(intent);
        }
        // click event for add button, changes associated widget visibility states to be usd
        private void AddButton_Click(object sender, EventArgs e)
        {

            mAddCardQuestionText.Visibility = ViewStates.Visible;
            mAddCardAnswerText.Visibility = ViewStates.Visible;
            mDoneButton.Visibility = ViewStates.Visible;
            mAddCardNextButton.Visibility = ViewStates.Visible;

        }
        // Opens a new Dialog Fragment for cycling through deck
        private void PlayButton_Click(object sender, EventArgs e)
        {

            PlayListShuffle();
            deckPlayFragment fragment = new deckPlayFragment(mShuffledQuestions, mShuffledAnswers);
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragment.Show(fragmentTx, "Play through deck fragment");
        }

        // database function, adds cards to list
        private bool AddCard_db(string _Username, string _SetName, string _Question, string _Answer)
        {
            try
            {
                
                if (_Question.Length > 0 || _Answer.Length > 0)
                {
                    DataBase.Cards CardBuffer = new DataBase.Cards(_Username, _SetName, _Question, _Answer, "0", "0");
                    DataBase.DBAdapter db = new DataBase.DBAdapter(mContext);
                    db.openDB();
                    ICursor CardCheck = db.GetSpecificCard(_Username, _SetName, _Question, _Answer);
                        if (CardCheck.Count == 0)
                    {
                        if (db.SetCard(CardBuffer))
                        {
                            RetrieveCards(_Username, _SetName);

                            db.CloseDB();
                            return true;
                        }
                        else
                            throw new System.ArgumentException("Failed to save new Card", "CardSave");
                    }
                        else
                        throw new System.ArgumentException("Card already exists", "CardSave");
                }
                else
                    throw new System.ArgumentException("Answer or Question is Blank", "CardSave");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(mContext, "Failed to add card", ToastLength.Short).Show();
                return false;
            }
        }
        // database function for getting cards frm list
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

                // mCardTabListView.Adapter = ListAdapter;

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(mContext, "Failed to retrieve Cards", ToastLength.Short).Show();
            }

        }
        // database function. updates cards
        private void UpdateCard(DataBase.Cards _Card, string _NewQuestion, string _NewAnswer)
        {
            DataBase.DBAdapter db = new DataBase.DBAdapter(mContext);
            db.openDB();
            if (!db.UpdateCard(_Card, _NewQuestion, _NewAnswer))
                Toast.MakeText(mContext, "Failed to Update Cards", ToastLength.Short).Show();
            db.CloseDB();
        }
        // database function for removing card
        private void DeleteCard(string _Username, string _SetName, string _Question, string _Answer)
        {
            DataBase.DBAdapter db = new DataBase.DBAdapter(mContext);
            db.openDB();
            if (!db.DeleteCard(_Username, _SetName, _Question, _Answer))
                Toast.MakeText(mContext, "Failed to Delete Cards", ToastLength.Short).Show();
            db.CloseDB();
        }
    }
    public class DeckCardDialogFragment : DialogFragment
    {

        enum cardState { QUESTION_STATE, ANSWER_STATE, ALL_STATES } // enum for card states
        enum mode { DEFAULT_MODE, PLAY_MODE, ALL_MODES }
        List<string> mQuestions, mAnswers; // lists to hold questions and answers for each card
        int mPosition; // current position in list of cards
        EditText mCardText; // widget used to display/edit text
        TextView mCardTextView, mInfoTextView;
        cardState mCurrState = cardState.QUESTION_STATE; // current state of card on display
        DeckCardTabFragment mDeck;
        Button mFlipButton, mEditButton, mDeleteButton;
        mode mCurrMode;
        //private GestureDetector _gestureDetector;

        //string mQuestionHolder, mAnswerHolder;
        public DeckCardDialogFragment(List<string> _questions, List<string> _answers, int pos, DeckCardTabFragment deck)
        {
            mQuestions = new List<string>();
            mQuestions = _questions;
            mPosition = pos;
            mAnswers = new List<string>();
            mAnswers = _answers;
            mDeck = deck;
        }
        // takes off title bar on creation
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Dialog d = base.OnCreateDialog(savedInstanceState);
            d.Window.RequestFeature(WindowFeatures.NoTitle);
            return d;
        }
        // creates view and sets up all widgets
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DeckCardDialogBox, container, false);

            // set up EditText widget for card 
            mCardText = view.FindViewById<EditText>(Resource.Id.cardDialogEditTextID);
            mCardText.Visibility = ViewStates.Gone;
            mCardText.Text = mQuestions[mPosition];
            mCardText.AfterTextChanged += CardText_AfterTextChanged;

            mInfoTextView = view.FindViewById<TextView>(Resource.Id.QuestionAnswerTextViewID);
            mInfoTextView.Text = "Question:";
            

            mEditButton = view.FindViewById<Button>(Resource.Id.CardEditButtonID);
            mEditButton.Click += EditButton_Click;

            mDeleteButton = view.FindViewById<Button>(Resource.Id.CardDeleteButtonID);
            mDeleteButton.Click += DeleteButton_Click;

            mFlipButton = view.FindViewById<Button>(Resource.Id.cardFlipButtonID);
            mFlipButton.Click += FlipButton_Click;
            mCardTextView = view.FindViewById<TextView>(Resource.Id.cardDialogTextViewID);
            mCardTextView.Text = mQuestions[mPosition];
            return view;
        }
        // handles click event for delete
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);
            alert.SetTitle("Delete?");

            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                // remove card from mDeck

                mDeck.Remove(mPosition);
                Dismiss();
                return;
            });
            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                return;
            });
            alert.Show();
        }
        // handles click event for edit button
        private void EditButton_Click(object sender, EventArgs e)
        {
            if (mCardTextView.Visibility == ViewStates.Gone)
            {
                mCardTextView.Visibility = ViewStates.Visible;
                mCardText.Visibility = ViewStates.Gone;
                mFlipButton.Visibility = ViewStates.Visible;
                AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);
                mEditButton.Text = "EDIT";

                if (mCurrState == cardState.QUESTION_STATE)
                    mCardTextView.Text = mQuestions[mPosition];
                else
                    mCardTextView.Text = mAnswers[mPosition];

                alert.SetTitle("Card edited");

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
                mEditButton.Text = "DONE";

                mInfoTextView.Text = "Currently editing";
                mCardText.Visibility = ViewStates.Visible;
                mCardTextView.Visibility = ViewStates.Gone;

                mFlipButton.Visibility = ViewStates.Gone;

                AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);

                alert.SetTitle("You can now edit");

                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    return;
                });

                this.Activity.RunOnUiThread(() =>
                {
                    alert.Show();
                });
            }

            if (mCurrState == cardState.QUESTION_STATE)
                mCardText.Text = mQuestions[mPosition];
            else
                mCardText.Text = mAnswers[mPosition];
        }

        // when flip button is tapped, switch state and text. 
        // QUESTION_STATE displays question. ANSWER_STATE display answers
        private void FlipButton_Click(object sender, EventArgs e)
        {

            if (mCurrState == cardState.QUESTION_STATE)
            {
                mCurrState = cardState.ANSWER_STATE;
                mCardText.Text = mAnswers[mPosition];
                mCardTextView.Text = mAnswers[mPosition];
                mInfoTextView.Text = "Answer";
            }
            else
            {
                mCurrState = cardState.QUESTION_STATE;
                mCardText.Text = mQuestions[mPosition];
                mCardTextView.Text = mQuestions[mPosition];
                mInfoTextView.Text = "Question";
            }
        }

        // deals with card as its being edited
        private void CardText_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            if (mCurrState == cardState.QUESTION_STATE)
                mDeck.AddCardToList(mCardText.Text, mAnswers[mPosition], mPosition);
            else
                mDeck.AddCardToList(mQuestions[mPosition], mCardText.Text, mPosition);
        }

    }
    public class deckPlayFragment : DialogFragment
    {
        List<string> mQuestions, mAnswers; // list of questions and answers
        int mPosition; // position in List
        Button mNextButton, mAnswerButton; // buttons to navigate dialog interface 
        TextView mTxtView, mInfoTxtView; // text view to display question/answer 

        // initialize all variables
        public deckPlayFragment(List<string> _questions, List<string> _answers)
        {
            mQuestions = new List<string>();
            mAnswers = new List<string>();

            mQuestions = _questions;
            mAnswers = _answers;
            mPosition = 0;
        }
        // removes title bar from dialogfragment on creation
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Dialog d = base.OnCreateDialog(savedInstanceState);
            d.Window.RequestFeature(WindowFeatures.NoTitle);
            return d;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        // creates view and sets up all widgets
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.deckPlayLayout, container, false);

            // set up buttons for navigating interface
            mNextButton = view.FindViewById<Button>(Resource.Id.nextCardButton);
            mAnswerButton = view.FindViewById<Button>(Resource.Id.viewAnswerButton);

            // set up textview for questions/answers
            mTxtView = view.FindViewById<TextView>(Resource.Id.deckTextView);

            // set text view to first question (mPosition = 0)
            mTxtView.Text = mQuestions[mPosition];
            // turn off next button while not necessary
            mNextButton.Enabled = false;
            // click events for next and answer button
            mNextButton.Click += NextButton_Click;
            mAnswerButton.Click += AnswerButton_Click;

            mInfoTxtView = view.FindViewById<TextView>(Resource.Id.PlayInfoTextViewID);
            mInfoTxtView.Text = "Question:";
            return view;
        }
        // click event for answer button. turns next button on, and view answer button off. 
        // also changes text view to the current answer
        private void AnswerButton_Click(object sender, EventArgs e)
        {
            if (mTxtView.Text == mQuestions[mPosition])
            {
                mInfoTxtView.Text = "Answer:";
                mTxtView.Text = mAnswers[mPosition];
                mNextButton.Enabled = true; 
            }
            else
            {
                mInfoTxtView.Text = "Question:";
                mTxtView.Text = mQuestions[mPosition];
                mNextButton.Enabled = false;
            }

        }
        // click event for next button while viewing answer. increments position. if at end of list, dismiss fragment.
        // if not at end, set the textview to the next question. turn off next button, enable the view answer button. 
        private void NextButton_Click(object sender, EventArgs e)
        {
            // check if next position is valid, if so increment
            if (mPosition + 1 < mQuestions.Count)
                mPosition++;
            // else dismiss
            else
                Dismiss();

            // set TextView text to current question
            mTxtView.Text = mQuestions[mPosition];
            mNextButton.Enabled = false;
        }
    }
   
}