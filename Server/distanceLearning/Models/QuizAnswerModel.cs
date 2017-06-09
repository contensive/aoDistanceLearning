
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
    public class QuizAnswerModel
    {
        //
        //-- const
        public const string primaryContentName = "Quiz Answers";
        private const string primaryContentTableName = "quizAnswers";
        //
        // -- instance properties
        public int id;
        public string name;
        public string guid;
        public bool Correct;
        public int QuestionID;
        //public string AText; <-- use name
        public string SortOrder;
        //public int QOrder;  // <-- use sortorder
        public int points;
        //      
        //
        //public bool Active;
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
        public QuizAnswerModel() {}
        //
        //====================================================================================================
        /// <summary>
        /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId">The id of the record to be read into the new object</param>
        public static QuizAnswerModel create(CPBaseClass cp, int recordId)
        {
            QuizAnswerModel result = null;
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
        public static QuizAnswerModel create(CPBaseClass cp, string recordGuid)
        {
            QuizAnswerModel result = null;
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
        private static QuizAnswerModel loadObject(CPBaseClass cpCore, string sqlCriteria)
        {
            QuizAnswerModel result = null;
            try
            {
                //
                // -- upgrade Atext -> name
                cpCore.Db.ExecuteSQL("update quizAnswers set name=atext where name is null");
                cpCore.Db.ExecuteSQL("update quizAnswers set sortOrder=qorder where sortOrder is null");
                CPCSBaseClass cs = cpCore.CSNew();
                if (cs.Open(primaryContentName, sqlCriteria))
                {
                    result = new QuizAnswerModel();
                    //
                    // -- populate result model
                    result.id = cs.GetInteger("id");
                    result.name = cs.GetText("name");
                    result.guid = cs.GetText("ccGuid");
                    result.Correct = cs.GetBoolean("Correct");
                    result.createKey = cs.GetInteger("createKey");
                    result.QuestionID = cs.GetInteger("QuestionID");
                    result.SortOrder = cs.GetText("SortOrder");
                    result.points = cs.GetInteger("points");
                    //result.AText = cs.GetText("AText");
                    //result.QOrder = cs.GetInteger("qorder");
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
                    //cs.SetField("AText", AText);
                    cs.SetField("Correct", Correct.ToString());
                    cs.SetField("QuestionID", QuestionID.ToString());
                    if (string.IsNullOrEmpty( SortOrder )) SortOrder = id.ToString().PadLeft(7, '0');
                    cs.SetField("SortOrder", SortOrder.ToString());
                    //cs.SetField("qorder", QOrder.ToString());
                    cs.SetField("points", points.ToString());
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
        public static List<QuizAnswerModel> getAnswersForQuestionList(CPBaseClass cp, int QuestionID)
        {
            List<QuizAnswerModel> result = new List<QuizAnswerModel>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(primaryContentName, "(QuestionID=" + QuestionID + ")", "sortorder", true, "id")))
                {
                    QuizAnswerModel instance = null;
                    do
                    {
                        instance = QuizAnswerModel.create(cp, cs.GetInteger("id"));
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
        /// <summary>
        /// Add record method
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        //

        public static QuizAnswerModel add(CPBaseClass cp)
        {
            QuizAnswerModel answer = create(cp, cp.Content.AddRecord(primaryContentName));
            answer.SortOrder = genericController.getSortOrderFromInteger( answer.id) ;
            return answer;
        }
    }
}

