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

namespace Quizard
{
    public class FlashSetCommand
    {
        private ArrayList mFlashSets;
        private ArrayAdapter mAdapter;

        public FlashSetCommand(ArrayList flashSets, ArrayAdapter adapter)
        {
            mFlashSets = flashSets;
            mAdapter = adapter;
        }

        public bool CreateAFlashSet(string flashSetSubject)
        {
            try
            {
                mFlashSets.Add(flashSetSubject);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateAFlashSet(string flashSetUpdatedSubject, int flashSetIndex)
        {
            try
            {
                mFlashSets.RemoveAt(flashSetIndex);
                mFlashSets.Insert(flashSetIndex, flashSetUpdatedSubject);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteAFlashSet(int flashSetIndex)
        {
            try
            {
                mFlashSets.RemoveAt(flashSetIndex);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    [Activity(MainLauncher = false /* MainLauncher does NOT need to be changed unless another layout or diaglog fragment needs to be tested*/, Theme = "@style/CustomActionToolbarTheme")]
    public class HomeActivity : Activity
    {
        private ArrayList mFlashSets;
        private ArrayAdapter mAdapter;
        private ListView mFlashSetList;
        private FlashSetCommand mFlashSetSelected;
        private EditText mFlashSetSubject;
        private ImageButton mCreateAFlashSet, mCancel, mUpdateAFlashSet, mDeleteAFlashSet, mSettings;
        private TextView mCreateAFlashSetLabel, mCancelLabel, mUpdateAFlashSetLabel, mDeleteAFlashSetLabel, mSettingsLabel;
        private SearchView mSearchThroughFlashSets;
        private Button mAddToFlashSetList, mEnterIntoSelectedFlashSet;
        private int mSelectedFlashSet = -1;
        private string mEmptySubject = "";

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

            #region Class Variable FindViewById<> Assignments
            mFlashSetList = FindViewById<ListView>(Resource.Id.flashSetListViewID);
            mFlashSetSubject = FindViewById<EditText>(Resource.Id.flashSetSubjectEditTextID);
            mCreateAFlashSetLabel = FindViewById<TextView>(Resource.Id.createAFlashSetTextViewID);
            mCancelLabel = FindViewById<TextView>(Resource.Id.cancelTextViewID);
            mUpdateAFlashSetLabel = FindViewById<TextView>(Resource.Id.updateFlashSetTextViewID);
            mDeleteAFlashSetLabel = FindViewById<TextView>(Resource.Id.deleteFlashSetTextViewID);
            mSettingsLabel = FindViewById<TextView>(Resource.Id.settingsTextViewID);
            mCreateAFlashSet = FindViewById<ImageButton>(Resource.Id.createAFlashSetImageButtonID);
            mCancel = FindViewById<ImageButton>(Resource.Id.cancelImageButtonID);
            mUpdateAFlashSet = FindViewById<ImageButton>(Resource.Id.updateFlashSetImageButtonID);
            mDeleteAFlashSet = FindViewById<ImageButton>(Resource.Id.deleteFlashSetImageButtonID);
            mSettings = FindViewById<ImageButton>(Resource.Id.settingsImageButtonID);
            mSearchThroughFlashSets = FindViewById<SearchView>(Resource.Id.searchFlashSetsSearchViewID);
            mAddToFlashSetList = FindViewById<Button>(Resource.Id.addToFlashSetListButtonID);
            mEnterIntoSelectedFlashSet = FindViewById<Button>(Resource.Id.enterIntoSelectedFlashSetButtonID);
            #endregion

            mUserInformation = new DataBase.UserInfo();
            mAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, mSetNameList);

            string Username_Buffer = Intent.GetStringExtra("UserName") ?? "Data not available";

            if (Username_Buffer != "Data not available")
            {
                DataBase.User NewUser = new DataBase.User();

                NewUser.SetUsername(Username_Buffer);
                NewUser.SetPassword("");
                mUserInformation.SetUser(NewUser);
            }
            else
                Toast.MakeText(this, "Unable to retreve username!", ToastLength.Short).Show();

            RetreiveSet(mFlashSetList, Username_Buffer);
            mFlashSets = new ArrayList();

            //mFlashSetSelected = new FlashSetCommand(mFlashSets, mAdapter);
            // mAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, mFlashSets);
            //mFlashSetList.Adapter = mAdapter;

            mSearchThroughFlashSets.QueryTextChange += delegate (object sender, SearchView.QueryTextChangeEventArgs e)
            {
                mAdapter.Filter.InvokeFilter(e.NewText);
            };

            // If the user taps the add button to add a new flash set into the list view...
            mAddToFlashSetList.Click += delegate (object sender, EventArgs e)
            {
                // TODO: Implement the flash set error checking for subject name

                // AddSet(UserInformation.GetUser().GetUsername(), mFlashSetSubject.Text);

                if (AddSet(mUserInformation.GetUser().GetUsername(), mFlashSetSubject.Text))
                {
                    mFlashSetSubject.Text = mEmptySubject;

                    //mAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, mFlashSets);
                    //AddSet(UserInformation.GetUser().GetUsername(), mFlashSetSubject.Text);
                    //RetreiveSet(mFlashSetList, UserInformation.GetUser().GetUsername());
                    // mFlashSetList.Adapter = mAdapter;
                }
            };

            // If the user taps an existing flash set item in the list view...
            mFlashSetList.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                mSelectedFlashSet = e.Position;
                mFlashSetSubject.Text = mSetNameList[mSelectedFlashSet].ToString();

                mFlashSetSubject.Visibility = ViewStates.Visible;
                mEnterIntoSelectedFlashSet.Visibility = ViewStates.Visible;

                #region Toolbar Image Button Visibility Assignments
                mCancel.Visibility = ViewStates.Visible;
                mCancelLabel.Visibility = ViewStates.Visible;

                mUpdateAFlashSet.Visibility = ViewStates.Visible;
                mUpdateAFlashSetLabel.Visibility = ViewStates.Visible;

                mDeleteAFlashSet.Visibility = ViewStates.Visible;
                mDeleteAFlashSetLabel.Visibility = ViewStates.Visible;
                #endregion
            };

            // If the user decides to go into a flash set after tapping the enter flash set button...
            mEnterIntoSelectedFlashSet.Click += delegate (object sender, EventArgs e)
            {
                Intent intent = new Intent(this, typeof(DeckActivity));
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

                mUpdateAFlashSet.Visibility = ViewStates.Invisible;
                mUpdateAFlashSetLabel.Visibility = ViewStates.Invisible;

                mDeleteAFlashSet.Visibility = ViewStates.Invisible;
                mDeleteAFlashSetLabel.Visibility = ViewStates.Invisible;

                mSettings.Visibility = ViewStates.Visible;
                mSettingsLabel.Visibility = ViewStates.Visible;
                #endregion

                mFlashSetSubject.Text = mEmptySubject;
            };

            // If the user taps the update image button on the toolbar...
            /*mUpdateAFlashSet.Click += delegate (object sender, EventArgs e)
            {
                RetreiveSet(mFlashSetList, mUserInformation.GetUser().GetUsername());
                //if (mFlashSetSelected.UpdateAFlashSet(mFlashSetSubject.Text, mSelectedFlashSet))
                //{
                mFlashSetSubject.Visibility = ViewStates.Invisible;
                mAddToFlashSetList.Visibility = ViewStates.Invisible;

                #region Toolbar Image Button Visibility Assignments
                mCancel.Visibility = ViewStates.Invisible;
                mCancelLabel.Visibility = ViewStates.Invisible;

                mUpdateAFlashSet.Visibility = ViewStates.Invisible;
                mUpdateAFlashSetLabel.Visibility = ViewStates.Invisible;

                mDeleteAFlashSet.Visibility = ViewStates.Invisible;
                mDeleteAFlashSetLabel.Visibility = ViewStates.Invisible;
                #endregion

                //mAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, mFlashSets);
                //mFlashSetList.Adapter = mAdapter;

                Toast.MakeText(this, "Flash Set Updated", ToastLength.Short).Show();
                //}
            };*/

            // If the user taps the delete image button on the toolbar...
            mDeleteAFlashSet.Click += delegate (object sender, EventArgs e)
            {
                if (DeleteSet(mUserInformation.GetUser().GetUsername(), mSetNameList[mSelectedFlashSet].ToString()))
                {
                    mFlashSetSubject.Text = mEmptySubject;

                    mFlashSetSubject.Visibility = ViewStates.Invisible;
                    mAddToFlashSetList.Visibility = ViewStates.Invisible;
                    mEnterIntoSelectedFlashSet.Visibility = ViewStates.Invisible;

                    #region Toolbar Image Button Visibility Assignments
                    mCreateAFlashSet.Visibility = ViewStates.Visible;
                    mCreateAFlashSetLabel.Visibility = ViewStates.Visible;

                    mCancel.Visibility = ViewStates.Invisible;
                    mCancelLabel.Visibility = ViewStates.Invisible;

                    mUpdateAFlashSet.Visibility = ViewStates.Invisible;
                    mUpdateAFlashSetLabel.Visibility = ViewStates.Invisible;

                    mDeleteAFlashSet.Visibility = ViewStates.Invisible;
                    mDeleteAFlashSetLabel.Visibility = ViewStates.Invisible;

                    mSettings.Visibility = ViewStates.Visible;
                    mSettingsLabel.Visibility = ViewStates.Visible;
                    #endregion

                    //mAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, mFlashSets);
                    //mFlashSetList.Adapter = mAdapter;

                    //Toast.MakeText(this, "Flash Set Deleted", ToastLength.Short).Show();
                }
            };
        }

        // Implementation to retrive a users flash sets
        private void RetreiveSet(ListView m_FlashSet, string m_Username)
        {
            try
            {
                DataBase.DBAdapter db = new DataBase.DBAdapter(this);
                db.openDB();

                ICursor SetInfo = db.GetSets(m_Username);
                mSetNameList.Clear();

                while (SetInfo.MoveToNext())
                {
                    string name = SetInfo.GetString(1);
                    mSetNameList.Add(name);
                }

                db.CloseDB();
                m_FlashSet.Adapter = mAdapter;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(this, "Failed to retrieve flash sets from the database", ToastLength.Short).Show();
            }
        }

        private bool AddSet(string m_Username, string m_SetName)
        {
            try
            {
                DataBase.DBFunction DataFunc = new DataBase.DBFunction();
                DataBase.DBAdapter db = new DataBase.DBAdapter(this);

                db.openDB();
                ICursor UserInfo = db.GetSpecificSet(m_Username, m_SetName);

                if (UserInfo.Count == 0)
                {
                    if (m_Username.Length > 0 && m_SetName.Length > 0)
                    {
                        if (DataFunc.SaveSet(m_Username, m_SetName, this))
                        {
                            Toast.MakeText(this, m_SetName + " Created", ToastLength.Short).Show();
                            RetreiveSet(mFlashSetList, m_Username);
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
                Toast.MakeText(this, "Unable to create a new Set", ToastLength.Short).Show();
                return false;
            }
        }

        private bool DeleteSet(string m_Username, string m_SetName)
        {
            DataBase.DBAdapter db = new DataBase.DBAdapter(this);
            db.openDB();

            if (db.DeleteRowSet_tb(m_Username, m_SetName))
            {
                RetreiveSet(mFlashSetList, m_Username);
                Toast.MakeText(this, m_SetName + " was deleted", ToastLength.Short).Show();

                db.CloseDB();
                return true;
            }
            else
            {
                Toast.MakeText(this, "Unable to delete " + m_SetName, ToastLength.Short).Show();

                db.CloseDB();
                return false;
            }
        }
    }
}