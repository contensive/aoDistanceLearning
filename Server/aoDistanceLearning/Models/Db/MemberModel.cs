﻿//using System;
//using System.Collections.Generic;
//using Contensive.BaseClasses;
//using Contensive.Models.Db;
//using Newtonsoft.Json;

//namespace Contensive.Addons.DistanceLearning.Models {

//    public class MemberModel {
//        public static readonly DbBaseTableMetadataModel tableMetadata = new DbBaseTableMetadataModel("people", "ccmembers", "default", false);
//        //
//        //====================================================================================================
//        /// <summary>
//        /// Create an empty object. needed for deserialization
//        /// </summary>
//        public MemberModel() { }
//        //
//        //====================================================================================================
//        /// <summary>
//        /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="recordId">The id of the record to be read into the new object</param>
//        public static MemberModel create(CPBaseClass cp, int recordId)
//        {
//            MemberModel result = null;
//            try
//            {
//                if (recordId > 0)
//                {
//                    if ((result == null))
//                    {
//                        result = loadObject(cp, "id=" + recordId.ToString());
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                cp.Site.ErrorReport(ex);
//                throw;
//            }
//            return result;
//        }
//        //
//        //====================================================================================================
//        /// <summary>
//        /// open an existing object
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="recordGuid"></param>
//        public static MemberModel create(CPBaseClass cp, string recordGuid)
//        {
//            MemberModel result = null;
//            try
//            {
//                if (!string.IsNullOrEmpty(recordGuid))
//                {
//                    if ((result == null))
//                    {
//                        result = loadObject(cp, "ccGuid=" + cp.Db.EncodeSQLText(recordGuid));
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                cp.Site.ErrorReport(ex);
//                throw;
//            }
//            return result;
//        }
//        //
//        //====================================================================================================
//        /// <summary>
//        /// open an existing object
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="sqlCriteria"></param>
//        private static MemberModel loadObject(CPBaseClass cpCore, string sqlCriteria)
//        {
//            MemberModel result = null;
//            try
//            {
//                CPCSBaseClass cs = cpCore.CSNew();
//                if (cs.Open(tableMetadata.contentName, sqlCriteria))
//                {
//                    result = new MemberModel {
//                        id = cs.GetInteger("id"),
//                        name = cs.GetText("name"),
//                        ccguid = cs.GetText("ccGuid"),
//                        createKey = cs.GetInteger("createKey")
//                    };
//                }
//                cs.Close();
//            }
//            catch (Exception ex)
//            {
//                cpCore.Site.ErrorReport(ex);
//                throw;
//            }
//            return result;
//        }
//        //
//        //====================================================================================================
//        /// <summary>
//        /// save the instance properties to a record with matching id. If id is not provided, a new record is created.
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <returns></returns>
//        public int saveObject(CPBaseClass cp)
//        {
//            try
//            {
//                CPCSBaseClass cs = cp.CSNew();
//                if ((id > 0))
//                {
//                    if (!cs.Open(primaryContentName, "id=" + id))
//                    {
//                        id = 0;
//                        cs.Close();
//                        throw new ApplicationException("Unable to open record in content [" + primaryContentName + "], with id [" + id + "]");
//                    }
//                }
//                else
//                {
//                    if (!cs.Insert(primaryContentName))
//                    {
//                        cs.Close();
//                        id = 0;
//                        throw new ApplicationException("Unable to insert record in content [" + primaryContentName + "]");
//                    }
//                }
//                if (cs.OK())
//                {
//                    id = cs.GetInteger("id");
//                    cs.SetField("name", name);
//                    cs.SetField("ccGuid", ccguid);
//                    cs.SetField("createKey", createKey.ToString());
//                }
//                cs.Close();
//            }
//            catch (Exception ex)
//            {
//                cp.Site.ErrorReport(ex);
//                throw;
//            }
//            return id;
//        }
//        //
//        //====================================================================================================
//        /// <summary>
//        /// delete an existing database record
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="recordId"></param>
//        public static void delete(CPBaseClass cp, int recordId)
//        {
//            try
//            {
//                if ((recordId > 0))
//                {
//                    cp.Db.ExecuteNonQuery("delete from " + cp.Content.GetTable(primaryContentName) + " where (id=" + recordId.ToString() + ")");
//                }
//            }
//            catch (Exception ex)
//            {
//                cp.Site.ErrorReport(ex);
//                throw;
//            }
//        }
//        //
//        //====================================================================================================
//        /// <summary>
//        /// delete an existing database record
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="recordId"></param>
//        public static void delete(CPBaseClass cp, string guid)
//        {
//            try
//            {
//                if ((!string.IsNullOrEmpty(guid)))
//                {
//                    cp.Db.ExecuteNonQuery("delete from " + cp.Content.GetTable(primaryContentName) + " where (ccguid=" + cp.Db.EncodeSQLText(guid) + ")");
//                }
//            }
//            catch (Exception ex)
//            {
//                cp.Site.ErrorReport(ex);
//                throw;
//            }
//        }
//        //
//        //====================================================================================================
//        /// <summary>
//        /// get a list of objects from this model
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="someCriteria"></param>
//        /// <returns></returns>
//        public static List<MemberModel> getMemberList(CPBaseClass cp, int id)
//        {
//            List<MemberModel> result = new List<MemberModel>();
//            try
//            {
//                CPCSBaseClass cs = cp.CSNew();
//                List<string> ignoreCacheNames = new List<string>();
//                if ((cs.Open(primaryContentName, "(id=" + id + ")", "name", true)))
//                {
//                    MemberModel instance = null;
//                    do
//                    {
//                        instance = MemberModel.create(cp, cs.GetInteger("id"));
//                        if ((instance != null))
//                        {
//                            result.Add(instance);
//                        }
//                        cs.GoNext();
//                    } while (cs.OK());
//                }
//                cs.Close();
//            }
//            catch (Exception ex)
//            {
//                cp.Site.ErrorReport(ex);
//            }
//            return result;
//        }
//        /// <summary>
//        /// Add record method
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <returns></returns>
//        //

//        public static MemberModel add(CPBaseClass cp)
//        {
//            return create(cp, cp.Content.AddRecord(primaryContentName));
//        }
//    }
//}

