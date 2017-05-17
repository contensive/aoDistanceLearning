using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning.Interfaces
{
    //
    // 1) Change the namespace to the collection name
    // 2) Change this class name to the addon name
    // 3) Create a Contensive Addon record with the namespace apCollectionName.ad
    //
    public class quizReportClass : Contensive.BaseClasses.AddonBaseClass
    {
        //
        // execute method is the only public
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string s = "Hello World";
            try
            {
                // rqs defined once and passed so when we do ajax integration, the rqs will be passed from
                //      from the ajax call so it can be the rqs of the original page, not the /remoteMethod rqs
                //      you get from cp.doc.RefreshQueryString.
                // srcFormId - passed from a submitting form. Otherwise 0.
                // dstFormId - the next for to display. used for links to new pages. Will be over-ridden
                //      by the formProcessing of srcFormId if it is present.
                // appId - Forms typically save data back to the Db. The 'application' is the table
                //      when the data is saved.
                // rightNow - the date and time when the page is hit. Set once and passed as argument to
                //      enable a test-mode where the time can be hard-coded.
                //
                int srcFormId = cp.Utils.EncodeInteger(cp.Doc.GetProperty(constants.rnSrcFormId, ""));
                int dstFormId = cp.Utils.EncodeInteger(cp.Doc.GetProperty(constants.rnDstFormId, ""));
                int appId = cp.Utils.EncodeInteger(cp.Doc.GetProperty(constants.rnAppId, ""));
                string rqs = cp.Doc.RefreshQueryString;
                DateTime rightNow = DateTime.Now;
                CPCSBaseClass cs = cp.CSNew();
                adminFramework.pageWithNavClass page = new adminFramework.pageWithNavClass();
                //
                blankClass blank = new blankClass();
                quizListClass quizList = new quizListClass();
                quizDetailsClass quizDetails = new quizDetailsClass();
                //
                //------------------------------------------------------------------------
                // process submitted form
                //------------------------------------------------------------------------
                //
                if (srcFormId != 0)
                {
                    switch (srcFormId)
                    {

                        case constants.formIdQuizList:
                            dstFormId = quizList.processForm(cp, srcFormId, rqs, rightNow, ref appId);
                            break;
                        case constants.formIdQuizDetails:
                            dstFormId = quizDetails.processForm(cp, srcFormId, rqs, rightNow, ref appId);
                            break;
                        case constants.formIdBlank:
                            dstFormId = blank.processForm(cp, srcFormId, rqs, rightNow, ref appId);
                            break;
                    }
                }
                //
                //------------------------------------------------------------------------
                // get the next form
                //------------------------------------------------------------------------
                //
                page.title = "Quiz Results";
                page.description = "";
                //
                switch (dstFormId)
                {
                    case (constants.formIdQuizDetails):
                        page.body = quizDetails.getForm(cp, dstFormId, rqs, rightNow, ref appId);
                        break;
                    case (constants.formIdBlank):
                        page.body = blank.getForm(cp, dstFormId, rqs, rightNow, ref appId);
                        break;
                    default:
                        page.body = quizList.getForm(cp, dstFormId, rqs, rightNow, ref appId);
                        break;
                }
                s = page.getHtml(cp);
                cp.Doc.AddHeadStyle(page.styleSheet);
            }
            catch (Exception ex)
            {
                errorReport(cp, ex, "execute");
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
