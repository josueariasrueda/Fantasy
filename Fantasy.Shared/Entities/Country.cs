﻿using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entities;

public class Country
{
    public int Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;

    [MaxLength(100)]
    [Required]
    public string Code { get; set; } = null!;

    public string CallingCode { get; set; } = null!;
}