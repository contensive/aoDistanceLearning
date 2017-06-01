﻿using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
   public class quizOverviewDetailsClass : Contensive.BaseClasses.AddonBaseClass   
        {
            public override object Execute(CPBaseClass cp)
            {
            string result = "";
            try
            {
                string qs;
                //
                // -- process buttons
                string button = cp.Doc.GetText("button");
                switch (button)
                {
                    case "Cancel":
                        //
                        // -- no quiz provided, go back to dashboard (can't tell which feature is dashboard so just go to landing for this portal)
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", "");
                        cp.Response.Redirect("?" + qs);
                        return "";
                }
                string innerBody = "";
                CPCSBaseClass cs = cp.CSNew();
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger("QuizId"));
                if (quiz == null)
                {
                    //
                    // -- no id passed in, test if a name was included
                    if (!string.IsNullOrEmpty(cp.Doc.GetText("quizName")))
                    {
                        //
                        // -- create new quiz from name
                        quiz = QuizModel.add(cp);
                        quiz.name = cp.Doc.GetText("quizName");
                        quiz.saveObject(cp);
                    }
                    else
                    {
                        //
                        // -- no quiz provided, go back to dashboard (can't tell which feature is dashboard so just go to landing for this portal)
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", "");
                        cp.Response.Redirect("?" + qs);
                        return "";
                    }
                }
                //
                // -- setup form
                adminFramework.formNameValueRowsClass form = new adminFramework.formNameValueRowsClass();
                form.isOuterContainer = false;
                form.body = innerBody;
                form.addFormButton("Cancel", "button");
                form.title = quiz.name;
                //
                // -- add row
                form.addRow();
                form.rowName = "Questions";
                qs = cp.Doc.RefreshQueryString;
                qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewSetting, true);
                qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                form.rowValue = ("<div><a href=\"?" + qs + "\">One question per page: subjects; Users can retake quiz; max 5 questions</a></div>");
                //
                // -- wrap in tabs and output finished form
                result = genericController.getTabWrapper(cp, form.getHtml(cp), "Details",quiz.id);
                cp.Doc.AddHeadStyle(form.styleSheet);
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

