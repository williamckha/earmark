using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;

namespace Earmark.Services
{
    public class NavigationService : INavigationService
    {
        private Dictionary<Type, Type> _viewModelsToViews = new();

        public void RegisterViewModelForView<TViewModel, TView>()
        {
            _viewModelsToViews[typeof(TViewModel)] = typeof(TView);
        }

        public void NavigateTo(Type viewModel, object parameter)
        {
            App.Current.Window.NavigationViewFrame
                .Navigate(_viewModelsToViews[viewModel], parameter, new SuppressNavigationTransitionInfo());
        }
    }
}
