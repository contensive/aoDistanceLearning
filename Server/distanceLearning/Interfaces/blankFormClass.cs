
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning.Interfaces
{
    class blankClass
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
                string form = "";
                CPBlockBaseClass layout = cp.BlockNew();
                string js = "";
                CPCSBaseClass cs = cp.CSNew();
                //
                // open layout, grab form, add hiddens, replace back into layout
                //
                layout.OpenLayout("layout");
                form = layout.GetInner("#fbForm");
                form += cp.Html.Hidden(Constants.rnSrcFormId, dstFormId.ToString(), "", "");
                form += cp.Html.Hidden(Constants.rnAppId, appId.ToString(), "", "");
                if (!cp.UserError.OK())
                {
                    form = cp.Html.div(cp.UserError.GetList(), "", "", "") + form;
                }
                form = cp.Html.Form(form, "", "", "", "", "");
                layout.SetOuter("#fbForm", form);
                //
                // Populate the layout
                // attempt to open the application record. It is created in the process so this may fail.
                //      if not cs.OK(), the getFormField will return blank.
                //
                cs.Open(Constants.cnApps, "id=" + appId.ToString(), "", true, "", 1, 1);
                if (true)
                {
                    //
                    // either server-side
                    //
                    layout.SetInner("#fbSampleFile .rowValue", cp.Html.InputText("fbSampleField", genericController.getFormField(cp, cs, Constants.rnSampleField), "", "", false, "", ""));
                }
                else
                {
                    //
                    // or client-side
                    //
                    //js += statics.cr + "jQuery('-------').html('" + statics.getFormField(cp, cs, statics.rnSampleField) + "')";
                }
                cs.Close();
                //
                // apply any javascript to doc
                //
                if (js != "")
                {
                    cp.Doc.AddHeadJavascript("jQuery(document).ready(function(){" + js + Constants.cr + "});");
                }
                //
                // return converted layout
                //
                s = layout.GetHtml();
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
