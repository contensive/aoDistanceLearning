
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Models.Db;
using System.Linq;

namespace Contensive.Addons.DistanceLearning.Models {
    /// <summary>
    /// A response is a set of answers from one user to one quiz. The answer to each question is a ResponseDetailModel. 
    /// Quiz processing is based on having a complete set of response defaults, even if they do not have answers yet.
    /// </summary>
    public class QuizResponseModel : DbBaseModel {
        //
        public static  DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Quiz Responses", "quizResponses", "default", false);
        //
        public  int memberID { get; set; }
        public int quizID { get; set; }
        public int attemptNumber { get; set; }
        public DateTime dateSubmitted { get; set; }
        public int totalQuestions { get; set; }
        public int totalCorrect { get; set; }
        public int totalPoints { get; set; }
        public double score { get; set; }
        public DateTime dateStarted { get; set; }
        public int lastPageNumber { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// get a list of objects from this model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="QuizId"></param>
        /// <returns></returns>
        public static List<QuizResponseModel> getResponseList(CPBaseClass cp, int QuizId) {
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
        public static List<QuizResponseModel> getResponseList(CPBaseClass cp, int QuizId, int userId) {
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
        /// <summary>
        /// create a response model for a quiz and a user that might be be completed yet.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quizId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static QuizResponseModel createLastForThisUser(CPBaseClass cp, int quizId, int userId) {
            QuizResponseModel result = null;
            try {
                if ((result == null)) {
                    result = loadLastObject(cp, "(memberId=" + userId.ToString() + ")and(quizId=" + quizId.ToString() + ")");
                }
            } catch (Exception ex) {
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
        private static QuizResponseModel loadLastObject(CPBaseClass cp, string sqlCriteria) {
            try {
                List<QuizResponseModel> ListOfOne = createList<QuizResponseModel>(cp, sqlCriteria, "id desc", 1, 1);
                if(ListOfOne.Count.Equals(0)) { return null; }
                return ListOfOne.First();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }

    }
}

