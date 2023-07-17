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
        public int CategoryGroupId { get; }

        public CategoryGroupRemovedMessage(int categoryGroupId)
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
        public int CategoryId { get; }

        public CategoryRemovedMessage(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
