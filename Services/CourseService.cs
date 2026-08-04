using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
using Tms.Api.Dtos;
using TmsApi.Services;
using System.Security.Cryptography;

public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
PagedRequest request, CancellationToken ct)
{
// TODO 1: Start with a no-tracking IQueryable<Course>:
// IQueryable<Course> query = context.Courses.AsNoTracking();
// TODO 2: If request.Search has a value, append a Where clause:
// query = query.Where(c => EF.Functions.ILike(c.Title, $"%{request.Search}%")
//st.Search}%"));|| EF.Functions.ILike(c.Code, $"%{reque
// ILike is the case-insensitive LIKE in PostgreSQL using it here means
// the search "fund" finds "Web Development Fundamentals" withoutlearners
// being surprised by case-sensitivity at lab time.
// TODO 3: Count BEFORE paging:
// var totalCount = await query.CountAsync(ct);
// This produces one SELECT COUNT(*) statement. If you Count after Skip/Take,
// you would get the count of the page, not the total.
// TODO 4: Apply OrderBy, then Skip/Take, then Select projection.
// For OrderBy, branch on request.OrderBy ∈ { "Title", "Code", "MaxCapacity" }
// and apply Descending if request.Descending. Reject unknown OrderBy values
// silently by falling back to "Title" never let an arbitrary string
// into the LINQ tree.
// TODO 5: Materialise:
// var items = await sortedQuery
//.Skip((request.Page- 1) * request.PageSize)
//
//.Take(request.PageSize).Select(c => new CourseResponseDto(c.Id, c.Code, c.Title,c.MaxCapacity, c.Enrollments.Count))
//.ToListAsync(ct);
// TODO 6: Return new PagedResponse<CourseResponseDto> { Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize };
throw new NotImplementedException();
}