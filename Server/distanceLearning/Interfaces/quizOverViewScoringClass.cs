
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
                        //quiz.aPassingGrade = cp.Doc.GetBoolean("aPassingGrade");
                        quiz.BCaption = cp.Doc.GetText("BCaption");
                        quiz.BPercentile = cp.Doc.GetNumber("BPercentile");
                        //quiz.bPassingGrade = cp.Doc.GetBoolean("bPassingGrade");
                        quiz.CCaption = cp.Doc.GetText("CCaption");
                        quiz.CPercentile = cp.Doc.GetNumber("CPercentile");
                        //quiz.cPassingGrade = cp.Doc.GetBoolean("cPassingGrade");
                        quiz.DCaption = cp.Doc.GetText("DCaption");
                        quiz.DPercentile = cp.Doc.GetNumber("DPercentile");
                        //quiz.dPassingGrade = cp.Doc.GetBoolean("dPassingGrade");
                        quiz.FCaption = cp.Doc.GetText("FCaption");
                        quiz.FPercentile = cp.Doc.GetNumber("FPercentile");
                        //quiz.fPassingGrade = cp.Doc.GetBoolean("fPassingGrade");
                        quiz.saveObject(cp);
                        break;
                }

                //
                // -- create the upper part of the page, the list of scoring
                adminFramework.reportListClass gradingForm = new adminFramework.reportListClass(cp);
                gradingForm.isOuterContainer = false;
                gradingForm.addColumn();
                gradingForm.columnCaption = "Captions";
                gradingForm.columnCaptionClass = "";
                gradingForm.columnDownloadable = false;
                gradingForm.columnVisible = true;
                gradingForm.columnSortable = false;
                gradingForm.columnCellClass = "";
                gradingForm.columnName = "quizGradingCaptions";
                //
                gradingForm.addColumn();
                gradingForm.columnCaption = "Percentile";
                gradingForm.columnCaptionClass = "";
                gradingForm.columnDownloadable = false;
                gradingForm.columnVisible = true;
                gradingForm.columnSortable = false;
                gradingForm.columnCellClass = "";
                gradingForm.columnName = "quizGradingPercentile";
                //
                gradingForm.addColumn();
                gradingForm.columnCaption = "Passing Grade";
                gradingForm.columnCaptionClass = "";
                gradingForm.columnDownloadable = false;
                gradingForm.columnVisible = true;
                gradingForm.columnSortable = false;
                gradingForm.columnCellClass = "";
                gradingForm.columnName = "quizGradingSuccess";
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("aCaption", quiz.ACaption));
                gradingForm.setCell(cp.Html.InputText("aPercentile", quiz.APercentile.ToString()));
                gradingForm.setCell(cp.Html.CheckBox("aPassingGrade", true));
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("bCaption", quiz.BCaption));
                gradingForm.setCell(cp.Html.InputText("bPercentile", quiz.BPercentile.ToString()));
                gradingForm.setCell(cp.Html.CheckBox("bPassingGrade", true));
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("cCaption", quiz.CCaption));
                gradingForm.setCell(cp.Html.InputText("cPercentile", quiz.CPercentile.ToString()));
                gradingForm.setCell(cp.Html.CheckBox("cPassingGrade", true));
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("dCaption", quiz.DCaption));
                gradingForm.setCell(cp.Html.InputText("dPercentile", quiz.DPercentile.ToString()));
                gradingForm.setCell(cp.Html.CheckBox("dPassingGrade", true));
                //
                gradingForm.addRow();
                gradingForm.setCell(cp.Html.InputText("fCaption", quiz.DCaption));
                gradingForm.setCell("&nbsp;");
                gradingForm.setCell(cp.Html.CheckBox("dPassingGrade", false));
                //
                adminFramework.formNameValueRowsClass scoringForm = new adminFramework.formNameValueRowsClass();
                // 
                scoringForm.addRow();
                scoringForm.rowName = "Add CECs";
                scoringForm.rowValue = cp.Html.CheckBox("", true)
                    + "If Success completion add CECs to user's account"
                    + cp.Html.div("---select certification CECs---", "", "afwRowValueHelpBox");
                // 
                scoringForm.addRow();
                scoringForm.rowName = "Add Certificate";
                scoringForm.rowValue = cp.Html.CheckBox("", true)
                    + "If Success completion add Certificate record to user's account"
                    + cp.Html.div("---select certification type---", "", "afwRowValueHelpBox");
                // 
                scoringForm.addRow();
                scoringForm.rowName = "Success Message";
                scoringForm.rowValue = cp.Html.CheckBox("", true)
                    + "If Success completion add text to results page"
                    + cp.Html.div(cp.Html.InputWysiwyg("asdf", "sample text"), "", "afwRowValueHelpBox");
                //
                adminFramework.formSimpleClass outerForm = new adminFramework.formSimpleClass();
                outerForm.addFormButton(constants.buttonSave);
                outerForm.addFormButton(constants.buttonCancel);
                outerForm.addFormHidden(constants.rnQuizId, quiz.id.ToString());
                outerForm.body = gradingForm.getHtml(cp) + scoringForm.getHtml(cp);
                //
                // -- wrap in tabs and output finished form
                result = outerForm.getHtml(cp);
                result = genericController.getTabWrapper(cp, result, "Scoring", quiz.id);
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





