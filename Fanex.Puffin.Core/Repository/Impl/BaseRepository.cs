// --------------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.22">Created</change>
//   <change who="Ricky.Nguyen" date="2012.12.11">Update Get Data Function</change>
// </history>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Fanex.Data;
using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Repository.Impl
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseRepository : DynamicRepository
    {
        /// <summary>
        /// Fetch Data from Criteria
        /// </summary>
        /// <typeparam name="S">Class</typeparam>
        /// <param name="criteria">Criteria</param>
        /// <param name="serverName">Server Name will be executed sp</param>
        /// <param name="mapObjectFunction">Read data reader and pass object</param>
        /// <returns>Collection of object</returns>
        public virtual IList<S> Fetch<S>(ICriteria criteria, Func<IDataReader, S> mapObjectFunction) where S : class
        {
            IList<S> listObject = new List<S>();
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                    {
                        while (reader.Read())
                        {
                            listObject.Add(mapObjectFunction(reader));
                        }
                    }
                }
            }

            return listObject;
        }

        /// <summary>
        /// Execute command to Db
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="serverIp"></param>
        /// <returns></returns>
        public virtual Hashtable ExecuteCommand(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                return db.GetOuputParams();
            }
        }

        /// <summary>
        /// Get DataTable and Output params from Criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="outputParams"></param>
        /// <returns></returns>
        public virtual DataTable Fetch(ICriteria criteria, ref Hashtable outputParams)
        {
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                    if (dataSet != null
                        && dataSet.Tables.Count > 0)
                    {
                        outputParams = db.GetOuputParams();
                        return dataSet.Tables[0];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get DataTable from Criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="outputParams"></param>
        /// <returns></returns>
        public virtual DataTable Fetch(ICriteria criteria)
        {
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                    if (dataSet != null
                        && dataSet.Tables.Count > 0)
                    {
                        return dataSet.Tables[0];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get DataTable and Output params from Criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="outputParams"></param>
        /// <returns></returns>
        public virtual IList<S> Fetch<S>(ICriteria criteria, Func<DataRow, S> mapObjectFunction) where S : class
        {
            IList<S> listData = new List<S>();
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                    if (dataSet != null
                        && dataSet.Tables.Count > 0)
                    {
                        DataTable currentTable = dataSet.Tables[0];
                        foreach (DataRow row in currentTable.Rows)
                        {
                            listData.Add(mapObjectFunction(row));
                        }
                    }
                }
            }

            return listData;
        }

        /// <summary>
        /// Get DataTable and Output params from Criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="outputParams"></param>
        /// <returns></returns>
        public virtual IList<S> Fetch<S>(ICriteria criteria, Func<DataRow, S> mapObjectFunction, ref Hashtable outputParams) where S : class
        {
            IList<S> listData = new List<S>();
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                    outputParams = db.GetOuputParams();
                    if (dataSet != null
                        && dataSet.Tables.Count > 0)
                    {
                        DataTable currentTable = dataSet.Tables[0];
                        foreach (DataRow row in currentTable.Rows)
                        {
                            listData.Add(mapObjectFunction(row));
                        }
                    }
                }
            }

            return listData;
        }
    }
}
