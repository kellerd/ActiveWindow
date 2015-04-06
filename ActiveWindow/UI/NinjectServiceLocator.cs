using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveWindow;
using Ninject.Extensions.Conventions;
using System.IO;
using System.Reflection;
using ActiveWindowLib;
using ActiveWindow.ViewModels;
namespace ActiveWindow.Ioc
{
    public class NinjectServiceLocator
    {
        private readonly IKernel kernel;

        public NinjectServiceLocator()
        {
            kernel = new StandardKernel();
            kernel.Bind(scanner =>
            {
                scanner.FromThisAssembly().SelectAllClasses().BindToSelf();
                scanner.FromAssemblyContaining<ActiveWindowLib.IOperation>().SelectAllClasses().BindToSelf();
            });
        }

        public MainViewModel MainViewModel
        {
            get { return kernel.Get<MainViewModel>(); }
        }
    }
}
