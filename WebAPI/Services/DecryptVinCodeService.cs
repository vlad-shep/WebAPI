using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI
{
    public class DecryptVinCodeService : IDecryptVinCodeService
    {
        private readonly IConfiguration _configuration;
        private readonly IDescriptionPartsService _descriptionPartsService;
        private readonly IMaskService _maskService;
        private readonly IDependenciesService _dependenciesService;

        public DecryptVinCodeService(IConfiguration configuration,
            IDescriptionPartsService descriptionPartsService,
            IDependenciesService dependenciesService,
            IMaskService maskService)
        {
            _configuration = configuration;
            _descriptionPartsService = descriptionPartsService;
            _dependenciesService = dependenciesService;
            _maskService = maskService;
        }

        public DataTable DecryptVin(string vinCode)
        {
            if (ValidateVinCode(vinCode))
            {
                MaskService maskService = new(_configuration);
                DescriptionPartsService descriptionPartsService = new(_configuration);

                List<string> independentMasks = _maskService.GetIndependentMasks();
                List<int> descriptionPartId = _descriptionPartsService.GetDescriptionPartID(independentMasks, vinCode);
                DataTable decryptIndependentMasks = _descriptionPartsService.GetDecryptIndependentMasks(independentMasks, vinCode);

                var getDivision = GetDivisionAndIndependentParts(independentMasks, descriptionPartId, vinCode);

                return GetDecryptedVinCode(getDivision, descriptionPartId, decryptIndependentMasks);
            }
            else
            {
                return null;
            }
        }

        public Tuple<List<VinNumberParts>, List<VinNumberParts>> GetDivisionAndIndependentParts(List<string> independetMasks, List<int> descriptionPartId, string vinCode)
        {
            MaskService maskService = new(_configuration);
            DescriptionPartsService descriptionPartsService = new(_configuration);
            
            List<VinNumberParts> vinParts = _maskService.GetTrimCharsFromDependentMasks(descriptionPartId, vinCode);
            List<int> dependentPartmaskID = _descriptionPartsService.GetMasksIDDependentPart(descriptionPartId);

            List<int> check = _descriptionPartsService.GetAllPartsID(vinParts, descriptionPartId);
            List<VinNumberParts> indepParts = _descriptionPartsService.GetIndependentParts(vinParts, check, dependentPartmaskID);
            List<VinNumberParts> depParts = _descriptionPartsService.GetIndependParts(vinParts, check, dependentPartmaskID);

            return Tuple.Create(indepParts, depParts);
        }
        public DataTable GetDecryptedVinCode(Tuple<List<VinNumberParts>, List<VinNumberParts>> getDivision, List<int> descriptionPartId, DataTable decryptIndependentMasks)
        {
            MaskService maskService = new(_configuration);
            DescriptionPartsService descriptionPartsService = new(_configuration);
            DependenciesService dependenciesService = new(_configuration);
           
            List<VinNumberParts> independentPart = getDivision.Item1;
            List<VinNumberParts> dependentPart = getDivision.Item2;
            List<int> masksID = GetParts(independentPart);
            List<int> parentID = _dependenciesService.GetParentDependentParts(descriptionPartId);
            DataTable decryptDependentParts = _descriptionPartsService.GetDecryptDependentParts(masksID, dependentPart, parentID);
            DataTable decryptIndependentParts = _descriptionPartsService.GetDecryptInDependentParts(independentPart);

            return _descriptionPartsService.GetConcatDataTables(decryptIndependentMasks, decryptIndependentParts, decryptDependentParts);
        }

        public bool ValidateVinCode (string vinCode)
        {
            if(string.IsNullOrEmpty(vinCode) || vinCode.Length < 16)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public List<int> GetParts(List<VinNumberParts> dependentPart)
        {
            List<int> partMasksID = new List<int>();

            foreach (var vinPart in dependentPart)
            {
                partMasksID.Add(vinPart.MaskID);
            }
            return partMasksID;
        }
    }
}
