using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Converters;

public static class RecurrentEventSettingConverter
{
	private static long ToDalDayOfWeek(this ISet<DayOfWeek> dayOfWeeks)
	{
		return dayOfWeeks.Aggregate(0, (current, dayOfWeek) => current | 1 << (int)dayOfWeek);
	}

	private static ISet<DayOfWeek> ToDayOfWeek(this long dalDayOfWeek)
	{
		return Enumerable.Range(0, 7).Where(i => ( dalDayOfWeek >> i & 1 ) > 0).Select(i => (DayOfWeek)i).ToHashSet();
	}
	
	public static ICollection<DalRecurrentSetting> ToDalSettings(this RecurrentEventSettings settings)
	{
		var list = new List<DalRecurrentSetting>
		{
			new()
			{
				EventId = settings.EventId,
				Key = RecurrentSettingKeys.RepeatType.ToString(),
				Value = (int)settings.RepeatType
			}
		};

		if (settings.UntilUtc.HasValue)
		{
			list.Add(new DalRecurrentSetting
			{
				EventId = settings.EventId,
				Key = RecurrentSettingKeys.Until.ToString(),
				Value = settings.UntilUtc.Value.Ticks
			});
		}
		if (settings.RepeatCount.HasValue)
		{
			list.Add(new DalRecurrentSetting
			{
				EventId = settings.EventId,
				Key = RecurrentSettingKeys.Count.ToString(),
				Value = settings.RepeatCount.Value
			});
		}
		if (settings.RepeatType == RepeatType.OnWeekDays)
		{
			list.Add(new DalRecurrentSetting
			{
				EventId = settings.EventId,
				Key = RecurrentSettingKeys.DayOfWeeks.ToString(),
				Value = settings.DayOfWeeks!.ToDalDayOfWeek()
			});
		}

		return list;
	}

	public static RecurrentEventSettings? ToRecurrentSettings(this IEnumerable<DalRecurrentSetting> settingCollection)
	{
		RecurrentEventSettings? settings = null;
		
		foreach (DalRecurrentSetting setting in settingCollection)
		{
			if (!Enum.TryParse(setting.Key, out RecurrentSettingKeys key))
			{
				continue;
			}
			
			settings ??= new RecurrentEventSettings(setting.EventId, default, null, null, null);

			switch (key)
			{
				case RecurrentSettingKeys.RepeatType:
					settings.RepeatType = (RepeatType)setting.Value;
					break;
				case RecurrentSettingKeys.DayOfWeeks:
					settings.DayOfWeeks = setting.Value.ToDayOfWeek();
					break;
				case RecurrentSettingKeys.Count:
					settings.RepeatCount = Convert.ToUInt32(setting.Value);
					break;
				case RecurrentSettingKeys.Until:
					settings.UntilUtc = new DateTime(setting.Value, DateTimeKind.Utc);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		return settings;
	}
}