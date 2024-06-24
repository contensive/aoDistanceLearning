
using System;
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.BaseClasses;
using Controllers;
using Models.View;

namespace Contensive.Addons.DistanceLearning {
    namespace Views {
        // 
        // ====================================================================================================
        /// <summary>
        ///     ''' Design block with a centered headline, image, paragraph text and a button.
        ///     ''' </summary>
        public class QuizClass : AddonBaseClass {
            // 
            // ====================================================================================================
            // 
            public override object Execute(CPBaseClass cp) {
                const string designBlockName = "Quiz";
                try {
                    // 
                    // -- read instanceId, guid created uniquely for this instance of the addon on a page
                    var result = string.Empty;
                    var settingsGuid = DesignBlockController.getSettingsGuid(cp, designBlockName, ref result);
                    if ((string.IsNullOrEmpty(settingsGuid)))
                        return result;
                    // 
                    // -- locate or create a data record for this guid
                    var settings = QuizModel.createOrAddSettings(cp, settingsGuid);
                    if ((settings == null)) { throw new ApplicationException("Could not create the design block settings record."); }
                    //
                    string legacyQuiz = QuizLegacyClass.getLegacyQuiz(cp, settings);
                    // 
                    // -- translate the Db model to a view model and mustache it into the layout
                    var viewModel = QuizViewModel.create(cp, settings, legacyQuiz);
                    if ((viewModel == null))
                        throw new ApplicationException("Could not create design block view model.");
                    result = cp.Mustache.Render(Properties.Resources.DesignBlockLayout, viewModel);
                    // 
                    // -- if editing enabled, add the link and wrapperwrapper
                    return cp.Content.GetEditWrapper(result, QuizModel.tableMetadata.contentName, settings.id);
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex);
                    return "<!-- " + designBlockName + ", Unexpected Exception -->";
                }
            }
        }
    }
}