using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
   public class quizOverviewSelectClass : Contensive.BaseClasses.AddonBaseClass   
        {
            public override object Execute(CPBaseClass cp)
            {
            string result = "";
            try
            {
               
                string qs;
                string qsBase;
                string rqs = "";
                string innerBody = "";
                CPCSBaseClass cs = cp.CSNew();
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger("QuizId"));
                if (quiz == null)
                {
                    return "";
                }
                else
                {
                    //cp.Doc.AddRefreshQueryString("quizId", quiz.id.ToString());
                    //cp.Doc.AddRefreshQueryString("addonId", "");
                    //cp.Doc.AddRefreshQueryString("addonGuid", constants.quizOverViewSelectAddon); 
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
                    //qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnAddonguid, constants.quizOverViewSettingsAddon, true);
                    int setportal = cp.Doc.GetInteger("");
                    //qsBase = cp.Utils.ModifyQueryString(cp.Doc.RefreshQueryString, constants.rnAddonId, constants.portalAddonId, true);
                    qsBase = cp.Doc.RefreshQueryString;
                    qsBase = cp.Utils.ModifyQueryString(qsBase, "dstFeatureGuid", constants.portalSettingPageAddon, true);

                    adminFramework.formNameValueRowsClass form = new adminFramework.formNameValueRowsClass();
                    qs = cp.Utils.ModifyQueryString(qsBase, "QuizId", quiz.id.ToString(), true);                   
                    form.isOuterContainer = false;
                    form.body = innerBody;
                    //form.addFormButton("Save", "button");
                    form.addFormButton("Cancel", "button");
                    form.addRow();
                    form.title = "<b>Quiz Name: My Sample Quiz </b></br>";
                    form.addRow();        
                    form.rowValue = ( "<div><a href=\"?" + qs +  "\">One question perpage: subjects; Users can retake quiz; max 5 questions</a></div>");
                    //
                    result = genericController.getTabWrapper(cp, form.getHtml(cp), "Details" );
                       
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


