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
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger(Constants.rnQuizId));
                QuizResponseModel Myquizresponse = QuizResponseModel.create(cp, cp.Doc.GetInteger(Constants.rnQuizId));
                if (quiz == null)
                {
                    //
                    // -- no quiz provided, go back to quiz list
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button)
                {
                    case "Cancel":
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        cp.Response.Redirect("?" + qs);
                        break;
                    case "Refresh":
                        //qs = cp.Doc.RefreshQueryString;
                        //qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewResults, true);
                        //qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        //cp.Response.Redirect("?" + qs);
                        break;
                        
                }
               
                //
                // -- load filters
                DateTime filterFromDate = cp.Doc.GetDate(Constants.rnFilterDateFrom);
                DateTime filterToDate = cp.Doc.GetDate(Constants.rnFilterDateTo);
                //
                adminFramework.ReportListClass form = new adminFramework.ReportListClass(cp);
                form.addFormHidden(Constants.rnQuizId, quiz.id.ToString());
                form.addFormButton(Constants.buttonCancel);
                form.addFormButton(Constants.buttonRefresh);
                form.title = "Results";
                form.description = "";
                //quizUserDetailsForm.addColumn();
                //quizUserDetailsForm.columnCaption = "Quiz";
                //quizUserDetailsForm.columnCaptionClass = "afwTextAlignLeft";
                form.addColumn();
                form.columnCaption = "User";
                form.columnCaptionClass = "afwTextAlignCenter";
                form.columnCellClass = "afwTextAlignLeft";
                form.addColumn();
                form.columnCaption = "Submitted";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth100px";
                form.columnCellClass = "afwTextAlignRight";
                form.addColumn();
                form.columnCaption = "Attempt";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                form.columnCellClass = "afwTextAlignRight";
                form.addColumn();
                form.columnCaption = "Score";
                form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                form.columnCellClass = "afwTextAlignRight";
                //form.addColumn();
                //form.columnCaption = "Questions";
                //form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                //form.columnCellClass = "afwTextAlignRight";
                //form.addColumn();
                //form.columnCaption = "Correct";
                //form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                //form.columnCellClass = "afwTextAlignRight";
                //form.addColumn();
                //form.columnCaption = "Points";
                //form.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                //form.columnCellClass = "afwTextAlignRight";
                //
                List<QuizResponseModel.quizResponseReportModel> quizResponseList = QuizResponseModel.GetQuizOverviewResponseList(cp, quiz.id, filterFromDate, filterToDate);
                foreach (QuizResponseModel.quizResponseReportModel quizResponse in quizResponseList)
                {
                    MemberModel member = MemberModel.create(cp, cp.Doc.GetInteger(Constants.rnMemberId));
                    
                    form.addRow();
                    //quizUserDetailsForm.setCell(quizResponse.quizName); 
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewResultsDetails, true);
                    qs = cp.Utils.ModifyQueryString(qs, Constants.rnResponseId, quizResponse.id.ToString(), true);
                    qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                    string name = quizResponse.userFirstName + " " + quizResponse.userLastName;
                    if (string.IsNullOrEmpty(name.Trim())) name = quizResponse.userName;
                    form.setCell("<div><a href=\"?" + qs + "\"> " + name  + " </ a></div>");
                    form.setCell( genericController.getShortDateString(  quizResponse.dateSubmitted));
                    form.setCell(quizResponse.attemptNumber.ToString());
                    form.setCell( Convert.ToInt32( 0.5 + quizResponse.score).ToString() + "%");
                    //form.setCell(quizResponse.totalQuestions.ToString());
                    //form.setCell(quizResponse.totalCorrect.ToString());
                    //form.setCell(quizResponse.totalPoints.ToString());
                };
                form.htmlLeftOfTable = ""
                    + Constants.cr + "<h3 class=\"afwFilterHead\">filters</h3>"
                    + Constants.cr + "<h4 class=\"afwFilterCaption\">Date</h4>"
                    + Constants.cr + "<div class=\"afwFilterRow\"><label for=fromfilter>from</label><input type=\"date\" name=\"" + Constants.rnFilterDateFrom + "\" value=\"" + Controllers.genericController.getDateForHtmlInput( filterFromDate ) + "\" class=\"afwFilterDate\" id=\"js-fromdate\" /></div>"
                    + Constants.cr + "<div class=\"afwFilterRow\"><label for=tofilter>to</label><input type=\"date\" name=\"" + Constants.rnFilterDateTo + "\" value=\"" + Controllers.genericController.getDateForHtmlInput( filterToDate ) + "\" class=\"afwFilterDate\" id=\"js-fromdate\" /></div>"
                    + "";
                //cp.Doc.AddHeadJavascript(""
                //    + constants.cr + "jQuery(document).ready(function(){"
                //    + constants.cr2 + "jQuery('.abAccountAddButton').click(function(){"
                //    + constants.cr2 + "window.location='" + cp.Site.GetProperty("adminUrl") + "?af=4&id=0&cid=" + cp.Content.GetID("people") + "';"
                //    + constants.cr2 + "return false;"
                //    + constants.cr2 + "});"
                //    + constants.cr + "})"
                //    + "");
                result = form.getHtml(cp);
                result = genericController.getTabWrapper(cp, result, "Results", quiz);
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



