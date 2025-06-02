using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;

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
                BaseClasses.LayoutBuilder.LayoutBuilderNameValueBaseClass layout = cp.AdminUI.CreateLayoutBuilderNameValue();
                layout.isOuterContainer = false;
                layout.body = innerBody;
                layout.addFormButton("Cancel", "button");
                layout.title = quiz.name;


                //
                // -- add row
                layout.addRow();
                layout.rowName = "Questions";
                qs = cp.Doc.RefreshQueryString;
                qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewSetting, true);
                qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                //form.rowValue = ("<div><a href=\"?" + qs + "\"><img src=\"/myDistanceLearning/NavRecord.gif\" display:inline;></a>" + quiz.questionPresentation + " ; Using Subjects; " + quiz.includeSubject + "User can retake quiz:  " + quiz.allowRetake + "; Max " + quiz.maxNumberQuest + " Questions </div>");
                layout.rowValue = ("<div><a href=\"?" + qs + "\"><img src=\"/myDistanceLearning/NavRecord.gif\" display:inline;></a>User can retake quiz:  " + quiz.allowRetake + "; Max " + quiz.maxNumberQuest + " Questions </div>");
                layout.addRow();
                layout.rowValue = ("<div><a href=\"?" + qs + "\"><img src=\"/myDistanceLearning/NavRecord.gif\" display:inline;></a>" + quiz.maxNumberQuest + " total Questions in Quiz (or in each subject area if subjects used.)</div>");
                qs = cp.Doc.RefreshQueryString;
                qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewScoring, true);
                qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                layout.addRow();

                layout.rowValue = ("<div><a href=\"?" + qs + "\"><img src=\"/myDistanceLearning/NavRecord.gif\" display:inline;></a>" + quiz.ACaption + "  " + quiz.APercentile + "+ </br> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + quiz.BCaption + "  "
                    + quiz.BPercentile + "+ </br> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + quiz.CCaption + "  "
                    + quiz.CPercentile + "+ </br> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + quiz.DCaption + "  "
                    + quiz.DPercentile + "+ </br> &nbsp;&nbsp;&nbsp;&nbsp; " + quiz.FCaption + " &nbsp;&nbsp;&nbsp;&nbsp; Successful completion adds  "
                    + quiz.certificationCECs + " CECs to user accredidation </br></div>");
                // -- wrap in tabs and output finished form
                result = GenericController.getTabWrapper(cp, layout.getHtml(), "Details", quiz);
                cp.Doc.AddHeadStyle(layout.styleSheet);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "execute");
            }
            return result;


        }
    }
}