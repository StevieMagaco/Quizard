using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Content;

using Android.Gms.Wearable;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using System.Linq;

namespace QuizardWatch
{
    [Activity(Label = "FakeQuizardPhone", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IDataApiDataListener
    {
        EditText textToSend;
        Button myButton;

        // Used for interaction with the watch
        private GoogleApiClient GoogleClient;
        const string _syncPath = "/WearDemo/Data";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            textToSend = FindViewById<EditText>(Resource.Id.textToSend);
            myButton = FindViewById<Button>(Resource.Id.MyButton);



            myButton.Click += (object sender, EventArgs e) =>
            {
                SendData(textToSend.Text);
            };

            IntentFilter filter = new IntentFilter(Intent.ActionSend);
            MessageReciever receiver = new MessageReciever(this);
            LocalBroadcastManager.GetInstance(this).RegisterReceiver(receiver, filter);

        }

        // The Data string being sent in will be a Json string to send more data at once, and then
        private void SendData(string Data)
        {
            try
            {
                var request = PutDataMapRequest.Create(_syncPath);
                var map = request.DataMap;
                map.PutString("Message", Data);
                map.PutLong("UpdatedAt", DateTime.UtcNow.Ticks);
                WearableClass.DataApi.PutDataItem(GoogleClient, request.AsPutDataRequest());
            }
            finally
            {
                GoogleClient.Disconnect();
            }
        }

        public void ProcessMessage(Intent intent)
        {
            textToSend.Text = intent.GetStringExtra("WearMessage");
        }

        public void OnDataChanged(DataEventBuffer dataEvents)
        {
            textToSend.Text = Intent.GetStringExtra("WearMessage");
        }

        internal class MessageReciever : BroadcastReceiver
        {
            MainActivity _main;
            public MessageReciever(MainActivity owner) { this._main = owner; }
            public override void OnReceive(Context context, Intent intent)
            {
                _main.ProcessMessage(intent);
            }
        }
    }
}

