using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using WebAPI.Repository;
using WebAPI.Models;
using System.Data;
using System.Web.Mvc;

namespace WebAPI.Services
{
    public class MaskService : IMaskService
    {
        private readonly IConfiguration _configuration;

        public MaskService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private MaskRepository _maskRepository;

        private MaskRepository MaskRepository
        {
            get
            {
                if (_maskRepository == null)
                {
                    _maskRepository = new MaskRepository(_configuration);
                }
                return _maskRepository;
            }
        }

        public List<string> GetIndependentMasks()
        {
            return MaskRepository.
                GetIndependentMasks();
        }

        public List<VinNumberParts> GetTrimCharsFromDependentMasks(List<int> descriptionPartId, string vinCode)
        {
            List<int> descrPartID = new List<int>(descriptionPartId);
            int descID = 0;
            while (descrPartID.Count != 0)
            {
                descID = descrPartID.ToArray()[0];
                descrPartID.RemoveAt(0);
            }

            return MaskRepository.
                GetTrimCharsFromDependentMasks(descID, vinCode);
        }
        public DataTable GetAllMasks()
        {
            return MaskRepository.GetAllMasks();
        }

        public int GetInsertMask(Masks mask)
        {
           return MaskRepository.GetInsertMask(mask);
        }

        public void GetUpdateMask(Masks mask)
        {
            MaskRepository.GetUpdateMask(mask);
        }

        public void GetDeleteMask(int maskID)
        {
            MaskRepository.GetDeleteMask(maskID);
        }
    }
}
