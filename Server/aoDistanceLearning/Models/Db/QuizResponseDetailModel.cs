
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Models {
    public class QuizResponseDetailModel : DbBaseModel {
        //
        public static  DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Quiz Response Details", "quizResponseDetails", "default", false);
        //
        // -- instance properties
        public int responseId { get; set; }
        public int questionId { get; set; }
        public int answerId { get; set; }
        public int pageNumber { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// get a list of objects from this model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="someCriteria"></param>
        /// <returns></returns>
        public static List<QuizResponseDetailModel> getObjectListForQuizDisplay(CPBaseClass cp, int responseId) {
            List<QuizResponseDetailModel> result = new List<QuizResponseDetailModel>();
            try {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(tableMetadata.contentName, "(responseId=" + responseId + ")", "pageNumber,sortOrder", true, "id"))) {
                    QuizResponseDetailModel instance = null;
                    do {
                        instance = DbBaseModel.create<QuizResponseDetailModel>(cp, cs.GetInteger("id"));
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
        /// <summary>
        /// get list of completed response details
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="someCriteria"></param>
        /// <returns></returns>
        public static List<QuizResponseDetailModel> getAnsweredObjectList(CPBaseClass cp, int responseId) {
            List<QuizResponseDetailModel> result = new List<QuizResponseDetailModel>();
            try {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(tableMetadata.contentName, "(answerId is null)or(answerId=0)", "name", true, "id"))) {
                    QuizResponseDetailModel instance = null;
                    do {
                        instance = DbBaseModel.create<QuizResponseDetailModel>(cp, cs.GetInteger("id"));
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

