using System;
using System.Collections.Generic;
using System.Data;
using WebAPI.Models;

namespace WebAPI
{
    public interface IDecryptVinCodeService
    {
        DataTable DecryptVin(string vinCode);
        DataTable GetDecryptedVinCode(Tuple<List<VinNumberParts>, List<VinNumberParts>> getDivision, List<int> descriptionPartId, DataTable decryptIndependentMasks);
        Tuple<List<VinNumberParts>, List<VinNumberParts>> GetDivisionAndIndependentParts(List<string> independetMasks, List<int> descriptionPartId, string vinCode);
        List<int> GetParts(List<VinNumberParts> independentPart);
    }
}