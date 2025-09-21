
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MasjidConnect.Infranstructure.DbHelpers
{
    public interface IDbExecuter
    {
        Task<DataSet> ExecuteProcedureDsAsync(string procedureName, params SqlParameter[] parameters);
        Task<DataTable> ExecuteProcedureDtAsync(string procedureName, params SqlParameter[] parameters);
        Task<int> ExecuteNonQueryAsync(string procedureName, params SqlParameter[] parameters);
        Task<object?> ExecuteScalarAsync(string procedureName, params SqlParameter[] parameters);
        Task<SqlDataReader> ExecuteProcedureDrAsync(string procedureName, params SqlParameter[] parameters);
    }

    /// <summary>
    /// A DbExecuter class that has methods to execute proc and return different return types
    /// </summary>
    public class DbExecuter : IDbExecuter
    {
        private readonly string _connectionString;

        public DbExecuter(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Executes a stored procedure asynchronously and returns a DataTable.
        /// </summary>
        public async Task<DataSet> ExecuteProcedureDsAsync(string procedureName, params SqlParameter[] parameters)
        {
            var ds = new DataSet();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    if (con.State != ConnectionState.Open)
                        await con.OpenAsync();

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds); // synchronous fill
                    }
                }
            }
            catch (SqlException ex)
            {
                // SQL Server specific errors
                throw new Exception($"Database error while executing {procedureName}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // General errors
                throw new Exception($"Unexpected error while executing {procedureName}: {ex.Message}", ex);
            }

            return ds;
        }
        /// <summary>
        /// Executes a stored procedure asynchronously and returns a DataTable.
        /// </summary>
        public async Task<DataTable> ExecuteProcedureDtAsync(string procedureName, params SqlParameter[] parameters)
        {
            var dt = new DataTable();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    if (con.State != ConnectionState.Open)
                        await con.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                // SQL Server specific errors
                throw new Exception($"Database error while executing {procedureName}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // General errors
                throw new Exception($"Unexpected error while executing {procedureName}: {ex.Message}", ex);
            }

            return dt;
        }


        /// <summary>
        /// Executes a stored procedure asynchronously and returns a DataTable.
        /// </summary>
        public async Task<SqlDataReader> ExecuteProcedureDrAsync(string procedureName, params SqlParameter[] parameters)
        {
            try
            {
                var con = new SqlConnection(_connectionString);
                if (con.State != ConnectionState.Open)
                    await con.OpenAsync();

                var cmd = new SqlCommand(procedureName, con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                // reader will own the connection, so when reader.Dispose() → connection closes
                return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);


                //using (var con = new SqlConnection(_connectionString))
                //using (var cmd = new SqlCommand(procedureName, con))
                //{
                //    cmd.CommandType = CommandType.StoredProcedure;

                //    if (parameters != null && parameters.Length > 0)
                //        cmd.Parameters.AddRange(parameters);

                //    if (con.State != ConnectionState.Open)
                //        await con.OpenAsync();

                //    return await cmd.ExecuteReaderAsync();                    
                //}
            }
            catch (SqlException ex)
            {
                // SQL Server specific errors
                throw new Exception($"Database error while executing {procedureName}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // General errors
                throw new Exception($"Unexpected error while executing {procedureName}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a stored procedure asynchronously that doesn’t return rows (INSERT, UPDATE, DELETE).
        /// Returns number of rows affected.
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string procedureName, params SqlParameter[] parameters)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    if (con.State != ConnectionState.Open)
                        await con.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error while executing {procedureName}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while executing {procedureName}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a stored procedure asynchronously and returns a single value.
        /// </summary>
        public async Task<object?> ExecuteScalarAsync(string procedureName, params SqlParameter[] parameters)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    if (con.State != ConnectionState.Open)
                        await con.OpenAsync();
                    return await cmd.ExecuteScalarAsync();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error while executing {procedureName}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while executing {procedureName}: {ex.Message}", ex);
            }
        }
    }
}
