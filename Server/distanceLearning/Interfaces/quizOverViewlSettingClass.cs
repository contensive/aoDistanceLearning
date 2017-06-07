using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
    public class quizOverViewSettingClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            string innerBody = "";
            string qs = "";
            int setportalId = cp.Doc.GetInteger("setPortalId");
            
            try
            {
                QuizModel quiz;
                string quizName = cp.Doc.GetText("quizName");
                if (quizName != String.Empty)
                {
                    quiz = QuizModel.add(cp );
                    quiz.name = quizName;
                    quiz.saveObject(cp);
                }
                else
                {
                    quiz = QuizModel.create(cp, cp.Doc.GetInteger("QuizId"));
                }
                if (quiz == null)
                {
                    return "";
                }

                string button = cp.Doc.GetText("button");
                switch (button)
                {
                    case "Save":
                        quiz.allowRetake = cp.Doc.GetBoolean("allowRetake");
                        quiz.questionPresentation = cp.Doc.GetText("questionPresentation");
                        quiz.maxNumberQuest = cp.Doc.GetInteger("maxNumberQuest");
                        quiz.includeSubject = cp.Doc.GetText("includeSubject");
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
                form.addFormHidden("QuizId", quiz.id.ToString());
                form.body = innerBody;
                form.addFormButton("Save", "button");
                form.addFormButton("Cancel", "button");
                //


                form.title = "Settings";
                form.isOuterContainer = false;
                form.addRow();
                form.rowName = "Question Presentation";
                form.rowValue = cp.Html.SelectList("questionPresentation", quiz.questionPresentation, quiz.questionPresentation, "All subject questions per page")
                    + "<p>You can choose to display one question per page, all questions on one page , or all subject"
                    + " questions per page</p>";
                form.addRow();
                form.rowName = "Allow users to retake";
                form.rowValue = cp.Html.CheckBox("allowRetake", quiz.allowRetake)
                    + "<p>If this box is checked users can choose to retake the quiz.</p>";
                form.addRow();
                form.rowName = "Max questions to display";
                form.rowValue = cp.Html.InputText("maxNumberQuest", quiz.maxNumberQuest.ToString())
                    + "<p>This is the max number of questions that will display per quiz or per subject(if subjects are used ). The system will randomly select"
                    + " questions from the available pool up to the max number entered. If user is allowed to retake it will display a random selection from the pool"
                    + " so the user doesnt get the same quiz twice</p>";
                form.addRow();
                form.rowName = "Include Subjects";
                form.rowValue = cp.Html.InputTextExpandable("includeSubject", quiz.includeSubject)
                    + "<p>If you wish to organize your questions by subject, enter the subject section in the text box one subject per line."
                    + " if this quiz has no sections leave blank</p>";
                //
                result = genericController.getTabWrapper(cp, form.getHtml(cp), "Settings", quiz);
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


