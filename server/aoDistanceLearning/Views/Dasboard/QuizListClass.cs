
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.DistanceLearning {
    public class QuizListClass {
        //
        // ===============================================================================
        // process Form
        // ===============================================================================
        //
        public int processForm(CPBaseClass cp, int srcFormId, string rqs, DateTime rightNow, ref int appId) {
            int nextFormId = srcFormId;
            try {
                string button = cp.Doc.GetProperty(Constants.rnButton, "");
                CPCSBaseClass cs = cp.CSNew();
                //
                if (button != "") {
                    GenericController.checkRequiredFieldText(cp, Constants.rnSampleField, "Sample Field");
                    //
                    if (cp.UserError.OK()) {
                        if (!cs.Open(Constants.cnApps, "id=" + appId, "", true, "", 1, 1)) {
                            cs.Close();
                            cs.Insert(Constants.cnApps);
                        }
                        cs.SetField("sampleField", cp.Doc.GetProperty(Constants.rnSampleField, ""));
                        cs.Close();
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "processForm");
            }
            return nextFormId;
        }
        //
        // ===============================================================================
        // get Form
        // ===============================================================================
        //
        public string getForm(CPBaseClass cp, int dstFormId, string rqs, DateTime rightNow, ref int appId) {
            string s = "";
            try {
                CPCSBaseClass cs = cp.CSNew();
                BaseClasses.LayoutBuilder.LayoutBuilderListBaseClass layout = cp.AdminUI.CreateLayoutBuilderList();
                string sql = "";
                string qs;
                string qsBase;
                string userName = "";
                string filterForm;
                DateTime filterDateFrom = cp.Utils.EncodeDate(cp.Doc.GetText(Constants.rnFilterDateFrom));
                DateTime filterDateTo = cp.Utils.EncodeDate(cp.Doc.GetText(Constants.rnFilterDateTo));
                DateTime tmpDate;
                string sqlWhere = "";
                //
                if ((filterDateTo != DateTime.MinValue) & (filterDateTo < filterDateFrom)) {
                    tmpDate = filterDateFrom;
                    filterDateFrom = filterDateTo;
                    filterDateTo = tmpDate;
                }
                //
                // reportList.addRow();
                // reportList.setCell("<p>test</p>");

                //
                layout.title = "Submitted Quizzes";
                layout.description = "A list of all submitted responses to all online quizzes. Use filters to search for individual quizzes and time periods.";
                //
                layout.addColumn();
                layout.columnCaption = "Quiz";
                layout.columnCaptionClass = "afwTextAlignLeft";
                //
                layout.addColumn();
                layout.columnCaption = "User";
                layout.columnCaptionClass = "afwTextAlignLeft afwWidth200px";
                //
                layout.addColumn();
                layout.columnCaption = "Date";
                layout.columnCaptionClass = "afwTextAlignLeft afwWidth200px";
                //
                layout.addColumn();
                layout.columnCaption = "Attempt";
                layout.columnCaptionClass = "afwTextAlignCenter afwWidth100px";
                //
                sqlWhere = "(dateSubmitted is not null)";
                if (!GenericController.isDateEmpty(filterDateFrom)) {
                    sqlWhere += "and(dateSubmitted>=" + cp.Db.EncodeSQLDate(filterDateFrom) + ")";
                }
                if (!GenericController.isDateEmpty(filterDateTo)) {
                    if (filterDateTo == filterDateTo.Date) {
                        sqlWhere += "and(dateSubmitted<" + cp.Db.EncodeSQLDate(filterDateTo.AddDays(1)) + ")";
                    } else {
                        sqlWhere += "and(dateSubmitted<=" + cp.Db.EncodeSQLDate(filterDateTo) + ")";
                    }
                }
                if (sqlWhere != "") {
                    sqlWhere = " where " + sqlWhere;
                }
                sql = "select"
                    + " q.name as quizName"
                    + " ,r.dateSubmitted as dateSubmitted"
                    + " ,r.id as responseId"
                    + " ,r.attemptNumber as attemptNumber"
                    + " ,u.name as userName"
                    + " ,u.id as userId"
                    + " "
                    + " from"
                    + " ((quizResponses r"
                    + " left join quizzes q on q.id=r.quizId)"
                    + " left join ccMembers u on u.id=r.memberId)"
                    + " "
                    + sqlWhere
                    + " "
                    + " order by"
                    + " r.quizId,u.name,u.id,r.attemptNumber"
                    + " "
                    + "";
                if (!cs.OpenSQL(sql, "", 100, 1)) {
                } else {
                    qsBase = cp.Utils.ModifyQueryString(rqs, Constants.rnDstFormId, Constants.formIdQuizDetails.ToString(), true);
                    while (cs.OK()) {
                        //
                        userName = cs.GetText("userName");
                        if (userName.ToLower() == "guest") { userName += " #" + cs.GetInteger("userId"); }
                        layout.addRow();
                        qs = cp.Utils.ModifyQueryString(qsBase, "id", cs.GetInteger(Constants.rnResponseId).ToString(), true);
                        layout.setCell("<a href=\"?" + qs + "\">" + cs.GetText("quizName") + "</a>");
                        layout.setCell(userName);
                        layout.setCell(GenericController.getShortDateString(cs.GetDate("dateSubmitted")));
                        //
                        layout.columnCellClass = "afwTextAlignCenter";
                        layout.setCell(cs.GetText("attemptNumber"));
                        cs.GoNext();
                    }
                }
                cs.Close();
                //
                // add filter under chart
                //
                filterForm = "Only show quizzes submitted:"
                    + cp.Html.div("from " + cp.Html.InputDate(Constants.rnFilterDateFrom, GenericController.getShortDateString(filterDateFrom), "", "", ""), "", "dlFilterRow", "")
                    + cp.Html.div("to " + cp.Html.InputDate(Constants.rnFilterDateTo, GenericController.getShortDateString(filterDateTo), "", "", ""), "", "dlFilterRow", "")
                    + "";
                filterForm = ""
                    + Constants.cr + cp.Html.h2("Filters", "", "", "")
                    + Constants.cr + cp.Html.div(filterForm, "", "", "")
                    + Constants.cr + cp.Html.div(cp.Html.Button("button", Constants.rnbuttonApplyFilter, " btn btn-primary", ""), "", "", "")
                    + "";
                filterForm = cp.Html.Form(filterForm, "", "", "", "", "");
                layout.htmlAfterBody = filterForm;
                //reportList.htmlBeforeTable = "hello world";
                //
                // return converted layout
                //
                s = layout.getHtml();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "getForm");
            }
            return s;
        }

    }
}