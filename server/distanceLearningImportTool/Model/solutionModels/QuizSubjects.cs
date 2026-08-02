using System;
using System.Collections.Generic;
using System.Linq;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.solutionModels {
    // 
    public class QuizSubjects {
        // 
        public string value;
        public List<QuizQuestions> questions = new List<QuizQuestions>();
        // 
        // 
        // 
        public static List<QuizSubjects> LoadCsvObject(CPBaseClass CP, List<csvFileRow> CSVRowListObject) {
            var oQuizSubjectsList = new List<QuizSubjects>();
            try {
                // 
                bool isNewSubject = false;
                foreach (var csvRowObject in CSVRowListObject) {
                    // 
                    isNewSubject = false;
                    var Subject = oQuizSubjectsList.FirstOrDefault(myObject => (myObject.value ?? "") == (csvRowObject.Subject ?? ""));
                    if (Subject is null) {
                        // Subject not exit
                        Subject = new QuizSubjects();
                        Subject.value = csvRowObject.Subject;
                        isNewSubject = true;
                    }
                    // 
                    Subject.questions.Add(QuizQuestions.LoadQuestion(CP, csvRowObject));
                    // 
                    if (isNewSubject) {
                        oQuizSubjectsList.Add(Subject);
                    }
                    // 
                }
                // 
            } catch (Exception) {

            }
            return oQuizSubjectsList;
        }
        // 
        // 
        // 
    }
    // 
}