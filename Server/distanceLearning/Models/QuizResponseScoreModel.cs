﻿
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Contensive.BaseClasses;
using Newtonsoft.Json;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning.Models
{
    public class QuizResponseScoreModel
    {
        //
        //-- const
        public const string primaryContentName = "Quiz Response Scores";
        private const string primaryContentTableName = "quizResponseScores";
        //
        // -- instance properties
        public int id;
        public string name;
        public string guid;
        //
        //public int QuizResponseID;
        //public int QuizSubjectID;
        //public double score;
        //public int points;
        //
        //public bool Active;
        //public string SortOrder;
        //public DateTime DateAdded;
        //public int CreatedBy;
        //public DateTime ModifiedDate;
        //public int ModifiedBy;
        //
        // -- publics not exposed to the UI (test/internal data)
        [JsonIgnore()]
        public int createKey;
        //
        //====================================================================================================
        /// <summary>
        /// Create an empty object. needed for deserialization
        /// </summary>
        public QuizResponseScoreModel() {}
        //
        //====================================================================================================
        /// <summary>
        /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId">The id of the record to be read into the new object</param>
        public static QuizResponseScoreModel create(CPBaseClass cp, int recordId)
        {
            QuizResponseScoreModel result = null;
            try
            {
                if (recordId > 0)
                {
                    if ((result == null))
                    {
                        result = loadObject(cp, "id=" + recordId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// open an existing object
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordGuid"></param>
        public static QuizResponseScoreModel create(CPBaseClass cp, string recordGuid)
        {
            QuizResponseScoreModel result = null;
            try
            {
                if (!string.IsNullOrEmpty(recordGuid))
                {
                    if ((result == null))
                    {
                        result = loadObject(cp, "ccGuid=" + cp.Db.EncodeSQLText(recordGuid));
                    }
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// open an existing object
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="sqlCriteria"></param>
        private static QuizResponseScoreModel loadObject(CPBaseClass cpCore, string sqlCriteria)
        {
            QuizResponseScoreModel result = null;
            try
            {
                CPCSBaseClass cs = cpCore.CSNew();
                if (cs.Open(primaryContentName, sqlCriteria))
                {
                    result = new QuizResponseScoreModel();
                    //
                    // -- populate result model
                    result.id = cs.GetInteger("id");
                    result.name = cs.GetText("name");
                    result.guid = cs.GetText("ccGuid");
                    result.createKey = cs.GetInteger("createKey");
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cpCore.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// save the instance properties to a record with matching id. If id is not provided, a new record is created.
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public int saveObject(CPBaseClass cp)
        {
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                if ((id > 0))
                {
                    if (!cs.Open(primaryContentName, "id=" + id))
                    {
                        id = 0;
                        cs.Close();
                        throw new ApplicationException("Unable to open record in content [" + primaryContentName + "], with id [" + id + "]");
                    }
                }
                else
                {
                    if (!cs.Insert(primaryContentName))
                    {
                        cs.Close();
                        id = 0;
                        throw new ApplicationException("Unable to insert record in content [" + primaryContentName + "]");
                    }
                }
                if (cs.OK())
                {
                    id = cs.GetInteger("id");
                    cs.SetField("name", name);
                    cs.SetField("ccGuid", guid);
                    cs.SetField("createKey", createKey.ToString());
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return id;
        }
        //
        //====================================================================================================
        /// <summary>
        /// delete an existing database record
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId"></param>
        public static void delete(CPBaseClass cp, int recordId)
        {
            try
            {
                if ((recordId > 0))
                {
                    cp.Db.ExecuteSQL("delete from " + cp.Content.GetTable(primaryContentName) + " where (id=" + recordId.ToString()  + ")");
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// delete an existing database record
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId"></param>
        public static void delete(CPBaseClass cp, string guid)
        {
            try
            {
                if ((!string.IsNullOrEmpty(guid)))
                {
                    cp.Db.ExecuteSQL( "delete from " + cp.Content.GetTable( primaryContentName) + " where (ccguid=" + cp.Db.EncodeSQLText(guid) + ")");
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// get a list of objects from this model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="someCriteria"></param>
        /// <returns></returns>
        public static List<QuizResponseScoreModel> getObjectList(CPBaseClass cp, int someCriteria)
        {
            List<QuizResponseScoreModel> result = new List<QuizResponseScoreModel>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(primaryContentName, "(someCriteria=" + someCriteria + ")", "name", true, "id")))
                {
                    QuizResponseScoreModel instance = null;
                    do
                    {
                        instance = QuizResponseScoreModel.create(cp, cs.GetInteger("id"));
                        if ((instance != null))
                        {
                            result.Add(instance);
                        }
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}

