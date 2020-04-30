using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.DistanceLearning.Models {
    public class QuizAnswerModel : DbBaseModel {
        //
        public static  DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Quiz Answers", "quizAnswers", "default", false);        //
        //
        // -- instance properties
        public string copy { get; set; }
        public bool correct { get; set; }
        public int questionID { get; set; }
        public int points { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// get a list of objects from this model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="someCriteria"></param>
        /// <returns></returns>
        public static List<QuizAnswerModel> getAnswersForQuestionList(CPBaseClass cp, int QuestionID) {
            List<QuizAnswerModel> result = new List<QuizAnswerModel>();
            try {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(tableMetadata.contentName, "(QuestionID=" + QuestionID + ")", "sortorder", true, "id"))) {
                    QuizAnswerModel instance = null;
                    do {
                        instance = DbBaseModel.create<QuizAnswerModel>(cp, cs.GetInteger("id"));
                        if ((instance != null)) {
                            result.Add(instance);
                        }
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}

