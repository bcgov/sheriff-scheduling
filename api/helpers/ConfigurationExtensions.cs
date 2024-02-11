﻿using Microsoft.Extensions.Configuration;
using SS.Api.infrastructure.exceptions;

namespace SS.Api.helpers
{
    public static class ConfigurationExtension
    {
        public static string GetNonEmptyValue(this IConfiguration configuration, string key)
        {
            var configurationValue = configuration.GetValue<string>(key);
            return string.IsNullOrEmpty(configurationValue)
                ? throw new ConfigurationException($"Configuration '{key}' is invalid or missing.")
                : configurationValue;
        }

        public static string GetBoolValue(this IConfiguration configuration, string key)
        {
            var configurationValue = configuration.GetValue<string>(key);
            return string.IsNullOrEmpty(configurationValue)
                ? "false"
                : configurationValue;
        }
    }
}