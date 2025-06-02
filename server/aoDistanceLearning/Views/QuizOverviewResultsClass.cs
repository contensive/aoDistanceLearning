
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.DistanceLearning {
    public class QuizOverviewResultsClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            string result = "";
            string qs = "";
            try {
                QuizModel quiz = DbBaseModel.create<QuizModel>(cp, cp.Doc.GetInteger(Constants.rnQuizId));
                QuizResponseModel Myquizresponse = DbBaseModel.create<QuizResponseModel>(cp, cp.Doc.GetInteger(Constants.rnQuizId));
                if (quiz == null) {
                    //
                    // -- no quiz provided, go back to quiz list
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button) {
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
                BaseClasses.LayoutBuilder.LayoutBuilderListBaseClass layout = cp.AdminUI.CreateLayoutBuilderList();
                layout.addFormHidden(Constants.rnQuizId, quiz.id.ToString());
                layout.addFormButton(Constants.buttonCancel);
                layout.addFormButton(Constants.buttonRefresh);
                layout.title = "Results";
                layout.description = "";
                //quizUserDetailsForm.addColumn();
                //quizUserDetailsForm.columnCaption = "Quiz";
                //quizUserDetailsForm.columnCaptionClass = "afwTextAlignLeft";
                layout.addColumn();
                layout.columnCaption = "User";
                layout.columnCaptionClass = "afwTextAlignCenter";
                layout.columnCellClass = "afwTextAlignLeft";
                layout.addColumn();
                layout.columnCaption = "Submitted";
                layout.columnCaptionClass = "afwTextAlignCenter afwWidth100px";
                layout.columnCellClass = "afwTextAlignRight";
                layout.addColumn();
                layout.columnCaption = "Attempt";
                layout.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                layout.columnCellClass = "afwTextAlignRight";
                layout.addColumn();
                layout.columnCaption = "Score";
                layout.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                layout.columnCellClass = "afwTextAlignRight";
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
                List<QuizResponseReportModel> quizResponseList = QuizResponseReportModel.getQuizOverviewResponseList(cp, quiz.id, filterFromDate, filterToDate);
                foreach (QuizResponseReportModel quizResponse in quizResponseList) {
                    var member = DbBaseModel.create<PersonModel>(cp, cp.Doc.GetInteger(Constants.rnMemberId));

                    layout.addRow();
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewResultsDetails, true);
                    qs = cp.Utils.ModifyQueryString(qs, Constants.rnResponseId, quizResponse.id.ToString(), true);
                    qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                    string name = quizResponse.userFirstName + " " + quizResponse.userLastName;
                    if (string.IsNullOrEmpty(name.Trim())) name = quizResponse.userName;
                    layout.setCell("<div><a href=\"?" + qs + "\"> " + name + " </ a></div>");
                    layout.setCell(GenericController.getShortDateString(quizResponse.dateSubmitted));
                    layout.setCell(quizResponse.attemptNumber.ToString());
                    layout.setCell(Convert.ToInt32(0.5 + quizResponse.score).ToString() + "%");
                    //form.setCell(quizResponse.totalQuestions.ToString());
                    //form.setCell(quizResponse.totalCorrect.ToString());
                    //form.setCell(quizResponse.totalPoints.ToString());
                };
                layout.htmlLeftOfBody = ""
                    + Constants.cr + "<h3 class=\"afwFilterHead\">filters</h3>"
                    + Constants.cr + "<h4 class=\"afwFilterCaption\">Date</h4>"
                    + Constants.cr + "<div class=\"afwFilterRow\"><label for=fromfilter>from</label><input type=\"date\" name=\"" + Constants.rnFilterDateFrom + "\" value=\"" + Controllers.GenericController.getDateForHtmlInput(filterFromDate) + "\" class=\"afwFilterDate\" id=\"js-fromdate\" /></div>"
                    + Constants.cr + "<div class=\"afwFilterRow\"><label for=tofilter>to</label><input type=\"date\" name=\"" + Constants.rnFilterDateTo + "\" value=\"" + Controllers.GenericController.getDateForHtmlInput(filterToDate) + "\" class=\"afwFilterDate\" id=\"js-fromdate\" /></div>"
                    + "";
                //cp.Doc.AddHeadJavascript(""
                //    + constants.cr + "jQuery(document).ready(function(){"
                //    + constants.cr2 + "jQuery('.abAccountAddButton').click(function(){"
                //    + constants.cr2 + "window.location='" + cp.Site.GetProperty("adminUrl") + "?af=4&id=0&cid=" + cp.Content.GetID("people") + "';"
                //    + constants.cr2 + "return false;"
                //    + constants.cr2 + "});"
                //    + constants.cr + "})"
                //    + "");
                result = layout.getHtml(cp);
                result = GenericController.getTabWrapper(cp, result, "Results", quiz);
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