//using System;
//using Contensive.Addons.DistanceLearning.Controllers;
//using Contensive.Addons.DistanceLearning.Models;
//using Contensive.BaseClasses;

//namespace Models.View
//{
//    public class ViewBaseModel
//    {
//        private static object genericController;

//        public string styleBackgroundImage { get; set; }
//        public string styleheight { get; set; }
//        public string contentContainerClass { get; set; }
//        public string outerContainerClass { get; set; }
//        public string btnStyleSelector { get; set; }
//        // 
//        // ====================================================================================================
//        /// <summary>
//        ///         ''' Populate the view model from the entity model
//        ///         ''' </summary>
//        ///         ''' <param name="cp"></param>
//        ///         ''' <param name="settings"></param>
//        ///         ''' <returns></returns>
//        public static T create<T>(CPBaseClass cp, QuizModel settings) where T : ViewBaseModel
//        {
//            T result = null;
//            try
//            {
//                Type instanceType = typeof(T);
//                result = (T)Activator.CreateInstance(instanceType);
//                // 
//                // -- base fields
//                result.styleheight = GenericController.encodeStyleHeight(settings.styleheight);
//                result.styleBackgroundImage = ""
//                    + GenericController.encodeStyleBackgroundImage(cp, settings.backgroundImageFilename)
//                    + "";
//                result.outerContainerClass = ""
//                    + (settings.fontStyleId == 0 ? string.Empty : " " + Contensive.Models.Db.DbBaseModel.getRecordName<DesignBlockFontModel>(cp, settings.fontStyleId))
//                    + (settings.themeStyleId.Equals(0) ? string.Empty : " " + Contensive.Models.Db.DbBaseModel.getRecordName<DesignBlockThemeModel>(cp, settings.themeStyleId))
//                    + "";
//                result.contentContainerClass = ""
//                    + (settings.asFullBleed ? " container" : string.Empty)
//                    + (settings.padTop ? " pt-5" : " pt-0")
//                    + (settings.padRight ? " pr-4" : " pr-0")
//                    + (settings.padBottom ? " pb-5" : " pb-0")
//                    + (settings.padLeft ? " pl-4" : " pl-0")
//                    + "";
//                result.btnStyleSelector = string.IsNullOrWhiteSpace(settings.btnStyleSelector) ? "btn btn-primary" : settings.btnStyleSelector;
//            }
//            catch (Exception ex)
//            {
//                cp.Site.ErrorReport(ex);
//            }
//            return result;
//        }
//    }
//}
