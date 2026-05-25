using MediatR;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.Report;
using RescueSystem.Application.Features.Report.Queries.GetActivityChart;

namespace RescueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/report")]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/report/activity-chart?days=7
        [HttpGet("activity-chart")]
        public async Task<IActionResult> GetActivityChart([FromQuery] GetActivityChartQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<ActivityChartItemDTO>>.SuccessResponse(
                data: result,
                message: "Lấy dữ liệu biểu đồ hoạt động thành công",
                statusCode: 200
            ));
        }
    }
}
