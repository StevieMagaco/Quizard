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
using Android;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Quizard.Resources.layout
{
    // turn off MainLauncher after testing
    [Activity(Label = "DeckActivity")]
    public class DeckActivity : Activity
    {
        List<string> questions;
        List<string> answers;

        List<string> quizAnswers;
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
            // Set our view from the "main" layout resource
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
            tab.TabUnselected += delegate (object sender, ActionBar.TabEventArgs e) {
                e.FragmentTransaction.Remove(view);
            };
            this.ActionBar.AddTab(tab);
        }
    }
    }


