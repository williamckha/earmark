using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Earmark.WebApi.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet("~/api/category-groups")]
        public IEnumerable<CategoryGroupDTO> GetCategoryGroups()
        {
            return _categoriesService.GetCategoryGroups().Select(x => CategoryGroupToDTO(x));
        }

        [HttpGet("~/api/category-groups/{id:int}")]
        public ActionResult<CategoryGroupDTO> GetCategoryGroup(int id)
        {
            var categoryGroup = _categoriesService
                .GetCategoryGroups()
                .FirstOrDefault(x => x.Id == id);

            if (categoryGroup is null)
            {
                return NotFound();
            }

            return CategoryGroupToDTO(categoryGroup);
        }

        [HttpGet("")]
        public IEnumerable<CategoryDTO> GetCategories()
        {
            return _categoriesService.GetCategories().Select(x => CategoryToDTO(x));
        }

        [HttpGet("{id:int}")]
        public ActionResult<CategoryDTO> GetCategory(int id)
        {
            var category = _categoriesService
                .GetCategories()
                .FirstOrDefault(x => x.Id == id);

            if (category is null)
            {
                return NotFound();
            }

            return CategoryToDTO(category);
        }

        [HttpGet("~/api/category-groups/{groupId:int}/categories")]
        public ActionResult<IEnumerable<CategoryDTO>> GetCategoriesInGroup(int groupId)
        {
            var categoryGroup = _categoriesService
                .GetCategoryGroups()
                .FirstOrDefault(x => x.Id == groupId);

            if (categoryGroup is null)
            {
                return NotFound();
            }

            return Ok(categoryGroup.Categories.Select(x => CategoryToDTO(x)));
        }

        [HttpPost("~/api/category-groups")]
        public ActionResult<CategoryGroupDTO> AddCategoryGroup(CategoryGroupDTO categoryGroup)
        {
            try
            {
                var createdCategoryGroup = _categoriesService.AddCategoryGroup(categoryGroup.Name);
                var categoryGroupDTO = CategoryGroupToDTO(createdCategoryGroup);
                return CreatedAtAction(nameof(GetCategoryGroup), new { id = categoryGroupDTO.Id }, categoryGroupDTO);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [HttpPost("~/api/category-groups/{groupId:int}/categories")]
        public ActionResult<CategoryDTO> AddCategory(int groupId, CategoryDTO category)
        {
            try
            {
                var createdCategory = _categoriesService.AddCategory(groupId, category.Name);
                var categoryDTO = CategoryToDTO(createdCategory);
                return CreatedAtAction(nameof(GetCategory), new { id = categoryDTO.Id }, categoryDTO);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        private CategoryGroupDTO CategoryGroupToDTO(CategoryGroup categoryGroup)
        {
            return new CategoryGroupDTO()
            {
                Id = categoryGroup.Id,
                Position = categoryGroup.Position,
                Name = categoryGroup.Name,
                IsIncome = categoryGroup.IsIncome
            };
        }

        private CategoryDTO CategoryToDTO(Category category)
        {
            return new CategoryDTO()
            {
                Id = category.Id,
                Position = category.Position,
                Name = category.Name,
                IsIncome = category.IsIncome,
            };
        }
    }
}
