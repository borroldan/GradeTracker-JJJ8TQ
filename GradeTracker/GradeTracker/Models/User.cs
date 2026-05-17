using System.Text.Json.Serialization;

namespace GradeTracker.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Admin), typeDiscriminator: "admin")]
[JsonDerivedType(typeof(Teacher), typeDiscriminator: "teacher")]
[JsonDerivedType(typeof(Student), typeDiscriminator: "student")]
public abstract record User(Guid Id, string Name, UserRole Role);
