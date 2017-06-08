
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
    public class QuizSubjectModel
    {
        //
        //-- const
        public const string primaryContentName = "Quiz Subjects";
        private const string primaryContentTableName = "quizSubjects";
        //
        // -- instance properties
        public int id;
        public string name;
        public string guid;
        //public string ACaption;
        //public string BCaption;
        //public string CCaption;
        //public string DCaption;
        //public string FCaption;
        //public double APercentile;
        //public double BPercentile;
        //public double CPercentile;
        //public double DPercentile;
        //public double FPercentile;
        //
        //public bool Active;
        //public string SortOrder;
        //public DateTime DateAdded;
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
        public QuizSubjectModel() {}
        //
        //====================================================================================================
        /// <summary>
        /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId">The id of the record to be read into the new object</param>
        public static QuizSubjectModel create(CPBaseClass cp, int recordId)
        {
            QuizSubjectModel result = null;
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
        /// <param name="recordName"></param>
        public static QuizSubjectModel createByName(CPBaseClass cp, string recordName)
        {
            QuizSubjectModel result = null;
            try
            {
                if (!string.IsNullOrEmpty(recordName))
                {
                    if ((result == null))
                    {
                        result = loadObject(cp, "name=" + cp.Db.EncodeSQLText(recordName));
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
        private static QuizSubjectModel loadObject(CPBaseClass cpCore, string sqlCriteria)
        {
            QuizSubjectModel result = null;
            try
            {
                CPCSBaseClass cs = cpCore.CSNew();
                if (cs.Open(primaryContentName, sqlCriteria))
                {
                    result = new QuizSubjectModel();
                    //
                    // -- populate result model
                    result.id = cs.GetInteger("id");
                    result.name = cs.GetText("name");
                    result.guid = cs.GetText("ccGuid");
                    result.createKey = cs.GetInteger("createKey");
                    //result.ACaption = cs.GetText("ACaption");
                    //result.APercentile = cs.GetNumber("APercentile");
                    //result.BCaption = cs.GetText("BCaption");
                    //result.BPercentile = cs.GetNumber("BPercentile");
                    //result.CCaption = cs.GetText("CCaption");
                    //result.CPercentile = cs.GetNumber("CPercentile");
                    //result.DCaption = cs.GetText("DCaption");
                    //result.DPercentile = cs.GetNumber("DPercentile");
                    //result.FCaption = cs.GetText("FCaption");
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
                    //cs.SetField("ACaption", ACaption);
                    //cs.SetField("APercentile", APercentile.ToString());
                    //cs.SetField("BCaption", BCaption);
                    //cs.SetField("BPercentile", BPercentile.ToString());
                    //cs.SetField("CCaption", CCaption);
                    //cs.SetField("CPercentile", CPercentile.ToString());
                    //cs.SetField("DCaption", DCaption);
                    //cs.SetField("DPercentile", DPercentile.ToString());
                    //cs.SetField("FCaption", FCaption);
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
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static List<QuizSubjectModel> getObjectList(CPBaseClass cp)
        {
            List<QuizSubjectModel> result = new List<QuizSubjectModel>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(primaryContentName, "", "name", true, "id")))
                {
                    QuizSubjectModel instance = null;
                    do
                    {
                        instance = QuizSubjectModel.create(cp, cs.GetInteger("id"));
                        if ((instance != null))
                        {
                            result.Add(instance);
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
            return result;
        }
        public static QuizSubjectModel add(CPBaseClass cp)
        {
            return create(cp, cp.Content.AddRecord(primaryContentName));
        }
    }
}

