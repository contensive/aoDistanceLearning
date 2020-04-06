using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Controllers;
using static Contensive.Addons.DistanceLearning.Constants;

namespace Models.View {
    public class QuizViewModel : ViewBaseModel {
        // 
        public string headline { get; set; }
        public string description { get; set; }
        public List<string> questions { get; set; }
        public List<AnswerViewModel> answers { get; set; }
        public List<string> subjects { get; set; }
        public string progressText { get; set; }
        public bool displaySubmitButton { get; set; }
        public bool displaySaveButton { get; set; }
        public bool displayContinueButton { get; set; }
        public bool displayPreviousButton { get; set; }
        // 
        // ====================================================================================================
        /// <summary>
        ///         ''' Populate the view model from the entity model
        ///         ''' </summary>
        ///         ''' <param name="cp"></param>
        ///         ''' <param name="settings"></param>
        ///         ''' <returns></returns>
        public static QuizViewModel create(CPBaseClass cp, QuizModel quiz, QuizResponseModel response, ref string adminHint, ref List<string> userMessages) {
            try {
                // 
                // -- base fields
                var result = ViewBaseModel.create<QuizViewModel>(cp, quiz);
                // 
                // -- custom
                // 
                result.headline = quiz.headline;
                result.description = quiz.description;
                result.questions = new List<string>();
                result.answers = new List<AnswerViewModel>();

                try {
                    int answerCnt = 0;
                    string q;
                    string buttonCopy = "";
                    double quizProgress;
                    string jsHead;
                    // Dim questionSubjectId As Integer
                    // Dim subjectName As String = ""
                    if (GenericController.isDateEmpty(response.dateStarted)) {
                        response.dateStarted = DateTime.Now;
                        response.saveObject(cp);
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
                        if (responseDetail.pageNumber == response.lastPageNumber) {
                            QuizQuestionModel question = QuizQuestionModel.create(cp, responseDetail.questionId);
                            QuizSubjectModel subject = QuizSubjectModel.create(cp, question.SubjectID);
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

                            result.questions.Add(q);

                            List<QuizAnswerModel> answerList = QuizAnswerModel.getAnswersForQuestionList(cp, question.id);

                            foreach (QuizAnswerModel answer in answerList)
                            {
                                string answerCopy = answer.copy;
                                bool isChecked = false;
                                if (answer.id == responseDetail.answerId) {
                                    isChecked = true;
                                }

                                if (cp.User.IsEditingAnything) {
                                    answerCopy = GenericController.addEditWrapper(cp, answerCopy, answer.id, answer.name, "Quiz Answers");
                                }
                                result.answers.Add(new AnswerViewModel { answerText = answerCopy, isChecked = isChecked });
                                answerCnt = answerCnt + 1;
                            }

                            if (cp.User.IsEditingAnything) {
                                result.answers.Add(new AnswerViewModel { answerText = cp.Content.GetAddLink("Quiz Answers", "questionid=" + question.id),
                                    isChecked = false });
                            }

                            questionCnt = questionCnt + 1;
                        }
                    }

                    if (cp.User.IsEditingAnything) {
                        result.questions.Add(cp.Content.GetAddLink("Quiz Questions", "quizid=" + quiz.id + ", pageOrder=" + rnDstPageOrder));
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
                }
                catch (Exception ex)
                {
                    cp.Site.ErrorReport(ex);
                }
                return result;
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                return null;
            }
        }

        public class AnswerViewModel {
            public string answerText { get; set; }
            public bool isChecked { get; set; }
        }
    }
}
