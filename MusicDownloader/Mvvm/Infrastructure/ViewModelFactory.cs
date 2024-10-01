using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MusicDownloader.Mvvm.Infrastructure
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IResolutionRoot _root;

        private readonly Regex _modelTypeNameRegex = new(
            @"^(?<ns>[A-z0-9\.]+\.)(Models)(?<class>\.[A-z0-9\.]+)(Model)$",
            RegexOptions.Compiled);

        public ViewModelFactory(IResolutionRoot root)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
        }

        public IViewModel Create<TModel>(TModel model)
            where TModel : ModelBase
        {
            var modelType = model.GetType();

            // TODO: log

            var modelTypeName = modelType.FullName;
            var modelTypeAssemblyName = modelType.GetTypeInfo().Assembly.FullName;

            if (!_modelTypeNameRegex.IsMatch(modelTypeName))
            {
                // TODO: log
            }

            var viewModelTypeName = _modelTypeNameRegex.Replace(modelTypeName, "${ns}ViewModels${class}ViewModel");
            var viewModelNameWithAssembly = $"{viewModelTypeName}, {modelTypeAssemblyName}";
            var viewModelType = Type.GetType(viewModelNameWithAssembly);

            // TODO: log

            try
            {
                var viewModel = _root.Get(viewModelType, new ConstructorArgument("model", model));

                if (viewModel is IViewModel viewModelTyped)
                {
                    return viewModelTyped;
                }

                // TODO: log
                throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                // TODO: log
                throw;
            }
        }
    }
}
