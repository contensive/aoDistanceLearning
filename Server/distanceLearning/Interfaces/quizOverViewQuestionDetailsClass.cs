using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
   public class quizOverViewQuestionsDetailsClass : Contensive.BaseClasses.AddonBaseClass

    {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            try
            {

                string qs;               
                CPCSBaseClass cs = cp.CSNew();
                QuizQuestionModel question = QuizQuestionModel.create(cp, cp.Doc.GetInteger( constants.rnQuestionId));
                QuizModel quiz = null;
                if (question == null)
                {
                    int quizId = cp.Doc.GetInteger(constants.rnQuizId);
                    quiz = QuizModel.create(cp, quizId );
                    if (quiz == null)
                    {
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureDashboard);
                        cp.Response.Redirect("?" + qs);
                    }
                    else
                    {
                        question = new QuizQuestionModel();
                        question.quizId = quiz.id;
                    }
                }
                if (quiz == null) quiz = QuizModel.create(cp, question.quizId);
                string innerBody = "";
                string button = cp.Doc.GetText("button");
                switch (button)
                {

                    case constants.buttonSave:
                        question.QText = cp.Doc.GetText("Qtext");
                        question.points = cp.Doc.GetInteger("points");
                        question.instructions = cp.Doc.GetText("instructions");
                        question.quizId = cp.Doc.GetInteger("quizId");
                        question.SubjectID = cp.Doc.GetInteger("SubjectId");
                        question.name= cp.Doc.GetText("Qtext");
                        question.qOrder = cp.Doc.GetInteger("qOrder");
                        question.SortOrder = cp.Doc.GetInteger("SortOrder");
                        question.saveObject(cp);
                        List<QuizAnswerModel> quizAnswersList = QuizAnswerModel.getAnswersForQuestionList(cp, question.id);
                        foreach (QuizAnswerModel quizAnswer in quizAnswersList)
                        {
                            quizAnswer.AText = cp.Doc.GetText("AText" + quizAnswer.id);
                            quizAnswer.Correct = cp.Doc.GetBoolean("Correct" + quizAnswer.id);
                            quizAnswer.saveObject(cp);
                        }

                        for (int ptr = 0; ptr <= constants.maxQuestionAnswer-1; ptr++)
                        {
                            string AText = cp.Doc.GetText(constants.rnAtextBlank + ptr.ToString() );
                            if (!string.IsNullOrEmpty(AText))
                            {
                                QuizAnswerModel newAnswer = new QuizAnswerModel();
                                newAnswer.AText = AText;
                                newAnswer.Correct = cp.Doc.GetBoolean(constants.rnCorrectBlank + ptr.ToString());
                                newAnswer.QuestionID = question.id;
                                newAnswer.saveObject(cp);
                            }
                        }
                            break;
                    case constants.buttonCancel:
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, constants.rnQuizId, question.quizId.ToString());
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewQuestions);
                        cp.Response.Redirect("?" + qs);
                        break;
                }

                //
                adminFramework.formNameValueRowsClass questionForm = new adminFramework.formNameValueRowsClass();
                questionForm.isOuterContainer = false;
                questionForm.addFormHidden("questionId", question.id.ToString());
                questionForm.addFormHidden(constants.rnQuizId, question.quizId.ToString());
                questionForm.body = innerBody;
                questionForm.addFormButton(constants.buttonSave, "button");
                questionForm.addFormButton(constants.buttonCancel, "button");
                questionForm.isOuterContainer = false;
                questionForm.addRow();
                questionForm.title = "<b>Question" + question.id + "  </b></br>";
                questionForm.addRow();
                questionForm.rowName = "Question </b>";
                questionForm.rowValue = cp.Html.InputText("Qtext", question.QText, "5", "", false, "qtext", "js-qText");
                questionForm.addRow();
                questionForm.rowName = "Sort Order </b>";
                questionForm.rowValue = cp.Html.InputText("Sort Order", question.SortOrder.ToString());
                questionForm.addRow();
                questionForm.rowName = "Points* </b>";
                questionForm.rowValue = cp.Html.InputText("points", question.points.ToString());
                //
                //
               List<QuizAnswerModel> quizAnswerList = QuizAnswerModel.getAnswersForQuestionList(cp, question.id);
                int descnt = 0;
                int cnt = constants.maxQuestionAnswer;
                foreach (QuizAnswerModel quizAnswer in quizAnswerList )
                {
                    descnt++;
                    questionForm.addRow();
                    questionForm.rowName = "Answer " + descnt;
                    questionForm.rowValue = cp.Html.InputText("Atext" + quizAnswer.id, quizAnswer.AText, "1", "300", false, "answerOneClass", "js-answerOne")
                        + "&nbsp;" + cp.Html.CheckBox("Correct" + quizAnswer.id, quizAnswer.Correct) + " Correct Answer";
                    cnt--;
                    
                }

                if (cnt > 0)
                {
                    for (int ptr=0; ptr < cnt; ptr++)
                    {
                        descnt++;
                        questionForm.addRow();
                        questionForm.rowName = "Answer " + descnt;
                        questionForm.rowValue = cp.Html.InputText(constants.rnAtextBlank + ptr, "", "1", "300", false, "answerOneClass", "js-answerOne")
                            + "&nbsp;" + cp.Html.CheckBox(constants.rnCorrectBlank + ptr, false) + " Correct Answer";
                    }
                }

                questionForm.addRow();
                questionForm.rowName = "Subject</b>";
                questionForm.rowValue = cp.Html.SelectContent(constants.rnSubjectId , question.SubjectID.ToString(), constants.cnQuizSubjects,"","Select Subject");
                questionForm.addRow();
                questionForm.rowName = "Question Instructions</b>";
                questionForm.rowValue = cp.Html.InputWysiwyg("instructions", question.instructions, CPHtmlBaseClass.EditorUserScope.CurrentUser, CPHtmlBaseClass.EditorContentScope.Page, "10", "700") +
                    "<p>This is a list of instructions that go on the Start page. You can describe the quiz, its purpose, how you take it, etc. </p>";
                //
                result = genericController.getTabWrapper(cp, questionForm.getHtml(cp), "Questions",  quiz );

                cp.Doc.AddHeadStyle(questionForm.styleSheet);
                List<QuizQuestionModel> questionList = QuizQuestionModel.getQuestionsForQuizList(cp, question.id);
                //
                // 
                foreach (QuizQuestionModel questions in questionList)
                {
                    //
                    //this next statement is like a cs.open but opens the object to get there field
                }
            }
            catch (Exception ex)
            {
                errorReport(cp, ex, "execute");
            }
            return result;


        }
        //
        // ===============================================================================
        // handle errors for this class
        // ===============================================================================
        //
        private void errorReport(CPBaseClass cp, Exception ex, string method)
        {
            cp.Site.ErrorReport(ex, "error in addonTemplateCs2005.blankClass.getForm");
        }
    }
}



