using Ninject;

namespace MusicDownloader.NinjectRoot
{
    /// <summary>
    /// Factory of Ninject singletone kernel (a.k.a. "root").
    /// </summary>
    public sealed class RootFactory
    {
        /// <summary>
        /// Creates "root".
        /// </summary>
        public static StandardKernel CreateRoot()
        {
            var root = new StandardKernel();

            var defaultModule = new DefaultModule();
            root.Load(defaultModule);

            return root;
        }
    }
}
