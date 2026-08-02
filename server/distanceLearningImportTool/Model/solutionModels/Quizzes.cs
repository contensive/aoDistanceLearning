using System;
using System.Collections.Generic;
using System.IO;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.solutionModels {
    // 
    public class Quizzes {
        // 
        public string value;
        public List<QuizSubjects> subjects = new List<QuizSubjects>();
        // 
        // 
        // 
        public static Quizzes GetQuizzesObject(CPBaseClass CP, string csv_file_path, List<csvFileRow> csvObjectlist, ref List<architectureModels.errorClass> errorList) {
            var oQuizzes = new Quizzes();
            try {
                string fileName = Path.GetFileName(csv_file_path);
                string @extension = Path.GetExtension(fileName);
                oQuizzes.value = fileName.Replace(@extension, "").Trim();
                oQuizzes.subjects = QuizSubjects.LoadCsvObject(CP, csvObjectlist);

            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Quizzes.GetQuizzesObject");
            }
            return oQuizzes;
        }
        // 
        // 
        // 
        public static bool CreateDBRecords(CPBaseClass CP, Quizzes Quiz) {
            bool result = false;
            try {

                int QuizId = 0;
                int QuizSubjectId = 0;
                int QuizQuestionId = 0;
                int QuizAnswerId = 0;

                // 
                // Create Quiz Record
                // 
                QuizId = dbModels.Quizzes.createRecord(CP, Quiz.value);

                foreach (var Subject in Quiz.subjects) {
                    // 
                    // Create Quiz Subjects Records
                    // 
                    QuizSubjectId = dbModels.QuizSubjects.createRecord(CP, Subject.value, QuizId);
                    // 
                    foreach (var Question in Subject.questions) {
                        // 
                        // Create Quiz Questions Record
                        // 
                        QuizQuestionId = dbModels.QuizQuestions.createRecord(CP, Question.value, QuizId, QuizSubjectId);
                        // 
                        foreach (var Answer in Question.answers)
                            // 
                            // Create Quiz Answers Record
                            // 
                            // 
                            QuizAnswerId = dbModels.QuizAnswers.createRecord(CP, Answer.Value, QuizQuestionId, Answer.correctAnswer);
                        // 
                    }
                    // 
                }
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Quizzes.CreateDBRecords");
            }
            return result;
        }
        // 
        // 
        // 
    }
    // 
}