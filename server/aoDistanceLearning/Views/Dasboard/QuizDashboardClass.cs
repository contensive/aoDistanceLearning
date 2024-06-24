
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.PortalFramework;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.DistanceLearning {
    namespace Views {
        public class QuizDashboardClass : Contensive.BaseClasses.AddonBaseClass {
            public override object Execute(CPBaseClass cp) {

                string result = "";
                ReportListClass listReport = new ReportListClass(cp);
                try {
                    string button = cp.Doc.GetText("button");
                    switch (button) {
                        case "Edit": {
                                int quizId = cp.Doc.GetInteger(Constants.rnQuizId);
                                cp.Response.Redirect("?aa=0&tx=&ad=0&af=4&asf=1&cid=" + cp.Content.GetID(QuizModel.tableMetadata.contentName) + "&id=" + quizId);
                                break;
                            }
                        case "Delete": {
                                int quizId = cp.Doc.GetInteger(Constants.rnQuizId);
                                DbBaseModel.delete<QuizModel>(cp, quizId);
                                break;
                            }
                    }


                    string inputForm;
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
                        listReport.title = "Distance Learning Online Quizes";
                        listReport.description = "To add a new quiz, navigate to the page where the quiz will be taken and add the Online Quiz Design Block to the page. A small sample quiz will be constructed. Turn on Content Edit to add and modify questions and answers. Return here to the Distance Learning Manager to manage more advanced features.";
                        //
                        listReport.addColumn();
                        listReport.columnCaption = "Quiz";
                        listReport.columnCaptionClass = "afwTextAlignLeft";
                        //
                        listReport.addColumn();
                        listReport.columnCaption = "Attempts";
                        listReport.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                        //
                        listReport.addColumn();
                        listReport.columnCaption = "Date Created";
                        listReport.columnCaptionClass = "afwTextAlignCenter afwWidth100px";
                        //
                        listReport.addColumn();
                        listReport.columnCaption = "Manage";
                        listReport.columnCaptionClass = "afwTextAlignCenter afwWidth200px";
                        //
                        List<QuizModel> quizList = DbBaseModel.createList<QuizModel>(cp);
                        int rowPtr = 0;
                        foreach (QuizModel quiz in quizList) {
                            if (string.IsNullOrWhiteSpace(quiz.name)) { 
                                quiz.name = "Quiz " + quiz.id;
                                quiz.save(cp);
                            }
                            List<QuizResponseModel> responseList = QuizResponseModel.getResponseList(cp, quiz.id);
                            listReport.addRow();
                            string qs = cp.Doc.RefreshQueryString;
                            qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                            qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                            listReport.setCell("<a href=\"?" + qs + "\">" + quiz.name + "</a>");
                            //
                            listReport.columnCellClass = "afwTextAlignCenter";
                            listReport.setCell(responseList.Count.ToString());
                            //
                            listReport.columnCellClass = "afwTextAlignRight";
                            listReport.setCell((quiz.dateAdded == null) ? "" : cp.Utils.EncodeDate(quiz.dateAdded).ToShortDateString());
                            //
                            string miniForm = "";
                            miniForm += cp.Html.Button("button", "Edit", "btn btn-sm mr-2 btn-primary questionEdit", "js-questionEdit");
                            miniForm += cp.Html.Button("button", "Delete", "btn btn-sm mr-2 btn-danger questionDelete", "js-questionDelete");
                            miniForm += cp.Html.Hidden(Constants.rnQuizId, quiz.id.ToString());
                            miniForm = cp.Html.Form(miniForm);
                            listReport.columnCellClass = "afwTextAlignCenter";
                            listReport.setCell(miniForm);
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
                        string qs = cp.Doc.RefreshQueryString;
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
}
