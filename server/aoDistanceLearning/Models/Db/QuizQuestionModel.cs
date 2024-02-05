
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Models {
    public class QuizQuestionModel : DbBaseModel {
        //
        public static  DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Quiz Questions", "quizQuestions", "default", false);        //
        //
        // -- instance properties
        public int subjectID { get; set; }
        public int quizId { get; set; }
        public int points { get; set; }
        public string instructions { get; set; }
        public string copy { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// get a list of questions, sorted by sortOrder
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public static List<QuizQuestionModel> getQuestionsForQuizList(CPBaseClass cp, int quizId) {
            List<QuizQuestionModel> result = new List<QuizQuestionModel>();
            try {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(tableMetadata.contentName, "(quizid=" + quizId + ")", "SortOrder", true, "id"))) {
                    QuizQuestionModel instance = null;
                    do {
                        instance = DbBaseModel.create<QuizQuestionModel>(cp, cs.GetInteger("id"));
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

