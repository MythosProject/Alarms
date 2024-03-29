﻿using Mythos.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Alarms;

/// <summary>
/// Context object that contains Alarm details.
/// </summary>
public class Alarm
{
    /// <summary>
    /// Gets or sets the Database Id.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the Name of the alarm.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the days of the week that this alarm will fire.
    /// </summary>
    public DaysOfTheWeek Days { get; set; }

    /// <summary>
    /// Gets or sets the Time of this Alarm.
    /// Date component is ignored.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if this alarm is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }


    [Required]
    public string? UserId { get; set; }
}

public class AlarmDTO
{
    /// <summary>
    /// Gets or sets the Name of the alarm.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the days of the week that this alarm will fire.
    /// </summary>
    public DaysOfTheWeek Days { get; set; }

    /// <summary>
    /// Gets or sets the Time of this Alarm.
    /// Date component is ignored.
    /// </summary>
    public DateTimeOffset Time { get; set; }
}
