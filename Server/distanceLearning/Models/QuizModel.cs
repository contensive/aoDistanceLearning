
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Contensive.BaseClasses;
using Newtonsoft.Json;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning.Models
{
    public class QuizModel
    {
        //
        //-- const
        public const string primaryContentName = "Quizzes";
        private const string primaryContentTableName = "quizzes";
        //
        // -- instance properties
        public int id;
        public string name;
        public string guid;
        public DateTime DateAdded;
        public int maxNumberQuest;
        public int questionPresentation;
        public string includeSubject;
        public bool allowRetake;
        public string courseMaterial;
        public string customTopCopy;
        public string customButtonCopy;
        //public file Video;
        public string videoEmbedCode;
        public string ACaption;
        public string BCaption;
        public string CCaption;
        public string DCaption;
        public string FCaption;
        public double APercentile;
        public double BPercentile;
        public double CPercentile;
        public double DPercentile;
        public double FPercentile;
        public bool APassingGrade;
        public bool BPassingGrade;
        public bool CPassingGrade;
        public bool DPassingGrade;
        public bool FPassingGrade;
        public int certificateTypeId;
        public int certificationTypeId;
        public double certificationCECs;
        public bool addSuccessCopy;
        public string successCopy;
        public bool includeStudyPage;
        public string studyCopy;
        public bool allowCustomButtonCopy;
        public int typeId;
        public bool requireAuthentication;
        public bool allowCustomTopCopy;
        //public bool Active;
        //public string SortOrder;
        //public int CreatedBy;
        //public DateTime ModifiedDate;
        //public int ModifiedBy;
        //
        // -- publics not exposed to the UI (test/internal data)
        [JsonIgnore()]
        public int createKey;
        //
        //====================================================================================================
        /// <summary>
        /// Create an empty object. needed for deserialization
        /// </summary>
        public QuizModel() { }
        //
        //====================================================================================================
        /// <summary>
        /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId">The id of the record to be read into the new object</param>
        public static QuizModel create(CPBaseClass cp, int recordId)
        {
            QuizModel result = null;
            try
            {
                if (recordId > 0)
                {
                    if ((result == null))
                    {
                        result = loadObject(cp, "id=" + recordId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// open an existing object
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordGuid"></param>
        public static QuizModel create(CPBaseClass cp, string recordGuid)
        {
            QuizModel result = null;
            try
            {
                if (!string.IsNullOrEmpty(recordGuid))
                {
                    if ((result == null))
                    {
                        result = loadObject(cp, "ccGuid=" + cp.Db.EncodeSQLText(recordGuid));
                    }
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// open an existing object
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="sqlCriteria"></param>
        private static QuizModel loadObject(CPBaseClass cpCore, string sqlCriteria)
        {
            QuizModel result = null;
            try
            {
                CPCSBaseClass cs = cpCore.CSNew();
                if (cs.Open(primaryContentName, sqlCriteria))
                {
                    result = new QuizModel();
                    //
                    // -- populate result model
                    result.id = cs.GetInteger("id");
                    result.name = cs.GetText("name");
                    result.guid = cs.GetText("ccGuid");
                    result.createKey = cs.GetInteger("createKey");
                    result.DateAdded = cs.GetDate("dateadded");
                    result.maxNumberQuest = cs.GetInteger("maxNumberQuest");
                    result.questionPresentation = cs.GetInteger("questionPresentation");
                    result.includeSubject = cs.GetText("includeSubject");
                    result.allowRetake = cs.GetBoolean("allowRetake");
                    result.customTopCopy = cs.GetText("customTopCopy");
                    result.courseMaterial = cs.GetText("courseMaterial");
                    result.customButtonCopy = cs.GetText("customButtonCopy");
                    result.ACaption = cs.GetText("ACaption");
                    result.APercentile = cs.GetNumber("APercentile");
                    result.APassingGrade = cs.GetBoolean("APassingGrade");
                    result.BCaption = cs.GetText("BCaption");
                    result.BPercentile = cs.GetNumber("BPercentile");
                    result.BPassingGrade = cs.GetBoolean("BPassingGrade");
                    result.CCaption = cs.GetText("CCaption");
                    result.CPercentile = cs.GetNumber("CPercentile");
                    result.CPassingGrade = cs.GetBoolean("CPassingGrade");
                    result.DCaption = cs.GetText("DCaption");
                    result.DPercentile = cs.GetNumber("DPercentile");
                    result.DPassingGrade = cs.GetBoolean("DPassingGrade");
                    result.FCaption = cs.GetText("FCaption");
                    result.FPassingGrade = cs.GetBoolean("FPassingGrade");
                    result.certificateTypeId = cs.GetInteger("certificateTypeId");
                    result.certificationTypeId = cs.GetInteger("certificationTypeId");
                    result.certificationCECs = cs.GetNumber("certificationCECs");
                    result.addSuccessCopy = cs.GetBoolean("addSuccessCopy");
                    result.successCopy = cs.GetText("successCopy");
                    result.includeStudyPage = cs.GetBoolean("includeStudyPage");
                    result.studyCopy = cs.GetText("studyCopy");
                    result.allowCustomButtonCopy = cs.GetBoolean("allowCustomButtonCopy");
                    result.typeId = cs.GetInteger("typeId");
                    result.requireAuthentication = cs.GetBoolean("requireAuthentication");
                    result.allowCustomTopCopy = cs.GetBoolean("allowCustomTopCopy");
                    result.videoEmbedCode = cs.GetText("videoEmbedCode");
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cpCore.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// save the instance properties to a record with matching id. If id is not provided, a new record is created.
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public int saveObject(CPBaseClass cp)
        {
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                if ((id > 0))
                {
                    if (!cs.Open(primaryContentName, "id=" + id))
                    {
                        id = 0;
                        cs.Close();
                        throw new ApplicationException("Unable to open record in content [" + primaryContentName + "], with id [" + id + "]");
                    }
                }
                else
                {
                    if (!cs.Insert(primaryContentName))
                    {
                        cs.Close();
                        id = 0;
                        throw new ApplicationException("Unable to insert record in content [" + primaryContentName + "]");
                    }
                }
                if (cs.OK())
                {
                    id = cs.GetInteger("id");
                    if (string.IsNullOrEmpty(name)) name = "Quiz " + id.ToString();
                    cs.SetField("name", name);
                    cs.SetField("ccGuid", guid);
                    cs.SetField("createKey", createKey.ToString());
                    cs.SetField("dateadded", DateAdded.ToString());
                    cs.SetField("maxNumberQuest", maxNumberQuest.ToString());
                    cs.SetField("questionPresentation", questionPresentation.ToString());
                    cs.SetField("includeSubject", includeSubject);
                    cs.SetField("allowRetake", allowRetake.ToString());
                    cs.SetField("customTopCopy", customTopCopy);
                    cs.SetField("courseMaterial", courseMaterial);
                    cs.SetField("customButtonCopy", customButtonCopy);
                    cs.SetField("ACaption", ACaption);
                    cs.SetField("APercentile", APercentile.ToString());
                    cs.SetField("APassingGrade", APassingGrade.ToString());
                    cs.SetField("BCaption", BCaption);
                    cs.SetField("BPercentile", BPercentile.ToString());
                    cs.SetField("BPassingGrade", BPassingGrade.ToString());
                    cs.SetField("CCaption", CCaption);
                    cs.SetField("CPercentile", CPercentile.ToString());
                    cs.SetField("CPassingGrade", CPassingGrade.ToString());
                    cs.SetField("DCaption", DCaption);
                    cs.SetField("DPercentile", DPercentile.ToString());
                    cs.SetField("DPassingGrade", DPassingGrade.ToString());
                    cs.SetField("FCaption", FCaption);
                    cs.SetField("FPassingGrade", FPassingGrade.ToString());
                    cs.SetField("certificateTypeId", certificateTypeId.ToString());
                    cs.SetField("certificationTypeId", certificationTypeId.ToString());
                    cs.SetField("certificationCECs", certificationCECs.ToString());
                    cs.SetField("addSuccessCopy", addSuccessCopy.ToString());
                    cs.SetField("successCopy", successCopy.ToString());
                    cs.SetField("includeStudyPage", includeStudyPage.ToString());
                    cs.SetField("studyCopy", studyCopy.ToString());
                    cs.SetField("allowCustomButtonCopy", allowCustomButtonCopy.ToString());
                    cs.SetField("typeId", typeId.ToString());
                    cs.SetField("requireAuthentication", requireAuthentication.ToString());
                    cs.SetField("allowCustomTopCopy", allowCustomTopCopy.ToString());
                    cs.SetField("videoEmbedCode", videoEmbedCode.ToString());
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return id;
        }
        //
        //====================================================================================================
        /// <summary>
        /// delete an existing database record
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId"></param>
        public static void delete(CPBaseClass cp, int recordId)
        {
            try
            {
                if ((recordId > 0))
                {
                    cp.Db.ExecuteSQL("delete from " + cp.Content.GetTable(primaryContentName) + " where (id=" + recordId.ToString() + ")");
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// delete an existing database record
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId"></param>
        public static void delete(CPBaseClass cp, string guid)
        {
            try
            {
                if ((!string.IsNullOrEmpty(guid)))
                {
                    cp.Db.ExecuteSQL("delete from " + cp.Content.GetTable(primaryContentName) + " where (ccguid=" + cp.Db.EncodeSQLText(guid) + ")");
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// get a list of objects from this model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="someCriteria"></param>
        /// <returns></returns>
        public static List<QuizModel> getQuizList(CPBaseClass cp)
        {
            List<QuizModel> modelList = new List<QuizModel>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(primaryContentName, "", "name", true, "id")))
                {
                    QuizModel quiz = null;
                    do
                    {
                        quiz = QuizModel.create(cp, cs.GetInteger("id"));
                        if ((quiz != null))
                        {
                            modelList.Add(quiz);
                        }
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return modelList;
        }
        //
        //====================================================================================================
        /// <summary>
        /// get a list of objects from this model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="someCriteria"></param>
        /// <returns></returns>
        public static List<QuizModel> getObjectList(CPBaseClass cp, int someCriteria)
        {
            List<QuizModel> modelList = new List<QuizModel>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(primaryContentName, "(someCriteria=" + someCriteria + ")", "name", true, "id")))
                {
                    QuizModel quiz = null;
                    do
                    {
                        quiz = QuizModel.create(cp, cs.GetInteger("id"));
                        if ((quiz != null))
                        {
                            modelList.Add(quiz);
                        }
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return modelList;
        }
        /// <summary>
        /// Add record method
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        //

        public static QuizModel add(CPBaseClass cp)
        {
            return create(cp, cp.Content.AddRecord(primaryContentName));
        }
        /// <summary>
        /// filename field type. Handles upload method
        /// </summary>
        public class file
        {
            private CPBaseClass cp;
            private QuizModel quiz;
            private string requestName;
            private string contentName;
            private string fieldName;
            private int recordId;
            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="cp"></param>
            public file(CPBaseClass cp, string contentName, string fieldName, int recordId)
            {
                this.cp = cp;
                this.contentName = contentName;
                this.fieldName = fieldName;
                this.recordId = recordId;
            }
            public bool upload(string requestName)
            {
                bool result = false;
                try
                {
                    CPCSBaseClass cs = cp.CSNew();
                    if (cs.Open(contentName, "id=" + recordId)) cs.SetFormInput(fieldName, requestName);
                    cs.Close();
                }
                catch (Exception ex)
                {
                    cp.Site.ErrorReport(ex);
                }
                return result;
            }
            public string filename()
            {
                string result = "";
                try
                {
                    CPCSBaseClass cs = cp.CSNew();
                    if (cs.Open(contentName, "id=" + recordId))
                    {
                        result = cs.GetFilename(fieldName);
                    }
                    cs.Close();
                }
                catch (Exception ex)
                {
                    cp.Site.ErrorReport(ex);
                }
                return result;
            }
        }
    }
}

