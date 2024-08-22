using System.Collections.Generic;
using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IMaskService
    {
        List<string> GetIndependentMasks();
        List<VinNumberParts> GetTrimCharsFromDependentMasks(List<int> descriptionPartId, string vinCode);
    }
}