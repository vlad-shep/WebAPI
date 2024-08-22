using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class DependenciesService : IDependenciesService
    {
        private readonly IConfiguration _configuration;

        public DependenciesService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private DependenciesRepository _dependenciesRepository;
        private DependenciesRepository DependenciesRepository
        {
            get
            {
                if (_dependenciesRepository == null)
                {
                    _dependenciesRepository = new DependenciesRepository(_configuration);
                }
                return _dependenciesRepository;
            }
        }

        public List<int> GetParentDependentParts(List<int> descriptionPartId)
        {
            List<int> parentID = new List<int>(descriptionPartId);
            int descID = GetDescPartID(parentID);

            return DependenciesRepository.
                GetParentDependentParts(descID);
        }

        public int GetDescPartID(List<int> descriptionPartId)
        {
            int descID = 0;
            while (descriptionPartId.Count != 0)
            {
                descID = descriptionPartId.ToArray()[0];
                descriptionPartId.RemoveAt(0);
            }

            return descID;
        }
    }
}
