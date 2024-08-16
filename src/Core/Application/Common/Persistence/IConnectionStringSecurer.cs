﻿namespace EMS20.WebApi.Application.Common.Persistence;

public interface IConnectionStringSecurer
{
    string? MakeSecure(string? connectionString, string? dbProvider = null);
}