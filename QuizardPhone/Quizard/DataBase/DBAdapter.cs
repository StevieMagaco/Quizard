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
using Android.Database.Sqlite;
using Android.Database;

namespace Quizard.DataBase
{
    class DBAdapter
    {
        private Context mContext;
        private SQLiteDatabase mdb;
        private DBHelper mhelper;

        public DBAdapter(Context c)
        {
            this.mContext = c;
            mhelper = new DBHelper(mContext);
        }

        // Open DB connection
        public DBAdapter openDB()
        {
            try
            {
                mdb = mhelper.WritableDatabase;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return this;
        }

        // Close 
        public void CloseDB()
        {
            try
            {
                mhelper.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //  return this;
        }
        public SQLiteDatabase GetDataBase()
        {
            return mdb;
        }
        // Add a new User to a users table pass Username, Password, FirstName, Lastname
        // In that order
        public bool AddUser(String _Username, String _Password)
        {
            try
            {
                ContentValues insertValues = new ContentValues();
                insertValues.Put(Constants.Users_UserName, _Username);
                insertValues.Put(Constants.Users_Password, _Password);
                mdb.Insert(Constants.Users_TB_Name, null, insertValues);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        //Get the user with that UserName and Passord
        public ICursor GetUser(String _UserName, String _Password)
        {
            String whereclause;
            String[] Clause2 = { _UserName, _Password };
            String[] Clause1 = { _UserName };
            String[] columns = { Constants.Users_UserName, Constants.Users_Password };

            if (_UserName.Length > 0 && _Password.Length > 0)
            {
                whereclause = Constants.Users_UserName + " = ? and " + Constants.Users_Password + " = ?";
                return mdb.Query(Constants.Users_TB_Name, columns, whereclause, Clause2, null, null, null); ;
            }
            else if (_UserName.Length > 0 && _Password == "")
            {
                whereclause = Constants.Users_UserName + " = ? ";
                return mdb.Query(Constants.Users_TB_Name, columns, whereclause, Clause1, null, null, null); ;
            }
            else
            {
                return mdb.Query(Constants.Users_TB_Name, columns, null, null, null, null, null);
            }
        }

        public bool SaveSet(Sets _Set)
        {
            try
            {
                ContentValues insertValues = new ContentValues();
                insertValues.Put(Constants.Sets_UserName, _Set.GetUsername());
                insertValues.Put(Constants.Sets_SetName, _Set.GetSetName());
                insertValues.Put(Constants.Sets_Notify, _Set.GetNotify());
                insertValues.Put(Constants.Sets_Correct, _Set.GetCorrect());
                insertValues.Put(Constants.Sets_Incorrect, _Set.GetIncorrect());
                mdb.Insert(Constants.Sets_TB_Name, null, insertValues);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public ICursor GetSets(String _UserName)
        {
            String whereclause;
            String[] Clause = { _UserName };
            whereclause = Constants.Users_UserName + " = ?";
            String[] columns = { Constants.Sets_UserName, Constants.Sets_SetName, Constants.Sets_Notify, Constants.Sets_Correct, Constants.Sets_Incorrect };
            return mdb.Query(Constants.Sets_TB_Name, columns, whereclause, Clause, null, null, null);
        }
       public ICursor GetSpecificSet(String _Username, String _SetName)
        {
            String whereclause;
            String[] Clause = { _Username, _SetName };
            whereclause = Constants.Users_UserName + " = ? and " + Constants.Sets_SetName + " = ?";
            String[] columns = { Constants.Sets_UserName, Constants.Sets_SetName, Constants.Sets_Notify, Constants.Sets_Correct, Constants.Sets_Incorrect };
            return mdb.Query(Constants.Sets_TB_Name, columns, whereclause, Clause, null, null, null);
        }
        public ICursor GetRemeberMe()
        {
            String[] columns = {Constants.RememberMe_Username, Constants.RememberMe_Password };
            return mdb.Query(Constants.RememberMe_TB_Name, columns, null, null, null, null, null);
        }
        public ICursor GetCards(String _Username, String _SetName)
        {
            String whereclause;
            String[] Clause = { _Username, _SetName };
            whereclause = Constants.Cards_UserName + " = ? and " + Constants.Cards_SetName + " = ?";
            String[] columns = { Constants.Cards_UserName, Constants.Cards_SetName, Constants.Cards_Question, Constants.Cards_Answer, Constants.Cards_NumberBox, Constants.Cards_PreRun};
            return mdb.Query(Constants.Cards_TB_Name, columns, whereclause, Clause, null, null, null);
        }
        public bool SetCard(Cards _Card)
        {
            try
            {
                ContentValues insertValues = new ContentValues();
                insertValues.Put(Constants.Cards_UserName, _Card.GetUserName());
                insertValues.Put(Constants.Cards_SetName, _Card.GetSetName());
                insertValues.Put(Constants.Cards_Answer, _Card.GetAnswer());
                insertValues.Put(Constants.Cards_Question,_Card.GetQuestion());
                insertValues.Put(Constants.Cards_NumberBox,_Card.GetNumberBox());
                insertValues.Put(Constants.Cards_PreRun,_Card.GetPreRun());
                mdb.Insert(Constants.Cards_TB_Name, null, insertValues);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public bool DeleteRowSet_tb(string _Username,string _SetName)
        {
            String whereclause = Constants.Users_UserName + " = ? and " + Constants.Sets_SetName + " = ?";
            String[] Clause = { _Username, _SetName };
            if(mdb.Delete(Constants.Sets_TB_Name, whereclause, Clause) > 0)
                return true;
            else
            return false;
        }
        public bool UpdateRowSets(Sets _Set, string NewSetName)
        {
            String whereclause = Constants.Users_UserName + " = ? and " + Constants.Sets_SetName + " = ?";
            String[] Clause = { _Set.GetUsername(), _Set.GetSetName() };
            ContentValues insertValues = new ContentValues();
            insertValues.Put(Constants.Sets_UserName, _Set.GetUsername());
            insertValues.Put(Constants.Sets_SetName, NewSetName);
            insertValues.Put(Constants.Sets_Notify, _Set.GetNotify());
            insertValues.Put(Constants.Sets_Correct, _Set.GetCorrect());
            insertValues.Put(Constants.Sets_Incorrect, _Set.GetIncorrect());
            if (mdb.Update(Constants.Sets_TB_Name, insertValues, whereclause, Clause) > 0)
                return true;
            else
                return false;
        }
        public bool AddRemeberMe_tb( string _Username, string _Password)
        {

            try
            {
                mdb.Delete(Constants.RememberMe_TB_Name, null, null);
                ContentValues insertValues = new ContentValues();
                insertValues.Put(Constants.RememberMe_Username, _Username);
                insertValues.Put(Constants.RememberMe_Password, _Password);
                mdb.Insert(Constants.RememberMe_TB_Name, null, insertValues);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;

        }
    }
}


// Add card to database Code 
/*
   private bool AddCard_db(string _Username, string _SetName, string _Question, string _Answer)
        {
            try
            {
                DataBase.Cards CardBuffer = new DataBase.Cards(_Username, _SetName, _Question, _Answer, "", "");
                DataBase.DBAdapter db = new DataBase.DBAdapter(mContext);
                db.openDB();
                if (db.SetCard(CardBuffer))
                {
                    RetrieveCards(_Username, _SetName);

                    db.CloseDB();
                    return true;
                }
                else
                    throw new System.ArgumentException("Failed to save new Card", "CardSave");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(mContext, "Failed to add card", ToastLength.Short).Show();
                return false;
            }
        }
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

                mCardTabListView.Adapter = mAdapterQ;

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toast.MakeText(mContext, "Failed to retrieve Cards", ToastLength.Short).Show();
            }

        }
 */
