using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Quizard.Resources.layout
{
    public class DeckCardTabFragment : Fragment
    {
        List<string> questions, answers;
        ListView cardTabListView;
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

            var sampleListView = view.FindViewById<ListView>(Resource.Id.cardTabListView);

            ArrayAdapter<string> ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, questions);

            sampleListView.Adapter = ListAdapter;

            sampleListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                string selectedFromList = (string)sampleListView.GetItemAtPosition(e.Position);
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                DeckCardDialogFragment aDifferentDetailsFrag = new DeckCardDialogFragment(questions,answers, e.Position);
                aDifferentDetailsFrag.Show(fragmentTx, "dialog_fragment");
            };

            return view;
        }
    }
}
