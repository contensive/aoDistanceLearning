using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.solutionModels {
    // 
    public class csvFileRow {
        // 
        public string Subject;
        // 
        public int rowNumber;
        // 
        public string Question;
        public string Answer;
        // 
        public int OptionCount;
        // 
        public string OptionA;
        public string OptionB;
        public string OptionC;
        public string OptionD;
        public string OptionE;
        public string OptionF;
        public string OptionG;
        // 
        public static csvFileRow GetRow(CPBaseClass CP, string Subject, string Answer, string Question, int OptionCount, int rowNumber, string OptionA, string OptionB = "", string OptionC = "", string OptionD = "", string OptionE = "", string OptionF = "", string OptionG = "") {
            // 
            var oCsvFileRow = new csvFileRow();
            // 
            try {
                // 
                // **************************************
                // Validate the number of Answer/Options
                // **************************************
                // 
                if (string.IsNullOrEmpty(OptionG) & OptionCount == 7) {
                    OptionCount = 6;
                }
                if (string.IsNullOrEmpty(OptionF) & OptionCount >= 6) {
                    OptionCount = 5;
                }
                if (string.IsNullOrEmpty(OptionE) & OptionCount >= 5) {
                    OptionCount = 4;
                }
                if (string.IsNullOrEmpty(OptionD) & OptionCount >= 4) {
                    OptionCount = 3;
                }
                if (string.IsNullOrEmpty(OptionC) & OptionCount >= 3) {
                    OptionCount = 2;
                }
                if (string.IsNullOrEmpty(OptionB) & OptionCount >= 2) {
                    OptionCount = 1;
                }
                if (string.IsNullOrEmpty(OptionA) & OptionCount >= 1) {
                    OptionCount = 0;
                }
                // 
                // **************************************
                // 
                oCsvFileRow.Subject = Subject;
                oCsvFileRow.Answer = Answer;
                oCsvFileRow.Question = Question;
                // 
                oCsvFileRow.OptionCount = OptionCount;
                oCsvFileRow.rowNumber = rowNumber;
                // 
                oCsvFileRow.OptionA = OptionA;
                oCsvFileRow.OptionB = OptionB;
                oCsvFileRow.OptionC = OptionC;
                oCsvFileRow.OptionD = OptionD;
                oCsvFileRow.OptionE = OptionE;
                oCsvFileRow.OptionF = OptionF;
                oCsvFileRow.OptionG = OptionG;
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in csvFileRow.GetRow");
            }
            // 
            return oCsvFileRow;
        }
        // 
    }
    // 
}