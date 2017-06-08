using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
    public class quizoverviewStartPageClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            try
            {
                string qs;
                string quizName = cp.Doc.GetText("quizName");
                string customTopCopy = cp.Doc.GetText("customTopCopy");
                string Video = cp.Doc.GetText("Video");
                string customButtonCopy = cp.Doc.GetText("customButtonCopy");
                string courseMaterial = cp.Doc.GetText("CorseMaterial");
                string innerBody = "";
                //
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger("QuizId"));
                if (quiz == null)
                {
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button)
                {
                    case "Save":
                        quiz.customTopCopy = cp.Doc.GetText("customTopCopy");
                        quiz.Video.upload("Video");
                        quiz.courseMaterial = cp.Doc.GetText("CorseMaterial");
                        if ( !string.IsNullOrEmpty( quiz.courseMaterial ))
                        {
                            cp.Html.ProcessInputFile(quiz.courseMaterial, "");
                        }
                        quiz.customButtonCopy = cp.Doc.GetText("customButtonCopy");
                        quiz.saveObject(cp);
                        break;
                    case "Cancel":
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeaturesQuizOverviewDetails, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        cp.Response.Redirect("?" + qs);
                        break;
                }
                //
                adminFramework.formNameValueRowsClass form = new adminFramework.formNameValueRowsClass();                         
                form.isOuterContainer = false;             
                form.addFormHidden("quizId", quiz.id.ToString());
                form.body = innerBody;
                form.addFormButton("Save", "button");
                form.addFormButton("Cancel", "button");
                form.isOuterContainer = false;
                form.addRow();
                form.title = "<b>Start Page </b></br>";
                //
                form.addRow();
                form.rowName = "Start Page Text </b>";
                form.rowValue = cp.Html.InputWysiwyg("customTopCopy", quiz.customTopCopy,CPHtmlBaseClass.EditorUserScope.CurrentUser, CPHtmlBaseClass.EditorContentScope.Page, "10", "700")
                        + "This is the list of instructions that go on the Start Page. You can describe the quiz, it's purpose, how to take it, etc.";
                //form.rowValue = cp.Html.InputTextExpandable("customTopCopy", quiz.customTopCopy)
                // + "This is the list of instructions that go on the Start Page. You can describe the quiz, it's purpose, how to take it, etc.";
                form.addRow();
                form.rowName = "Start Page Video link </b>";
                form.rowValue = cp.Html.InputFile("Video")
                    + "</br> When included, a video can be presented on the start page.";
                form.addRow();
                form.rowName = "Course Materials </b>";
                form.rowValue = cp.Html.InputFile("CorseMaterial", "addCourseMaterialClass", "js-addCourseMaterialButtonId")
                + "</br> When included, a file can be uploaded on the start page.";
                form.addRow();               
                form.rowName = "Start Quiz Button </b>";
                form.rowValue = cp.Html.InputText("customButtonCopy", "Start")
                + "</br> This is the text that will be shown on the start button for the quiz.";
                // 
                result =  genericController.getTabWrapper(cp, form.getHtml(cp), "Start Page", quiz);

                cp.Doc.AddHeadStyle(form.styleSheet);
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



