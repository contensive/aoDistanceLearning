using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Views;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning {
    public class QuizOverviewDetailsClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            string result = "";
            try {
                string qs;
                //
                // -- process buttons
                string button = cp.Doc.GetText("button");
                switch (button) {
                    case "Cancel":
                        //
                        // -- no quiz provided, go back to dashboard (can't tell which feature is dashboard so just go to landing for this portal)
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", "");
                        cp.Response.Redirect("?" + qs);
                        return "";
                }
                string requestQuizName = cp.Doc.GetText("quizName");
                int requestQuizId = cp.Doc.GetInteger("quizId");
                string innerBody = "";
                QuizModel quiz = DbBaseModel.create<QuizModel>(cp, requestQuizId);
                List<QuizQuestionModel> questions = QuizQuestionModel.getQuestionsForQuizList(cp, requestQuizId);
                if (quiz == null) {
                    //
                    // -- no id passed in, test if a name was included
                    if (!string.IsNullOrEmpty(requestQuizName)) {
                        //
                        // -- create new quiz from name
                        quiz = DbBaseModel.addDefault<QuizModel>(cp);
                        quiz.name = cp.Doc.GetText("quizName");
                        quiz.save(cp);
                    } else {
                        //
                        // -- no quiz provided, go back to dashboard (can't tell which feature is dashboard so just go to landing for this portal)
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", "");
                        cp.Response.Redirect("?" + qs);
                        return "";
                    }
                }
                //
                // -- setup form
                adminFramework.formNameValueRowsClass form = new adminFramework.formNameValueRowsClass();
                form.isOuterContainer = false;
                form.body = innerBody;
                form.addFormButton("Cancel", "button");
                form.title = quiz.name;


                //
                // -- add row
                form.addRow();
                form.rowName = "Questions";
                qs = cp.Doc.RefreshQueryString;
                qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewSetting, true);
                qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                //form.rowValue = ("<div><a href=\"?" + qs + "\"><img src=\"/myDistanceLearning/NavRecord.gif\" display:inline;></a>" + quiz.questionPresentation + " ; Using Subjects; " + quiz.includeSubject + "User can retake quiz:  " + quiz.allowRetake + "; Max " + quiz.maxNumberQuest + " Questions </div>");
                form.rowValue = ("<div><a href=\"?" + qs + "\"><img src=\"/myDistanceLearning/NavRecord.gif\" display:inline;></a>User can retake quiz:  " + quiz.allowRetake + "; Max " + quiz.maxNumberQuest + " Questions </div>");
                form.addRow();
                form.rowValue = ("<div><a href=\"?" + qs + "\"><img src=\"/myDistanceLearning/NavRecord.gif\" display:inline;></a>" + quiz.maxNumberQuest + " total Questions in Quiz (or in each subject area if subjects used.)</div>");
                qs = cp.Doc.RefreshQueryString;
                qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewScoring, true);
                qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                form.addRow();

                form.rowValue = ("<div><a href=\"?" + qs + "\"><img src=\"/myDistanceLearning/NavRecord.gif\" display:inline;></a>" + quiz.ACaption + "  " + quiz.APercentile + "+ </br> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + quiz.BCaption + "  "
                    + quiz.BPercentile + "+ </br> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + quiz.CCaption + "  "
                    + quiz.CPercentile + "+ </br> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + quiz.DCaption + "  "
                    + quiz.DPercentile + "+ </br> &nbsp;&nbsp;&nbsp;&nbsp; " + quiz.FCaption + " &nbsp;&nbsp;&nbsp;&nbsp; Successful completion adds  "
                    + quiz.certificationCECs + " CECs to user accredidation </br></div>");
                // -- wrap in tabs and output finished form
                result = GenericController.getTabWrapper(cp, form.getHtml(cp), "Details", quiz);
                cp.Doc.AddHeadStyle(form.styleSheet);
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


