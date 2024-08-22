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
    internal class DescriptionPartsRepository
    {
        private readonly IConfiguration _configuration;

        public DescriptionPartsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public DataTable GetAllDescriptionParts()
        {
            SqlDataReader sqlDataReader;
            DataTable descriptionPartsTable = new DataTable();
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"SELECT descriptionPartID, tbDescriptionParts.maskID, tbMasks.maskContent, tbDescriptionParts.descriptiveCodeID, 
                                    tbDescriptiveCodes.descriptiveCodeName, descriptionPartSymbols, 
                                    characteristicDescriptionPartSymbols 
                             FROM tbDescriptionParts 
                             JOIN tbMasks 
                                ON tbDescriptionParts.maskID = tbMasks.maskID
                             JOIN tbDescriptiveCodes
                                 ON tbDescriptionParts.descriptiveCodeID = tbDescriptiveCodes.descriptiveCodeID";
            using(SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using(SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlDataReader = sqlCommand.ExecuteReader();
                    descriptionPartsTable.Load(sqlDataReader);
                    sqlDataReader.Close();
                }
            }
            return descriptionPartsTable;
        }

        public int GetInsertDescriptionParts(DescriptionParts descriptionParts)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"INSERT INTO tbDescriptionParts (maskID, descriptiveCodeID, descriptionPartSymbols, characteristicDescriptionPartSymbols) VALUES (" + descriptionParts.maskID + @"," +
                " " + descriptionParts.descriptiveCodeID + ", '" + descriptionParts.descriptionPartSymbols + "', '" + descriptionParts.characteristicDescriptionPartSymbols + "' )"
                + "SELECT CAST(scope_identity() AS int)";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    descriptionParts.descriptionPartID = (Int32)sqlCommand.ExecuteScalar();
                }
            }
            return descriptionParts.descriptionPartID;
        }

        public void GetUpdateDescriptionParts(DescriptionParts descriptionParts)
        {
            SqlDataReader sqlDataReader;
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"UPDATE tbDescriptionParts 
                             SET maskID = " + descriptionParts.maskID + @",
                                 descriptiveCodeID = " + descriptionParts.descriptiveCodeID + @",
                                 descriptionPartSymbols = '" + descriptionParts.descriptionPartSymbols + @"',
                                 characteristicDescriptionPartSymbols = ' " + descriptionParts.characteristicDescriptionPartSymbols + @"'
                             WHERE descriptionPartID = " + descriptionParts.descriptionPartID + "";
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

        public void GetDeleteDescriptionParts(int descriptionPartID)
        {
            SqlDataReader sqlDataReader;
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"DELETE FROM tbDescriptionParts WHERE descriptionPartID = " + descriptionPartID + @"";
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

        public List<int> GetDescriptionPartID(string trimChars)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");
            List<int> descriptionPartID = new List<int>();
            string query = $@"SELECT TOP 1 descriptionPartID FROM tbDescriptionParts
                             WHERE descriptionPartSymbols LIKE '{trimChars}'";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    int descPartID = (int)sqlCommand.ExecuteScalar();
                    descriptionPartID.Add(descPartID);
                }
            }
            return descriptionPartID;
        }
        public DataTable GetDecryptIndependentMasks(string trimChars, string maskContent)
        {
            DataTable decryptVinCodeDataTable = new DataTable();
            string connection = _configuration.GetConnectionString("DefaultConnection");
            const string query = @"SELECT descCodes.descriptiveCodeName, descParts.characteristicDescriptionPartSymbols
			                       FROM tbDescriptionParts AS descParts 
			                       JOIN  tbDescriptiveCodes descCodes ON descCodes.descriptiveCodeID = descParts.descriptiveCodeID 
			                       WHERE descParts.descriptionPartSymbols = @symbol AND descParts.maskID =  
                                   (SELECT maskID FROM tbMasks WHERE maskContent = @mask)";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("symbol", trimChars);
                    sqlCommand.Parameters.AddWithValue("mask", maskContent);
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    decryptVinCodeDataTable.Load(sqlDataReader);
                }
            }
            return decryptVinCodeDataTable;
        }

        public List<int> GetMasksIDDependentPart(int descID)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");
            List<int> descriptionPartID = new List<int>();
            const string query = @"SELECT maskID  
								FROM tbDescriptionParts  
								WHERE descriptionPartID IN  
									(SELECT childDependentPart FROM tbDependencies WHERE parentDependentPart IN  
									(SELECT childDependentPart FROM tbDependencies WHERE parentDependentPart = @descID INTERSECT 
									 SELECT parentDependentPart FROM tbDependencies GROUP BY parentDependentPart)) GROUP BY maskID";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("descID", descID);

                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            int descPartID = (int)sqlDataReader.GetValue(0);
                            descriptionPartID.Add(descPartID);
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            return descriptionPartID;
        }

        public List<int> GetAllPartsID(int descID, string symbols, string mask, List<int> descriptId)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");
            const string query = @"SELECT descripPartID FROM fGetParts(@descID, @symbol, @mask)";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("descID", descID);
                    sqlCommand.Parameters.AddWithValue("symbol", symbols);
                    sqlCommand.Parameters.AddWithValue("mask", mask);
                    try
                    {
                        descriptId.Add((int)sqlCommand.ExecuteScalar());
                    }
                    catch
                    {
                        sqlConnection.Close();
                    }
                }
            }
            return descriptId;
        }

        public DataTable GetDecryptDependentParts(int[] myarr, List<VinNumberParts> dependentParts, string symbols, string maskContent, DataTable decryptVinCodeDataTable)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = $@"SELECT descCodes.descriptiveCodeName, descPart.characteristicDescriptionPartSymbols  
				              FROM tbDescriptionParts AS descPart 
					          JOIN tbDescriptiveCodes descCodes ON descPart.descriptiveCodeID = descCodes.descriptiveCodeID 
					          JOIN tbMasks ON descPart.maskID = tbMasks.maskID   
						            WHERE descriptionPartSymbols = @symbol AND tbMasks.maskContent = @mask AND descriptionPartID IN  
						            (SELECT childDependentPart FROM tbDependencies WHERE parentDependentPart = @parentPart)";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("symbol", symbols);
                    sqlCommand.Parameters.AddWithValue("mask", maskContent);
                    sqlCommand.Parameters.AddWithValue("parentPart", myarr[0]);

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    decryptVinCodeDataTable.Load(sqlDataReader);
                }
            }
            return decryptVinCodeDataTable;
        }

        public DataTable GetDecryptInDependentParts(List<VinNumberParts> independentParts, int partID, string symbols, string maskContent, DataTable masksDataTable)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"SELECT descCodes.descriptiveCodeName, descPart.characteristicDescriptionPartSymbols FROM tbDescriptionParts AS descPart 
		                      JOIN tbDescriptiveCodes descCodes ON descPart.descriptiveCodeID = descCodes.descriptiveCodeID 
		                      JOIN tbMasks mask ON descPart.maskID = mask.maskID  
		                      WHERE descPart.descriptionPartID = @descPartID AND descPart.descriptionPartSymbols = @symbol AND mask.maskContent = @mask";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("descPartID", partID);
                    sqlCommand.Parameters.AddWithValue("symbol", symbols);
                    sqlCommand.Parameters.AddWithValue("mask", maskContent);
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                    masksDataTable.Load(sqlDataReader);
                }
            }
            return masksDataTable;
        }
    }
}
