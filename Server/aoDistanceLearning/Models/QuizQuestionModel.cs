
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
using System.Reflection;

namespace Contensive.Addons.DistanceLearning.Models
{
    public class QuizQuestionModel
    {
        //
        //-- const
        public const string primaryContentName = "Quiz Questions";
        private const string primaryContentTableName = "quizQuestions";
        //
        // -- instance properties
        public int id{get;set;}
        public string name{get;set;}
        public string ccguid{get;set;}
        public int SubjectID{get;set;}
        public int quizId{get;set;}
        public int points{get;set;}
        public string instructions{get;set;}
        public string copy{get;set;}
        public string SortOrder { get; set; }
        public int createKey { get; set; }
        public int QOrder { get; set; }
        //
        //public int pageOrder;
        //public bool Active;
        //public DateTime DateAdded;
        //public int CreatedBy;
        //public DateTime ModifiedDate;
        //public int ModifiedBy;
        //
        // -- publics not exposed to the UI (test/internal data)
        //
        //====================================================================================================
        /// <summary>
        /// Create an empty object. needed for deserialization
        /// </summary>
        public QuizQuestionModel()
        {
            copy = "";
            name = "";
            ccguid = "";
            instructions = "";
            SortOrder = "";           
        }
        //
        //====================================================================================================
        /// <summary>
        /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId">The id of the record to be read into the new object</param>
        public static QuizQuestionModel create(CPBaseClass cp, int recordId)
        {
            QuizQuestionModel result = null;
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
        public static QuizQuestionModel create(CPBaseClass cp, string recordGuid)
        {
            QuizQuestionModel result = null;
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
        /// <summary>
        /// Add record method
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        //

        public static QuizQuestionModel add(CPBaseClass cp)
        {
            return create(cp, cp.Content.AddRecord(primaryContentName));
        }
        //
        //====================================================================================================
        /// <summary>
        /// open an existing object
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="sqlCriteria"></param>
        private static QuizQuestionModel loadObject(CPBaseClass cpCore, string sqlCriteria)
        {
            QuizQuestionModel result = null;
            try
            {
                CPCSBaseClass cs = cpCore.CSNew();
                if (cs.Open(primaryContentName, sqlCriteria))
                {
                    result = new QuizQuestionModel();
                    //
                    // -- populate result model
                    result.name = result.copy;
                    if (result.name.Length > 255) result.name = result.name.Substring(0, 255);
                    //result.id = cs.GetInteger("id");
                    //result.name = cs.GetText("name");
                    //result.copy = cs.GetText("copy");
                    //result.ccguid = cs.GetText("ccGuid");
                    //result.createKey = cs.GetInteger("createKey");
                    //result.quizId = cs.GetInteger("QuizID");
                    //result.SubjectID = cs.GetInteger("SubjectID");
                    //result.points = cs.GetInteger("points");
                    //result.instructions = cs.GetText("instructions");
                    //result.SortOrder = cs.GetText("SortOrder");
                    //if (string.IsNullOrEmpty(result.name)) { result.name = result.copy.Substring(0, 100); }
                    // iterate through all the public instance properties, saving to Db fields with the same name
                    foreach (PropertyInfo property in result.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        switch (property.Name.ToLower())
                        {
                            case "exception":
                                break;
                            default:
                                switch (property.PropertyType.Name)
                                {
                                    case "Int32":
                                        property.SetValue(result,cs.GetInteger(property.Name),null);
                                        break;
                                    case "Boolean":
                                        property.SetValue(result, cs.GetBoolean(property.Name), null);
                                        break;
                                    case "DateTime":
                                        property.SetValue(result, cs.GetDate(property.Name), null);
                                        break;
                                    case "Double":
                                        property.SetValue(result, cs.GetNumber(property.Name), null);
                                        break;
                                    default:
                                        property.SetValue(result, cs.GetText(property.Name), null);
                                        break;
                                }
                                break;
                        }
                    }
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
                    // iterate through all the public instance properties, saving to Db fields with the same name
                    foreach (PropertyInfo property in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        switch(property.Name.ToLower())
                        {
                            case "id":
                                id = cs.GetInteger("id");
                                break;
                            case "name":
                                if (string.IsNullOrEmpty(name))
                                {
                                    name = copy;
                                    if (name.Length > 100) { name = name.Substring(0, 100)+"...."; }
                                }
                                cs.SetField("name",name);
                                break;
                            default:
                                string value= property.GetValue(this, null).ToString();
                                cs.SetField(property.Name, value);
                                break;
                        }
                    }
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
        /// get a list of questions, sorted by sortOrder
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public static List<QuizQuestionModel> getQuestionsForQuizList(CPBaseClass cp, int quizId)
        {
            List<QuizQuestionModel> result = new List<QuizQuestionModel>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                if ((cs.Open(primaryContentName, "(quizid=" + quizId + ")", "SortOrder", true, "id")))
                {
                    QuizQuestionModel instance = null;
                    do
                    {
                        instance = QuizQuestionModel.create(cp, cs.GetInteger("id"));
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
    }
}

