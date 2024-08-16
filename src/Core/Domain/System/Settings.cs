using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EMS20.WebApi.Domain.System;

public class Settings : AuditableEntity, IAggregateRoot
{
    public Settings()
    {
        
    }
    public Settings(string groupParam, string nameParam, bool lockedParam, string payloadParam)
    {
        Group = groupParam;
        Name = nameParam;
        Locked = lockedParam;
        Payload = payloadParam;
    }

    public string? Group { get; private set; }
    public string? Name { get; private set; }
    public bool? Locked { get; private set; }
    public string? Payload { get; private set; }

    public Settings Update(string? newGroup, string? newName, bool? newLocked, string? newPayload)
    {
        if (!string.IsNullOrEmpty(newGroup) && !newGroup.Equals(Group))
            Group = newGroup;

        if (!string.IsNullOrEmpty(newName) && !newName.Equals(Name))
            Name = newName;

        if (newLocked.HasValue)
            Locked = newLocked.Value;

        if (!string.IsNullOrEmpty(newPayload) && !newPayload.Equals(Payload))
            Payload = newPayload;

        return this;
    }
}
