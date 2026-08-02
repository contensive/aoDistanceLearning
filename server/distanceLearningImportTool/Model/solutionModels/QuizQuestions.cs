using System;
using System.Collections.Generic;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.solutionModels {
    // 
    public class QuizQuestions {
        // 
        public string value;
        public int answerPos;
        public string answerValue;
        // 
        public List<QuizAnswers> answers = new List<QuizAnswers>();
        // 
        public static QuizQuestions LoadQuestion(CPBaseClass CP, csvFileRow csvRowObject) {
            var Question = new QuizQuestions();
            try {
                // 
                // Add the question
                // 
                Question.value = csvRowObject.Question;
                Question.answerValue = csvRowObject.Answer;

                switch (csvRowObject.Answer.ToUpper() ?? "") {
                    case "A": {
                            Question.answerPos = 1;
                            break;
                        }
                    case "B": {
                            Question.answerPos = 2;
                            break;
                        }
                    case "C": {
                            Question.answerPos = 3;
                            break;
                        }
                    case "D": {
                            Question.answerPos = 4;
                            break;
                        }
                    case "E": {
                            Question.answerPos = 5;
                            break;
                        }
                    case "F": {
                            Question.answerPos = 6;
                            break;
                        }
                    case "G": {
                            Question.answerPos = 7;
                            break;
                        }
                }

                // 
                // Add the answers
                // 
                for (int i = 1, loopTo = csvRowObject.OptionCount; i <= loopTo; i++) {
                    var answer = new QuizAnswers();
                    answer.pos = i;
                    if (answer.pos == Question.answerPos) {
                        answer.correctAnswer = true;
                    }
                    // 
                    switch (i) {
                        case 1: {
                                answer.Value = csvRowObject.OptionA;
                                break;
                            }
                        case 2: {
                                answer.Value = csvRowObject.OptionB;
                                break;
                            }
                        case 3: {
                                answer.Value = csvRowObject.OptionC;
                                break;
                            }
                        case 4: {
                                answer.Value = csvRowObject.OptionD;
                                break;
                            }
                        case 5: {
                                answer.Value = csvRowObject.OptionE;
                                break;
                            }
                        case 6: {
                                answer.Value = csvRowObject.OptionF;
                                break;
                            }
                        case 7: {
                                answer.Value = csvRowObject.OptionG;
                                break;
                            }
                    }
                    // 
                    Question.answers.Add(answer);
                    // 
                }

                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in QuizQuestions.LoadQuestion");
            }
            return Question;
        }
        // 
    }
    // 
}