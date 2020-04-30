
using System;
using System.Collections.Generic;
using System.Linq;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Models {
    public class QuizResultMessageModel : DbBaseModel {
        //
        public static  DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Quiz Result Messages", "quizResultMessages", "default", false);
        //
        // -- instance properties
        public int quizID;
        public int pointThreshold;
        public string copy;
        //
        //====================================================================================================
        /// <summary>
        /// return a quiz result message for the point threshold.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId">The id of the record to be read into the new object</param>
        public static QuizResultMessageModel createByPointThreshold(CPBaseClass cp, int pointThreshold) {
            try {
                var ListOfOne = createList<QuizResultMessageModel>(cp, "(pointThreshold <= " + pointThreshold + ")","id",1,1);
                if (ListOfOne.Count.Equals(0)) { return null; }
                return ListOfOne.First();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}

