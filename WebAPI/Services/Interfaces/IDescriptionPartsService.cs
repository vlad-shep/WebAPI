using System;
using System.Collections.Generic;
using System.Data;
using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IDescriptionPartsService
    {
        List<int> GetAllPartsID(List<VinNumberParts> vinParts, List<int> descriptionPartId);
        DataTable GetConcatDataTables(DataTable independentMasks, DataTable vin, DataTable vinCode);
        DataTable GetDecryptDependentParts(List<int> part, List<VinNumberParts> dependentParts, List<int> parentIDS);
        DataTable GetDecryptIndependentMasks(List<string> independetMasks, string vinCode);
        DataTable GetDecryptInDependentParts(List<VinNumberParts> independentParts);
        int GetDescPartID(List<int> descriptionPartId);
        List<int> GetDescriptionPartID(List<string> independetMasks, string vinCode);
        List<VinNumberParts> GetIndependentParts(List<VinNumberParts> vinParts, List<int> descriptID, List<int> dependentMasksID);
        List<VinNumberParts> GetIndependParts(List<VinNumberParts> vinParts, List<int> descriptID, List<int> dependentMasksID);
        List<int> GetMasksIDDependentPart(List<int> descriptionPartId);
        string GetTrimChars(List<string> masks, string vinCode);
        Tuple<int, string, string> GetVinNumberPartString(List<VinNumberParts> vinParts);
    }
}