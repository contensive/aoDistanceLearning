using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.dbModels {
    // 
    public class QuizQuestions : BasicRecord {
        // 
        // 
        // 
        public static int createRecord(CPBaseClass CP, string value, int quizId, int subjectId) {
            int recordId = 0;
            try {
                // 
                var cs = CP.CSNew();
                if (!string.IsNullOrEmpty(value)) {
                    if (cs.Insert(common.cnQuizQuestions)) {
                        // 
                        recordId = cs.GetInteger("id");
                        cs.SetField("name", value);
                        cs.SetField("copy", value);
                        cs.SetField("quizId", quizId);
                        // 
                        if (subjectId != 0) {
                            cs.SetField("subjectId", subjectId);
                        }
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