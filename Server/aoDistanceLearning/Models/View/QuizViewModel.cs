
using System;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;

namespace Models.View {
    public class QuizViewModel : DesignBlockBaseModel {
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
                var result = DesignBlockBaseModel.create<QuizViewModel>(cp, quiz.id);
                result.legacyQuizHtml = legacyQuizHtml;
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return null;
            }
        }
    }
}
