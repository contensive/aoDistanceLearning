
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;


namespace Contensive.Addons.DistanceLearning
{
   public class quizOverViewScoringClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            try
            {
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger(constants.rnQuizId));
                string qs;
                if (quiz == null)
                {
                    //
                    // -- no quiz provided, go back to quiz list
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button)
                {
                    case constants.buttonCancel:
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, constants.rnQuizId, quiz.id.ToString());
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeaturesQuizOverviewDetails);
                        cp.Response.Redirect("?" + qs);
                        break;
                    case constants.buttonSave:
                        quiz.ACaption = cp.Doc.GetText("aCaption");
                        quiz.APercentile = cp.Doc.GetNumber("APercentile");
                        quiz.APassingGrade = cp.Doc.GetBoolean("aPassingGrade");
                        quiz.BCaption = cp.Doc.GetText("BCaption");
                        quiz.BPercentile = cp.Doc.GetNumber("BPercentile");
                        quiz.BPassingGrade = cp.Doc.GetBoolean("BPassingGrade");
                        quiz.CCaption = cp.Doc.GetText("CCaption");
                        quiz.CPercentile = cp.Doc.GetNumber("CPercentile");
                        quiz.CPassingGrade = cp.Doc.GetBoolean("CPassingGrade");
                        quiz.DCaption = cp.Doc.GetText("DCaption");
                        quiz.DPercentile = cp.Doc.GetNumber("DPercentile");
                        quiz.DPassingGrade = cp.Doc.GetBoolean("DPassingGrade");
                        quiz.FCaption = cp.Doc.GetText("FCaption");
                        quiz.FPercentile = cp.Doc.GetNumber("FPercentile");
                        quiz.FPassingGrade = cp.Doc.GetBoolean("FPassingGrade");
                        quiz.certificateTypeId = cp.Doc.GetInteger("certificateTypeId");
                        quiz.certificationTypeId = cp.Doc.GetInteger(constants.rnCertificationTypeId);
                        quiz.certificationCECs = cp.Doc.GetNumber("certificationCECs");
                        quiz.addSuccessCopy = cp.Doc.GetBoolean("addSuccessCopy");
                        quiz.successCopy = cp.Doc.GetText("successCopy");
                        quiz.saveObject(cp);
                        break;
                }

                //
                // -- create the upper part of the page, the list of scoring
                adminFramework.ReportListClass gradingForm = new adminFramework.ReportListClass(cp);
                gradingForm.title = "Grading";
                gradingForm.isOuterContainer = false;
                gradingForm.includeBodyColor = false;
                gradingForm.includeBodyPadding = false;
                //
                gradingForm.addColumn();
                gradingForm.columnCaption = "Percentile";
                gradingForm.columnCaptionClass = "";
                gradingForm.columnDownloadable = false;
                gradingForm.columnVisible = true;
                gradingForm.columnSortable = false;
                gradingForm.columnCellClass = "afwTextAlignCenter afwWidth50px";
                gradingForm.columnName = "quizGradingPercentile";
                //
                gradingForm.addColumn();
                gradingForm.columnCaption = "Pass";
                gradingForm.columnCaptionClass = "";
                gradingForm.columnDownloadable = false;
                gradingForm.columnVisible = true;
                gradingForm.columnSortable = false;
                gradingForm.columnCellClass = "afwTextAlignCenter afwWidth50px";
                gradingForm.columnName = "quizGradingSuccess";
                //
                gradingForm.addColumn();
                gradingForm.columnCaption = "Caption";
                gradingForm.columnCaptionClass = "";
                gradingForm.columnDownloadable = false;
                gradingForm.columnVisible = true;
                gradingForm.columnSortable = false;
                gradingForm.columnCellClass = "afwTextAlignLeft";
                gradingForm.columnName = "quizGradingCaptions";
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("aPercentile", quiz.APercentile.ToString(),"","",false,"afwInput").Replace(">", " style=\"width:100%;text-align:right;\">"));
                gradingForm.setCell(cp.Html.CheckBox("aPassingGrade", quiz.APassingGrade, "afwInput"));
                gradingForm.setCell(cp.Html.InputText("aCaption", quiz.ACaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("bPercentile", quiz.BPercentile.ToString(), "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:right;\">"));
                gradingForm.setCell(cp.Html.CheckBox("bPassingGrade", quiz.BPassingGrade, "afwInput"));
                gradingForm.setCell(cp.Html.InputText("bCaption", quiz.BCaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("cPercentile", quiz.CPercentile.ToString(), "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:right;\">"));
                gradingForm.setCell(cp.Html.CheckBox("cPassingGrade", quiz.CPassingGrade, "afwInput"));
                gradingForm.setCell(cp.Html.InputText("cCaption", quiz.CCaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("dPercentile", quiz.DPercentile.ToString(), "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:right;\">"));
                gradingForm.setCell(cp.Html.CheckBox("dPassingGrade", quiz.DPassingGrade, "afwInput"));
                gradingForm.setCell(cp.Html.InputText("dCaption", quiz.DCaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                gradingForm.addRow();
                gradingForm.setCell("&nbsp;");
                gradingForm.setCell(cp.Html.CheckBox("fPassingGrade", quiz.FPassingGrade, "afwInput"));
                gradingForm.setCell(cp.Html.InputText("fCaption", quiz.FCaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                adminFramework.formNameValueRowsClass certificationForm = new adminFramework.formNameValueRowsClass();
                certificationForm.title = "Certifications";
                certificationForm.isOuterContainer = false;
                certificationForm.includeBodyColor = false;
                certificationForm.includeBodyPadding = false;
                // 
                certificationForm.addRow();
                certificationForm.rowName = "Add CECs";
                certificationForm.rowValue = "&nbsp;If passing grade, add CECs to user's account"
                    + cp.Html.div( cp.Html.InputText(constants.rnCertificationCECs, quiz.certificationCECs.ToString(),"","",false, "afwRowTextInput afwInput") + " CECs towards Certification Type " + cp.Html.SelectContent(constants.rnCertificationTypeId, quiz.certificationTypeId.ToString(), constants.cnCertificationTypes, "", "No Certification Types", "afwInput"),"", "afwRowValueHelpBox")
                    + "";
                certificationForm.rowHelp = "";
                // 
                certificationForm.addRow();
                certificationForm.rowName = "Add Certificate";
                certificationForm.rowValue = "&nbsp;If passing grade, add Certificate record to user's account"
                    + cp.Html.div(cp.Html.SelectContent(constants.rnCertificateTypeId, quiz.certificateTypeId.ToString(), constants.cnCertificateTypes, "", "No Certificate", "afwInput"), "", "afwRowValueHelpBox")
                    + "";
                certificationForm.rowHelp = "";
                // 
                certificationForm.addRow();
                certificationForm.rowName = "Success Message";
                certificationForm.rowValue = cp.Html.CheckBox(constants.rnAddSuccessCopy, quiz.addSuccessCopy, "afwInput")
                    + "If passing grade, add text to results page"
                    + cp.Html.div(cp.Html.InputWysiwyg(constants.rnSuccessCopy, quiz.successCopy,CPHtmlBaseClass.EditorUserScope.CurrentUser,CPHtmlBaseClass.EditorContentScope.Page,"","","afwInput"), "", "afwRowValueHelpBox");
                //
                adminFramework.formSimpleClass outerForm = new adminFramework.formSimpleClass();
                outerForm.addFormButton(constants.buttonSave);
                outerForm.addFormButton(constants.buttonCancel);
                outerForm.addFormHidden(constants.rnQuizId, quiz.id.ToString());
                outerForm.body = gradingForm.getHtml(cp) + certificationForm.getHtml(cp);
                //
                // -- wrap in tabs and output finished form
                result = outerForm.getHtml(cp);
                result = genericController.getTabWrapper(cp, result, "Scoring", quiz);
                cp.Doc.AddHeadStyle(gradingForm.styleSheet);
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





