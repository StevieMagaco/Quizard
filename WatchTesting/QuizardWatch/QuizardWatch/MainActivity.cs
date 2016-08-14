using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Java.Interop;
using Android.Views.Animations;
using System.Collections.Generic;


using Android.Gms.Wearable;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using System.Linq;
using Android.Support.V4.Content;

namespace QuizardWatch
{
    [Activity(Label = "QuizardWatch", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IDataApiDataListener, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        //private ListView QuizList;
        //Temporary List of Dummy data
        //private List<string> tempList = new List<string>();

        private Button myButton;
        private EditText myText;

        // Used for interaction with the phone
        private GoogleApiClient GoogleClient;

        const string QuesSetPath = "/sets";
        const string QuesSetKey = "sets";


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Set our view from the "main" layout resource

            GoogleClient = new GoogleApiClient.Builder(this, this, this).AddApi(WearableClass.API).Build();
            

            SetContentView(Resource.Layout.Main);

            var v = FindViewById<WatchViewStub>(Resource.Id.watch_view_stub);
            v.LayoutInflated += delegate
            {
                myButton = FindViewById<Button>(Resource.Id.myButton);
                myText = FindViewById<EditText>(Resource.Id.myText);
                myButton.Click += (object sender, EventArgs e) =>
                {
                    SendData("Test");
                };
            };

            


            //tempList.Add("History");
            //tempList.Add("Mathematics");
            //tempList.Add("Literature");
            //
            //QuizList = FindViewById<ListView>(Resource.Id.QuizList);
            //
            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, tempList);
            //
            //QuizList.Adapter = adapter;

            // This isn't really used for something, it's just here to test to see if I can send data from the watch to the phone
            //QuizList.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            //{
            //    SendData(tempList.ElementAt(e.Position));
            //};
        }

        private void SendData(string Data)
        {
            try
            {
                var request = PutDataMapRequest.Create(QuesSetPath);
                var map = request.DataMap;
                map.PutString(QuesSetKey, Data);
                map.PutLong("UpdatedAt", DateTime.UtcNow.Ticks);
                WearableClass.DataApi.PutDataItem(GoogleClient, request.AsPutDataRequest());
            }
            finally
            {
                GoogleClient.Disconnect();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            GoogleClient.Connect();
        }

        public void OnConnected(Bundle connectionHint)
        {
            WearableClass.DataApi.AddListener(GoogleClient, this);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Android.Util.Log.Error("GMS", "Connection failed " + result.ErrorCode);
        }

        public async void OnConnectionSuspended(int cause)
        {
            //Android.Util.Log.Error("GMS", "Connection suspended " + reason);
            await WearableClass.DataApi.AddListenerAsync(GoogleClient, this);
        }

        protected override void OnStop()
        {
            base.OnStop();
            GoogleClient.Disconnect();
        }

        public void OnDataChanged(DataEventBuffer dataEvents)
        {

        }

        
    }
}


