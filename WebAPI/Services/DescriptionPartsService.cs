using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Repository;
using WebAPI.Models;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace WebAPI.Services
{
    public class DescriptionPartsService : IDescriptionPartsService
    {
        private readonly IConfiguration _configuration;

        public DescriptionPartsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private DescriptionPartsRepository _descriptionPartRepository;
        private DescriptionPartsRepository DescriptionPartsRepository
        {
            get
            {
                if (_descriptionPartRepository == null)
                {
                    _descriptionPartRepository = new DescriptionPartsRepository(_configuration);
                }
                return _descriptionPartRepository;
            }
        }

        public DataTable GetAllDescriptionParts()
        {
            return DescriptionPartsRepository.GetAllDescriptionParts();
        }

        public int GetInsertDescriptionParts(DescriptionParts descriptionParts)
        {
            return DescriptionPartsRepository.GetInsertDescriptionParts(descriptionParts);
        }

        public void GetUpdateDescriptionParts(DescriptionParts descriptionParts)
        {
            DescriptionPartsRepository.GetUpdateDescriptionParts(descriptionParts);
        }

        public void GetDeleteDescriptionParts(int descriptionpartID)
        {
            DescriptionPartsRepository.GetDeleteDescriptionParts(descriptionpartID);
        }
        public List<int> GetDescriptionPartID(List<string> independetMasks, string vinCode)
        {
            List<string> masks = new List<string>(independetMasks);
            string trimChars = "";
            while (masks.Count != 0)
            {
                trimChars = GetTrimChars(masks, vinCode);
                masks.RemoveAt(0);
            }

            return DescriptionPartsRepository.
                    GetDescriptionPartID(trimChars);
        }

        public DataTable GetDecryptIndependentMasks(List<string> independetMasks, string vinCode)
        {
            List<string> masks = new List<string>(independetMasks);
            string trimChars = "", maskContent = "";
            while (masks.Count != 0)
            {
                trimChars = GetTrimChars(masks, vinCode);
                maskContent = masks.ToArray()[0];
                masks.RemoveAt(0);
            }
            return DescriptionPartsRepository.
                    GetDecryptIndependentMasks(trimChars, maskContent);
        }

        public List<int> GetMasksIDDependentPart(List<int> descriptionPartId)
        {
            List<int> descrPartID = new List<int>(descriptionPartId);
            int descID = GetDescPartID(descrPartID);

            return DescriptionPartsRepository.
                GetMasksIDDependentPart(descID);
        }

        public List<int> GetAllPartsID(List<VinNumberParts> vinParts, List<int> descriptionPartId)
        {
            List<int> descriptId = new List<int>();
            List<VinNumberParts> vinNumberParts = new List<VinNumberParts>(vinParts);
            int descID = descriptionPartId.ToArray()[0];
            string symbol = "";
            string mask = "";

            while (vinNumberParts.Count != 0)
            {
                var vinPart = GetVinNumberPartString(vinNumberParts);
                symbol = vinPart.Item2;
                mask = vinPart.Item3;
                descriptId = DescriptionPartsRepository.
                   GetAllPartsID(descID, symbol, mask, descriptId);
                vinNumberParts.RemoveAt(0);
            }
            return descriptId;
        }

        public Tuple<int, string, string> GetVinNumberPartString(List<VinNumberParts> vinParts)
        {
            int maskID = 0;
            string symbols = "";
            string mask = "";
            List<VinNumberParts> parts = new List<VinNumberParts>(vinParts);
            foreach (var vinPart in parts)
            {
                maskID = vinPart.MaskID;
                symbols = vinPart.Symbols.ToString();
                mask = vinPart.MaskContent.ToString();
                parts.RemoveAt(0);
                break;
            }
            return Tuple.Create(maskID, symbols, mask);
        }

        public List<VinNumberParts> GetIndependentParts(List<VinNumberParts> vinParts, List<int> descriptID, List<int> dependentMasksID)
        {
            List<int> descID = new List<int>(descriptID);
            List<VinNumberParts> vinNumberParts = new List<VinNumberParts>(vinParts);
            List<VinNumberParts> listIndependentIDParts = new List<VinNumberParts>();
            List<int> dependentMask = new List<int>(dependentMasksID);
            while (descID.Count != 0)
            {
                VinNumberParts vinnumParts = new VinNumberParts();
                var vinPart = GetVinNumberPartString(vinNumberParts);
                if (dependentMask.BinarySearch(vinPart.Item1) < 0)
                {
                    vinnumParts.MaskID = descID.ToArray()[0];
                    vinnumParts.Symbols = vinPart.Item2;
                    vinnumParts.MaskContent = vinPart.Item3;
                    listIndependentIDParts.Add(vinnumParts);
                }
                vinNumberParts.RemoveAt(0);
                descID.RemoveAt(0);
            }
            return listIndependentIDParts;
        }

        public List<VinNumberParts> GetIndependParts(List<VinNumberParts> vinParts, List<int> descriptID, List<int> dependentMasksID)
        {
            List<VinNumberParts> vinNumberParts = new List<VinNumberParts>(vinParts);
            List<VinNumberParts> listDependentIDParts = new List<VinNumberParts>();
            List<int> dependentMask = new List<int>(dependentMasksID);
            while (descriptID.Count != 0)
            {
                VinNumberParts vinnumParts = new VinNumberParts();
                var vinPart = GetVinNumberPartString(vinNumberParts);
                if (dependentMask.BinarySearch(vinPart.Item1) >= 0)
                {
                    vinnumParts.MaskID = descriptID.ToArray()[0];
                    vinnumParts.Symbols = vinPart.Item2;
                    vinnumParts.MaskContent = vinPart.Item3;
                    listDependentIDParts.Add(vinnumParts);
                }
                vinNumberParts.RemoveAt(0);
                descriptID.RemoveAt(0);
            }
            return listDependentIDParts;
        }
        public DataTable GetDecryptDependentParts(List<int> part, List<VinNumberParts> dependentParts, List<int> parentIDS)
        {
            string symbol = "";
            string mask = "";
            DataTable decryptVinCodeDataTable = new DataTable();
            DataTable dependentTable = new DataTable();
            var intersecting = parentIDS.Intersect(part);
            int[] myarr = intersecting.ToArray();

            while (dependentParts.Count != 0)
            {
                var vinPart = GetStringDependentParts(dependentParts);
                symbol = vinPart.Item2;
                mask = vinPart.Item3;
                decryptVinCodeDataTable = DescriptionPartsRepository.
                   GetDecryptDependentParts(myarr, dependentParts, symbol, mask, decryptVinCodeDataTable);
                dependentParts.RemoveAt(0);
            }
            return decryptVinCodeDataTable;
        }

        public DataTable GetDecryptInDependentParts(List<VinNumberParts> independentParts)
        {
            int partID = 0;
            string symbol = "";
            string mask = "";
            DataTable masksDataTable = new DataTable();
            while (independentParts.Count != 0)
            {
                var vinPart = GetStringDependentParts(independentParts);
                partID = vinPart.Item1;
                symbol = vinPart.Item2;
                mask = vinPart.Item3;

                masksDataTable = DescriptionPartsRepository.
                   GetDecryptInDependentParts(independentParts, partID, symbol, mask, masksDataTable);
                independentParts.RemoveAt(0);
            }
            return masksDataTable;
        }

        public DataTable GetConcatDataTables(DataTable independentMasks, DataTable vin, DataTable vinCode)
        {
            DataTable decryptVinCodeTable = independentMasks.Copy();
            decryptVinCodeTable.Merge(vin, false, MissingSchemaAction.Ignore);
            decryptVinCodeTable.Merge(vinCode, false, MissingSchemaAction.Ignore);

            return decryptVinCodeTable;
        }

        public Tuple<int, string, string> GetStringDependentParts(List<VinNumberParts> dependentParts)
        {
            int partID = 0;
            string symbols = "";
            string maskContent = "";

            foreach (var item in dependentParts)
            {
                partID = item.MaskID;
                symbols = item.Symbols.ToString();
                maskContent = item.MaskContent.ToString();
                break;
            }

            return Tuple.Create(partID, symbols, maskContent);
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

        public string GetTrimChars(List<string> masks, string vinCode)
        {
            string trimChars = "";
            foreach (var maskContent in masks)
            {
                string mask = maskContent.ToString();
                int trimPart = mask.IndexOf('X');
                int countChars = mask.Where(x => "X".IndexOf(x) != -1).Count();
                trimChars = vinCode.Substring(trimPart, countChars);
            }
            return trimChars;
        }

    }
}
