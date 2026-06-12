using AmisPayroll.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AmisPayroll.Api.Controllers.Base
{
    [ApiController]
    [Route("api/v1/[controller]s")]
    public abstract class BaseController<TDto, TCreateDto, TUpdateDto> : ControllerBase
    {
        protected readonly IBaseService<TDto, TCreateDto, TUpdateDto> _baseService;

        protected BaseController(IBaseService<TDto, TCreateDto, TUpdateDto> baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// API Lấy chi tiết 1 bản ghi theo ID
        /// </summary>
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var result = await _baseService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// API Thêm mới bản ghi (Đã xử lý luôn lỗi 405 Method Not Allowed)
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TCreateDto dto)
        {
            var result = await _baseService.InsertAsync(dto);
            return StatusCode(201, new
            {
                Success = true,
                Data = result,
                Message = "Thêm mới thành công."
            });
        }

        /// <summary>
        /// API Cập nhật bản ghi
        /// </summary>
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(Guid id, [FromBody] TUpdateDto dto)
        {
            var result = await _baseService.UpdateAsync(id, dto);
            return Ok(new
            {
                Success = true,
                Data = result,
                Message = "Cập nhật thành công."
            });
        }

        /// <summary>
        /// API Xóa bản ghi
        /// </summary>
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var result = await _baseService.DeleteAsync(id);
            return Ok(new
            {
                Success = true,
                Data = result,
                Message = "Xóa thành công."
            });
        }
    }
}