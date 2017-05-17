////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;

////namespace Contensive.Addons.DistanceLearning3.models
////{
    
   
//using Microsoft.VisualBasic;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
//using System.Text;
//using Contensive.BaseClasses;
//using Newtonsoft.Json;


//namespace Contensive.Addons.DistanceLearning3
//{
//        //
//        //====================================================================================================
//        // entity model pattern
//        //   factory pattern load because if a record is not found, must return nothing
//        //   new() - empty constructor to allow deserialization
//        //   saveObject() - saves instance properties (nonstatic method)
//        //   create() - loads instance properties and returns a model 
//        //   delete() - deletes the record that matches the argument
//        //   getObjectList() - a pattern for creating model lists.
//        //   invalidateFIELDNAMEcache() - method to invalide the model cache. One per cache
//        //
//        //	1) set the primary content name in const cnPrimaryContent. avoid constants Like cnAddons used outside model
//        //	2) find-And-replace "_blankModel" with the name for this model
//        //	3) when adding model fields, add in three places: the Public Property, the saveObject(), the loadObject()
//        //	4) when adding create() methods to support other fields/combinations of fields, 
//        //       - add a secondary cache For that new create method argument in loadObjec()
//        //       - add it to the injected cachename list in loadObject()
//        //       - add an invalidate
//        //
//        // Model Caching
//        //   caching applies to model objects only, not lists of models (for now)
//        //       - this is because of the challenge of invalidating the list object when individual records are added or deleted
//        //
//        //   a model should have 1 primary cache object which stores the data and can have other secondary cacheObjects which do not hold data
//        //    the cacheName of the 'primary' cacheObject for models and db records (cacheNamePrefix + ".id." + #id)
//        //    'secondary' cacheName is (cacheNamePrefix + . + fieldName + . + #)
//        //
//        //   cacheobjects can be used to hold data (primary cacheobjects), or to hold only metadata (secondary cacheobjects)
//        //       - primary cacheobjects are like 'personModel.id.99' that holds the model for id=99
//        //           - it is primary because the .primaryobject is null
//        //           - invalidationData. This cacheobject is invalid after this datetime
//        //           - dependentobjectlist() - this object is invalid if any of those objects are invalid
//        //       - secondary cachobjects are like 'person.ccguid.12345678'. It does not hold data, just a reference to the primary cacheobject
//        //
//        //   cacheNames spaces are replaced with underscores, so "addon collections" should be addon_collections
//        //
//        //   cacheNames that match content names are treated as caches of "any" record in the content, so invalidating "people" can be used to invalidate
//        //       any non-specific cache in the people table, by including "people" as a dependant cachename. the "people" cachename should not clear
//        //       specific people caches, like people.id.99, but can be used to clear lists of records like "staff_list_group"
//        //       - this can be used as a fallback strategy to cache record lists: a remote method list can be cached with a dependancy on "add-ons".
//        //       - models should always clear this content name cache entry on all cache clears
//        //
//        //   when a model is created, the code first attempts to read the model's cacheobject. if it fails, it builds it and saves the cache object and tags
//        //       - when building the model, is writes object to the primary cacheobject, and writes all the secondaries to be used
//        //       - when building the model, if a database record is opened, a dependantObject Tag is created for the tablename+'id'+id
//        //       - when building the model, if another model is added, that model returns its cachenames in the cacheNameList to be added as dependentObjects
//        //
//        //
//        class quizModel
//        {
//            //
//            //-- const
//            //<------ set content name
//            public const string primaryContentName = "Quizzes";
//            //<------ set to tablename for the primary content (used for cache names)
//            private const string primaryContentTableName = "Quizzes";
//            //
//            //private string cacheName = "";
//            //
//            // -- instance properties
//            public int id;
//            public string name;
//            public string guid;
//            //
//            // -- publics not exposed to the UI (test/internal data)
//            [JsonIgnore()]
//            public int createKey;
//            //
//            // -- when an object is created, this is the name of the cache entry it was saved to. The consuming object
//            //    should use this in it's cache tag list so if another process modifies this data, the consuming object's
//            //    cache will be invalidated.
//            //   - There can be more than one cacheName (object-id, object-guid, etc). Add them all the parent's tag list.
//            //   - 
//            //
//            //<JsonIgnore>
//            //Public cacheNameList As List(Of String)
//            //
//            //====================================================================================================
//            /// <summary>
//            /// Create an empty object. needed for deserialization
//            /// </summary>
//            public quizModel()
//            {
//                //
//            }
//            //
//            //====================================================================================================
//            /// <summary>
//            /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
//            /// </summary>
//            /// <param name="cp"></param>
//            /// <param name="recordId">The id of the record to be read into the new object</param>
//            /// <param name="cacheNameList">Any cachenames effected by this record will be added to this list. If the method consumer creates a cache object, add these cachenames to its dependent cachename list.</param>
//            public static quizModel create(CPBaseClass cp, int recordId)
//            {
//                quizModel result = null;
//                try
//                {
//                    if (recordId > 0)
//                    {
//                        //tring cacheName = typeof(quizModel).FullName + getCacheName("id", recordId.ToString());
//                        //result = cp.cache.getObject<quizModel>(cacheName);
//                        if ((result == null))
//                        {
//                            result = loadObject(cp, "id=" + recordId.ToString());
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    cp.Site.ErrorReport(ex);
//                    throw;
//                }
//                return result;
//            }
//            //
//            //====================================================================================================
//            /// <summary>
//            /// open an existing object
//            /// </summary>
//            /// <param name="cp"></param>
//            /// <param name="recordGuid"></param>
//            public static quizModel create(CPBaseClass cp, string recordGuid)
//            {
//                quizModel result = null;
//                try
//                {
//                    if (!string.IsNullOrEmpty(recordGuid))
//                    {
//                        //string cacheName = typeof(quizModel).FullName + getCacheName("ccguid", recordGuid);
//                        //result = cpCore.cache.getObject<quizModel>(cacheName);
//                        if ((result == null))
//                        {
//                            result = loadObject(cp, "ccGuid=" + cp.Db.encodeSQLText(recordGuid));
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    cp.s(ex);
//                    throw;
//                }
//                return result;
//            }
//            //
//            //====================================================================================================
//            /// <summary>
//            /// open an existing object
//            /// </summary>
//            /// <param name="cp"></param>
//            /// <param name="sqlCriteria"></param>
//            private static quizModel loadObject(CPBaseClass cpCore, string sqlCriteria)
//            {
//                quizModel result = null;
//                try
//                {
//                    csController cs = new csController(cpCore);
//                    if (cs.open(primaryContentName, sqlCriteria))
//                    {
//                        result = new quizModel();
//                        var _with1 = result;
//                        //
//                        // -- populate result model
//                        _with1.id = cs.getInteger("id");
//                        _with1.name = cs.getText("name");
//                        _with1.guid = cs.getText("ccGuid");
//                        _with1.createKey = cs.getInteger("createKey");
//                    }
//                    cs.Close();
//                }
//                catch (Exception ex)
//                {
//                    cpCore.handleExceptionAndRethrow(ex);
//                    throw;
//                }
//                return result;
//            }
//            //
//            //====================================================================================================
//            /// <summary>
//            /// save the instance properties to a record with matching id. If id is not provided, a new record is created.
//            /// </summary>
//            /// <param name="cp"></param>
//            /// <returns></returns>
//            public int saveObject(CPBaseClass cp)
//            {
//                try
//                {
//                    csController cs = new csController(cp);
//                    if ((id > 0))
//                    {
//                        if (!cs.open(primaryContentName, "id=" + id))
//                        {
//                            id = 0;
//                            cs.Close();
//                            throw new ApplicationException("Unable to open record in content [" + primaryContentName + "], with id [" + id + "]");
//                        }
//                    }
//                    else
//                    {
//                        if (!cs.Insert(primaryContentName))
//                        {
//                            cs.Close();
//                            id = 0;
//                            throw new ApplicationException("Unable to insert record in content [" + primaryContentName + "]");
//                        }
//                    }
//                    if (cs.ok())
//                    {
//                        id = cs.getInteger("id");
//                        cs.SetField("name", name);
//                        cs.SetField("ccGuid", guid);
//                        cs.SetField("createKey", createKey.ToString());
//                    }
//                    cs.Close();
//                }
//                catch (Exception ex)
//                {
//                    cp.handleExceptionAndRethrow(ex);
//                    throw;
//                }
//                return id;
//            }
//            //
//            //====================================================================================================
//            /// <summary>
//            /// delete an existing database record
//            /// </summary>
//            /// <param name="cp"></param>
//            /// <param name="recordId"></param>
//            public static void delete(CPBaseClass cpCore, int recordId)
//            {
//                try
//                {
//                    if ((recordId > 0))
//                    {
//                        cpCore.db.deleteContentRecords(primaryContentName, "id=" + recordId.ToString);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    cpCore.handleExceptionAndRethrow(ex);
//                    throw;
//                }
//            }
//            //
//            //====================================================================================================
//            /// <summary>
//            /// delete an existing database record
//            /// </summary>
//            /// <param name="cp"></param>
//            /// <param name="recordId"></param>
//            public static void delete(CPBaseClass cpCore, string guid)
//            {
//                try
//                {
//                    if ((!string.IsNullOrEmpty(guid)))
//                    {
//                        cpCore.db.deleteContentRecords(primaryContentName, "(ccguid=" + cpCore.db.encodeSQLText(guid) + ")");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    cpCore.handleExceptionAndRethrow(ex);
//                    throw;
//                }
//            }
//            //
//            //====================================================================================================
//            /// <summary>
//            /// get a list of objects from this model
//            /// </summary>
//            /// <param name="cp"></param>
//            /// <param name="someCriteria"></param>
//            /// <returns></returns>
//            public static List<quizModel> getObjectList(CPBaseClass cpCore, int someCriteria)
//            {
//                List<quizModel> result = new List<quizModel>();
//                try
//                {
//                    csController cs = new csController(cpCore);
//                    List<string> ignoreCacheNames = new List<string>();
//                    if ((cs.open(primaryContentName, "(someCriteria=" + someCriteria + ")", "name", true, "id")))
//                    {
//                        quizModel instance = null;
//                        do
//                        {
//                            instance = quizModel.create(cpCore, cs.getInteger("id"), ref ignoreCacheNames);
//                            if ((instance != null))
//                            {
//                                result.Add(instance);
//                            }
//                            cs.goNext();
//                        } while (cs.ok());
//                    }
//                    cs.Close();
//                }
//                catch (Exception ex)
//                {
//                    cpCore.handleExceptionAndRethrow(ex);
//                }
//                return result;
//            }
//        }
//    }

