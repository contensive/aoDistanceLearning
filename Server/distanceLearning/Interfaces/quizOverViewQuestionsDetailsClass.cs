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
                    string qsBase;
                    string rqs = "";
                CPCSBaseClass cs = cp.CSNew();
                QuizQuestionModel question = QuizQuestionModel.create(cp, cp.Doc.GetInteger("QuizId"));

                string innerBody = "";

                    if (question == null)
                    {
                        return "";
                    }
                    else
                    {

                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalQuestionDetailsPageaddon, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", question.id.ToString(), true);
                    //
                    adminFramework.formNameValueRowsClass questionForm = new adminFramework.formNameValueRowsClass();
                    questionForm.isOuterContainer = false;
                    questionForm.addFormHidden("questionId", question.id.ToString());
                    questionForm.body = innerBody;
                    questionForm.addFormButton("Save", "button");
                    questionForm.addFormButton("Cancel", "button");
                        string button = cp.Doc.GetText("button");
                        switch (button)
                        {

                            case "Save":
                            
                                break;
                            case "Cancel":

                            return result; 
                        }

                        //qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnAddonguid, constants.quizOverViewSettingsAddon, true);
                        qsBase = cp.Doc.RefreshQueryString;
                        qsBase = cp.Utils.ModifyQueryString(qsBase, "setPortalId", "1", true);
                        qsBase = cp.Utils.ModifyQueryString(qsBase, "dstFeatureGuid", constants.portalStartPageAddon, true);
                         //adminFramework.formNameValueRowsClass questionForm = new adminFramework.formNameValueRowsClass();
                        qs = cp.Utils.ModifyQueryString(qsBase, "QuestionId", cs.GetInteger("responseId").ToString(), true);

                    questionForm.isOuterContainer = false;
                    questionForm.addRow();
                    questionForm.title = "<b>Question 1 </b></br>";
                    questionForm.addRow();
                    questionForm.rowName = "Start Page Text </b>";
                    questionForm.rowValue = cp.Html.InputText("Qtext", question.name,"5","",false,"qtext","js-qText");
                    questionForm.addRow();
                    questionForm.rowName = "Points* </b>";
                    questionForm.rowValue = cp.Html.InputText("points", question.points.ToString());                   
                    questionForm.addRow();
                    questionForm.rowName = "Answer 1 </b>";
                    questionForm.rowValue = cp.Html.InputText("answerOne", "", "1", "300", false, "answerOneClass", "js-answerOne")
                        + cp.Html.RadioBox("rbAnswerOne", "Correct Answer", "0", "answerOneRadioClass", "js-RbAnswerOne") + "Correct Answer";
                    questionForm.addRow();
                    questionForm.rowName = "Answer 2 </b>";
                    questionForm.rowValue = cp.Html.InputText("answerTwo", "", "1", "300", false, "answerTwoClass", "js-answerTwo")
                    + cp.Html.RadioBox("rbAnswerTwo", "Correct Answer", "0", "answerTwoRadioClass", "js-RbAnswerOne") + "Correct Answer";
                    questionForm.addRow();
                    questionForm.rowName = "Answer 3 </b>";
                    questionForm.rowValue = cp.Html.InputText("answerThree", "", "1", "300", false, "answerThreeClass", "js-answerThree") +
                    cp.Html.RadioBox("rbanswerThree", "Correct Answer", "0", "answerTHreeRadioClass", "js-RbAnswerTHree") + "Correct Answer";
                    questionForm.addRow();
                    questionForm.rowName = "Answer 4 </b>";
                    questionForm.rowValue = cp.Html.InputText("answerFour", "", "1", "300", false, "answerFourClass", "js-answerFour")
                        + cp.Html.RadioBox("rbAnswerFour", "Correct Answer", "0", "answerFourRadioClass", "js-RbAnswerFour") + "Correct Answer";
                    questionForm.addRow();
                    questionForm.rowName = "Section</b>";
                    questionForm.rowValue = cp.Html.SelectList("Subject", "Subject", "subjects","","questionDetailsClass", "js-questionDetails");
                    questionForm.addRow();                    
                    questionForm.rowName = "Question*</b>";
                    questionForm.rowValue = cp.Html.InputWysiwyg("questionInstructions","What is the capitol of montinegro?",CPHtmlBaseClass.EditorUserScope.CurrentUser, CPHtmlBaseClass.EditorContentScope.Page,"10","700");

                    //form.rowValue = ("<div><a href=\"?" + qs + quiz.id + "\">One question perpage: subjects; Users can retake quiz; max 5 questions</a></div>");
                    //
                    result = genericController.getTabWrapper(cp, questionForm.getHtml(cp), "Questions");

                        cp.Doc.AddHeadStyle(questionForm.styleSheet);
                    List<QuizQuestionModel> questionList = QuizQuestionModel.getQuestionsForQuizList(cp, question.id);
                    //
                    // the following is modifying a refresshquery string to add to the query model
                    qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnAddonguid, constants.quizOverViewSelectAddon, true);
                    //
                    // 
                    foreach (QuizQuestionModel questions in questionList)
                    {
                        //
                        //this next statement is like a cs.open but opens the object to get there field
                     



                    }
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



