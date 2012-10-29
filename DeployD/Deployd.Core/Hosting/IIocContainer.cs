using System.Collections.Generic;
using Ninject.Activation.Blocks;

namespace Deployd.Core.Hosting
{
	public interface IIocContainer
	{
	    T GetType<T>();
	    IEnumerable<T> GetTypes<T>();
	    IActivationBlock BeginBlock();
	}
}
