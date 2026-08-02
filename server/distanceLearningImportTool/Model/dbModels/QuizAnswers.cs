using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.dbModels {
    // 
    public class QuizAnswers : BasicRecord {
        // 
        // 
        // 
        public static int createRecord(CPBaseClass CP, string value, int questionId, bool correct) {
            int recordId = 0;
            try {
                // 
                var cs = CP.CSNew();
                if (!string.IsNullOrEmpty(value)) {
                    if (cs.Insert(common.cnQuizAnswers)) {
                        // 
                        recordId = cs.GetInteger("id");
                        cs.SetField("name", value);
                        cs.SetField("copy", value);
                        cs.SetField("questionId", questionId);
                        cs.SetField("correct", correct);
                        // 
                        cs.SetField("SortOrder", recordId.ToString("D10"));
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