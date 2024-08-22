using System.Collections.Generic;

namespace WebAPI.Services
{
    public interface IDependenciesService
    {
        int GetDescPartID(List<int> descriptionPartId);
        List<int> GetParentDependentParts(List<int> descriptionPartId);
    }
}