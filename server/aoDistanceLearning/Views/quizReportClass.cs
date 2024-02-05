using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Views;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning {
    namespace Views {
        //
        // 1) Change the namespace to the collection name
        // 2) Change this class name to the addon name
        // 3) Create a Contensive Addon record with the namespace apCollectionName.ad
        //
        public class QuizReportClass : AddonBaseClass {
            //
            // execute method is the only public
            //
            public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
                string s = "Hello World";
                try {
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
                    int srcFormId = cp.Utils.EncodeInteger(cp.Doc.GetProperty(Constants.rnSrcFormId, ""));
                    int dstFormId = cp.Utils.EncodeInteger(cp.Doc.GetProperty(Constants.rnDstFormId, ""));
                    int appId = cp.Utils.EncodeInteger(cp.Doc.GetProperty(Constants.rnAppId, ""));
                    string rqs = cp.Doc.RefreshQueryString;
                    DateTime rightNow = DateTime.Now;
                    CPCSBaseClass cs = cp.CSNew();
                    adminFramework.pageWithNavClass page = new adminFramework.pageWithNavClass();
                    //
                    blankClass blank = new blankClass();
                    QuizListClass quizList = new QuizListClass();
                    QuizDetailsClass quizDetails = new QuizDetailsClass();
                    //
                    //------------------------------------------------------------------------
                    // process submitted form
                    //------------------------------------------------------------------------
                    //
                    if (srcFormId != 0) {
                        switch (srcFormId) {

                            case Constants.formIdQuizList:
                                dstFormId = quizList.processForm(cp, srcFormId, rqs, rightNow, ref appId);
                                break;
                            case Constants.formIdQuizDetails:
                                dstFormId = quizDetails.processForm(cp, srcFormId, rqs, rightNow, ref appId);
                                break;
                            case Constants.formIdBlank:
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
                    switch (dstFormId) {
                        case (Constants.formIdQuizDetails):
                            page.body = quizDetails.getForm(cp, dstFormId, rqs, rightNow, ref appId);
                            break;
                        case (Constants.formIdBlank):
                            page.body = blank.getForm(cp, dstFormId, rqs, rightNow, ref appId);
                            break;
                        default:
                            page.body = quizList.getForm(cp, dstFormId, rqs, rightNow, ref appId);
                            break;
                    }
                    s = page.getHtml(cp);
                    cp.Doc.AddHeadStyle(page.styleSheet);
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "execute");
                }
                return s;
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