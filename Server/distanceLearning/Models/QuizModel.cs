
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
    //
    //====================================================================================================
    // entity model pattern
    //   factory pattern load because if a record is not found, must return nothing
    //   new() - empty constructor to allow deserialization
    //   saveObject() - saves instance properties (nonstatic method)
    //   create() - loads instance properties and returns a model 
    //   delete() - deletes the record that matches the argument
    //   getObjectList() - a pattern for creating model lists.
    //   invalidateFIELDNAMEcache() - method to invalide the model cache. One per cache
    //
    //	1) set the primary content name in const cnPrimaryContent. avoid constants Like cnAddons used outside model
    //	2) find-And-replace "QuizAnswersModel" with the name for this model
    //	3) when adding model fields, add in three places: the Public Property, the saveObject(), the loadObject()
    //	4) when adding create() methods to support other fields/combinations of fields, 
    //       - add a secondary cache For that new create method argument in loadObjec()
    //       - add it to the injected cachename list in loadObject()
    //       - add an invalidate
    //
    class QuizModel
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
        public string questionPresentation;
        public string includeSubject;
        public bool allowRetake;
        public string courseMaterial;
        //public int typeId;
        //public int typeId;
        //public bool allowRetake;
        //public bool requireAuthentication;
        //public bool allowCustomTopCopy;
        public string customTopCopy;
        //public bool allowCustomButtonCopy;
        public string customButtonCopy;
        //public bool includeStudyPage;
        //public string studyCopy;
        public string Video;
        //
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
        public QuizModel() {}
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
                    result.questionPresentation = cs.GetText("questionPresentation");
                    result.includeSubject = cs.GetText("includeSubject");
                    result.allowRetake = cs.GetBoolean("allowRetake");
                    result.customTopCopy = cs.GetText("customTopCopy");
                    result.Video = cs.GetText("Video");
                    result.courseMaterial = cs.GetText("courseMaterial");
                    result.customButtonCopy = cs.GetText("customButtonCopy");                  

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
                    cs.SetField("name", name);
                    cs.SetField("ccGuid", guid);
                    cs.SetField("createKey", createKey.ToString());
                    cs.SetField("dateadded", DateAdded.ToString());
                    cs.SetField("maxNumberQuest", maxNumberQuest.ToString());
                    cs.SetField("questionPresentation", questionPresentation);
                    cs.SetField("includeSubject", includeSubject);
                    cs.SetField("allowRetake", allowRetake.ToString());
                    cs.SetField("customTopCopy", customTopCopy);
                    cs.SetField("Video", Video);
                    cs.SetField("courseMaterial", courseMaterial);
                    cs.SetField("customButtonCopy", customButtonCopy);
                
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
                    cp.Db.ExecuteSQL("delete from " + cp.Content.GetTable(primaryContentName) + " where (id=" + recordId.ToString()  + ")");
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
                    cp.Db.ExecuteSQL( "delete from " + cp.Content.GetTable( primaryContentName) + " where (ccguid=" + cp.Db.EncodeSQLText(guid) + ")");
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

    }
}

