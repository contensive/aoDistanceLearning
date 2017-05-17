
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.DistanceLearning3
{
    class quizDetailsClass
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
                string button = cp.Doc.GetProperty(statics.rnButton, "");
                CPCSBaseClass cs = cp.CSNew();
                //
                if (button != "")
                {
                    statics.checkRequiredFieldText(cp, statics.rnSampleField, "Sample Field");
                    //
                    if (cp.UserError.OK())
                    {
                        if (!cs.Open(statics.cnApps, "id=" + appId, "", true, "", 1, 1))
                        {
                            cs.Close();
                            cs.Insert(statics.cnApps);
                        }
                        cs.SetField("sampleField", cp.Doc.GetProperty(statics.rnSampleField, ""));
                        cs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                errorReport(cp, ex, "processForm");
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
                qs = cp.Utils.ModifyQueryString(qs, statics.rnDstFormId, statics.formIdQuizList.ToString(), true);
                //
                cp.Doc.set_Var("id", responseId.ToString());
                form.body = ""
                    + statics.cr + "<div class=\"\">return to <a href=\"?" + qs + "\">Quiz List</a></div>"
                    + statics.cr2 + "<div class=\"onlineQuiz\">"
                    + cp.Utils.ExecuteAddon(statics.scoreCardAddon )
                    + statics.cr2 + "</div>"
                    + "";
                s = form.getHtml(cp);
            }
            catch (Exception ex)
            {
                errorReport(cp, ex, "getForm");
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
