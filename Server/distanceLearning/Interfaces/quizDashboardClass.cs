
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
          
            string s = "Hello World";
           
             try
            {
                string inputForm;
                string sql = "";
                string qs;
                string qsBase;
              //  string userName = "";
                string rqs = "";
              
                CPBlockBaseClass layout = cp.BlockNew();
                CPCSBaseClass cs = cp.CSNew();
                adminFramework.reportListClass reportList = new adminFramework.reportListClass(cp);
              
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
                reportList.description = "A list of all online quizzes. ";

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
                sqlWhere = "(dateSubmitted is not null)";
                if (filterDateFrom != DateTime.MinValue)
                {
                    sqlWhere += "and(dateSubmitted>=" + cp.Db.EncodeSQLDate(filterDateFrom) + ")";
                }
                if (filterDateTo != DateTime.MinValue)
                {
                    if (filterDateTo == filterDateTo.Date)
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
                    + " ,q.DateAdded as DateCreated"
                    + " ,r.id as responseId"
                    + " ,r.attemptNumber as attemptNumber"            
                    + " "
                    + " from"
                    + " quizResponses r"
                    + " left join quizzes q on q.id=r.quizId"
                    + " "
                    + sqlWhere
                    + " "
                    + " order by"
                    + " r.quizId,r.attemptNumber"
                    + " "
                    + "";
                if (!cs.OpenSQL2(sql, "", 100, 1))
                {
                }
                else
                {
                    qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnDstFormId, constants.formIdQuizDetails.ToString(), true);
                    while (cs.OK())
                    {
                        //
                       // userName = cs.GetText("userName");
                       // if (userName.ToLower() == "guest") { userName += " #" + cs.GetInteger("userId"); }
                        reportList.addRow();
                        qs = cp.Utils.ModifyQueryString(qsBase, "id", cs.GetInteger("responseId").ToString(), true);
                        reportList.setCell("<a href=\"?" + qs + "\">" + cs.GetText("quizName") + "</a>");                   
                        //
                        reportList.columnCellClass = "afwTextAlignCenter";
                        reportList.setCell(cs.GetText("attemptNumber"));
                        reportList.columnCellClass = "afwTextAlignRight";
                        reportList.setCell(genericController.getShortDateString(cs.GetDate("DateCreated")));
                        cs.GoNext();
                    }
                }
                cs.Close();
                inputForm = "<div class=\"afwTextAlignRight\"><input type=\"text\" name=\"QuizName\" placeholder=\"Name-for-Quiz\" value=\"\">"
                    + cp.Html.Button("button", constants.rnbuttonInputNewQuiz, "addQuizClass","js-addQuizButtonId")
                    + "</div></br>";
                reportList.columnCellClass = "afwTextAlignRight";
                reportList.htmlBeforeTable= inputForm;
                cp.Doc.AddHeadStyle(reportList.styleSheet);
                reportList.isOuterContainer = true;
                s = reportList.getHtml(cp);
               
            }
           
            catch (Exception ex)
            {
                errorReport(cp, ex, "execute");
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

