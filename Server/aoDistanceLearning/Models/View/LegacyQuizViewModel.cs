using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Controllers;
using static Contensive.Addons.DistanceLearning.Constants;
using Contensive.Models.Db;

namespace Models.View {
    public class LegacyQuizViewModel {
        // 
        public string headline { get; set; }
        public string description { get; set; }
        public List<QuestionViewModel> questions { get; set; }
        public List<string> subjects { get; set; }
        public string progressText { get; set; }
        public bool displaySubmitButton { get; set; }
        public bool displaySaveButton { get; set; }
        public bool displayContinueButton { get; set; }
        public bool displayPreviousButton { get; set; }
        public string formAction { get; set; }
        public string responseId { get; set; }
        public string answerCount { get; set; }
        public string lastPageNumber { get; set; }
        public string quizId { get; set; }
        public string quizName { get; set; }
        public string adminHint { get; set; }
        // 
        // ====================================================================================================
        /// <summary>
        /// Populate the view model from the entity model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quiz"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static LegacyQuizViewModel create(CPBaseClass cp, QuizModel quiz, QuizResponseModel response, string adminHint) {
            try {
                // 
                // -- base fields
                var result = new LegacyQuizViewModel();
                result.headline = "";
                result.description = "";
                result.adminHint = adminHint;
                // 
                // -- custom
                // 
                result.questions = new List<QuestionViewModel>();
                int answerCnt = 0;

                try {
                    string q;
                    string buttonCopy = "";
                    double quizProgress;
                    string quizProgressText;
                    string jsHead;

                    // Dim questionSubjectId As Integer
                    // Dim subjectName As String = ""
                    if (GenericController.isDateEmpty(response.dateStarted)) {
                        response.dateStarted = DateTime.Now;
                        response.save(cp);
                    }

                    // 
                    // -- progress bar for all but first page
                    List<QuizResponseDetailModel> responseDetailList = QuizResponseDetailModel.getObjectListForQuizDisplay(cp, response.id);
                    quizProgress = 0;
                    if (responseDetailList.Count > 0) {
                        int answeredCount = 0;
                        foreach (QuizResponseDetailModel responseDetail in responseDetailList) {
                            if (responseDetail.answerId > 0)
                                answeredCount += 1;
                        }
                        quizProgress = answeredCount / (double)responseDetailList.Count;
                        result.progressText = System.Convert.ToString(Conversion.Int(quizProgress * 100));
                        jsHead = "$(document).ready(function(){$(\"#progressbar\").progressbar({value:" + result.progressText + "});});";
                        cp.Doc.AddHeadJavascript(jsHead);
                    }

                    if (quiz.allowCustomButtonCopy)
                        buttonCopy = quiz.customButtonCopy;
                    else if (cp.User.IsAuthenticated)
                        buttonCopy = defaultButtonWithSaveCopy;
                    else
                        buttonCopy = defaultButtonCopy;

                    int questionCnt = 0;
                    int lastSubjectId = -1;

                    foreach (QuizResponseDetailModel responseDetail in responseDetailList) {

                        QuestionViewModel questionModel = new QuestionViewModel();
                        questionModel.answers = new List<AnswerViewModel>();

                        if (responseDetail.pageNumber == response.lastPageNumber) {
                            QuizQuestionModel question = DbBaseModel.create<QuizQuestionModel>(cp, responseDetail.questionId);
                            QuizSubjectModel subject = DbBaseModel.create<QuizSubjectModel>(cp, question.subjectID);
                            if (subject == null)
                                subject = new QuizSubjectModel();

                            answerCnt = 0;
                            answerCnt += 1;
                            // -- add subject header if required
                            if ((subject.id > 0) & (subject.id != lastSubjectId) & (!string.IsNullOrEmpty(subject.name))) {
                                lastSubjectId = subject.id;
                                result.subjects.Add(subject.name);
                            }

                            q = question.copy;

                            if (cp.User.IsEditingAnything) {
                                q = GenericController.addEditWrapper(cp, q, question.id, question.name, "Quiz Questions");
                            }

                            questionModel.questionText = q;

                            List<QuizAnswerModel> answerList = QuizAnswerModel.getAnswersForQuestionList(cp, question.id);

                            foreach (QuizAnswerModel answer in answerList) {
                                string answerCopy = answer.copy;
                                bool isChecked = false;
                                if (answer.id == responseDetail.answerId) {
                                    isChecked = true;
                                }

                                if (cp.User.IsEditingAnything) {
                                    answerCopy = GenericController.addEditWrapper(cp, answerCopy, answer.id, answer.name, "Quiz Answers");
                                }
                                questionModel.answers.Add(new AnswerViewModel { answerText = answerCopy, isChecked = isChecked, isAnswerLink = false, answerId = answer.id.ToString(), answerName = "q" + question.id.ToString() + "a" });

                                answerCnt = answerCnt + 1;
                            }

                            if (cp.User.IsEditingAnything) {
                                questionModel.answers.Add(new AnswerViewModel {
                                    answerText = cp.Content.GetAddLink("Quiz Answers", "questionid=" + question.id),
                                    isChecked = false,
                                    isAnswerLink = true
                                });
                            }

                            questionCnt = questionCnt + 1;
                        }

                        result.questions.Add(questionModel);
                    }

                    if (cp.User.IsEditingAnything) {
                        result.questions.Add(new QuestionViewModel { questionText = cp.Content.GetAddLink("Quiz Questions", "quizid=" + quiz.id + ", pageOrder=" + rnDstPageOrder), answers = null, isQuestionLink = true });
                    }

                    bool isFirstPage = (response.lastPageNumber < 2);
                    bool isLastPage = true;

                    if (responseDetailList.Count > 0) {
                        isLastPage = (response.lastPageNumber >= responseDetailList[responseDetailList.Count - 1].pageNumber);
                    }

                    if ((!isFirstPage) | quiz.includeStudyPage) {
                        result.displayPreviousButton = true;
                    } else {
                        result.displayPreviousButton = false;
                    }

                    if (cp.User.IsAuthenticated) {
                        result.displaySaveButton = true;
                    } else {
                        result.displaySaveButton = false;
                    }

                    if (!isLastPage) {
                        result.displayContinueButton = true;
                        result.displaySubmitButton = false;
                    } else {
                        result.displaySubmitButton = true;
                        result.displayContinueButton = false;
                    }
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex);
                }

                result.quizId = quiz.id.ToString();
                result.quizName = quiz.name;
                result.responseId = response.id.ToString();
                result.answerCount = answerCnt.ToString();
                string qs = cp.Doc.RefreshQueryString;
                result.formAction = "?" + qs;

                return result;

            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return null;
            }
        }

        public class QuestionViewModel {
            public string questionText;
            public List<AnswerViewModel> answers;
            public bool isQuestionLink;
        }

        public class AnswerViewModel {
            public string answerText { get; set; }
            public string answerId;
            public string answerName;
            public bool isChecked { get; set; }
            public bool isAnswerLink;
        }
    }
}
