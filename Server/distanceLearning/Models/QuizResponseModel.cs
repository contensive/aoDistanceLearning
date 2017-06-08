
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

    public class QuizResponseModel
    {
        //
        //-- const
        public const string primaryContentName = "Quiz Responses";
        private const string primaryContentTableName = "quizResponses";
        //
        // -- instance properties
        public int id;
        public string name;
        public string guid;
        public DateTime DateAdded;
        //public int maxNumberQuest;
        //public string questionPresentation;
        //public string includeSubject;
        //public bool allowRetake;
        //
        public int MemberID;
        public int QuizID;
        public int attemptNumber;
        public DateTime dateSubmitted;
        public int totalQuestions;
        public int totalCorrect;
        public int totalPoints;
        public double Score;
        //public bool lastDisplayedStudyPage;
        //public int lastDisplayedPageOrder;
        public DateTime dateStarted;
        public string SortOrder;
        public int currentPageNumber;
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
        public QuizResponseModel() {}
        //
        //====================================================================================================
        /// <summary>
        /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId">The id of the record to be read into the new object</param>
        public static QuizResponseModel create(CPBaseClass cp, int recordId)
        {
            QuizResponseModel result = null;
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
        /// <summary>
        /// open a response model for this user, quiz and responseId
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public static QuizResponseModel create(CPBaseClass cp, int recordId, int quizId, int userId)
        {
            QuizResponseModel result = null;
            try
            {
                if (recordId > 0)
                {
                    if ((result == null))
                    {
                        result = loadObject(cp, "(id=" + recordId.ToString() + ")and(quizId=" + quizId.ToString() + ")and(memberId=" + userId.ToString() + ")");
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
        /// <summary>
        /// create a response model for a quiz and a user that might be be completed yet.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quizId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static QuizResponseModel create(CPBaseClass cp, int quizId, int userId )
        {
            QuizResponseModel result = null;
            try
            {
                if ((result == null))
                {
                    result = loadObject(cp, "(memberId=" + userId.ToString() + ")and(dateSubmitted is null)and(quizId=" + quizId.ToString() + ")");
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
        public static QuizResponseModel create(CPBaseClass cp, string recordGuid)
        {
            QuizResponseModel result = null;
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
        /// open the newest response that matches the criteria 
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="sqlCriteria"></param>
        private static QuizResponseModel loadObject(CPBaseClass cpCore, string sqlCriteria)
        {
            QuizResponseModel result = null;
            try
            {
                CPCSBaseClass cs = cpCore.CSNew();
                if (cs.Open(primaryContentName, sqlCriteria,"id desc"))
                {
                    result = new QuizResponseModel();
                    //
                    // -- populate result model
                    result.id = cs.GetInteger("id");
                    result.name = cs.GetText("name");
                    result.guid = cs.GetText("ccGuid");
                    result.createKey = cs.GetInteger("createKey");
                    result.DateAdded = cs.GetDate("dateadded");
                    //result.maxNumberQuest = cs.GetInteger("maxNumberQuest");
                    //result.questionPresentation = cs.GetText("questionPresentation");
                    //result.includeSubject = cs.GetText("includeSubject");
                    //result.allowRetake = cs.GetBoolean("allowRetake");
                    result.QuizID = cs.GetInteger("QuizID");
                    result.MemberID = cs.GetInteger("MemberID");
                    result.attemptNumber = cs.GetInteger("attemptNumber");
                    result.dateSubmitted = cs.GetDate("dateSubmitted");
                    result.totalQuestions = cs.GetInteger("totalQuestions");
                    result.totalCorrect = cs.GetInteger("totalCorrect");
                    result.totalPoints = cs.GetInteger("totalPoints");
                    result.Score = cs.GetNumber("Score");
                    //result.lastDisplayedStudyPage = cs.GetBoolean("lastDisplayedStudyPage");
                    //result.lastDisplayedPageOrder = cs.GetInteger("lastDisplayedPageOrder");
                    result.dateStarted = cs.GetDate("dateStarted");
                    result.SortOrder = cs.GetText("SortOrder");
                    result.currentPageNumber = cs.GetInteger("lastPageNumber");
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
                    cs.SetField("dateadded", DateAdded.ToString());
                    //cs.SetField("maxNumberQuest", maxNumberQuest.ToString());
                    //cs.SetField("questionPresentation", questionPresentation);
                    //cs.SetField("includeSubject", includeSubject);
                    //cs.SetField("allowRetake", allowRetake.ToString());
                    cs.SetField("QuizID", QuizID.ToString());
                    cs.SetField("MemberID", MemberID.ToString());
                    cs.SetField("attemptNumber", attemptNumber.ToString());
                    cs.SetField("dateSubmitted", dateSubmitted.ToString());
                    cs.SetField("totalQuestions", totalQuestions.ToString());
                    cs.SetField("totalCorrect", totalCorrect.ToString());
                    cs.SetField("totalPoints", totalPoints.ToString());
                    cs.SetField("Score", Score.ToString());
                    //cs.SetField("lastDisplayedStudyPage", lastDisplayedStudyPage.ToString());
                    //cs.SetField("lastDisplayedPageOrder", lastDisplayedPageOrder.ToString());
                    cs.SetField("dateStarted", dateStarted.ToString());
                    cs.SetField("SortOrder", SortOrder.ToString());
                    cs.SetField("lastPageNumber", currentPageNumber.ToString());
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
        /// <param name="QuizId"></param>
        /// <returns></returns>
        public static List<QuizResponseModel> GetResponseList(CPBaseClass cp, int QuizId)
        {
            List<QuizResponseModel> modelList = new List<QuizResponseModel>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                if ((cs.Open(primaryContentName, "(QuizId=" + QuizId + ")", "name", true, "id")))
                {
                    QuizResponseModel instance = null;
                    do
                    {
                        instance = QuizResponseModel.create(cp, cs.GetInteger("id"));
                        if ((instance != null))
                        {
                            modelList.Add(instance);
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
            return modelList;
        }
        //
        //====================================================================================================
        /// <summary>
        /// get a list of submitted responses for this quiz and this person
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="QuizId"></param>
        /// <returns></returns>
        public static List<QuizResponseModel> GetResponseList(CPBaseClass cp, int QuizId, int userId)
        {
            List<QuizResponseModel> modelList = new List<QuizResponseModel>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                if ((cs.Open(primaryContentName, "(QuizId=" + QuizId + ")and(memberId=" + cp.User.Id + ")", "name", true, "id")))
                {
                    QuizResponseModel instance = null;
                    do
                    {
                        instance = QuizResponseModel.create(cp, cs.GetInteger("id"));
                        if ((instance != null))
                        {
                            modelList.Add(instance);
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
            return modelList;
        }
        public class quizResponseReportModel
        {
            public string quizName;
            public string userName;
            public DateTime dateSubmitted;
            public int attemptNumber;
            public double score;
            public int totalQuestions;
            public int totalCorrect;
            public int totalPoints;
            public int id;
        }
        //
        //====================================================================================================
        /// <summary>
        /// get the list of responses for the quiz overview reporting tab
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="QuizId"></param>
        /// <returns></returns>
        public static List<quizResponseReportModel> GetQuizOverviewResponseList(CPBaseClass cp, int QuizId, DateTime fromDate, DateTime toDate)
        {
            List<quizResponseReportModel> modelList = new List<quizResponseReportModel>();
            try
            {
                string sql = "select q.name as quizName, u.name as userName, r.id as responseId,r.dateSubmitted, r.attemptNumber, r.score, r.totalQuestions, r.totalCorrect, r.totalPoints"
                    + " from ((quizResponses r"
                    + " left join quizzes q on q.id=r.quizId)"
                    + " left join ccMembers u on u.id=r.memberId)"
                    + " where (QuizId=" + QuizId + ")";
                if (fromDate > DateTime.MinValue)
                {
                    string sqlFromDate = cp.Db.EncodeSQLDate(fromDate.Date);
                    string sqlFromNextDate = cp.Db.EncodeSQLDate(fromDate.AddDays(1).Date);
                    sql += "and(r.dateSubmitted>=" + sqlFromDate + ")and(r.dateSubmitted<" + sqlFromNextDate + ")";
                }
                if (toDate > DateTime.MinValue)
                {
                    string sqlToDate = cp.Db.EncodeSQLDate(toDate.Date);
                    string sqlToNextDate = cp.Db.EncodeSQLDate(toDate.AddDays(1).Date);
                    sql += "and(r.dateSubmitted>=" + sqlToDate + ")and(r.dateSubmitted<" + sqlToNextDate + ")";
                }
                CPCSBaseClass cs = cp.CSNew();
                if (cs.OpenSQL(sql))
                {
                    quizResponseReportModel instance = null;
                    do
                    {
                        instance = new quizResponseReportModel();
                        instance.id = cs.GetInteger("responseId");
                        instance.attemptNumber = cs.GetInteger("attemptNumber");
                        instance.dateSubmitted = cs.GetDate("dateSubmitted");
                        instance.quizName = cs.GetText("quizName");
                        instance.userName = cs.GetText("userName");
                        instance.score = cs.GetNumber("score");
                        instance.totalQuestions = cs.GetInteger("totalQuestions");
                        instance.totalCorrect = cs.GetInteger("totalCorrect");
                        instance.totalPoints = cs.GetInteger("totalPoints");
                        modelList.Add(instance);
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return modelList;
        }
        /// <summary>
        /// Add record method
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        //

        public static QuizResponseModel add(CPBaseClass cp, int quizId )
        {
            QuizResponseModel response = create(cp, cp.Content.AddRecord(primaryContentName));
            response.QuizID = quizId;
            response.MemberID = cp.User.Id;
            response.dateStarted = DateTime.Now;
            return response;
        }
    }
}

