using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
   public class quizOverviewReportingClass : Contensive.BaseClasses.AddonBaseClass
        {
         int QuizId;

        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            string qs = "";
            try
            {
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger(constants.rnQuizId));
              
              
                if (quiz == null)
                {
                    //
                    // -- no quiz provided, go back to quiz list
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                   // return "";
                }
                QuizId = cp.Doc.GetInteger("quizId");
 
                //
                // -- create the upper part of the page, the list of scoring
                 adminFramework.reportListClass ReportingFilterForm = new adminFramework.reportListClass(cp);
                ReportingFilterForm.addColumn();
                ReportingFilterForm.columnCaption = "Reporting";
                ReportingFilterForm.columnCaptionClass = "afwTextAlignLeft afwWidth100px";
                ReportingFilterForm.addColumn();
                ReportingFilterForm.columnCaption = "";
                ReportingFilterForm.columnCaptionClass = "afwTextAlignCenter afwWidth100px";
                ReportingFilterForm.addColumn();
                ReportingFilterForm.columnCaption = "";
                ReportingFilterForm.columnCaptionClass = "afwTextAlignRight afwWidth100px";
                ReportingFilterForm.setCell("Only show quiz resonses:");
                ReportingFilterForm.columnCellClass = "afwTextAlignLeft";
                ReportingFilterForm.setCell("<label for=fromfilter>   from : </label><input id=js-fromdate type=date value=2017 - 06 - 02 /></br><label for=tofilter> &nbsp;&nbsp;  to : </label><input id=js-fromdate type=date value=2017 - 06 - 02 />");              
                ReportingFilterForm.columnCellClass = "afwTextAlignLeft";
                ReportingFilterForm.setCell(cp.Html.Button("customButtonCopy", "Apply filter(s)"));
                ReportingFilterForm.columnCellClass = "afwTextAlignLeft";
                adminFramework.reportListClass quizUserDetailsForm = new adminFramework.reportListClass(cp);
                quizUserDetailsForm.addColumn();
                quizUserDetailsForm.columnCaption = "Quiz";
                quizUserDetailsForm.columnCaptionClass = "afwTextAlignLeft";
                quizUserDetailsForm.addColumn();
                quizUserDetailsForm.columnCaption = "User";
                quizUserDetailsForm.columnCaptionClass = "afwTextAlignCenter afwWidth400px";
                quizUserDetailsForm.addColumn();
                quizUserDetailsForm.columnCaption = "Date";
                quizUserDetailsForm.columnCaptionClass = "afwTextAlignCenter afwWidth200px";
                quizUserDetailsForm.addColumn();
                quizUserDetailsForm.columnCaption = "Attempt";
                quizUserDetailsForm.columnCaptionClass = "afwTextAlignCenter afwWidth200px";
              
                //List<QuizModel> quizList = QuizModel.getQuizList(cp);
                List<QuizResponseModel> quizResponseList = QuizResponseModel.GetResponseList(cp, quiz.id);
                foreach (QuizResponseModel quizResponse in quizResponseList)
                {
                    MemberModel member = MemberModel.create(cp, cp.Doc.GetInteger(constants.rnMemberId));
                    quizUserDetailsForm.addRow();
                    quizUserDetailsForm.setCell(quizResponse.QuizID.ToString());                  
                    quizUserDetailsForm.setCell(quizResponse.MemberID.ToString());
                    quizUserDetailsForm.setCell(quizResponse.DateAdded.ToString());
                    quizUserDetailsForm.setCell(quizResponse.attemptNumber.ToString());
                };

                adminFramework.formSimpleClass outerForm = new adminFramework.formSimpleClass();
                outerForm.addFormHidden(constants.rnQuizId, quiz.id.ToString());

                // -- wrap in tabs and output finished form
                result = ReportingFilterForm.getHtml(cp) + quizUserDetailsForm.getHtml(cp);
                result = genericController.getTabWrapper(cp, result, "Reporting", quiz.id);
                //cp.Doc.AddHeadStyle(gradingForm.styleSheet);
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



