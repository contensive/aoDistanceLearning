
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
            adminFramework.reportListClass reportList = new adminFramework.reportListClass(cp);
            try
            {
                string inputForm;
                string sql = "";
                string qs;
                string qsBase;
                string quizname = cp.Doc.GetText("QuizName");
                string rqs = "";
                string sendto = "";
                if (string.IsNullOrEmpty(quizname))
                {

                    CPBlockBaseClass layout = cp.BlockNew();
                    CPCSBaseClass cs = cp.CSNew();
                    //adminFramework.reportListClass reportList = new adminFramework.reportListClass(cp);

                    DateTime filterDateFrom = genericController.encodeMinDate(cp.Utils.EncodeDate(cp.Doc.get_Var(constants.rnFilterDateFrom)));
                    DateTime filterDateTo = genericController.encodeMinDate(cp.Utils.EncodeDate(cp.Doc.get_Var(constants.rnFilterDateTo)));
                    DateTime tmpDate;
                    string sqlWhere = "";

                    //
                    if ((filterDateTo != DateTime.MinValue) & (filterDateTo < filterDateFrom))
                    {
                        tmpDate = filterDateFrom;
                        filterDateFrom = filterDateTo;
                        filterDateTo = tmpDate;
                    }

                    reportList.title = "Distance Learning";
                    // reportList.description = "A list of all online quizzes. ";


                    reportList.addColumn();
                    reportList.columnCaption = "Sample Quiz";
                    reportList.columnCaptionClass = "afwTextAlignLeft afwWidth200px";
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
                        List<QuizResponseModel> responseList = QuizResponseModel.getObjectList(cp, quiz.id);
                        reportList.addRow();
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalDetailsPageAddon, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        reportList.setCell("<a href=\"?" + qs + "\">" + quiz.name + "</a>");
                        reportList.columnCellClass = "afwTextAlignCenter";
                        reportList.setCell(responseList.Count.ToString());
                        reportList.columnCellClass = "afwTextAlignRight";
                        reportList.setCell(quiz.DateAdded.ToShortDateString());
                    }

                    inputForm = "<div class=\"afwTextAlignRight\">"
                        + cp.Html.InputText("QuizName", "", "", "", false, "", "js-quizname")
                        + cp.Html.Button("button", constants.rnbuttonInputNewQuiz, "addQuizClass", "js-addQuizButtonId")
                        + "</div></br>";

                    reportList.columnCellClass = "afwTextAlignRight";
                    reportList.htmlBeforeTable = inputForm;
                    cp.Doc.AddHeadStyle(reportList.styleSheet);
                    reportList.isOuterContainer = true;
                    result = cp.Html.Form( reportList.getHtml(cp));
                }
                else
                {
                    qs = cp.Utils.ModifyQueryString(cp.Doc.RefreshQueryString, "addonGUID", constants.quizOverViewSettingsAddon);
                    qs = cp.Utils.ModifyQueryString(qs, "quizName", quizname);
                    qs = cp.Utils.ModifyQueryString(qs, "addonid", quizname);
                    cp.Response.Redirect("?" + qs);
                }
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

