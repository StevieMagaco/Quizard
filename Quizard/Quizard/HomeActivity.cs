using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Quizard
{
    public class Flashcard
    {
        public string flashcardSubject { get; set; }
        public string flashcardCount { get; set; }
    }

    public class RecyclerAdapter : RecyclerView.Adapter
    {
        private List<Flashcard> flashSetCards;
        private RecyclerView view;

        public class FlashSetView : RecyclerView.ViewHolder
        {
            public View flashSetView { get; set; }
            public TextView flashSetSubject { get; set; }
            public TextView flashSetCardCount { get; set; }

            public FlashSetView(View view) : base(view)
            {
                flashSetView = view;
            }
        }

        public RecyclerAdapter(List<Flashcard> flashSet, RecyclerView flashSetView)
        {
            flashSetCards = flashSet;
            view = flashSetView;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View flashSetRow = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FlashSetRowView, parent, false);

            TextView flashSetSubjectText = flashSetRow.FindViewById<TextView>(Resource.Id.flashsetSubjectTextViewID);
            TextView flashSetCardCountText = flashSetRow.FindViewById<TextView>(Resource.Id.flashsetCardCountTextViewID);

            FlashSetView view = new FlashSetView(flashSetRow)
            {
                flashSetSubject = flashSetSubjectText,
                flashSetCardCount = flashSetCardCountText
            };

            return view;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int flashSetPosition)
        {
            FlashSetView flashSetHolder = holder as FlashSetView;
            int indexPosition = (flashSetCards.Count - 1) - flashSetPosition;

            flashSetHolder.flashSetView.Click += (object sender, EventArgs e) =>
            {
                // TODO: Implement the navigation to the inner flash set layout
            };

            flashSetHolder.flashSetSubject.Text = flashSetCards[indexPosition].flashcardSubject;
            flashSetHolder.flashSetCardCount.Text = flashSetCards[indexPosition].flashcardCount;
        }

        public override int ItemCount
        {
            get { return flashSetCards.Count; }
        }
    }

    [Activity(Label = "Home", MainLauncher = false /*Keep the MainLauncher = false unless this dialog fragment needs to be tested*/)]
    public class HomeActivity : Activity
    {
        private Toolbar toolbar;
        private RecyclerView flashsetItem;
        private RecyclerView.LayoutManager layoutManager;
        private RecyclerView.Adapter adapter;
        private List<Flashcard> flashset;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "HomeLayout" layout resource
            SetContentView(Resource.Layout.HomeLayout);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbarID);
            flashsetItem = FindViewById<RecyclerView>(Resource.Id.flashsetRecyclerViewID);

            // For testing ////////////////////////////////////
            flashset = new List<Flashcard>();

            flashset.Add(new Flashcard()
            {
                flashcardSubject = "Computer Science",
                flashcardCount = "1"
            });
            ///////////////////////////////////////////////////

            layoutManager = new LinearLayoutManager(this);
            flashsetItem.SetLayoutManager(layoutManager);

            adapter = new RecyclerAdapter(flashset, flashsetItem);
            flashsetItem.SetAdapter(adapter);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ToolbarItems, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.addAFlashSetItemID:

                    // For testing ////////////////////////////////////
                    flashset.Add(new Flashcard()
                    {
                        flashcardSubject = "New Subject",
                        flashcardCount = "New Flashcard Count"
                    });
                    ///////////////////////////////////////////////////

                    adapter.NotifyItemInserted(0);
                    return true;

                case Resource.Id.deleteAFlashSetItemID:

                    // This code removes the most recent flashset
                    flashset.RemoveAt(flashset.Count - 1);

                    adapter.NotifyItemRemoved(0);
                    return true;

                case Resource.Id.settingsItemID:

                    // TODO: Implement a settings layout to replace this intent
                    Intent intent = new Intent(this, typeof(LoginActivity));
                    this.StartActivity(intent);
                    return true;

                default:
                    break;
            }

            return true;
        }
    }
}