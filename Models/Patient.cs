using System;

namespace WindowsFormsApp1
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string AddressCity { get; set; }
        public string AddressStreet { get; set; }
        public string AddressBuilding { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public byte[] Photo { get; set; }
    }
}