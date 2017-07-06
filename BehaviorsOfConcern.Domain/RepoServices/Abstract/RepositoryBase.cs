using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace BehaviorsOfConcern.Domain.RepoServices.Abstract {
    public abstract class RepositoryBase {
        protected string _connString;

        protected object DBNullToNull(object param) { if (param == DBNull.Value) return null; else return param; }

        protected T DBNullToDefault<T>(object param) { if (param == DBNull.Value) return default(T); else return (T)param; }

        /*
        protected SqlParameterCollection ExecuteNonQuery(string procName, SqlParameter[] parameters) {
            SqlConnection conn = new SqlConnection(_connString);
            using (conn) {
                SqlCommand cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                cmd.ExecuteNonQuery();
                return cmd.Parameters;
            }
        }
        */
        protected SqlParameterCollection ExecuteNonQuery(string procName, SqlParameter[] parameters) {
            SqlCommand cmd = PrepSqlCommand(procName, parameters);
            using (cmd.Connection) {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                return cmd.Parameters;
            }
        }

        protected object[] ExecuteReaderSingleEntity(string procName, SqlParameter[] parameters) {
            SqlConnection conn = new SqlConnection(_connString);
            using (conn) {
                SqlCommand cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    object[] values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    return values;
                } else
                    return null;
            }
        }

        //protected IEnumerable<TEntity> ExecuteReaderCollection<TEntity>(string procName, SqlParameter[] parameters) where TEntity:new() {
        //  pass delegate (lambda) param to work around M$'s omission of parameterized new() generic constraint
        //  (a less desirable solution would be to add some superfluous "Populate(object[])" method to EntityBase, and constrain the type that way)
        protected IList<TEntity> ExecuteReaderCollection<TEntity>(
                                           string procName, 
                                           SqlParameter[] parameters,
                                           Func<object[], TEntity> constructEntity) {
            List<TEntity> entities = new List<TEntity>();

            SqlConnection conn = new SqlConnection(_connString);
            using (conn) {
                SqlCommand cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                object[] values = new object[reader.FieldCount];
                while (reader.Read()) {
                    reader.GetValues(values);
                    //entities.Add(new TEntity(values));
                    entities.Add(constructEntity(values));
                }
            }
            return entities;
        }


        protected SqlCommand PrepSqlCommand(string procName, SqlParameter[] parameters) {
            SqlConnection conn = new SqlConnection(_connString);
            SqlCommand cmd = new SqlCommand(procName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);
            return cmd;
        }


        protected DataTable ExecuteFillTable(string procName, SqlParameter[] parameters) {
            SqlConnection conn = new SqlConnection(_connString);
            DataTable dataTable = new DataTable();
            using (conn) {
                SqlDataAdapter adapter = new SqlDataAdapter(procName, conn);
                adapter.Fill(dataTable);
            }
            return dataTable;
        }

















    }
}