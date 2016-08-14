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

using Android.Gms.Wearable;
using Android.Gms.Common.Apis;
using Android.Support.V4.Content;


namespace QuizardWatch
{
    [Service]
    [IntentFilter(new[] { "com.google.android.gms.wearable.BIND_LISTENER" })]
    class Listener : WearableListenerService
    {
        const string _syncPath = "/WearDemo/Data";
        GoogleApiClient GoogleClient;

        public override void OnCreate()
        {
            base.OnCreate();
            GoogleClient = new GoogleApiClient.Builder(this.ApplicationContext)
                    .AddApi(WearableClass.API)
                    .Build();

            GoogleClient.Connect();

            Android.Util.Log.Info("WearIntegration", "Created");
        }

        public override void OnDataChanged(DataEventBuffer dataEvents)
        {
            var dataEvent = Enumerable.Range(0, dataEvents.Count)
                                      .Select(i => dataEvents.Get(i).JavaCast<IDataEvent>())
                                      .FirstOrDefault(x => x.Type == DataEvent.TypeChanged && x.DataItem.Uri.Path.Equals(_syncPath));
            if (dataEvent == null)
                return;

            //get data from wearable
            var dataMapItem = DataMapItem.FromDataItem(dataEvent.DataItem);
            var map = dataMapItem.DataMap;
            string message = dataMapItem.DataMap.GetString("Message");

            Intent intent = new Intent();
            intent.SetAction(Intent.ActionSend);
            intent.PutExtra("WearMessage", message);
            LocalBroadcastManager.GetInstance(this).SendBroadcast(intent);
        }
    }
}