using Microsoft.VisualStudio.Language.NavigateTo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    
    internal class GDNavigator :
        INavigateToItemProvider
    {
        public bool TryCreateNavigateToItemProvider(IServiceProvider serviceProvider, out INavigateToItemProvider provider)
        {
        }

        public void StartSearch(INavigateToCallback callback, string searchValue)
        {
            throw new NotImplementedException();
        }

        public void StopSearch()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
