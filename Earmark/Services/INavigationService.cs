using System;

namespace Earmark.Services
{
    public interface INavigationService
    {
        void RegisterViewModelForView<TViewModel, TView>();

        void NavigateTo(Type viewModel, object parameter);
    }
}
