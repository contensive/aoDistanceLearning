
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning.Interfaces
{
    public class quizDashboardClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(CPBaseClass cp)

        {
          
            string result = "";
            adminFramework.ReportListClass reportList = new adminFramework.ReportListClass(cp);
            try
            {
                string inputForm;
                string qs;
                string quizname = cp.Doc.GetText("QuizName");
                if (string.IsNullOrEmpty(quizname))
                {
                    CPBlockBaseClass layout = cp.BlockNew();
                    CPCSBaseClass cs = cp.CSNew();
                    DateTime filterDateFrom = cp.Utils.EncodeDate(cp.Doc.get_Var(Constants.rnFilterDateFrom));
                    DateTime filterDateTo = cp.Utils.EncodeDate(cp.Doc.get_Var(Constants.rnFilterDateTo));
                    DateTime tmpDate;
                    //
                    if ((filterDateTo != DateTime.MinValue) & (filterDateTo < filterDateFrom))
                    {
                        tmpDate = filterDateFrom;
                        filterDateFrom = filterDateTo;
                        filterDateTo = tmpDate;
                    }
                    reportList.title = "Distance Learning";
                    //
                    reportList.addColumn();
                    reportList.columnCaption = "Sample Quiz";
                    reportList.columnCaptionClass = "afwTextAlignLeft";
                    //
                    reportList.addColumn();
                    reportList.columnCaption = "Attempts";
                    reportList.columnCaptionClass = "afwTextAlignCenter afwWidth50px";
                    //
                    reportList.addColumn();
                    reportList.columnCaption = "Date Created";
                    reportList.columnCaptionClass = "afwTextAlignRight afwWidth100px";
                    //

                    List<QuizModel> quizList = QuizModel.getQuizList(cp);
                    //qsBase = cp.Utils.ModifyQueryString(cp.Doc.RefreshQueryString, constants.rnAddonId, constants.portalAddonId, true);
                    //qsBase = cp.Utils.ModifyQueryString(qsBase, "setPortalId", "1", true);
                   
                    foreach (QuizModel quiz in quizList)
                    {
                        List<QuizResponseModel> responseList = QuizResponseModel.GetResponseList(cp, quiz.id);
                        reportList.addRow();
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        reportList.setCell("<a href=\"?" + qs + "\">" + quiz.name + "</a>");
                        reportList.columnCellClass = "afwTextAlignCenter";
                        reportList.setCell(responseList.Count.ToString());
                        reportList.columnCellClass = "afwTextAlignRight";
                        reportList.setCell(quiz.DateAdded.ToShortDateString());
                    }

                    inputForm = "<div class=\"afwTextAlignRight\">"
                        + cp.Html.InputText("QuizName", "", "", "", false, "", "js-quizname")
                        + cp.Html.Button("button", Constants.rnbuttonInputNewQuiz, "addQuizClass btn btn-primary", "js-addQuizButtonId")
                        + "</div></br>";

                    reportList.columnCellClass = "afwTextAlignRight";
                    reportList.htmlBeforeTable = inputForm;
                    cp.Doc.AddHeadStyle(reportList.styleSheet);
                    reportList.isOuterContainer = true;
                    result = cp.Html.Form( reportList.getHtml(cp));
                }
                else
                {
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                    qs = cp.Utils.ModifyQueryString(qs, "QuizId", "");
                    qs = cp.Utils.ModifyQueryString(qs, "quizName", quizname);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
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

