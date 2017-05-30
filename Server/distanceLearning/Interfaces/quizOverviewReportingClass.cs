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
        public override object Execute(CPBaseClass cp)
        {
            string result = "Hello World";
            try
            {
                string qs;
                string qsBase;
                string rqs = "";
                CPCSBaseClass cs = cp.CSNew();
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger("QuizId"));
                if (quiz == null)
                {
                    return "";
                }
                else
                {
                    cp.Doc.AddRefreshQueryString("quizId", quiz.id.ToString());
                    cp.Doc.AddRefreshQueryString("addonId", "");
                    cp.Doc.AddRefreshQueryString("addonGuid", constants.quizOverViewSelectAddon);
                    //

                    string button = cp.Doc.GetText("button");
                    switch (button)
                    {

                        case "Save":
                            quiz.saveObject(cp);
                            break;
                        case "Cancel":
                            return "";
                    }
                    //
                    qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnAddonguid, constants.quizOverViewReportingAddon, true);
                    adminFramework.formNameValueRowsClass form = new adminFramework.formNameValueRowsClass();
                    qs = cp.Utils.ModifyQueryString(qsBase, "QuizId", cs.GetInteger("responseId").ToString(), true);
                    form.isOuterContainer = false;
                    result = genericController.getTabWrapper(cp, form.getHtml(cp), "Reporting");

                    cp.Doc.AddHeadStyle(form.styleSheet);

                }
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



