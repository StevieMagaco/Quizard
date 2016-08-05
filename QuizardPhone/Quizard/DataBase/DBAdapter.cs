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
        private Context c;
        private SQLiteDatabase db;
        private DBHelper helper;

        public DBAdapter(Context c)
        {
            this.c = c;
            helper = new DBHelper(c);
        }

        // Open DB connection
        public DBAdapter openDB()
        {
            try
            {
                db = helper.WritableDatabase;

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
                helper.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //  return this;
        }
        // Add a new User to a users table pass Username, Password, FirstName, Lastname
        // In that order
        public bool AddUser(String Username, String Password)
        {
            try
            {
                ContentValues insertValues = new ContentValues();
                insertValues.Put(Constants.Users_UserName, Username);
                insertValues.Put(Constants.Users_Password, Password);
                db.Insert(Constants.Users_TB_Name, null, insertValues);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        //Get the user with that UserName and Passord
        public ICursor GetUser(String UserName, String Password)
        {
            String whereclause;
            String[] Clause = { UserName, Password };
            String[] columns = { Constants.Users_UserName, Constants.Users_Password };
            if (UserName.Length > 0 && Password.Length > 0)
            {
                whereclause = Constants.Users_UserName + " = ? and " + Constants.Users_Password + " = ?";
                return db.Query(Constants.Users_TB_Name, columns, whereclause, Clause, null, null, null); ;
            }
            else
            {
                return db.Query(Constants.Users_TB_Name, columns, null, null, null, null, null);
            }
        }
        public bool SaveSet(Sets m_Set)
        {
            try
            {
                ContentValues insertValues = new ContentValues();
                insertValues.Put(Constants.Sets_UserName, m_Set.GetUsername());
                insertValues.Put(Constants.Sets_SetName, m_Set.GetSetName());
                insertValues.Put(Constants.Sets_Notify, m_Set.GetNotify());
                insertValues.Put(Constants.Sets_Correct, m_Set.GetCorrect());
                insertValues.Put(Constants.Sets_Incorrect, m_Set.GetIncorrect());
                db.Insert(Constants.Sets_TB_Name, null, insertValues);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public ICursor GetSets(String UserName)
        {
            String[] columns = { Constants.Sets_UserName, Constants.Sets_SetName, Constants.Sets_Notify, Constants.Sets_Correct, Constants.Sets_Incorrect };
            return db.Query(Constants.Users_TB_Name, columns, null, null, null, null, null);
        }
    }
}