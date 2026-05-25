using FluentValidation;

namespace RescueSystem.Application.Features.Report.Queries.GetActivityChart
{
    public class GetActivityChartValidator : AbstractValidator<GetActivityChartQuery>
    {
        public GetActivityChartValidator()
        {
            RuleFor(x => x.Days)
                .GreaterThan(0)
                .LessThanOrEqualTo(365);
        }
    }
}
