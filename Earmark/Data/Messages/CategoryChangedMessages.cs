using Earmark.Backend.Models;
using System;

namespace Earmark.Data.Messages
{
    public class CategoryGroupAddedMessage
    {
        public CategoryGroup CategoryGroup { get; }

        public CategoryGroupAddedMessage(CategoryGroup categoryGroup)
        {
            CategoryGroup = categoryGroup;
        }
    }

    public class CategoryGroupRemovedMessage
    {
        public Guid CategoryGroupId { get; }

        public CategoryGroupRemovedMessage(Guid categoryGroupId)
        {
            CategoryGroupId = categoryGroupId;
        }
    }

    public class CategoryAddedMessage
    {
        public Category Category { get; }

        public CategoryAddedMessage(Category category)
        {
            Category = category;
        }
    }

    public class CategoryRemovedMessage
    {
        public Guid CategoryId { get; }

        public CategoryRemovedMessage(Guid categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
