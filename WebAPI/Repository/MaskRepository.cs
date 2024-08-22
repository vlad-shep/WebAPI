using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebAPI.Models;

namespace WebAPI.Repository
{
    public class MaskRepository
    {
        private readonly IConfiguration _configuration;

        public MaskRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetIndependentMasks()
        {
            List<string> masks = new List<string>();
            string connection = _configuration.GetConnectionString("DefaultConnection");
            const string query = @"SELECT masks.maskContent 
		                           FROM tbMasks masks 
			                          JOIN tbDescriptionParts descpart  
		                                  ON masks.maskID = descpart.maskID 
			                          LEFT JOIN tbDependencies depend 
		                                  ON descpart.descriptionPartID = depend.childDependentPart 
			                        WHERE depend.childDependentPart IS NULL 
			                        GROUP BY masks.maskID, masks.maskContent";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            masks.Add(sqlDataReader.GetValue(0).ToString());
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            return masks;
        }

        public List<VinNumberParts> GetTrimCharsFromDependentMasks(int descID, string vinCode)
        {
            List<VinNumberParts> vinParts = new List<VinNumberParts>();
            string connection = _configuration.GetConnectionString("DefaultConnection");
            const string query = @"SELECT maskID, SUBSTRING(@VINCode, CHARINDEX('X', maskContent), LEN(RTRIM(LTRIM(REPLACE(maskContent, '0', ' '))))) as symbols,  maskContent 
	                               FROM tbMasks  
	                               WHERE maskID IN (SELECT maskID FROM tbDescriptionParts WHERE descriptionPartID  
	                               IN (SELECT childDependentPart FROM tbDependencies WHERE parentDependentPart = @descID) GROUP BY maskID)";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("descID", descID);
                    sqlCommand.Parameters.AddWithValue("VINCode", vinCode);
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            VinNumberParts vinNumberParts = new VinNumberParts();
                            vinNumberParts.MaskID = (int)sqlDataReader.GetValue(0);
                            vinNumberParts.Symbols = sqlDataReader.GetValue(1).ToString();
                            vinNumberParts.MaskContent = sqlDataReader.GetValue(2).ToString();
                            vinParts.Add(vinNumberParts);
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            return vinParts;
        }

        public DataTable GetAllMasks()
        {
            SqlDataReader sqlDataReader;
            DataTable tableMasks = new DataTable();
            string connection = _configuration.GetConnectionString("DefaultConnection");
            const string query = @"SELECT maskID, maskContent FROM tbMasks";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlDataReader = sqlCommand.ExecuteReader();
                    tableMasks.Load(sqlDataReader);
                    sqlDataReader.Close();
                }
            }
            return tableMasks;
        }
        public int GetInsertMask(Masks mask)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"INSERT INTO tbMasks (maskContent) VALUES ('"+ mask.maskContent +"') " 
                          + "SELECT CAST(scope_identity() AS int)";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    mask.maskID = (Int32)sqlCommand.ExecuteScalar();
                }
            }
            return mask.maskID;
        }
        
        public void GetUpdateMask(Masks mask)
        {
            SqlDataReader sqlDataReader;
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = @"UPDATE tbMasks 
                             SET maskContent = '" + mask.maskContent + @"'
                             WHERE maskID = " + mask.maskID + @"";
            using(SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Close();
                }
            }
        }

        public void GetDeleteMask(int maskID)
        {
            SqlDataReader sqlDataReader;
            string connecttion = _configuration.GetConnectionString("DefaultConnection");
            string query = @"DELETE FROM tbMasks WHERE maskID = " + maskID + @"";
            using (SqlConnection sqlConnection = new SqlConnection(connecttion))
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
