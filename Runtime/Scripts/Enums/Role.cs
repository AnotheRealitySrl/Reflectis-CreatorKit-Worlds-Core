using System;

[Flags]
public enum Role
{
    Guest = 1,
    Student = 2,
    Teacher = 4,
    Principal = 8,
    Admin = 16
}