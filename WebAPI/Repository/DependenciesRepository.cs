using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Repository
{
    public class DependenciesRepository
    {
        private readonly IConfiguration _configuration;

        public DependenciesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<int> GetParentDependentParts(int descID)
        {
            string connection = _configuration.GetConnectionString("DefaultConnection");

            List<int> parentID = new List<int>();
            const string query = @"SELECT parentDependentPart FROM tbDependencies WHERE ChildDependentPart IN 
									(SELECT childDependentPart FROM tbDependencies WHERE parentDependentPart IN  
									(SELECT childDependentPart FROM tbDependencies WHERE parentDependentPart = @descID INTERSECT 
									 SELECT parentDependentPart FROM tbDependencies GROUP BY parentDependentPart)) GROUP BY parentDependentPart";
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
                            parentID.Add((int)sqlDataReader.GetValue(0));
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            return parentID;
        }
    }
}
