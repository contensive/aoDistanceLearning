
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;

namespace Contensive.Addons.DistanceLearning.Models {
    public class QuizResponseReportModel {
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
        //
        //====================================================================================================
        /// <summary>
        /// get the list of responses for the quiz overview reporting tab
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="QuizId"></param>
        /// <returns></returns>
        public static List<QuizResponseReportModel> getQuizOverviewResponseList(CPBaseClass cp, int QuizId, DateTime fromDate, DateTime toDate) {
            List<QuizResponseReportModel> modelList = new List<QuizResponseReportModel>();
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
                    QuizResponseReportModel instance = null;
                    do {
                        instance = new QuizResponseReportModel();
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
    }
}

