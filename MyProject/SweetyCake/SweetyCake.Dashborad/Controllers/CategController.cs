
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.Categories;
using OutbornE_commerce.DAL.Models;
using System.Threading;

namespace SweetyCake.Dashborad.Controllers
{
    public class CategController : DashboardBaseController
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategController(ICategoryRepository categoryRepository)
        {

            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {

            var categories = await _categoryRepository.FindAllAsync(null);

            var categoryDtos = categories.Adapt<List<CategoryDto>>();

            return View(categoryDtos);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var category = await _categoryRepository.Find(c => c.Id == id, true);
        
            var data = category.Adapt<CategoryDto>();
            return View(data);

        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CategoryDto model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model == null || string.IsNullOrWhiteSpace(model.NameEn) || string.IsNullOrWhiteSpace(model.NameAr))
                    {
                        return BadRequest();
                    }

                    var category = model.Adapt<Category>();
                    category.CreatedBy = "admin";

                    await _categoryRepository.BeginTransactionAsync();

                    var result = await _categoryRepository.Create(category);
                    await _categoryRepository.SaveAsync(cancellationToken);

                    await _categoryRepository.CommitTransactionAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Category_NameEn") == true || ex.InnerException?.Message.Contains("IX_Category_NameAr") == true)
                {
                    await _categoryRepository.RollbackTransactionAsync();

                    return Conflict();
                
                }
                catch (Exception ex)
                {
                    await _categoryRepository.RollbackTransactionAsync();

                    return BadRequest();
                }

            }

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryRepository.Find(c => c.Id == id, true);
            var categoryDto = category.Adapt<CategoryDto>();
            if (categoryDto == null)
                return NotFound();

            return View(categoryDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] CategoryDto model, CancellationToken cancellationToken)
        {
            if (model == null || model.Id == null || string.IsNullOrWhiteSpace(model.NameEn) || string.IsNullOrWhiteSpace(model.NameAr))
            {
                return BadRequest();
              
            }

            var category = await _categoryRepository.Find(c => c.Id == model.Id, false);
            if (category is null)
            {
                return NotFound();
               
            }

            try
            {
               
                category = model.Adapt(category);
                category.UpdatedBy = "admin";
                category.UpdatedOn = DateTime.UtcNow;

                await _categoryRepository.BeginTransactionAsync();

                _categoryRepository.Update(category);
                await _categoryRepository.SaveAsync(cancellationToken);

                await _categoryRepository.CommitTransactionAsync();

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                await _categoryRepository.RollbackTransactionAsync();

                return BadRequest();

            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _categoryRepository.Find(c => c.Id == id, true);
            var categoryDto = category.Adapt<CategoryDto>();

            if (categoryDto == null)
                return NotFound();


            return View(categoryDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryRepository.Find(c => c.Id == id, true);

                if (category == null)
                {
                    return NotFound();
                }

                category.IsDeleted = true;
                _categoryRepository.Delete(category);

                await _categoryRepository.SaveAsync(cancellationToken);

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return Conflict();
            }
        }
    }
}
