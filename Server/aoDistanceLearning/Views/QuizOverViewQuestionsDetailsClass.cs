using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning {
    namespace Views {
        /// <summary>
        /// addon
        /// </summary>
        public class QuizOverViewQuestionsDetailsClass : AddonBaseClass {
            public override object Execute(CPBaseClass cp) {
                string result = "";
                try {

                    string qs;
                    CPCSBaseClass cs = cp.CSNew();
                    QuizQuestionModel question = DbBaseModel.create<QuizQuestionModel>(cp, cp.Doc.GetInteger(Constants.rnQuestionId));
                    QuizModel quiz = null;
                    if (question == null) {
                        int quizId = cp.Doc.GetInteger(Constants.rnQuizId);
                        quiz = DbBaseModel.create<QuizModel>(cp, quizId);
                        if (quiz == null) {
                            qs = cp.Doc.RefreshQueryString;
                            qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureDashboard);
                            cp.Response.Redirect("?" + qs);
                        } else {
                            question = new QuizQuestionModel();
                            question.quizId = quiz.id;
                        }
                    }
                    if (quiz == null) quiz = DbBaseModel.create<QuizModel>(cp, question.quizId);
                    string button = cp.Doc.GetText("button");
                    switch (button) {
                        case Constants.buttonSave:
                            question.copy = cp.Doc.GetText("copy");
                            question.points = cp.Doc.GetInteger("points");
                            question.instructions = cp.Doc.GetText("instructions");
                            question.quizId = cp.Doc.GetInteger("quizId");
                            question.subjectID = cp.Doc.GetInteger("SubjectId");
                            question.sortOrder = cp.Doc.GetText("SortOrder");
                            question.save(cp);
                            List<QuizAnswerModel> quizAnswersList = QuizAnswerModel.getAnswersForQuestionList(cp, question.id);
                            foreach (QuizAnswerModel quizAnswer in quizAnswersList) {
                                quizAnswer.copy = cp.Doc.GetText(Constants.rnAnswerCopy + quizAnswer.id);
                                quizAnswer.correct = cp.Doc.GetBoolean("Correct" + quizAnswer.id);
                                quizAnswer.save(cp);
                                qs = cp.Doc.RefreshQueryString;
                                qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, question.quizId.ToString());
                                qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionList);
                                cp.Response.Redirect("?" + qs);
                            }

                            for (int ptr = 0; ptr <= Constants.maxQuestionAnswer - 1; ptr++) {
                                string answerCopy = cp.Doc.GetText(Constants.rnAnswerCopyBlank + ptr.ToString());
                                if (!string.IsNullOrEmpty(answerCopy)) {
                                    QuizAnswerModel newAnswer = new QuizAnswerModel();
                                    newAnswer.copy = answerCopy;
                                    newAnswer.correct = cp.Doc.GetBoolean(Constants.rnCorrectBlank + ptr.ToString());
                                    newAnswer.questionID = question.id;
                                    newAnswer.save(cp);
                                }
                            }
                            qs = cp.Doc.RefreshQueryString;
                            qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, question.quizId.ToString());
                            qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionList);
                            cp.Response.Redirect("?" + qs);
                            break;
                        case Constants.buttonCancel:
                            qs = cp.Doc.RefreshQueryString;
                            qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, question.quizId.ToString());
                            qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionList);
                            cp.Response.Redirect("?" + qs);
                            break;
                    }

                    //
                    adminFramework.formNameValueRowsClass questionForm = new adminFramework.formNameValueRowsClass();
                    questionForm.isOuterContainer = false;
                    questionForm.addFormHidden("questionId", question.id.ToString());
                    questionForm.addFormHidden(Constants.rnQuizId, question.quizId.ToString());
                    //questionForm.body = innerBody;
                    questionForm.addFormButton(Constants.buttonSave);
                    questionForm.addFormButton(Constants.buttonCancel);
                    questionForm.isOuterContainer = false;
                    //questionForm.addRow();
                    if (question.id == 0) {
                        questionForm.title = "<b>New Question</b></br>";
                    } else {
                        questionForm.title = "<b>Question " + question.id + "  </b></br>";
                    }
                    questionForm.addRow();
                    questionForm.rowName = "Question </b>";
                    questionForm.rowValue = cp.Html.InputText(Constants.rnQuestionCopy, question.copy, "4", "", false, "qtext", "js-qText");
                    questionForm.rowHelp = "The question as it appears on the quiz.";
                    //questionForm.addRow();
                    //questionForm.rowName = "Points* </b>";
                    //questionForm.rowValue = cp.Html.InputText("points", question.points.ToString());
                    //
                    //
                    List<QuizAnswerModel> quizAnswerList = QuizAnswerModel.getAnswersForQuestionList(cp, question.id);
                    int descnt = 0;
                    int cnt = Constants.maxQuestionAnswer;
                    foreach (QuizAnswerModel quizAnswer in quizAnswerList) {
                        descnt++;
                        questionForm.addRow();
                        questionForm.rowName = "Answer " + descnt;
                        questionForm.rowValue = cp.Html.InputText(Constants.rnAnswerCopy + quizAnswer.id, quizAnswer.copy, "2", "", false, "answerOneClass", "js-answerOne")
                            + "<br>" + cp.Html.CheckBox("Correct" + quizAnswer.id, quizAnswer.correct) + " Correct Answer";
                        cnt--;

                    }

                    if (cnt > 0) {
                        for (int ptr = 0; ptr < cnt; ptr++) {
                            descnt++;
                            questionForm.addRow();
                            questionForm.rowName = "Answer " + descnt;
                            questionForm.rowValue = cp.Html.InputText(Constants.rnAnswerCopyBlank + ptr, "", "2", "", false, "answerOneClass", "js-answerOne")
                                + "<br>" + cp.Html.CheckBox(Constants.rnCorrectBlank + ptr, false) + " Correct Answer";
                        }
                    }
                    questionForm.rowHelp = "";
                    questionForm.addRow();
                    questionForm.rowName = "Subject</b>";
                    questionForm.rowValue = cp.Html.SelectContent(Constants.rnSubjectId, question.subjectID.ToString(), Constants.cnQuizSubjects, "(quizid=" + quiz.id.ToString() + ")", "Select Subject");
                    questionForm.rowHelp = "Add a subject for this question. to add subjects, use the quiz details form.";
                    questionForm.addRow();
                    questionForm.rowName = "Sort Order </b>";
                    questionForm.rowValue = cp.Html.InputText("SortOrder", question.sortOrder.ToString());
                    questionForm.rowHelp = "Use this alphanumeric text field to order your questions. ";
                    questionForm.addRow();
                    questionForm.rowName = "Answer Information</b>";
                    questionForm.rowValue = cp.Html.InputWysiwyg("instructions", question.instructions, CPHtmlBaseClass.EditorUserScope.CurrentUser, CPHtmlBaseClass.EditorContentScope.Page);
                    questionForm.rowHelp = "<p>This is a list of instructions that go on the Start page. You can describe the quiz, its purpose, how you take it, etc. </p>";
                    //
                    result = GenericController.getTabWrapper(cp, questionForm.getHtml(cp), "Questions", quiz);

                    cp.Doc.AddHeadStyle(questionForm.styleSheet);
                    List<QuizQuestionModel> questionList = QuizQuestionModel.getQuestionsForQuizList(cp, question.id);
                    //
                    // 
                    foreach (QuizQuestionModel questions in questionList) {
                        //
                        //this next statement is like a cs.open but opens the object to get there field
                    }
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "execute");
                }
                return result;


            }
            //
            // ===============================================================================
            // handle errors for this class
            // ===============================================================================
            //
            private void errorReport(CPBaseClass cp, Exception ex, string method) {
                cp.Site.ErrorReport(ex, "error in addonTemplateCs2005.blankClass.getForm");
            }
        }
    }
}