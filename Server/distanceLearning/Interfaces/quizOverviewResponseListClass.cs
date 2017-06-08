using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
    public class quizOverviewResultsClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            string qs = "";
            try
            {
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger(constants.rnQuizId));
                QuizResponseModel Myquizresponse = QuizResponseModel.create(cp, cp.Doc.GetInteger(constants.rnQuizId));
                if (quiz == null)
                {
                    //
                    // -- no quiz provided, go back to quiz list
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button)
                {
                    case "Cancel":
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeaturesQuizOverviewDetails, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        cp.Response.Redirect("?" + qs);
                        break;
                    case "Refresh":
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewResults, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        cp.Response.Redirect("?" + qs);
                        break;
                        
                }
               
                //
                // -- load filters
                DateTime filterFromDate = cp.Doc.GetDate(constants.rnFilterDateFrom);
                DateTime filterToDate = cp.Doc.GetDate(constants.rnFilterDateTo);
                //
                adminFramework.reportListClass form = new adminFramework.reportListClass(cp);
                form.addFormHidden(constants.rnQuizId, quiz.id.ToString());
                form.addFormButton(constants.buttonCancel);
                form.addFormButton(constants.buttonRefresh);
                form.title = "Results";
                form.description = "";
                //quizUserDetailsForm.addColumn();
                //quizUserDetailsForm.columnCaption = "Quiz";
                //quizUserDetailsForm.columnCaptionClass = "afwTextAlignLeft";
                form.addColumn();
                form.columnCaption = "User";
                form.columnCaptionClass = "afwTextAlignCenter";
                form.addColumn();
                form.columnCaption = "Date";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth200px";
                form.addColumn();
                form.columnCaption = "Attempt";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth200px";
                form.addColumn();
                form.columnCaption = "Score";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                form.addColumn();
                form.columnCaption = "Questions";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                form.addColumn();
                form.columnCaption = "Correct";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                form.addColumn();
                form.columnCaption = "Points";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                //
                List<QuizResponseModel.quizResponseReportModel> quizResponseList = QuizResponseModel.GetQuizOverviewResponseList(cp, quiz.id, filterFromDate, filterToDate);
                foreach (QuizResponseModel.quizResponseReportModel quizResponse in quizResponseList)
                {
                    MemberModel member = MemberModel.create(cp, cp.Doc.GetInteger(constants.rnMemberId));
                    
                    form.addRow();
                    //quizUserDetailsForm.setCell(quizResponse.quizName); 
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewResultsDetails, true);
                    qs = cp.Utils.ModifyQueryString(qs, "ResponseId", quizResponse.id.ToString(), true);
                    qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                   
                    form.setCell("<div><a href=\"?" + qs + "\"> " + quizResponse.userName + " </ a></div>");
                    form.setCell(quizResponse.dateSubmitted.ToShortDateString());
                    form.setCell(quizResponse.attemptNumber.ToString());
                    form.setCell(quizResponse.score.ToString());
                    form.setCell(quizResponse.totalQuestions.ToString());
                    form.setCell(quizResponse.totalCorrect.ToString());
                    form.setCell(quizResponse.totalPoints.ToString());
                };
                form.htmlLeftOfTable = ""
                    + constants.cr + "<div class=\"afwFilterHead\">filters</div>"
                    + constants.cr + "<div class=\"afwFilterCaption\">Date</div>"
                    + constants.cr + "<div class=\"afwFilterRow\"><label for=fromfilter>from</label><input type=\"date\" name=\"" + constants.rnFilterDateFrom + "\" value=\"2017 - 06 - 02\" id=\"js-fromdate\" /></div>"
                    + constants.cr + "<div class=\"afwFilterRow\"><label for=tofilter>to</label><input type=\"date\" name=\"" + constants.rnFilterDateTo + "\" value=\"2017 - 06 - 02\" id=\"js-fromdate\" /></div>"
                    + "";
                cp.Doc.AddHeadJavascript(""
                    + constants.cr + "jQuery(document).ready(function(){"
                    + constants.cr2 + "jQuery('.abAccountAddButton').click(function(){"
                    + constants.cr2 + "window.location='" + cp.Site.GetProperty("adminUrl") + "?af=4&id=0&cid=" + cp.Content.GetID("people") + "';"
                    + constants.cr2 + "return false;"
                    + constants.cr2 + "});"
                    + constants.cr + "})"
                    + "");
                result = form.getHtml(cp);
                result = genericController.getTabWrapper(cp, result, "Results", quiz);
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



