using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Repository;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class DescriptiveCodesService
    {
        private readonly IConfiguration _configuration;

        public DescriptiveCodesService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private DescriptiveCodesRepository _descriptiveCodesRepository;

        private DescriptiveCodesRepository DescriptiveCodesRepository
        {
            get
            {
                if (_descriptiveCodesRepository == null)
                {
                    _descriptiveCodesRepository = new DescriptiveCodesRepository(_configuration);
                }
                return _descriptiveCodesRepository;
            }
        }

        public DataTable GetAllDescriptiveCodes()
        {
            return DescriptiveCodesRepository.GetAllDescriptiveCodes();
        }

        public int GetInsertDescriptiveCode(DescriptiveCodes descriptiveCodes)
        {
            return DescriptiveCodesRepository.GetInsertDescriptiveCode(descriptiveCodes);
        }

        public void GetUpdateDescriptiveCode(DescriptiveCodes descriptiveCodes)
        {
            DescriptiveCodesRepository.GetUpdateDescriptiveCode(descriptiveCodes);
        }

        public void GetDeleteDescriptiveCode(int descriptiveCodeID)
        {
            DescriptiveCodesRepository.GetDeleteDescriptiveCode(descriptiveCodeID);
        }
    }
}
