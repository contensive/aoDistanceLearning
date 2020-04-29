using System;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning {
    namespace Models {
        public class QuizModel : DesignBlockBaseModel {
            /// <summary>
            /// table definition
            /// </summary>
            public static readonly DbBaseTableMetadataModel tableMetadata = new DbBaseTableMetadataModel("Quizzes", "quizzes", "default", false);
            //
            public enum questionPresentationEnum {
                AllQuestionsOnOnePage = 1,
                OneSubjectPerPage = 2,
                OneQuestionPerPage = 3
            }
            //
            // -- instance properties
            public int maxNumberQuest { get; set; }
            public int questionPresentation { get; set; }
            public string includeSubject { get; set; }
            public bool allowRetake { get; set; }
            public file courseMaterial { get; set; }
            public string customTopCopy { get; set; }
            public string customButtonCopy { get; set; }
            public string videoEmbedCode { get; set; }
            public string ACaption { get; set; }
            public string BCaption { get; set; }
            public string CCaption { get; set; }
            public string DCaption { get; set; }
            public string FCaption { get; set; }
            public double APercentile { get; set; }
            public double BPercentile { get; set; }
            public double CPercentile { get; set; }
            public double DPercentile { get; set; }
            public double FPercentile { get; set; }
            public bool APassingGrade { get; set; }
            public bool BPassingGrade { get; set; }
            public bool CPassingGrade { get; set; }
            public bool DPassingGrade { get; set; }
            public bool FPassingGrade { get; set; }
            public int certificateTypeId { get; set; }
            public int certificationTypeId { get; set; }
            public double certificationCECs { get; set; }
            public bool addSuccessCopy { get; set; }
            public string successCopy { get; set; }
            public bool includeStudyPage { get; set; }
            public string studyCopy { get; set; }
            public bool allowCustomButtonCopy { get; set; }
            public int typeId { get; set; }
            public bool requireAuthentication { get; set; }
            public bool allowCustomTopCopy { get; set; }
            public int layoutId { get; set; }

            //
            // -- Sample property Types
            //public string sampleDbFieldText { get; set; }
            //public int sampleDbFieldInteger { get; set; }
            //public bool sampleDbFieldBoolean { get; set; }
            //public DateTime sampleDbFieldDate { get; set; }
            //public double sampleDbFieldNumber { get; set; }
            //
            // -- Sample file property Types
            //public FieldTypeCSSFile sampleDbFieldCssFile { get; set; }
            //public FieldTypeHTMLFile sampleDbFielHtmlFile { get; set; }
            //public FieldTypeJavascriptFile sampleDbFieldJavascriptFile { get; set; }
            //public FieldTypeTextFile sampleDbFieldTextFile { get; set; }
            //
            // -- Sample nullable property Types. These properties will save null if not initialized.
            //public int? sampleDbFieldNullableInteger { get; set; }
            //public bool? sampleDbFieldNullableBoolean { get; set; }
            //public DateTime? sampleDbFieldNullableDate { get; set; }
            //public double? sampleDbFieldNullableNumber { get; set; }
            // 
            // ====================================================================================================
            public static QuizModel createOrAddSettings(CPBaseClass cp, string requestQuizGuid) {
                QuizModel quiz = create<QuizModel>(cp, requestQuizGuid);
                if (quiz != null) { return quiz; }
                // 
                // -- create default content
                quiz = DesignBlockBaseModel.addDefault<QuizModel>(cp);
                quiz.name = tableMetadata.contentName + " " + quiz.id;
                quiz.ccguid = requestQuizGuid;
                quiz.fontStyleId = 0;
                quiz.themeStyleId = 0;
                quiz.padTop = false;
                quiz.padBottom = false;
                quiz.padRight = false;
                quiz.padLeft = false;
                // 
                //
                //
                // todo - this should be moved to the view model in createOrAdd...
                //
                //
                // -- no quiz but valid guid, create a new quiz
                quiz = DbBaseModel.addDefault<QuizModel>(cp);
                quiz.ccguid = requestQuizGuid;
                quiz.name = "Quiz " + quiz.id.ToString();
                quiz.allowRetake = true;

                LayoutModel layout = DbBaseModel.create<LayoutModel>(cp, Constants.defaultQuizLayoutGUID);
                if (layout == null) {
                    layout = DbBaseModel.addDefault<LayoutModel>(cp);
                    layout.ccguid = Constants.defaultQuizLayoutGUID;
                    layout.name = "Default Online Quiz Layout";
                    layout.layout.content = Properties.Resources.DefaultQuizLayout;
                    layout.save(cp);
                }

                quiz.layoutId = layout.id;
                quiz.studyCopy = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p><p>Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>";
                quiz.videoEmbedCode = "<iframe src=\"https://drive.google.com/file/d/1I5jLekO1-HKA_rgYZ_Rf_B3AlilZsLnm/preview\" width=\"640\" height=\"480\"></iframe>";
                quiz.save(cp);

                QuizQuestionModel question1 = addDefault<QuizQuestionModel>(cp);
                question1.name = "Question " + question1.id.ToString();
                question1.quizId = quiz.id;
                question1.QOrder = 1;
                question1.copy = "What is 1 + 1?";
                question1.save(cp);

                QuizAnswerModel answer1 = addDefault<QuizAnswerModel>(cp);
                answer1.name = "Answer " + answer1.id;
                answer1.QuestionID = question1.id;
                answer1.copy = "2";
                answer1.Correct = true;
                answer1.save(cp);

                QuizAnswerModel answer2 = addDefault<QuizAnswerModel>(cp);
                answer2.name = "Answer " + answer1.id;
                answer2.QuestionID = question1.id;
                answer2.copy = "5";
                answer2.Correct = false;
                answer2.save(cp);

                QuizAnswerModel answer3 = addDefault<QuizAnswerModel>(cp);
                answer3.name = "Answer " + answer1.id;
                answer3.QuestionID = question1.id;
                answer3.copy = "4";
                answer3.Correct = false;
                answer3.save(cp);

                QuizQuestionModel question2 = DbBaseModel.addDefault<QuizQuestionModel>(cp);
                question2.name = "Question " + question1.id.ToString();
                question2.quizId = quiz.id;
                question2.QOrder = 2;
                question2.copy = "What is 2 * 2?";
                question2.save(cp);

                QuizAnswerModel answer4 = addDefault<QuizAnswerModel>(cp);
                answer4.name = "Answer " + answer1.id;
                answer4.QuestionID = question2.id;
                answer4.copy = "2";
                answer4.Correct = false;
                answer4.save(cp);

                QuizAnswerModel answer5 = addDefault<QuizAnswerModel>(cp);
                answer5.name = "Answer " + answer1.id;
                answer5.QuestionID = question2.id;
                answer5.copy = "4";
                answer5.Correct = true;
                answer5.save(cp);

                QuizAnswerModel answer6 = addDefault<QuizAnswerModel>(cp);
                answer6.name = "Answer " + answer1.id;
                answer6.QuestionID = question2.id;
                answer6.copy = "7";
                answer6.Correct = false;
                answer6.save(cp);
                // 
                quiz.save(cp);
                // 
                // -- track the last modified date
                cp.Content.LatestContentModifiedDate.Track(quiz.modifiedDate);
                return quiz;
            }
        }
    }
    /// <summary>
    /// filename field type. Handles upload method
    /// </summary>
    public class file {
        private CPBaseClass cp;
        private string contentName;
        private string fieldName;
        private int recordId;
        //
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="cp"></param>
        //================================================================================
        public file(CPBaseClass cp, string contentName, string fieldName, int recordId) {
            this.cp = cp;
            this.contentName = contentName;
            this.fieldName = fieldName;
            this.recordId = recordId;
        }
        //================================================================================
        /// <summary>
        /// upload a file from a request field with the same name as the Db field
        /// </summary>
        /// <returns></returns>
        public bool processRequest() {
            return processRequest(fieldName);
        }
        //================================================================================
        /// <summary>
        /// upload a file
        /// </summary>
        /// <param name="requestName"></param>
        /// <returns></returns>
        public bool processRequest(string requestName) {
            bool result = false;
            try {
                bool requestDelete = (cp.Doc.GetBoolean(requestName + "_delete"));
                bool requestUpload = !string.IsNullOrEmpty(cp.Doc.GetText(requestName));
                if (requestDelete || (requestUpload)) {
                    CPCSBaseClass cs = cp.CSNew();
                    if (cs.Open(contentName, "id=" + recordId)) {
                        if (requestDelete) {
                            //
                            // -- the previous file is being deleted
                            _pathFilename = "";
                            cs.SetField(fieldName, _pathFilename);
                        }
                        if (requestUpload) {
                            //
                            // -- the file is being uploaded
                            cs.SetFormInput(fieldName, requestName);
                            _pathFilename = cs.GetFilename(requestName);
                        }
                    }
                    cs.Close();

                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        //================================================================================
        /// <summary>
        /// get the filename part of the current field's pathFilename
        /// </summary>
        public string filename {
            get {
                string result = "";
                if (!string.IsNullOrEmpty(_pathFilename)) {
                    string[] pathSegments = _pathFilename.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    result = pathSegments[pathSegments.GetUpperBound(0)];
                }
                return result;
            }
        }
        //================================================================================
        /// <summary>
        /// get/set the pathFilename of the current field. Use upload() method to upload a file
        /// </summary>
        public string pathFilename {
            get {
                return _pathFilename;
            }
            set {
                _pathFilename = value;
            }
        }
        private string _pathFilename;
        //
        //================================================================================
        /// <summary>
        /// Build the file input with delete and download link, using the fieldname as the requestname and class/id fileInput/fileInput_fieldname
        /// </summary>
        /// <returns></returns>
        public string getHtmlInput() {
            return getHtmlInput(fieldName);
        }
        //
        //================================================================================
        /// <summary>
        /// Build the file input with delete and download link, using class/id fileInput/fileInput_fieldname
        /// </summary>
        /// <returns></returns>
        public string getHtmlInput(string requestName) {
            string result = getHtmlInput(requestName, "fileInput", "fileInput_" + fieldName);
            result += "<style>"
                + ".fileInput{margin:2px;padding:4px;border:1px solid rgba(50,50,50,0.5);white-space:nowrap;}"
                + ".fileInputLink{}"
                + ".fileInputDelete{padding:0 0 0 14px}"
                + ".fileInputDelete input{vertical-align:middle}"
                + ".fileInputChoose{padding:0 0 0 14px}"
                + ".fileInput_" + fieldName + "{}"
                + "</style>";
            return result;
        }
        //
        //================================================================================
        /// <summary>
        /// Build the file input with delete and download link
        /// </summary>
        /// <returns></returns>
        public string getHtmlInput(string requestName, string htmlClass, string htmlId) {
            string result = "";
            result += "<div class=\"" + htmlClass + "\">";
            if (!string.IsNullOrEmpty(_pathFilename)) {
                result += "<span class=\"fileInputLink\"><a href=\"" + cp.Site.FilePath + _pathFilename + "\" target=\"_blank\">" + filename + "</a></span>";
                result += "<span class=\"fileInputDelete\">" + cp.Html.CheckBox(requestName + "_delete", false) + "&nbsp;Delete</span>";
            }
            result += "<span class=\"fileInputChoose\">" + cp.Html.InputFile(requestName) + "</span>";
            result += "</div>";
            return result;
        }

    }

}
