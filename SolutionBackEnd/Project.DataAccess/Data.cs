using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Milano.BackEnd.Dto;
using System.Dynamic;
using System.Collections;

namespace Milano.BackEnd.DataAccess
{

    /// <summary>
    /// Clase para acceso a base de datos
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Conexion de sql
        /// </summary>
        protected SqlConnection conn;

        /// <summary>
        /// Constructor por default
        /// </summary>
        public Data()
        {
        }

        /// <summary>
        /// Metodo para Obtener un conjunto de valores
        /// </summary>
        /// <param name="nameProcedure"> nombre del procedimiento almacenado</param>
        /// /// <param name="parameters"> arreglo de parametros</param>
        /// <returns></returns>
        public DataTable GetDataTable(string nameProcedure, IDictionary<string, object> parameters)
        {
            try
            {
                using (conn = this.GetConnection())
                {
                    DataSet ds = new DataSet();
                    SqlCommand command = new SqlCommand();
                    command.CommandTimeout = 120000;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = nameProcedure;
                    command.Connection = conn;
                    foreach (var p in parameters)
                    {
                        SqlParameter par = new SqlParameter();
                        par.ParameterName = p.Key;
                        par.Value = p.Value;
                        command.Parameters.Add(par);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);
                    return ds.Tables[0];
                }
            }
            catch (SqlException excepcion)
            {
                throw new Exception(excepcion.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Obtiene una coleccion de datos mediante un reader
        /// </summary>
        /// <param name="nameProcedure">nombre del procedimiento almacenado</param>
        /// <param name="parameters">parametros de entrada</param>
        /// <returns></returns>
        public IEnumerable<IDataRecord> GetDataReader(string nameProcedure, IDictionary<string, object> parameters)
        {
            using (conn = this.GetConnection())
            {
                SqlCommand command = new SqlCommand();
                command.CommandTimeout = 120000;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = nameProcedure;
                command.Connection = conn;
                foreach (var p in parameters)
                {
                    SqlParameter par = new SqlParameter();
                    par.ParameterName = p.Key;
                    par.Value = p.Value;
                    command.Parameters.Add(par);
                }
                using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    try
                    {
                        while (reader.Read())
                            yield return reader;
                    }
                    finally
                    {
                        reader.Close();
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado
        /// </summary>
        /// <param name="nameProcedure">nombre del procedimiento</param>
        /// <param name="parameters">parametros de entrada</param>
        /// <param name="parametersOut">parametros de salida</param>
        /// <returns></returns>
        public IDictionary<string, object> ExecuteProcedure(string nameProcedure, IDictionary<string, object> parameters, List<SqlParameter> parametersOut = null)
        {
            Dictionary<string, object> parametersList = new Dictionary<string, object>();
            try
            {
                using (conn = this.GetConnection())
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandTimeout = 120000;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = nameProcedure;
                    command.Connection = conn;
                    foreach (var p in parameters)
                    {
                        SqlParameter par = new SqlParameter();
                        par.ParameterName = p.Key;
                        if (p.Value.GetType() == Type.GetType("System.Decimal"))
                        {
                            decimal conversionDecimal = decimal.Parse(p.Value.ToString());
                            if (conversionDecimal != 0)
                            {
                                String nuevoValorDecimal = (Math.Truncate(100 * conversionDecimal) / 100).ToString();
                                decimal valorFinalDecimal = decimal.Parse(nuevoValorDecimal);
                                par.Value = valorFinalDecimal;
                            }
                            else
                            {
                                par.Value = p.Value;
                            }
                        }
                        else
                        {
                            par.Value = p.Value;
                        }
                        command.Parameters.Add(par);
                    }
                    if (parametersOut != null)
                    {

                        foreach (SqlParameter p in parametersOut)
                            command.Parameters.Add(p);
                    }

                    int result = command.ExecuteNonQuery();
                    foreach (SqlParameter p in command.Parameters)
                    {
                        if (p.Direction == ParameterDirection.Output)
                            parametersList.Add(p.ParameterName, p.Value);

                    }
                    return parametersList;
                }
            }
            catch (SqlException excepcion)
            {
                throw new Exception(excepcion.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Ejecuta un Script pasado como argumento
        /// </summary>
        /// <param name="script">Script de base de datos que debe ejecutarse</param>        
        /// <returns></returns>
        public void ExecuteDBScript(String script)
        {
            try
            {
                using (conn = this.GetConnection())
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandTimeout = 120000;
                        cmd.CommandText = script;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException excepcion)
            {
                throw new Exception(excepcion.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Metodo que genera una nueva conexion
        /// </summary>
        /// <returns></returns>
        private SqlConnection GetConnection()
        {
            string connection = ConfigurationManager.ConnectionStrings["milano"].ConnectionString;
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder(connection);
            csb.Password = Encriptar.DesencriptarCadena(csb.Password);
            conn = new SqlConnection(csb.ToString());
            conn.Open();
            return conn;
        }

    }
}

