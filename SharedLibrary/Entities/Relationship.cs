using System.Text.Json.Serialization;

namespace Shared.Entities;

public class Relationship
{
    [JsonIgnore]
    public List<Employee>? Employees { get; set; }
}