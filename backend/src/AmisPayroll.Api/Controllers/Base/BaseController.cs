using System;
using System.Threading.Tasks;
using AmisPayroll.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AmisPayroll.Api.Controllers.Base
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public abstract class BaseController<TDto, TCreateDto, TUpdateDto> : ControllerBase
    {
        protected readonly IBaseService<TDto, TCreateDto, TUpdateDto> _baseService;

        public BaseController(IBaseService<TDto, TCreateDto, TUpdateDto> baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var data = await _baseService.GetAllAsync();
            return Ok(new { Success = true, Data = data });
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var data = await _baseService.GetByIdAsync(id);
            return Ok(new { Success = true, Data = data });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TCreateDto dto)
        {
            var result = await _baseService.InsertAsync(dto);
            return StatusCode(201, new { Success = true, Data = result, Message = "Thêm mới thành công." });
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(Guid id, [FromBody] TUpdateDto dto)
        {
            var result = await _baseService.UpdateAsync(id, dto);
            return Ok(new { Success = true, Data = result, Message = "Cập nhật thành công." });
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var result = await _baseService.DeleteAsync(id);
            return Ok(new { Success = true, Data = result, Message = "Xóa thành công." });
        }
    }
}