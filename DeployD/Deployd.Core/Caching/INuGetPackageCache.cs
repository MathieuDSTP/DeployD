using System.Collections;
using System.Collections.Generic;
using NuGet;

namespace Deployd.Core.Caching
{
    public interface INuGetPackageCache
    {
        IEnumerable<IPackage> AllCachedPackages();
        IList<string> AvailablePackages { get; }
        IEnumerable<string> AvailablePackageVersions(string packageId);
        void Add(IPackage package);
        void Add(IEnumerable<IPackage> allAvailablePackages);
        IPackage GetLatestVersion(string packageId);
        IPackage GetSpecificVersion(string packageId, string version);
    }
}