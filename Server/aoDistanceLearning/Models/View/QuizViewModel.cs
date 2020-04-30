
using System;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Models.View;

namespace Models.View {
    public class QuizViewModel : DesignBlockViewBaseModel {
        // 
        public string legacyQuizHtml { get; set; }
        // 
        // ====================================================================================================
        /// <summary>
        /// Populate the view model from the entity model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quiz"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static QuizViewModel create(CPBaseClass cp, QuizModel quiz, string legacyQuizHtml) {
            try {
                // 
                // -- base fields
                var result = DesignBlockViewBaseModel.create<QuizViewModel>(cp, quiz);
                result.legacyQuizHtml = legacyQuizHtml;
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return null;
            }
        }
    }
}
