using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
    public class quizoverviewStudyPageClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            try
            {
                string innerBody = "";
                //
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger(Constants.rnQuizId));
                if (quiz == null)
                {
                    string qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button)
                {
                    case "Save":
                        quiz.customTopCopy = cp.Doc.GetText(nameof(quiz.customTopCopy));
                        quiz.videoEmbedCode=cp.Doc.GetText(nameof(quiz.videoEmbedCode));
                        quiz.courseMaterial.processRequest(nameof(quiz.courseMaterial));
                        quiz.studyCopy=cp.Doc.GetText(nameof(quiz.studyCopy));

                        quiz.saveObject(cp);
                        break;
                    case "Cancel":
                        string qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                        qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, quiz.id.ToString(), true);
                        cp.Response.Redirect("?" + qs);
                        break;
                }
                //
                adminFramework.formNameValueRowsClass form = new adminFramework.formNameValueRowsClass();                         
                form.isOuterContainer = false;             
                form.addFormHidden(Constants.rnQuizId, quiz.id.ToString());
                form.body = innerBody;
                form.addFormButton("Save", "button");
                form.addFormButton("Cancel", "button");
                form.isOuterContainer = false;
                form.title = "<b>Study Page</b></br>";
                //
                form.addRow();
                form.rowName = "Video Embed Code";
                form.rowValue = cp.Html.InputTextExpandable("videoEmbedCode", quiz.videoEmbedCode,5);
                form.rowHelp = "When included, a video can be presented on the study page.";
                //
                form.addRow();
                form.rowName = "Study Page Text";
                form.rowValue = cp.Html.InputWysiwyg("studyCopy", quiz.studyCopy, CPHtmlBaseClass.EditorUserScope.CurrentUser, CPHtmlBaseClass.EditorContentScope.Page);
                form.rowHelp = "This is the list of instructions that go on the study Page. You can describe the quiz, it's purpose, how to take it, etc.";
                //
                form.addRow();
                form.rowName = "Course Materials";
                form.rowValue = quiz.courseMaterial.getHtmlInput();
                form.rowHelp = "When included, a file can be uploaded on the study page.";

                result =  GenericController.getTabWrapper(cp, form.getHtml(cp), "Study", quiz);

                cp.Doc.AddHeadStyle(form.styleSheet);
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport( ex, "execute");
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



