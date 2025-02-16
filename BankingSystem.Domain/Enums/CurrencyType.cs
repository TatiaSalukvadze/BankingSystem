﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CurrenctType { GEL, USD, EUR };
}
