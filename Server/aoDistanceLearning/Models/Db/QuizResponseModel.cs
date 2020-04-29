
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Models {
    public class QuizResponseModel : DbBaseModel {
        //
        public static readonly DbBaseTableMetadataModel tableMetadata = new DbBaseTableMetadataModel("Quiz Responses", "quizResponses", "default", false);
        //
        public int MemberID;
        public int QuizID;
        public int attemptNumber;
        public DateTime dateSubmitted;
        public int totalQuestions;
        public int totalCorrect;
        public int totalPoints;
        public double Score;
        public DateTime dateStarted;
        public string SortOrder;
        public int lastPageNumber;

        //
        //====================================================================================================
        /// <summary>
        /// get a list of objects from this model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="QuizId"></param>
        /// <returns></returns>
        public static List<QuizResponseModel> GetResponseList(CPBaseClass cp, int QuizId) {
            List<QuizResponseModel> modelList = new List<QuizResponseModel>();
            try {
                CPCSBaseClass cs = cp.CSNew();
                if ((cs.Open( tableMetadata.contentName, "(QuizId=" + QuizId + ")", "name", true, "id"))) {
                    QuizResponseModel instance = null;
                    do {
                        instance = DbBaseModel.create<QuizResponseModel>(cp, cs.GetInteger("id"));
                        if ((instance != null)) {
                            modelList.Add(instance);
                        }
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            } catch (Exception ex) {
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
        public static List<QuizResponseModel> GetResponseList(CPBaseClass cp, int QuizId, int userId) {
            List<QuizResponseModel> modelList = new List<QuizResponseModel>();
            try {
                CPCSBaseClass cs = cp.CSNew();
                if ((cs.Open(tableMetadata.contentName, "(QuizId=" + QuizId + ")and(memberId=" + cp.User.Id + ")", "name", true, "id"))) {
                    QuizResponseModel instance = null;
                    do {
                        instance = DbBaseModel.create<QuizResponseModel>(cp, cs.GetInteger("id"));
                        if ((instance != null)) {
                            modelList.Add(instance);
                        }
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return modelList;
        }
        public class quizResponseReportModel {
            public string quizName;
            public string userName;
            public string userFirstName;
            public string userLastName;
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
        public static List<quizResponseReportModel> GetQuizOverviewResponseList(CPBaseClass cp, int QuizId, DateTime fromDate, DateTime toDate) {
            List<quizResponseReportModel> modelList = new List<quizResponseReportModel>();
            try {
                string sql = "select q.name as quizName, u.firstName as userFirstName, u.lastName as userLastName, u.name as userName, r.id as responseId,r.dateSubmitted, r.attemptNumber, r.score, r.totalQuestions, r.totalCorrect, r.totalPoints"
                    + " from ((quizResponses r"
                    + " left join quizzes q on q.id=r.quizId)"
                    + " left join ccMembers u on u.id=r.memberId)"
                    + " where (r.QuizId=" + QuizId + ")and(q.active>0)and(u.active>0)and(r.dateSubmitted is not null)";
                if (fromDate > DateTime.MinValue) {
                    string sqlFromDate = cp.Db.EncodeSQLDate(fromDate.Date);
                    string sqlFromNextDate = cp.Db.EncodeSQLDate(fromDate.AddDays(1).Date);
                    sql += "and(r.dateSubmitted>=" + sqlFromDate + ")";
                }
                if (toDate > DateTime.MinValue) {
                    string sqlToDate = cp.Db.EncodeSQLDate(toDate.Date);
                    string sqlToNextDate = cp.Db.EncodeSQLDate(toDate.AddDays(1).Date);
                    sql += "and(r.dateSubmitted<" + sqlToNextDate + ")";
                }
                CPCSBaseClass cs = cp.CSNew();
                if (cs.OpenSQL(sql)) {
                    quizResponseReportModel instance = null;
                    do {
                        instance = new quizResponseReportModel();
                        instance.id = cs.GetInteger("responseId");
                        instance.attemptNumber = cs.GetInteger("attemptNumber");
                        instance.dateSubmitted = cs.GetDate("dateSubmitted");
                        instance.quizName = cs.GetText("quizName");
                        instance.userName = cs.GetText("userName");
                        instance.userFirstName = cs.GetText("userFirstName");
                        instance.userLastName = cs.GetText("userLastName");
                        instance.score = cs.GetNumber("score");
                        instance.totalQuestions = cs.GetInteger("totalQuestions");
                        instance.totalCorrect = cs.GetInteger("totalCorrect");
                        instance.totalPoints = cs.GetInteger("totalPoints");
                        modelList.Add(instance);
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            } catch (Exception ex) {
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

        public static QuizResponseModel add(CPBaseClass cp, int quizId) {
            QuizResponseModel instance = DbBaseModel.create<QuizResponseModel>(cp, cp.Content.AddRecord(tableMetadata.contentName));
            instance.QuizID = quizId;
            instance.MemberID = cp.User.Id;
            instance.SortOrder = GenericController.getSortOrderFromInteger(instance.id);
            return instance;
        }
    }
}

