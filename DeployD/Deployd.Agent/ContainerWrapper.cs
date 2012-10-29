using System.Collections.Generic;
using Deployd.Core.Hosting;
using Ninject;
using Ninject.Activation.Blocks;

namespace Deployd.Agent
{
    public class ContainerWrapper : IIocContainer
    {
        private readonly IKernel _kernel;

        public ContainerWrapper(IKernel kernel)
        {
            _kernel = kernel;
        }

        public T GetType<T>()
        {
            return _kernel.Get<T>();
        }

        public IEnumerable<T> GetTypes<T>()
        {
            return _kernel.GetAll<T>();
        }

        public IActivationBlock BeginBlock()
        {
            return _kernel.BeginBlock();
        }
    }
}