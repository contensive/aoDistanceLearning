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
                    QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger("QuizId"));
                    string quizName = cp.Doc.GetText("quizName");
                    string customTopCopy = cp.Doc.GetText("customTopCopy");
                    string Video = cp.Doc.GetText("Video");
                    string customButtonCopy = cp.Doc.GetText("customButtonCopy");
                    string courseMaterial = cp.Doc.GetText("CorseMaterial");
                    string innerBody = "";

                    if (quiz == null)
                    {
                        return "";
                    }
                    else
                    {

                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalQuestionDetailsPageaddon, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        //
                        adminFramework.formNameValueRowsClass form = new adminFramework.formNameValueRowsClass();
                        form.isOuterContainer = false;
                        form.addFormHidden("quizId", quiz.id.ToString());
                        form.body = innerBody;
                        form.addFormButton("Save", "button");
                        form.addFormButton("Cancel", "button");
                        string button = cp.Doc.GetText("button");
                        switch (button)
                        {

                            case "Save":
                                quiz.customTopCopy = cp.Doc.GetText("customTopCopy");
                                quiz.Video = cp.Doc.GetText("Video");
                                quiz.courseMaterial = cp.Doc.GetText("CorseMaterial");
                                quiz.customButtonCopy = cp.Doc.GetText("customButtonCopy");
                                quiz.saveObject(cp);
                                cp.Response.Redirect("?" + qs);
                                break;
                            case "Cancel":
                                return "?" + cp.Utils.ModifyQueryString(cp.Doc.RefreshQueryString, "addonID", constants.dashBoardAddon);
                        }

                        //qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnAddonguid, constants.quizOverViewSettingsAddon, true);
                        qsBase = cp.Doc.RefreshQueryString;
                        qsBase = cp.Utils.ModifyQueryString(qsBase, "setPortalId", "1", true);
                        qsBase = cp.Utils.ModifyQueryString(qsBase, "dstFeatureGuid", constants.portalStartPageAddon, true);
                        // adminFramework.formNameValueRowsClass form = new adminFramework.formNameValueRowsClass();
                        qs = cp.Utils.ModifyQueryString(qsBase, "QuizId", cs.GetInteger("responseId").ToString(), true);

                        form.isOuterContainer = false;
                        form.addRow();
                        form.title = "<b>Question # 1 </b></br>";

                        form.addRow();
                        form.rowName = "Start Page Text </b>";
                        form.rowValue = cp.Html.InputTextExpandable("customTopCopy", quiz.customTopCopy)
                         + "This is the list of instructions that go on the Start Page. You can describe the quiz, it's purpose, how to take it, etc.";
                        form.addRow();
                        form.rowName = "Start Page Video link </b>";
                        form.rowValue = cp.Html.InputText("Video", quiz.Video)
                         + "</br> When included, a video can be presented on the start page.";
                        form.addRow();
                        form.rowName = "Course Materials </b>";
                        form.rowValue = cp.Html.InputFile("CorseMaterial", "addCourseMaterialClass", "js-addCourseMaterialButtonId")
                        + "</br> When included, a file can be uploaded on the start page.";
                        form.addRow();
                        form.rowName = "Start Quiz Button </b>";
                        form.rowValue = cp.Html.InputText("customButtonCopy", "Start")
                        + "</br> This is the text that will be shown on the start button for the quiz.";

                        //form.rowValue = ("<div><a href=\"?" + qs + quiz.id + "\">One question perpage: subjects; Users can retake quiz; max 5 questions</a></div>");
                        //
                        result = genericController.getTabWrapper(cp, form.getHtml(cp), "Start Page");

                        cp.Doc.AddHeadStyle(form.styleSheet);
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



