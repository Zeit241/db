using System;

public class Doctor
{
    public int DoctorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }
    public int SpecialtyId { get; set; }
    public int ExperienceYears { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string AddressCity { get; set; }
    public string AddressStreet { get; set; }
    public string AddressBuilding { get; set; }
    public byte[] Photo { get; set; }
    public DateTime DateOfBirth { get; set; }
    public char Gender { get; set; }

    public string FullName => $"{LastName} {FirstName} {Patronymic}".Trim();
}