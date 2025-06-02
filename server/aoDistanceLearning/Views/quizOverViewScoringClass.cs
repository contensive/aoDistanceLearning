
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;

namespace Contensive.Addons.DistanceLearning {
    public class QuizOverViewScoringClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            string result = "";
            try {
                QuizModel quiz = DbBaseModel.create<QuizModel>(cp, cp.Doc.GetInteger(Constants.rnQuizId));
                string qs;
                if (quiz == null) {
                    //
                    // -- no quiz provided, go back to quiz list
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button) {
                    case Constants.buttonCancel:
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, quiz.id.ToString());
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails);
                        cp.Response.Redirect("?" + qs);
                        break;
                    case Constants.buttonSave:
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
                        //quiz.FPercentile = cp.Doc.GetNumber("FPercentile");
                        quiz.FPassingGrade = cp.Doc.GetBoolean("FPassingGrade");
                        quiz.certificateTypeId = cp.Doc.GetInteger("certificateTypeId");
                        quiz.certificationTypeId = cp.Doc.GetInteger(Constants.rnCertificationTypeId);
                        quiz.certificationCECs = cp.Doc.GetNumber("certificationCECs");
                        quiz.addSuccessCopy = cp.Doc.GetBoolean("addSuccessCopy");
                        quiz.successCopy = cp.Doc.GetText("successCopy");
                        quiz.save(cp);
                        break;
                }
                //
                // -- create the upper part of the page, the list of scoring
                BaseClasses.LayoutBuilder.LayoutBuilderListBaseClass layout = cp.AdminUI.CreateLayoutBuilderList();
                layout.title = "Grading";
                layout.isOuterContainer = false;
                layout.includeBodyColor = false;
                layout.includeBodyPadding = false;
                //
                layout.addColumn();
                layout.columnCaption = "Percentile";
                layout.columnCaptionClass = "";
                layout.columnDownloadable = false;
                layout.columnVisible = true;
                layout.columnSortable = false;
                layout.columnCellClass = "afwTextAlignCenter afwWidth50px";
                layout.columnName = "quizGradingPercentile";
                //
                layout.addColumn();
                layout.columnCaption = "Pass";
                layout.columnCaptionClass = "";
                layout.columnDownloadable = false;
                layout.columnVisible = true;
                layout.columnSortable = false;
                layout.columnCellClass = "afwTextAlignCenter afwWidth50px";
                layout.columnName = "quizGradingSuccess";
                //
                layout.addColumn();
                layout.columnCaption = "Caption";
                layout.columnCaptionClass = "";
                layout.columnDownloadable = false;
                layout.columnVisible = true;
                layout.columnSortable = false;
                layout.columnCellClass = "afwTextAlignLeft";
                layout.columnName = "quizGradingCaptions";
                //
                layout.addRow();
                layout.setCell(cp.Html.InputText("aPercentile", quiz.APercentile.ToString(), "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:right;\">"));
                layout.setCell(cp.Html.CheckBox("aPassingGrade", quiz.APassingGrade, "afwInput"));
                layout.setCell(cp.Html.InputText("aCaption", quiz.ACaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                layout.addRow();
                layout.setCell(cp.Html.InputText("bPercentile", quiz.BPercentile.ToString(), "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:right;\">"));
                layout.setCell(cp.Html.CheckBox("bPassingGrade", quiz.BPassingGrade, "afwInput"));
                layout.setCell(cp.Html.InputText("bCaption", quiz.BCaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                layout.addRow();
                layout.setCell(cp.Html.InputText("cPercentile", quiz.CPercentile.ToString(), "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:right;\">"));
                layout.setCell(cp.Html.CheckBox("cPassingGrade", quiz.CPassingGrade, "afwInput"));
                layout.setCell(cp.Html.InputText("cCaption", quiz.CCaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                layout.addRow();
                layout.setCell(cp.Html.InputText("dPercentile", quiz.DPercentile.ToString(), "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:right;\">"));
                layout.setCell(cp.Html.CheckBox("dPassingGrade", quiz.DPassingGrade, "afwInput"));
                layout.setCell(cp.Html.InputText("dCaption", quiz.DCaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                layout.addRow();
                layout.setCell("&nbsp;");
                layout.setCell(cp.Html.CheckBox("fPassingGrade", quiz.FPassingGrade, "afwInput"));
                layout.setCell(cp.Html.InputText("fCaption", quiz.FCaption, "", "", false, "afwInput").Replace(">", " style=\"width:100%;text-align:left;\">"));
                //
                BaseClasses.LayoutBuilder.LayoutBuilderNameValueBaseClass certLayout = cp.AdminUI.CreateLayoutBuilderNameValue();
                certLayout.title = "Certifications";
                certLayout.isOuterContainer = false;
                certLayout.includeBodyColor = false;
                certLayout.includeBodyPadding = false;
                // 
                certLayout.addRow();
                certLayout.rowName = "Add CECs";
                certLayout.rowValue = "&nbsp;If passing grade, add CECs to user's account"
                    + cp.Html.div(cp.Html.InputText(Constants.rnCertificationCECs, quiz.certificationCECs.ToString(), "", "", false, "afwRowTextInput afwInput") + " CECs towards Certification Type " + cp.Html.SelectContent(Constants.rnCertificationTypeId, quiz.certificationTypeId.ToString(), Constants.cnCertificationTypes, "", "No Certification Types", "afwInput"), "", "afwRowValueHelpBox")
                    + "";
                certLayout.rowHelp = "";
                // 
                certLayout.addRow();
                certLayout.rowName = "Add Certificate";
                certLayout.rowValue = "&nbsp;If passing grade, add Certificate record to user's account"
                    + cp.Html.div(cp.Html.SelectContent(Constants.rnCertificateTypeId, quiz.certificateTypeId.ToString(), Constants.cnCertificateTypes, "", "No Certificate", "afwInput"), "", "afwRowValueHelpBox")
                    + "";
                certLayout.rowHelp = "";
                // 
                certLayout.addRow();
                certLayout.rowName = "Success Message";
                certLayout.rowValue = cp.Html.CheckBox(Constants.rnAddSuccessCopy, quiz.addSuccessCopy, "afwInput")
                    + "If passing grade, add text to results page"
                    + cp.Html.div(cp.Html.InputWysiwyg(Constants.rnSuccessCopy, quiz.successCopy, CPHtmlBaseClass.EditorUserScope.CurrentUser, CPHtmlBaseClass.EditorContentScope.Page, "", "", "afwInput"), "", "afwRowValueHelpBox");
                //
                BaseClasses.LayoutBuilder.LayoutBuilderBaseClass outerForm = cp.AdminUI.CreateLayoutBuilder();
                outerForm.addFormButton(Constants.buttonSave);
                outerForm.addFormButton(Constants.buttonCancel);
                outerForm.addFormHidden(Constants.rnQuizId, quiz.id.ToString());
                outerForm.body = layout.getHtml() + certLayout.getHtml();
                //
                // -- wrap in tabs and output finished form
                result = outerForm.getHtml();
                result = GenericController.getTabWrapper(cp, result, "Scoring", quiz);
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
