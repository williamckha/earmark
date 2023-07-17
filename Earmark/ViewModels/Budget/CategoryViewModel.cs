using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Data.Messages;
using System;

namespace Earmark.ViewModels.Budget
{
    public partial class CategoryViewModel : ObservableRecipient
    {
        /// <summary>
        /// The unique ID that identifies the category.
        /// </summary>
        public int Id { get; }
        
        /// <summary>
        /// The unique ID that identifies the category group of the category.
        /// </summary>
        public int GroupId { get; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        public string Name { get; }

        public CategoryViewModel(Category category) : base(StrongReferenceMessenger.Default)
        {
            Id = category.Id;
            GroupId = category.Group.Id;
            Name = category.Name;
        }

        [RelayCommand]
        public void RemoveCategory()
        {
            CategoryGroupViewModel categoryGroupViewModel = Messenger.Send(new CategoryGroupViewModelRequestMessage(), GroupId);
            categoryGroupViewModel.RemoveCategory(this);
        }
    }
}
