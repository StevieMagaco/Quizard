using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.Gms.Common;
using Android.Support.V4.Content;

using Newtonsoft.Json;

namespace Quizard
{
    // TODO: Implement updated menu system for the HomeLayout using this Dialog Fragment
    public class HomeMenuDialogFragment : DialogFragment
    {
        private Button mMenuEnter, mMenuEdit, mMenuDelete;
        HomeActivity mActivityReference; // To reference public data in HomeActivity

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Dialog d = base.OnCreateDialog(savedInstanceState);
            d.Window.RequestFeature(WindowFeatures.NoTitle);
            return d;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.HomeMenuDialogLayout, container, false);

            mMenuEnter = view.FindViewById<Button>(Resource.Id.EnterIntoFlashSetMenuButtonID);
            mMenuEdit = view.FindViewById<Button>(Resource.Id.EditFlashSetMenuButtonID);
            mMenuDelete = view.FindViewById<Button>(Resource.Id.DeleteFlashSetMenuButtonID);

            mMenuEnter.Click += delegate (object sender, EventArgs e)
            {
                // Enter the flash set
            };

            mMenuEdit.Click += delegate (object sender, EventArgs e)
            {
                // Edit selected flash set
            };

            mMenuDelete.Click += delegate (object sender, EventArgs e)
            {
                // Delete selected flash set
            };

