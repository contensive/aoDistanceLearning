
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Views;
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning.Views {
    public class QuizDashboardClass : Contensive.BaseClasses.AddonBaseClass {
        public override object Execute(CPBaseClass cp) {

            string result = "";
            adminFramework.ReportListClass listReport = new adminFramework.ReportListClass(cp);
            try {
                string inputForm;
                string qs;
                string quizname = cp.Doc.GetText("QuizName");
                if (string.IsNullOrEmpty(quizname)) {
                    CPBlockBaseClass layout = cp.BlockNew();
                    CPCSBaseClass cs = cp.CSNew();
                    DateTime filterDateFrom = cp.Utils.EncodeDate(cp.Doc.GetText(Constants.rnFilterDateFrom));
                    DateTime filterDateTo = cp.Utils.EncodeDate(cp.Doc.GetText(Constants.rnFilterDateTo));
                    DateTime tmpDate;
                    //
                    if ((filterDateTo != DateTime.MinValue) & (filterDateTo < filterDateFrom)) {
                        tmpDate = filterDateFrom;
                        filterDateFrom = filterDateTo;
                        filterDateTo = tmpDate;
                    }
                    listReport.title = "Distance Learning";
                    listReport.description = "All distance learning quizzes.";
                    //
                    listReport.addColumn();
                    listReport.columnCaption = "Sample Quiz";
                    listReport.columnCaptionClass = "afwTextAlignLeft";
                    //
                    listReport.addColumn();
                    listReport.columnCaption = "Attempts";
                    listReport.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                    //
                    listReport.addColumn();
                    listReport.columnCaption = "Date Created";
                    listReport.columnCaptionClass = "afwTextAlignRight afwWidth100px";
                    //
                    List<QuizModel> quizList = DbBaseModel.createList<QuizModel>(cp);
                    int rowPtr = 0;
                    foreach (QuizModel quiz in quizList) {
                        List<QuizResponseModel> responseList = QuizResponseModel.GetResponseList(cp, quiz.id);
                        listReport.addRow();
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        listReport.setCell("<a href=\"?" + qs + "\">" + quiz.name + "</a>");
                        listReport.columnCellClass = "afwTextAlignCenter";
                        listReport.setCell(responseList.Count.ToString());
                        listReport.columnCellClass = "afwTextAlignRight";
                        listReport.setCell((quiz.dateAdded==null) ? "" : cp.Utils.EncodeDate( quiz.dateAdded).ToShortDateString());
                        rowPtr++;
                    }
                    //inputForm = "<div class=\"afwTextAlignRight\">"
                    //    + cp.Html.InputText("QuizName","",255,"", "js-quizname")
                    //    + cp.Html.Button("button", Constants.rnbuttonInputNewQuiz, "addQuizClass btn btn-primary", "js-addQuizButtonId")
                    //    + "</div></br>";
                    //listReport.columnCellClass = "afwTextAlignRight";
                    //listReport.htmlBeforeTable = inputForm;
                    //listReport.htmlLeftOfTable = ""
                    //    + "<div>filters</div>"
                    //    + cp.Html.CheckBox("accountListAllowOpenOnly", accountListAllowOpenOnly, "", "accountListAllowOpenOnly") + "Open&nbsp;Only"
                    //    + "";
                    listReport.addFormButton(Constants.buttonAdd, Constants.rnButton, "", "abQuizAddButton");
                    listReport.addFormHidden(Constants.rnSrcFormId, Constants.formIdQuizList.ToString());
                    listReport.addFormHidden("rowCnt", rowPtr.ToString());
                    string addButtonJs = "jQuery('.abQuizAddButton').click(function(){window.location='" + cp.GetAppConfig(cp.Site.Name).adminRoute + "?af=4&id=0&cid=" + cp.Content.GetID("quizzes") + "';return false;});";
                    cp.Doc.AddHeadJavascript("document.addEventListener('DOMContentLoaded', function(event) {" + addButtonJs + "});");
                    cp.Doc.AddHeadStyle(listReport.styleSheet);
                    listReport.isOuterContainer = true;
                    result = cp.Html.Form(listReport.getHtml(cp));
                } else {
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                    qs = cp.Utils.ModifyQueryString(qs, "QuizId", "");
                    qs = cp.Utils.ModifyQueryString(qs, "quizName", quizname);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
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

