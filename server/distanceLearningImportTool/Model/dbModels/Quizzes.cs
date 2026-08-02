using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.dbModels {
    // 
    public class Quizzes : BasicRecord {
        // 
        // 
        // 
        public static int createRecord(CPBaseClass CP, string value) {
            int recordId = 0;
            try {
                // 
                var cs = CP.CSNew();
                if (!string.IsNullOrEmpty(value)) {
                    if (cs.Insert(common.cnQuizzes)) {
                        // 
                        recordId = cs.GetInteger("id");
                        cs.SetField("name", value);
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