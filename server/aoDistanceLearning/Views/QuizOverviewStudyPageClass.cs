using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;

namespace Contensive.Addons.DistanceLearning {
    public class QuizOverviewStudyPageClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            string result = "";
            try {
                string innerBody = "";
                //
                QuizModel quiz = DbBaseModel.create<QuizModel>(cp, cp.Doc.GetInteger(Constants.rnQuizId));
                if (quiz == null) {
                    string qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureDashboard, true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                string button = cp.Doc.GetText("button");
                switch (button) {
                    case "Save":
                        quiz.customTopCopy = cp.Doc.GetText(nameof(quiz.customTopCopy));
                        quiz.videoEmbedCode = cp.Doc.GetText(nameof(quiz.videoEmbedCode));
                        //
                        // todo -- need to use cp.db.CreateUploadFieldPath()
                        string courseMaterialFieldName = nameof(quiz.courseMaterial);
                        string courseMaterialPathFilename = cp.Db.CreateUploadFieldPathFilename(QuizModel.tableMetadata.tableNameLower, courseMaterialFieldName, quiz.id, cp.Doc.GetText(courseMaterialFieldName), CPContentBaseClass.FieldTypeIdEnum.File);
                        string courseMaterialPath = courseMaterialPathFilename.Replace(courseMaterialPathFilename, "");
                        cp.CdnFiles.SaveUpload(courseMaterialFieldName, courseMaterialPath, ref courseMaterialPathFilename);
                        quiz.courseMaterial = courseMaterialPathFilename;
                        //
                        //
                        quiz.courseMaterial = cp.Doc.GetText(nameof(quiz.courseMaterial));
                        quiz.studyCopy.content = cp.Doc.GetText(nameof(quiz.studyCopy));

                        quiz.save(cp);
                        break;
                    case "Cancel":
                        string qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                        qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, quiz.id.ToString(), true);
                        cp.Response.Redirect("?" + qs);
                        break;
                }
                //
                BaseClasses.LayoutBuilder.LayoutBuilderNameValueBaseClass layout = cp.AdminUI.CreateLayoutBuilderNameValue();
                layout.isOuterContainer = false;
                layout.addFormHidden(Constants.rnQuizId, quiz.id.ToString());
                layout.body = innerBody;
                layout.addFormButton("Save", "button");
                layout.addFormButton("Cancel", "button");
                layout.isOuterContainer = false;
                layout.title = "<b>Study Page</b></br>";
                //
                layout.addRow();
                layout.rowName = "Video Embed Code";
                layout.rowValue = cp.Html.InputTextExpandable("videoEmbedCode", quiz.videoEmbedCode, 5);
                layout.rowHelp = "When included, a video can be presented on the study page.";
                //
                layout.addRow();
                layout.rowName = "Study Page Text";
                layout.rowValue = cp.Html.InputWysiwyg("studyCopy", quiz.studyCopy.content, CPHtmlBaseClass.EditorUserScope.CurrentUser, CPHtmlBaseClass.EditorContentScope.Page);
                layout.rowHelp = "This is the list of instructions that go on the study Page. You can describe the quiz, it's purpose, how to take it, etc.";
                //
                layout.addRow();
                layout.rowName = "Course Materials";
                layout.rowValue = cp.AdminUI.GetFileEditor("courseMaterial");
                layout.rowHelp = "When included, a file can be uploaded on the study page.";

                result = GenericController.getTabWrapper(cp, layout.getHtml(), "Study", quiz);

                cp.Doc.AddHeadStyle(layout.styleSheet);
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