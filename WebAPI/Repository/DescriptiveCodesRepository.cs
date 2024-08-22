using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repository
{
    public class DescriptiveCodesRepository
    {
        private readonly IConfiguration _configuration;

        public DescriptiveCodesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DataTable GetAllDescriptiveCodes()
        {
            SqlDataReader sqlDataReader;
            DataTable descriptiveCodesTable = new DataTable();
            string connection = _configuration.GetConnectionString("DefaultConnection");
            const string query = @"SELECT descriptiveCodeID, descriptiveCodeName FROM tbDescriptiveCodes";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlDataReader = sqlCommand.ExecuteReader();
                    descriptiveCodesTable.Load(sqlDataReader);
                    sqlDataReader.Close();
                }
            }

            return descriptiveCodesTable;
        }

        public int GetInsertDescriptiveCode(DescriptiveCodes descriptiveCodes)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"INSERT INTO tbDescriptiveCodes (descriptiveCodeName) VALUES ('" + descriptiveCodes.descriptiveCodeName + "')"
                          + "SELECT CAST(scope_identity() AS int)"; 
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    descriptiveCodes.descriptiveCodeID = (Int32)sqlCommand.ExecuteScalar();
                }
            }
            return descriptiveCodes.descriptiveCodeID;
        }

        public void GetUpdateDescriptiveCode(DescriptiveCodes descriptiveCodes)
        {
            SqlDataReader sqlDataReader;
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"UPDATE tbDescriptiveCodes 
                             SET descriptiveCodeName = '" + descriptiveCodes.descriptiveCodeName + @"'
                             WHERE descriptiveCodeID = " + descriptiveCodes.descriptiveCodeID + @"";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Close();
                }
            }
        }

        public void GetDeleteDescriptiveCode(int descriptiveCodeID)
        {
            SqlDataReader sqlDataReader;
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"DELETE FROM tbDescriptiveCodes WHERE descriptiveCodeID = " + descriptiveCodeID + @"";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Close();
                }
            }
        }
    }
}
