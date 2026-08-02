using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.dbModels {
    // 
    public class QuizSubjects : BasicRecord {
        // 
        // 
        // 
        public static int createRecord(CPBaseClass CP, string value, int quizId) {
            int recordId = 0;
            try {
                // 
                var cs = CP.CSNew();
                if (!string.IsNullOrEmpty(value)) {
                    if (cs.Insert(common.cnQuizSubjects)) {
                        // 
                        recordId = cs.GetInteger("id");
                        cs.SetField("name", value);
                        cs.SetField("quizId", quizId);
                        // 
                    }
                    cs.Close();
                }
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Quizzes.createRecord");
            }
            return recordId;
        }
        // 
        // 
        // 
    }
    // 
}