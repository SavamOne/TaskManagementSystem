using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Converters;

public static class CalendarConverter
{
	public static DalCalendar ToDalCalendar(this Calendar calendar)
	{
		return new DalCalendar
		{
			Id = calendar.Id,
			Name = calendar.Name,
			Description = calendar.Description,
			CreationDate = calendar.CreationDateUtc,
			IsDeleted = false
		};
	}

	public static Calendar ToCalendar(this DalCalendar calendar)
	{
		if (calendar.IsDeleted)
		{
			throw new BusinessLogicException("Этот календарь удален");
		}

		return new Calendar(calendar.Id, calendar.Name, calendar.Description, calendar.CreationDate);
	}
}