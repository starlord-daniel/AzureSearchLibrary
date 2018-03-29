using AzureSearch.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    [Serializable]
    public class SqlConnector
    {
        private string _connectionString;
        private string _tableName;

        public SqlConnector(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        /// Call the database to get the follow up
        /// <param name="index">The index in the follow up table.</param>
        internal async Task<List<(FollowUpObject result, string validationResult)>> GetFollowUpResultAsync(int index)
        {
            var validation = await ValidateSqlCredentialsAsync();

            if (!validation.validated)
            {
                return new List<(FollowUpObject result, string validationResult)> { (null, validation.result) };
            }

            List<(FollowUpObject, string)> followUpList = new List<(FollowUpObject, string)>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var query = $"SELECT * FROM {_tableName} WHERE Id = {index};";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var id = Convert.ToDecimal(reader["ID"]);
                        var response = reader["RESPONSE"] == null ? "" : reader["RESPONSE"].ToString();
                        var options = reader["OPTIONS"] == null ? "" : reader["OPTIONS"].ToString();
                        var followUp = reader["FOLLOW_UP"] == null ? "" : reader["FOLLOW_UP"].ToString();

                        followUpList.Add((new FollowUpObject
                        {
                            Id = id,
                            Response = response,
                            Options = options,
                            FollowUp = followUp,
                        }, "valid")
                        );
                    }
                }
                connection.Close();
            }

            return followUpList;
        }

        private async Task<(string result, bool validated)> ValidateSqlCredentialsAsync()
        {
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                connection.Close();
            }
            catch (Exception e)
            {
                return (e.ToString(), false);
            }
            
            return ("valid", true);
        }
    }
}
