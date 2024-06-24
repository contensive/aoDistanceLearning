
using System;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning {
    namespace Views {
        public class QuizOverviewResponseDetailsClass : AddonBaseClass {
            public override object Execute(CPBaseClass cp) {
                string result = "";
                try {
                    int responseId = cp.Doc.GetInteger(Constants.rnResponseId);
                    QuizResponseModel response = DbBaseModel.create<QuizResponseModel>(cp, responseId);
                    QuizModel quiz = DbBaseModel.create<QuizModel>(cp, response.quizID);
                    var member = DbBaseModel.create<PersonModel>(cp, response.memberID);
                    if (member == null) {
                        member = DbBaseModel.addDefault<PersonModel>(cp, response.memberID);
                        member.name = "unknown";
                    }
                    if (cp.Doc.GetText(Constants.rnButton) == Constants.buttonCancel) {
                        // -- go back to response
                        string qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewResults, true);
                        qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, quiz.id.ToString(), true);
                        cp.Response.Redirect("?" + qs);
                        return "";
                    }
                    PortalFramework.LayoutBuilderSimple form = new PortalFramework.LayoutBuilderSimple();
                    form.title = "Quiz Response";
                    form.description = ""
                        + cp.Html.div("Date Completed: " + response.dateSubmitted.ToShortDateString())
                        + cp.Html.div("Participant: " + member.name)
                        + "";
                    cp.Doc.SetProperty("id", responseId.ToString());
                    form.body = cp.Html.div(cp.Utils.ExecuteAddon(Constants.scoreCardAddon), "", "onlineQuiz"); ;
                    form.addFormButton(Constants.buttonCancel);
                    form.addFormHidden(Constants.rnQuizId, quiz.id.ToString());
                    form.addFormHidden(Constants.rnResponseId, response.id.ToString());
                    result = form.getHtml(cp);
                    result = GenericController.getTabWrapper(cp, result, "Results", quiz);
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
}