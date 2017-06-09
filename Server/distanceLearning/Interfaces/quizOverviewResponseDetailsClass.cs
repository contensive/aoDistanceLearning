using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
    public class quizOverviewResponseDetailsClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            try
            {
                int responseId = cp.Doc.GetInteger("responseid");
                Models.QuizResponseModel response = Models.QuizResponseModel.create(cp, responseId);
                Models.QuizModel quiz = Models.QuizModel.create(cp, response.QuizID);
                if (cp.Doc.GetText(constants.rnButton) == constants.buttonCancel)
                {
                    // -- go back to response
                    string qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewResults, true);
                    qs = cp.Utils.ModifyQueryString(qs,  constants.rnQuizId, quiz.id.ToString() , true);
                    cp.Response.Redirect("?" + qs);
                    return "";
                }
                adminFramework.formSimpleClass form = new adminFramework.formSimpleClass();
                form.title = "Quiz Response";
                form.description = ""
                    + cp.Html.div("Date Completed: " + response.dateSubmitted.ToShortDateString() )
                    + cp.Html.div("Participant: #" + response.MemberID)
                    + "";
                cp.Doc.SetProperty("id", responseId.ToString());
                form.body = cp.Utils.ExecuteAddon(constants.scoreCardAddon);
                form.addFormButton(constants.buttonCancel);
                result = form.getHtml(cp);
                result = genericController.getTabWrapper(cp, result, "Results", quiz);
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


