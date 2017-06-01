﻿using System;
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

                string innerBody = "";

                if (question == null)
                {
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button)
                {

                    case constants.buttonSave:
                        question.QText = cp.Doc.GetText("Qtext");
                        question.points = cp.Doc.GetInteger("points");
                        question.instructions = cp.Doc.GetText("instructions");
                        question.saveObject(cp);
                        List<QuizAnswerModel> quizAnswersList = QuizAnswerModel.getAnswersForQuestionList(cp, question.id);
                        foreach (QuizAnswerModel quizAnswer in quizAnswersList)
                        {
                            quizAnswer.AText = cp.Doc.GetText("answer" + quizAnswer.id);
                            quizAnswer.Correct = cp.Doc.GetBoolean("rbAnswer" + quizAnswer.id);
                            quizAnswer.saveObject(cp);
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
                questionForm.body = innerBody;
                questionForm.addFormButton(constants.buttonSave, "button");
                questionForm.addFormButton(constants.buttonCancel, "button");
                questionForm.isOuterContainer = false;
                questionForm.addRow();
                questionForm.title = "<b>Question 1 </b></br>";
                questionForm.addRow();
                questionForm.rowName = "Question </b>";
                questionForm.rowValue = cp.Html.InputText("Qtext", question.QText, "5", "", false, "qtext", "js-qText");
                questionForm.addRow();
                questionForm.rowName = "Points* </b>";
                questionForm.rowValue = cp.Html.InputText("points", question.points.ToString());
                //
                //
               List<QuizAnswerModel> quizAnswerList = QuizAnswerModel.getAnswersForQuestionList(cp, question.id);
                foreach (QuizAnswerModel quizAnswer in quizAnswerList)
                {
                    questionForm.addRow();
                    questionForm.rowName = "Answer " + quizAnswer.id;
                    questionForm.rowValue = cp.Html.InputText("answer" + quizAnswer.id, quizAnswer.AText, "1", "300", false, "answerOneClass", "js-answerOne")
                        + cp.Html.CheckBox("rbAnswer" + quizAnswer.id, quizAnswer.Correct) + "Correct Answer";                  
                }

                questionForm.addRow();
                questionForm.rowName = "Subject</b>";
                questionForm.rowValue = cp.Html.SelectContent(constants.rnSubjectId , question.SubjectID.ToString(), constants.cnQuizSubjects,"","Select Subject");
                questionForm.addRow();
                questionForm.rowName = "Question*</b>";
                questionForm.rowValue = cp.Html.InputWysiwyg("instructions", question.instructions, CPHtmlBaseClass.EditorUserScope.CurrentUser, CPHtmlBaseClass.EditorContentScope.Page, "10", "700") +
                    "<p>This is a list of instructions that go on the Start page. You can describe the quiz, its purpose, how you take it, etc. </p>";
                //
                result = genericController.getTabWrapper(cp, questionForm.getHtml(cp), "Questions", question.quizId);

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