            return view;
        }
    }

    [Activity(MainLauncher = false /* MainLauncher does NOT need to be changed unless another layout or diaglog fragment needs to be tested*/, Theme = "@style/CustomActionToolbarTheme")]
    public class HomeActivity : Activity, IDataApiDataListener, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private ArrayAdapter mAdapter;
        private ListView mFlashSetList;
        private AutoCompleteTextView mFlashSetSubject;
        private ImageButton mCreateAFlashSet, mCancel, mEditAFlashSetsSubject, mDeleteAFlashSet, mSettings;
        private TextView mCreateAFlashSetLabel, mCancelLabel, mEditAFlashSetsSubjectLabel, mDeleteAFlashSetLabel, mSettingsLabel;
        private SearchView mSearchThroughFlashSets;
        private Button mAddToFlashSetList, mEnterIntoSelectedFlashSet;
        private int mSelectedFlashSet = -1;
        private const string mEmptySubject = "";

        #region Sending Data Variables
        private GoogleApiClient mGoogleClient;
        private const string mCardSetPath = "/CardSets";
        #endregion

        #region Database Variables
        private DataBase.UserInfo mUserInformation;
        private JavaList<string> mSetNameList = new JavaList<string>();
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.NoTitle);

            base.OnCreate(savedInstanceState);

            // Set our view from the "HomeLayout" layout resource
            SetContentView(Resource.Layout.HomeLayout);

            string[] autoCompleteOptions = new string[]
            {
                #region Subjects
                "Language Arts",
                "Mathematics",
                "Science",
                "Health",
                "Physical Education",
                "Art",
                "Music",
                "Speech",
                "Reading",
                "Debate",
                "English",
                "Basic Math",
                "Pre-Algebra",
                "Algebra",
                "Geometry",
                "Life Science",
                "Earth Science",
                "Physical Science",
                "Social Studies",
                "Geography",
                "U.S. History",
                "Literature",
                #endregion
            };

            mGoogleClient = new GoogleApiClient.Builder(this, this, this).AddApi(WearableClass.API).Build();

            #region Class Variable FindViewById<> Assignments
            mFlashSetList = FindViewById<ListView>(Resource.Id.flashSetListViewID);
            mFlashSetSubject = FindViewById<AutoCompleteTextView>(Resource.Id.flashSetSubjectAutoCompleteTextViewID);
            mCreateAFlashSet = FindViewById<ImageButton>(Resource.Id.createAFlashSetImageButtonID);
            mCancel = FindViewById<ImageButton>(Resource.Id.cancelImageButtonID);
            mEditAFlashSetsSubject = FindViewById<ImageButton>(Resource.Id.editFlashSetSubjectImageButtonID);
            mDeleteAFlashSet = FindViewById<ImageButton>(Resource.Id.deleteFlashSetImageButtonID);
            mSettings = FindViewById<ImageButton>(Resource.Id.settingsImageButtonID);
            mCreateAFlashSetLabel = FindViewById<TextView>(Resource.Id.createAFlashSetTextViewID);
            mCancelLabel = FindViewById<TextView>(Resource.Id.cancelTextViewID);
            mEditAFlashSetsSubjectLabel = FindViewById<TextView>(Resource.Id.editFlashSetSubjectTextViewID);
            mDeleteAFlashSetLabel = FindViewById<TextView>(Resource.Id.deleteFlashSetTextViewID);
            mSettingsLabel = FindViewById<TextView>(Resource.Id.settingsTextViewID);
            mSearchThroughFlashSets = FindViewById<SearchView>(Resource.Id.searchFlashSetsSearchViewID);
            mAddToFlashSetList = FindViewById<Button>(Resource.Id.addToFlashSetListButtonID);
            mEnterIntoSelectedFlashSet = FindViewById<Button>(Resource.Id.enterIntoSelectedFlashSetButtonID);
            #endregion

            mUserInformation = new DataBase.UserInfo();
            mAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, mSetNameList);

            ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, autoCompleteOptions);
            mFlashSetSubject.Adapter = autoCompleteAdapter;

            string Username_Buffer = Intent.GetStringExtra("UserName") ?? "Data not available";

            if (Username_Buffer != "Data not available")
            {
                DataBase.User NewUser = new DataBase.User();
                NewUser.SetUsername(Username_Buffer);
                NewUser.SetPassword("");
                mUserInformation.SetUser(NewUser);
            }
            else
            {
                Toast.MakeText(this, "Unable to retrieve username", ToastLength.Short).Show();
            }

            RetreiveSet(mFlashSetList, Username_Buffer);
            RetrieveCards();
            string json = JsonConvert.SerializeObject(mUserInformation);
            SendData(json);

            mSearchThroughFlashSets.QueryTextChange += delegate (object sender, SearchView.QueryTextChangeEventArgs e)
            {
                mAdapter.Filter.InvokeFilter(e.NewText);
            };

            // If the user taps the add button to add a new flash set into the list view...
            mAddToFlashSetList.Click += delegate (object sender, EventArgs e)
            {
                mSearchThroughFlashSets.Visibility = ViewStates.Visible;

                if (AddSet(mUserInformation.GetUser().GetUsername(), mFlashSetSubject.Text))
                {
                    mFlashSetSubject.Text = mEmptySubject;
                    RetreiveSet(mFlashSetList, Username_Buffer);
                    string newJson = JsonConvert.SerializeObject(mUserInformation);
                    SendData(newJson);
                }
            };

            // If the user taps an existing flash set item in the list view...
            mFlashSetList.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                mSelectedFlashSet = e.Position;
                mFlashSetSubject.Hint = "Edit \"" + ReturnRealSetName(mSetNameList[mSelectedFlashSet].ToString()) + "\"?";

                mFlashSetSubject.Visibility = ViewStates.Visible;
                mEnterIntoSelectedFlashSet.Visibility = ViewStates.Visible;

                #region Toolbar Image Button Visibility Assignments
                mCreateAFlashSet.Visibility = ViewStates.Invisible;
                mCreateAFlashSetLabel.Visibility = ViewStates.Invisible;

                mCancel.Visibility = ViewStates.Visible;
                mCancelLabel.Visibility = ViewStates.Visible;

                mEditAFlashSetsSubject.Visibility = ViewStates.Visible;
                mEditAFlashSetsSubjectLabel.Visibility = ViewStates.Visible;

                mDeleteAFlashSet.Visibility = ViewStates.Visible;
                mDeleteAFlashSetLabel.Visibility = ViewStates.Visible;

                mSettings.Visibility = ViewStates.Invisible;
                mSettingsLabel.Visibility = ViewStates.Invisible;
                #endregion

                #region Fragment Code
                //HomeMenuDialogFragment fragment = new HomeMenuDialogFragment();
                //FragmentTransaction transaction = FragmentManager.BeginTransaction();

                //// When a "mFlasSetList" ListView item is clicked, bring up the assigned dialog fragment
                //fragment.Show(transaction, "dialog fragment");
                #endregion
            };

            // If the user decides to go into a flash set after tapping the enter flash set button...
            mEnterIntoSelectedFlashSet.Click += delegate (object sender, EventArgs e)
            {
                Intent intent = new Intent(this, typeof(DeckActivity));
                string[] UserSetName = { mUserInformation.GetUser().GetUsername(), ReturnRealSetName(mSetNameList[mSelectedFlashSet].ToString()) };
                intent.PutExtra("Username/SetName", UserSetName);
                this.StartActivity(intent);
            };

            // If the user taps the create image button on the toolbar...
            mCreateAFlashSet.Click += delegate (object sender, EventArgs e)
            {
                mFlashSetSubject.Visibility = ViewStates.Visible;
                mAddToFlashSetList.Visibility = ViewStates.Visible;

                #region Toolbar Image Button Visibility Assignments
                mCreateAFlashSet.Visibility = ViewStates.Invisible;
                mCreateAFlashSetLabel.Visibility = ViewStates.Invisible;

                mCancel.Visibility = ViewStates.Visible;
                mCancelLabel.Visibility = ViewStates.Visible;

                mSettings.Visibility = ViewStates.Invisible;
                mSettingsLabel.Visibility = ViewStates.Invisible;
                #endregion

                mFlashSetSubject.Hint = "Subject";
            };

            // If the user needs to cancel out of a flash set creation, update, or delete command...
            mCancel.Click += delegate (object sender, EventArgs e)
            {
                mFlashSetSubject.Visibility = ViewStates.Invisible;
                mAddToFlashSetList.Visibility = ViewStates.Invisible;
                mEnterIntoSelectedFlashSet.Visibility = ViewStates.Invisible;

                #region Toolbar Image Button Visibility Assignments
                mCreateAFlashSet.Visibility = ViewStates.Visible;
                mCreateAFlashSetLabel.Visibility = ViewStates.Visible;

                mCancel.Visibility = ViewStates.Invisible;
                mCancelLabel.Visibility = ViewStates.Invisible;

                mEditAFlashSetsSubject.Visibility = ViewStates.Invisible;
                mEditAFlashSetsSubjectLabel.Visibility = ViewStates.Invisible;

                mDeleteAFlashSet.Visibility = ViewStates.Invisible;
                mDeleteAFlashSetLabel.Visibility = ViewStates.Invisible;

                mSettings.Visibility = ViewStates.Visible;
                mSettingsLabel.Visibility = ViewStates.Visible;
                #endregion

                mFlashSetSubject.Text = mEmptySubject;
            };

            // If the user taps the update image button on the toolbar...
            mEditAFlashSetsSubject.Click += delegate (object sender, EventArgs e)
            {
                if (UpdateSet(mUserInformation.GetUser().GetUsername(), mSetNameList[mSelectedFlashSet], mFlashSetSubject.Text))
                {
                    mFlashSetSubject.Visibility = ViewStates.Invisible;
                    mAddToFlashSetList.Visibility = ViewStates.Invisible;

                    #region Toolbar Image Button Visibility Assignments
                    mCancel.Visibility = ViewStates.Invisible;
                    mCancelLabel.Visibility = ViewStates.Invisible;

                    mEditAFlashSetsSubject.Visibility = ViewStates.Invisible;
                    mEditAFlashSetsSubjectLabel.Visibility = ViewStates.Invisible;

                    mDeleteAFlashSet.Visibility = ViewStates.Invisible;
                    mDeleteAFlashSetLabel.Visibility = ViewStates.Invisible;
                    #endregion
                }
            };

            // If the user taps the delete image button on the toolbar...
            mDeleteAFlashSet.Click += delegate (object sender, EventArgs e)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);

                alert.SetTitle("Delete the \"" + ReturnRealSetName(mSetNameList[mSelectedFlashSet].ToString()) + "\" flash set?");

                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    if (DeleteSet(mUserInformation.GetUser().GetUsername(), mSetNameList[mSelectedFlashSet].ToString()))
                    {
                        mFlashSetSubject.Text = mEmptySubject;

                        mFlashSetSubject.Visibility = ViewStates.Invisible;
                        mAddToFlashSetList.Visibility = ViewStates.Invisible;
                        mEnterIntoSelectedFlashSet.Visibility = ViewStates.Invisible;

                        if (mFlashSetList.Count == 0)
                            mSearchThroughFlashSets.Visibility = ViewStates.Invisible;

                        #region Toolbar Image Button Visibility Assignments
                        mCreateAFlashSet.Visibility = ViewStates.Visible;
                        mCreateAFlashSetLabel.Visibility = ViewStates.Visible;

                        mCancel.Visibility = ViewStates.Invisible;
                        mCancelLabel.Visibility = ViewStates.Invisible;

                        mEditAFlashSetsSubject.Visibility = ViewStates.Invisible;
                        mEditAFlashSetsSubjectLabel.Visibility = ViewStates.Invisible;

                        mDeleteAFlashSet.Visibility = ViewStates.Invisible;
                        mDeleteAFlashSetLabel.Visibility = ViewStates.Invisible;

                        mSettings.Visibility = ViewStates.Visible;
                        mSettingsLabel.Visibility = ViewStates.Visible;
                        #endregion
                    }
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    return;
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
            };

            mSettings.Click += delegate (object sender, EventArgs e)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);

                alert.SetTitle("Logout of Quizard?");

                alert.SetPositiveButton("Continue", (senderAlert, args) =>
                {
                    Intent intent = new Intent(this, typeof(LoginActivity));
                    this.StartActivity(intent);
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    return;
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
            };
        }

        #region Data Sending Functions
        public void OnDataChanged(DataEventBuffer dataEvents)
        {
            foreach (var dataEvent in dataEvents)
            {
                var dataItem = dataEvent.DataItem;
                if (dataItem.Uri.Path == mCardSetPath)
                {
                    var dataMap = DataMapItem.FromDataItem(dataItem).DataMap;

                }
            }
        }

        public void SendData(string _jsonData)
        {
            try
            {
                var putDataMapRequest = PutDataMapRequest.Create(mCardSetPath);
                putDataMapRequest.DataMap.PutString("Message", _jsonData);
                putDataMapRequest.DataMap.PutLong("UpdatedAt", DateTime.UtcNow.Ticks);
                var putDataRequest = putDataMapRequest.AsPutDataRequest();
                putDataRequest.SetUrgent();

                WearableClass.DataApi.PutDataItem(mGoogleClient, putDataRequest);
            }
            finally
            {
                // googleClient.Disconnect();
            }

        }

        protected override void OnStart()
        {
            base.OnStart();
            mGoogleClient.Connect();
        }

        public void OnConnected(Bundle connectionHint)
        {
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
        }
        #endregion

        #region DataBase Functions
        // Implementation to retrive a users flash sets
        private void RetreiveSet(ListView _FlashSet, string _Username)
        {
            try
            {
                DataBase.DBAdapter db = new DataBase.DBAdapter(this);
                db.openDB();

                ICursor SetInfo = db.GetSets(_Username);
                mSetNameList.Clear();
                DataBase.Sets BufferSet = new DataBase.Sets();
                while (SetInfo.MoveToNext())
                {
                    string name = SetInfo.GetString(1);
                    int CardCount = GetCardCount(mUserInformation.GetUser().GetUsername(), name);
                    mSetNameList.Add(name + "\n" + CardCount.ToString());
                    // mSetNameList.Add(name);
                    BufferSet = new DataBase.Sets();
                    BufferSet.SetUsername(_Username);
                    BufferSet.SetSetName(SetInfo.GetString(1));
                    BufferSet.SetNotify(SetInfo.GetString(2));
                    BufferSet.SetCorrect(SetInfo.GetString(3));
                    BufferSet.SetIncorrect(SetInfo.GetString(4));
                    mUserInformation.AddSet(BufferSet);
                }

                if (mSetNameList.Count > 0)
                    mSearchThroughFlashSets.Visibility = ViewStates.Visible;

                db.CloseDB();

                _FlashSet.Adapter = mAdapter;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(this, "Failed to retrieve flash sets from the database", ToastLength.Short).Show();
            }
        }
        private void RetrieveCards()
        {
            for (int loop = 0; loop < mSetNameList.Count; loop++)
            {
                try
                {
                    DataBase.DBAdapter db = new DataBase.DBAdapter(this);
                    db.openDB();

                    ICursor CardsInfo = db.GetCards(mUserInformation.GetUser().GetUsername(), ReturnRealSetName(mSetNameList[loop]));
                    DataBase.Cards BufferCard = new DataBase.Cards();
                    while (CardsInfo.MoveToNext())
                    {
                        BufferCard = new DataBase.Cards();
                        BufferCard.SetUserName(CardsInfo.GetString(0));
                        BufferCard.SetSetName(CardsInfo.GetString(1));
                        BufferCard.SetQuestion(CardsInfo.GetString(2));
                        BufferCard.SetAnswer(CardsInfo.GetString(3));
                        BufferCard.SetNumberBox(CardsInfo.GetString(4));
                        BufferCard.SetPreRun(CardsInfo.GetString(5));
                        mUserInformation.AddCard(BufferCard);
                    }
                    db.CloseDB();

                    // mCardTabListView.Adapter = ListAdapter;

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Toast.MakeText(this, "Failed to retrieve Cards", ToastLength.Short).Show();
                }
            }
        }
        private bool AddSet(string _Username, string _SetName)
        {
            try
            {
                _SetName = ReturnRealSetName(_SetName);
                DataBase.DBFunction DataFunc = new DataBase.DBFunction();
                DataBase.DBAdapter db = new DataBase.DBAdapter(this);
                db.openDB();

                ICursor UserInfo = db.GetSpecificSet(_Username, _SetName);

                if (UserInfo.Count == 0)
                {
                    if (_Username.Length > 0 && _SetName.Length > 0)
                    {
                        if (DataFunc.SaveSet(_Username, _SetName, this))
                        {
                            Toast.MakeText(this, _SetName + " Created", ToastLength.Short).Show();
                            RetreiveSet(mFlashSetList, _Username);
                            return true;
                        }
                        else
                            throw new System.ArgumentException("Failed to save new flash set", "SaveSet");
                    }
                    else
                        throw new System.ArgumentException("UserInfo is Size 0", "Username/Setname");
                }
                else
                    throw new System.ArgumentException("Setname already exists", "Setname");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(this, "Unable to create a new flash set", ToastLength.Short).Show();
                return false;
            }
        }

        private bool DeleteSet(string _Username, string _SetName)
        {
            DataBase.DBAdapter db = new DataBase.DBAdapter(this);
            db.openDB();
            _SetName = ReturnRealSetName(_SetName);
            if (db.DeleteRowSet_tb(_Username, _SetName))
            {
                RetreiveSet(mFlashSetList, _Username);
                Toast.MakeText(this, _SetName + " was deleted", ToastLength.Short).Show();
                db.CloseDB();
                return true;
            }
            else
            {
                Toast.MakeText(this, "Unable to delete " + _SetName, ToastLength.Short).Show();
                db.CloseDB();
                return false;
            }
        }

        private bool UpdateSet(string _Username, string _SetName, string _NewSetName)
        {
            if (_Username.Length > 0 && _NewSetName.Length > 0)
            {
                _SetName = ReturnRealSetName(_SetName);
                DataBase.DBAdapter db = new DataBase.DBAdapter(this);
                db.openDB();

                DataBase.Sets SetBuffer = new DataBase.Sets(_Username, _SetName, "", "", "");

                if (db.UpdateRowSets(SetBuffer, _NewSetName))
                {
                    UpdateCardSetName(_Username, _SetName, _NewSetName);
                    RetreiveSet(mFlashSetList, _Username);
                    Toast.MakeText(this, _SetName + " was updated to " + _NewSetName, ToastLength.Short).Show();
                    db.CloseDB();
                    return true;
                }
                else
                {
                    Toast.MakeText(this, "Unable to update " + _SetName, ToastLength.Short).Show();

                    db.CloseDB();
                    return false;
                }
            }
            else
            {
                Toast.MakeText(this, "Unable to update " + _SetName, ToastLength.Short).Show();
                return false;
            }

        }
        public int GetCardCount(string _Username, string _SetName)
        {
            try
            {
                DataBase.DBAdapter db = new DataBase.DBAdapter(this);
                db.openDB();
                ICursor CardsInfo = db.GetCards(_Username, _SetName);
                int CardCountBuffer = CardsInfo.Count;
                db.CloseDB();
                return CardCountBuffer;
                // mCardTabListView.Adapter = ListAdapter;

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(this, "Failed to retrieve Cards", ToastLength.Short).Show();
                return 0;
            }
        }
        public string ReturnRealSetName(string _Setname)
        {
            string SetNameBuffer = "";
            for (int loop = 0; loop < _Setname.Length; loop++)
            {
                if (_Setname[loop] != '\n')
                {
                    SetNameBuffer += _Setname[loop];
                }
                else
                    break;
            }
            return SetNameBuffer;
        }
        public void UpdateCardSetName(string _Username, string _Setname, string _NewSetname)
        {
            DataBase.DBAdapter db = new DataBase.DBAdapter(this);
            DataBase.Cards CardBuffer = new DataBase.Cards();
            db.openDB();
            ICursor Cards = db.GetCards(_Username, _Setname);
            while (Cards.MoveToNext())
            {
                CardBuffer.SetUserName(_Username);
                CardBuffer.SetSetName(_Setname);
                CardBuffer.SetQuestion(Cards.GetString(2));
                CardBuffer.SetAnswer(Cards.GetString(3));
                CardBuffer.SetNumberBox(Cards.GetString(4));
                CardBuffer.SetPreRun(Cards.GetString(5));
                db.UpdateCardsSetName(CardBuffer, _NewSetname);
            }
            db.CloseDB();
        }
    }
    #endregion
}