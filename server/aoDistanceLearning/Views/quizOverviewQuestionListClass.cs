using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Models.Db;
using Contensive.Addons.PortalFramework;

namespace Contensive.Addons.DistanceLearning {
    namespace Views {
        public class QuizOverviewQuestionListClass : AddonBaseClass {
            public override object Execute(CPBaseClass cp) {
                string result = "";
                try {
                    CPCSBaseClass cs = cp.CSNew();
                    QuizModel quiz = DbBaseModel.create<QuizModel>(cp, cp.Doc.GetInteger("QuizId"));
                    QuizQuestionModel quizQuestion = DbBaseModel.create<QuizQuestionModel>(cp, cp.Doc.GetInteger("ID"));
                    string button = cp.Doc.GetText("button");
                    switch (button) {
                        case "Edit":
                            string qs = cp.Doc.RefreshQueryString;
                            qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionDetails);
                            qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, cp.Doc.GetInteger(Constants.rnQuizId).ToString());
                            qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuestionId, cp.Doc.GetInteger(Constants.rnQuestionId).ToString());
                            cp.Response.Redirect("?" + qs);
                            break;
                        case "Delete":
                            DbBaseModel.delete<QuizQuestionModel>(cp, cp.Doc.GetInteger(Constants.rnQuestionId));
                            break;
                        case "AddQuestion":
                            qs = cp.Doc.RefreshQueryString;
                            qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionDetails);
                            qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, cp.Doc.GetInteger(Constants.rnQuizId).ToString());
                            cp.Response.Redirect("?" + qs);
                            // qs = cp.Utils.ModifyQueryString(qs, constants.rnQuestionId, " ");

                            break;
                    }

                    // string qs;
                    // string qsBase;
                    //string rqs = "";

                    if (quiz == null) {
                        return "";
                    }
                    //
                    // -- create view
                    ReportListClass reportList = new ReportListClass();
                    reportList.isOuterContainer = false;
                    //
                    // -- add button as link
                    reportList.addFormButton(Constants.buttonAdd, Constants.rnButton, "", "abQuizQuestionAddButton");
                    string addLink = cp.GetAppConfig(cp.Site.Name).adminRoute + "?af=4&id=0&cid=" + cp.Content.GetID(QuizQuestionModel.tableMetadata.contentName) + "&wc=quizid%3D" + quiz.id;
                    string addButtonJs = "jQuery('.abQuizQuestionAddButton').click(function(){window.location='" + addLink + "';return false;});";
                    cp.Doc.AddHeadJavascript("document.addEventListener('DOMContentLoaded', function(event) {" + addButtonJs + "});");
                    //
                    reportList.addColumn();
                    reportList.columnCaption = "Subject";
                    reportList.columnCaptionClass = "afwTextAlignLeft afwWidth200px";
                    //
                    reportList.addColumn();
                    reportList.columnCaption = "Questions    ";
                    reportList.columnCaptionClass = "afwTextAlignLeft";
                    //
                    reportList.addColumn();
                    reportList.columnCaption = "Sort Order";
                    reportList.columnCaptionClass = "afwTextAlignright afwWidth100px";
                    //
                    reportList.addColumn();
                    reportList.columnCaption = " ";
                    reportList.columnCaptionClass = "afwTextAlignright afwWidth200px";
                    //
                    QuizQuestionModel quizQuestions = DbBaseModel.create<QuizQuestionModel>(cp, cp.Doc.GetInteger("ID"));
                    string addButtonForm = "";
                    addButtonForm = cp.Html.Button("Button", "AddQuestion", "addQuestionClass btn btn-primary", "js-addQuestionButtonId");
                    addButtonForm += cp.Html.Hidden(Constants.rnQuizId, quiz.id.ToString());
                    addButtonForm = cp.Html.Form(addButtonForm);
                    reportList.htmlAfterTable = addButtonForm;
                    //
                    List<Models.QuizSubjectModel> subjectList = DbBaseModel.createList<QuizSubjectModel>(cp);
                    //
                    // - add empty subject at end for questions without subjects
                    subjectList.Add(new QuizSubjectModel() { });
                    List<int> usedSubjectIdList = new List<int>();
                    List<QuizQuestionModel> questionList = QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id);
                    foreach (QuizSubjectModel subject in subjectList) {
                        usedSubjectIdList.Add(subject.id);
                        //
                        // -- list all questions in this subject
                        foreach (QuizQuestionModel question in questionList) {
                            //
                            // -- list this question if it is in this subject
                            if (subject.id == question.subjectID) {
                                addQuestionToList(cp, question, subject, reportList);
                            }
                        }
                    }
                    foreach (QuizQuestionModel question in questionList) {
                        if (!usedSubjectIdList.Contains(question.subjectID)) {
                            addQuestionToList(cp, question, null, reportList);
                        }
                    }

                    cp.Doc.AddRefreshQueryString("quizId", quiz.id.ToString());
                    //
                    result = GenericController.getTabWrapper(cp, reportList.getHtml(cp), "Questions", quiz);

                    cp.Doc.AddHeadStyle(reportList.styleSheet);
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "execute");
                }
                return result;


            }
            //
            // ===============================================================================
            //
            private void errorReport(CPBaseClass cp, Exception ex, string method) {
                cp.Site.ErrorReport(ex, "error in addonTemplateCs2005.blankClass.getForm");
            }
            //
            // ===============================================================================
            //
            private void addQuestionToList(CPBaseClass cp, QuizQuestionModel question, QuizSubjectModel subject, ReportListClass reportList) {
                List<QuizResponseModel> responseList = QuizResponseModel.getResponseList(cp, question.id);
                reportList.addRow();
                reportList.columnCellClass = "afwTextAlignLeft";
                if (subject == null) {
                    reportList.setCell("");
                } else {
                    reportList.setCell(subject.name);
                }
                //
                reportList.columnCellClass = "afwTextAlignLeft";
                reportList.setCell(question.copy);
                reportList.setCell(question.sortOrder.ToString());
                //
                string miniForm = "";
                miniForm += cp.Html.Button("button", "Edit", "btn btn-sm mr-2 btn-primary questionEdit", "js-questionEdit");
                miniForm += cp.Html.Button("button", "Delete", "btn btn-sm mr-2 btn-danger questionDelete", "js-questionDelete");
                miniForm += cp.Html.Hidden(Constants.rnQuestionId, question.id.ToString());
                miniForm += cp.Html.Hidden(Constants.rnQuizId, question.quizId.ToString());
                miniForm = cp.Html.Form(miniForm);
                reportList.columnCellClass = "afwTextAlignRight";
                reportList.setCell(miniForm);
            }
        }
    }
}