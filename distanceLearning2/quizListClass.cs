
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace aoDistanceLearning3
{
    class quizListClass
    {
        //
        // ===============================================================================
        // process Form
        // ===============================================================================
        //
        public int processForm(CPBaseClass cp, int srcFormId, string rqs, DateTime rightNow, ref int appId)
        {
            int nextFormId = srcFormId;
            try
            {
                string button = cp.Doc.GetProperty(statics.rnButton, "");
                CPCSBaseClass cs = cp.CSNew();
                //
                if (button != "")
                {
                    statics.checkRequiredFieldText(cp, statics.rnSampleField, "Sample Field");
                    //
                    if (cp.UserError.OK())
                    {
                        if (!cs.Open(statics.cnApps, "id=" + appId, "", true, "", 1, 1))
                        {
                            cs.Close();
                            cs.Insert(statics.cnApps);
                        }
                        cs.SetField("sampleField", cp.Doc.GetProperty(statics.rnSampleField, ""));
                        cs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                errorReport(cp, ex, "processForm");
            }
            return nextFormId;
        }
        //
        // ===============================================================================
        // get Form
        // ===============================================================================
        //
        public string getForm(CPBaseClass cp, int dstFormId, string rqs, DateTime rightNow, ref int appId)
        {
            string s = "";
            try
            {
                CPBlockBaseClass layout = cp.BlockNew();
                CPCSBaseClass cs = cp.CSNew();
                adminFramework.reportListClass reportList = new adminFramework.reportListClass();
                string sql = "";
                string qs;
                string qsBase;
                string userName = "";
                string filterForm;
                DateTime filterDateFrom = statics.encodeMinDate( cp.Utils.EncodeDate(cp.Doc.get_Var(statics.rnFilterDateFrom)));
                DateTime filterDateTo = statics.encodeMinDate( cp.Utils.EncodeDate( cp.Doc.get_Var(statics.rnFilterDateTo)));
                DateTime tmpDate;
                string sqlWhere = "";
                //
                if ((filterDateTo != DateTime.MinValue) & (filterDateTo < filterDateFrom))
                {
                    tmpDate = filterDateFrom;
                    filterDateFrom = filterDateTo;
                    filterDateTo = tmpDate;
                }
                //
                reportList.title = "Submitted Quizzes";
                reportList.description = "A list of all submitted responses to all online quizzes. Use filters to search for individual quizzes and time periods.";
                //
                reportList.addColumn();
                reportList.columnCaption = "Quiz";
                reportList.columnCaptionClass = "afwTextAlignLeft";
                //
                reportList.addColumn();
                reportList.columnCaption = "User";
                reportList.columnCaptionClass = "afwTextAlignLeft afwWidth200px";
                //
                reportList.addColumn();
                reportList.columnCaption = "Date";
                reportList.columnCaptionClass = "afwTextAlignLeft afwWidth200px";
                //
                reportList.addColumn();
                reportList.columnCaption = "Attempt";
                reportList.columnCaptionClass = "afwTextAlignCenter afwWidth100px";
                //
                sqlWhere = "(dateSubmitted is not null)";
                if (filterDateFrom != DateTime.MinValue)
                {
                    sqlWhere += "and(dateSubmitted>=" + cp.Db.EncodeSQLDate(filterDateFrom) + ")";
                }
                if (filterDateTo != DateTime.MinValue)
                {
                    if (filterDateTo == filterDateTo.Date )
                    {
                        sqlWhere += "and(dateSubmitted<" + cp.Db.EncodeSQLDate(filterDateTo.AddDays(1)) + ")";
                    }
                    else
                    {
                        sqlWhere += "and(dateSubmitted<=" + cp.Db.EncodeSQLDate(filterDateTo) + ")";
                    }
                }
                if (sqlWhere != "")
                {
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
                if (!cs.OpenSQL2( sql, "", 100, 1 ))
                {
                }
                else
                {
                    qsBase = cp.Utils.ModifyQueryString(rqs, statics.rnDstFormId, statics.formIdQuizDetails.ToString(), true);
                    while (cs.OK())
                    {
                        //
                        userName = cs.GetText("userName");
                        if (userName.ToLower() == "guest") { userName += " #" + cs.GetInteger("userId"); }
                        reportList.addRow();
                        qs = cp.Utils.ModifyQueryString(qsBase, "id", cs.GetInteger("responseId").ToString(), true);
                        reportList.setCell("<a href=\"?" + qs + "\">" + cs.GetText("quizName") + "</a>");
                        reportList.setCell(userName );
                        reportList.setCell(statics.getShortDateString(cs.GetDate("dateSubmitted")));
                        //
                        reportList.columnCellClass = "afwTextAlignCenter";
                        reportList.setCell(cs.GetText("attemptNumber"));
                        cs.GoNext();
                    }
                }
                cs.Close();
                //
                // add filter under chart
                //
                filterForm = "Only show quizzes submitted:"
                    + cp.Html.div("from " + cp.Html.InputDate(statics.rnFilterDateFrom, statics.getShortDateString(filterDateFrom), "", "", ""), "", "dlFilterRow", "")
                    + cp.Html.div("to " + cp.Html.InputDate(statics.rnFilterDateTo, statics.getShortDateString(filterDateTo), "", "", ""), "", "dlFilterRow", "")
                    + "";
                filterForm = ""
                    + statics.cr + cp.Html.h2("Filters", "", "", "")
                    + statics.cr + cp.Html.div(filterForm, "", "", "")
                    + statics.cr + cp.Html.div( cp.Html.Button("button", statics.rnbuttonApplyFilter, "", ""),"","","")
                    + "";
                filterForm = cp.Html.Form(filterForm, "", "", "", "", "");
                reportList.htmlAfterTable = filterForm;
                //
                // return converted layout
                //
                s = reportList.getHtml( cp );
            }
            catch (Exception ex)
            {
                errorReport(cp, ex, "getForm");
            }
            return s;
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
