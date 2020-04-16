
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.DistanceLearning {
    public static class Constants {
        public static string cr = "\n\t";
        public static string cr2 = "\n\t\t";
        public static string cr3 = "\n\t\t\t";
        //
        public const string rnSrcFormId = "srcFormId";
        public const string rnDstFormId = "dstFormId";
        public const string rnButton = "button";
        public const string rnAppId = "appId";
        public const string rnFilterDateFrom = "filterDateFrom";
        public const string rnFilterDateTo = "filterDateTo";
        public const string rnAddonguid = "addonguid";
        public const string rnAddonId = "addonId";
        public const string dstFeatureGuid = "dstFeatureGuid";
        public const string rnQuizId = "quizId";
        public const string rnMemberId = "Id";
        public const string rnResponseId = "responseId";
        public const string rnQuestionCopy = "copy";
        //
        public const string rnSampleField = "sampleField";
        public const string rnQuestionId = "questionId";
        public const string rnAnswerId = "AnswerId";
        public const string rnSubjectId = "SubjectId";
        public const string rnCertificateTypeId = "certificateTypeId";
        public const string rnCertificationTypeId = "rnCertificationTypeId";
        public const string rnCertificationId = "certificationId";
        public const string rnCertificationCECs = "certificationCECs";
        public const string rnCertifications = "certifications";
        public const string rnAddSuccessCopy = "addSuccessCopy";
        public const string rnSuccessCopy = "successCopy";
        public const string rnSubjectNameEditList = "subjectNameEditList";
        public const string rnSubjectIdEditList = "subjectIdEditList";
        //
        public const string buttonSaveAndClose = "Save and Close";
        public const string buttonSaveAndContinue = "Save and Continue";
        public const string buttonLogin = "Login";
        public const string buttonContinue = "Continue";
        public const string buttonSave = "Save";
        public const string buttonCancel = "Cancel";
        public const string buttonRefresh = "Refresh";
        //
        //
        public const string rnCorrectBlank = "CorrectBlank";
        public const string rnAnswerCopyBlank = "ATextBlank";
        public const string rnAnswerCopy = "AText";
        public const string cnQuizSubjects = "Quiz Subjects";
        public const string cnCertificateTypes = "Certificate Types";
        public const string cnCertificationTypes = "Certification Types";
        public const string cnCertifications = "Certificates";
        public const int maxQuestionAnswer = 4;

        //
        // addon guids
        public const string portalAddonGuid = "{A1BCA00C-2657-42A1-8F98-BED1E5A42025}";
        public const string dashBoardAddon = "{061eafdb-147c-4f09-afb8-7e51aa5181f3}";
        public const string quizDetailsAddon = "{fd30f314-f37d-40e9-ac56-841e74de84fd}";
        public const string quizOverViewSelectAddon = "{dfafb5b3-7c68-40d7-b6ce-ef982aad3441}";
        public const string quizOverViewSettingsAddon = "{caedb3af-884c-46c5-af52-1903dddb3a2d}";
        public const string quizOverViewReportingAddon = "{dfafb5b3-7c68-40d7-b6ce-ef982aad3441}";
        public const string quizOverViewStartPageAddon = "{87f875ac-659e-40eb-9aca-1045120e6771}";
        public const string quizOverQuestionPageAddon = "{a364f297-9e69-4b21-a92d-a6e6b4e16cb6}";
        public const string quizOverQuestionDetailsPageAddon = "{71e8f8bd-a2f1-4f78-a036-506103877f75}";
        public const string quizOverViewManageScoringPageAddon = "{99758dd6-6df4-4260-98ef-f511a76458fa}";
        public const string portalFeatureQuizOverviewSetting = "{19136f9a-b976-4339-81a8-c5565380f648}";
        public const string portalFeaturesQuizOverviewDetails = "{f3600bc9-7323-4b27-a782-6da6133eeb96}";
        public const string portalFeatureQuizOverviewStudyPage = "{dfdb76e1-af7a-42a7-bc49-3af95417aee2}";
        public const string portalFeatureDashboard = "{6b921555-04b2-4ec0-83e1-24ac6c3bcd62}";
        public const string portalFeatureQuizOverviewQuestionList = "{12c1944f-5994-4c0b-9e13-3320f29b16c3}";
        public const string portalFeatureQuizOverviewQuestionDetails = "{2d1ad031-7f03-4b26-bdcc-0e48697236e4}";
        public const string portalFeatureQuizOverviewScoring = "{bc2f69da-ab79-4051-aed5-63ba98b88b8f}";
        public const string portalFeatureQuizOverviewResults = "{e75b026b-1f9c-4839-83bb-801518cec97d}";
        public const string portalFeatureQuizOverviewResultsDetails = "{20d45c8f-0989-4e79-a2d7-4f7991add604}";
        public const string portalDistanceLearning = "{771bfee5-1fa7-4edf-8b6c-1406ab7bacf9}";
        public const string defaultQuizLayoutGUID = "{d0896afb-cd43-4aee-b86d-25cd59aeb60e}";
        //
        public const int formIdHome = 1;
        public const int formIdBlank = 2;
        public const int formIdQuizList = 3;
        public const int formIdQuizDetails = 4;
        //
        public const string rnbuttonApplyFilter = "Apply Filter";
        public const string rnbuttonInputNewQuiz = "+ add new Quiz";
        //
        public const string cnApps = "Sample Application Content - Change to your application if needed";
        //
        public const string scoreCardAddon = "{55E3F437-B362-45D1-B793-A201D4D5C2C6}";

        // 
        // - rename namespace to the collection (to match the current project)
        // - rename localLogName to create a custom trace log folder, like 'accounting'
        // 
        // 
        public const string defaultTopVideoCopy = "<p>To complete this quiz, watch the video and submit your answers to the questions that follow.</p>";
        public const string defaultTopNoVideoCopy = "<p>To complete this quiz, submit your answers to the questions that follow. </p>";
        public const string defaultOneTime = "<p>You may submit this quiz only once.</p>";
        public const string defaultButtonCopy = "<p>Click continue to move to the next page, and submit when you are finished with the quiz.</p>";
        public const string defaultButtonWithSaveCopy = "<p>Click continue to move to the next page, and submit when you are finished with the quiz. If you are logged in, you may also save the quiz and return later.</p>";
        public const string QuizAlreadyTakenDefault = "<p>You have already taken this quiz. Your previous answers are as follows.</p>";
        // 
        public struct subjectsStruc {
            public int SubjectID;
            public string SubjectCaption;
            public int TotalQuestions;
            public int CorrectAnswers;
            public double Score;
            public int points;
        }
        // todo - make enum
        public const int quizTypeIdGraded = 1;
        public const int quizTypeIdPoints = 2;
        public const int quizTypeIdSurvey = 3;
        // 
        // site properties
        // 
        public const string spBatchProcessOnlyOne = "Account Billing Batch Process Only One";
        // 
        // messages
        // 
        public const string  blockedMessage = "<h2>Blocked Content</h2><p>Your account must have administrator access to view this content.</p>";
        // 
        // guids
        // 
        public const string  guidSample = "{B3FDBB50-1A40-42CA-81EC-EA825C55469A}";
        public const string guidLogin = "{37E8E661-A1D9-4A61-9A07-F48A1053B1BB}";
        public const string  onlineQuizAddonGuid = "{4A5EBAD7-27ED-4AB4-BEF0-D42E01598FC5}";
        //// 
        //// buttons
        //// 
        public const string  buttonPrintAll = " Print All ";
        public const string  buttonStartQuiz = "Start Quiz";
        public const string  buttonPrevious = "Previous";
        public const string  buttonSubmit = "Submit";
        public const string  buttonRetakeQuiz = "Retake Quiz";
        public const string  buttonResumeQuiz = "Resume Quiz";
        //// 
        //// request Names
        //// 
        public const string rnPageNumber = "pageOrder";
        public const string rnDstPageOrder = "dstPageOrder";
        //// 
        //// custom log path
        //// 
        //public static string traceLogPath = "sample";
    }

}
