
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning.Interfaces
{
   public class quizDetailsClass
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
                string button = cp.Doc.GetProperty(Constants.rnButton, "");
                CPCSBaseClass cs = cp.CSNew();
                //
                if (button != "")
                {
                    genericController.checkRequiredFieldText(cp, Constants.rnSampleField, "Sample Field");
                    //
                    if (cp.UserError.OK())
                    {
                        if (!cs.Open(Constants.cnApps, "id=" + appId, "", true, "", 1, 1))
                        {
                            cs.Close();
                            cs.Insert(Constants.cnApps);
                        }
                        cs.SetField("sampleField", cp.Doc.GetProperty(Constants.rnSampleField, ""));
                        cs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport( ex, "processForm");
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
                //string js = "";
                CPCSBaseClass cs = cp.CSNew();
                adminFramework.formSimpleClass form = new adminFramework.formSimpleClass();
                string qs = "";
                int responseId = cp.Utils.EncodeInteger(cp.Doc.get_Var("id"));
                //
                //
                // return converted layout
                //
                form.title = "Quiz Details";
                qs = rqs;
                qs = cp.Utils.ModifyQueryString(qs, Constants.rnDstFormId, Constants.formIdQuizList.ToString(), true);
                //
                cp.Doc.set_Var("id", responseId.ToString());
                form.body = ""
                    + Constants.cr + "<div class=\"\">return to <a href=\"?" + qs + "\">Quiz List</a></div>"
                    + Constants.cr2 + "<div class=\"onlineQuiz\">"
                    + cp.Utils.ExecuteAddon(Constants.scoreCardAddon )
                    + Constants.cr2 + "</div>"
                    + "";
                s = form.getHtml(cp);
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport( ex, "getForm");
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
