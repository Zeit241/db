using Npgsql;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.IO;
using System.Drawing;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
            _connectionString = "Host=194.87.161.31;Username=admin;Password=admin_password;Database=db";
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = string.Concat(password, salt);
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public bool RegisterUser(string username, string password)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Провер��ем, сущевует ли пользователь
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                    using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@username", username);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                        if (count > 0)
                        {
                            return false; // Пользователь уже существует
                        }
                    }

                    // Создаем нового пользователя
                    string salt = GenerateSalt();
                    string hashedPassword = HashPassword(password, salt);
                    
                    string query = @"
                        INSERT INTO users (username, password_hash, salt, user_role) 
                        VALUES (@username, @password_hash, @salt, @user_role)";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password_hash", hashedPassword);
                        command.Parameters.AddWithValue("@salt", salt);
                        command.Parameters.AddWithValue("@user_role", "user");

                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public (bool success, int userId, string role) ValidateUser(string username, string password)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT user_id, password_hash, salt, user_role FROM users WHERE username = @username";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string storedHash = reader.GetString(1);
                                string salt = reader.GetString(2);
                                string role = reader.GetString(3);

                                string hashedPassword = HashPassword(password, salt);
                                if (storedHash == hashedPassword)
                                {
                                    return (true, userId, role);
                                }
                            }
                        }
                    }
                }
                return (false, 0, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (false, 0, null);
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DataTable GetAllDoctors()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            d.doctor_id as ""ID"",
                            d.last_name as ""Фамилия"",
                            d.first_name as ""Имя"",
                            d.patronymic as ""Отчество"",
                            s.name as ""Специальность"",
                            d.experience_years as ""Стаж"",
                            d.phone_number as ""Телефон"",
                            d.email as ""Email"",
                            CONCAT(d.address_city, ', ', d.address_street, ', ', d.address_building) as ""Адрес"",
                            d.date_of_birth as ""Дата рождения"",
                            CASE 
                                WHEN d.gender = 'M' THEN 'Мужской'
                                WHEN d.gender = 'F' THEN 'Женский'
                            END as ""Пол""
                        FROM doctors d
                        LEFT JOIN specialties s ON d.specialty_id = s.specialty_id
                        ORDER BY d.last_name, d.first_name";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable doctorsTable = new DataTable();
                        adapter.Fill(doctorsTable);
                        return doctorsTable;
                    }
                }
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public bool DeleteDoctor(int doctorId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM doctors WHERE doctor_id = @doctorId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@doctorId", doctorId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateDoctor(int doctorId, string firstName, string lastName, string patronymic, 
                               int specialtyId, int experienceYears, string phoneNumber, string email,
                               string city, string street, string building, DateTime dateOfBirth, 
                               char gender, byte[] photo = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine($"Обновление доктора с ID: {doctorId}");

                    string query = @"
                        UPDATE doctors 
                        SET first_name = @firstName,
                            last_name = @lastName,
                            patronymic = @patronymic,
                            specialty_id = @specialtyId,
                            experience_years = @experienceYears,
                            phone_number = @phoneNumber,
                            email = @email,
                            address_city = @city,
                            address_street = @street,
                            address_building = @building,
                            date_of_birth = @dateOfBirth,
                            gender = @gender,
                            photo = @photo
                        WHERE doctor_id = @doctorId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@doctorId", doctorId);
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@patronymic", (object)patronymic ?? DBNull.Value);
                        command.Parameters.AddWithValue("@specialtyId", specialtyId);
                        command.Parameters.AddWithValue("@experienceYears", experienceYears);
                        command.Parameters.AddWithValue("@phoneNumber", (object)phoneNumber ?? DBNull.Value);
                        command.Parameters.AddWithValue("@email", (object)email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@city", city);
                        command.Parameters.AddWithValue("@street", street);
                        command.Parameters.AddWithValue("@building", building);
                        command.Parameters.AddWithValue("@dateOfBirth", NpgsqlDbType.Date, dateOfBirth);
                        command.Parameters.AddWithValue("@gender", NpgsqlDbType.Char, gender);

                        if (photo != null && photo.Length > 0)
                        {
                            Console.WriteLine($"Обновление фото размером: {photo.Length} байт");
                            command.Parameters.AddWithValue("@photo", NpgsqlDbType.Bytea, photo);
                        }
                        else
                        {
                            Console.WriteLine("Фото не обновляется");
                            command.Parameters.AddWithValue("@photo", DBNull.Value);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Обновлено записей: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении доктора: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public DataTable GetDoctorSchedule(int doctorId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            schedule_id as ""ID"",
                            work_date as ""Дата"",
                            start_time as ""Начало работы"",
                            end_time as ""Конец работы""
                        FROM doctor_schedule
                        WHERE doctor_id = @doctorId
                        ORDER BY work_date, start_time";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@doctorId", doctorId);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении расписания: {ex.Message}");
                return new DataTable();
            }
        }

        public bool CreateDoctor(string firstName, string lastName, string patronymic, 
                               int specialtyId, int experienceYears, string phoneNumber, 
                               string email, string city, string street, string building, 
                               DateTime dateOfBirth, char gender, byte[] photo = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Подключение к БД установлено");

                    string query = @"
                        INSERT INTO doctors (
                            first_name, last_name, patronymic, specialty_id, 
                            experience_years, phone_number, email, 
                            address_city, address_street, address_building,
                            date_of_birth, gender, photo)
                        VALUES (
                            @firstName, @lastName, @patronymic, @specialtyId,
                            @experienceYears, @phoneNumber, @email,
                            @city, @street, @building,
                            @dateOfBirth, @gender, @photo)
                        RETURNING doctor_id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@patronymic", (object)patronymic ?? DBNull.Value);
                        command.Parameters.AddWithValue("@specialtyId", specialtyId);
                        command.Parameters.AddWithValue("@experienceYears", experienceYears);
                        command.Parameters.AddWithValue("@phoneNumber", (object)phoneNumber ?? DBNull.Value);
                        command.Parameters.AddWithValue("@email", (object)email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@city", city);
                        command.Parameters.AddWithValue("@street", street);
                        command.Parameters.AddWithValue("@building", building);
                        command.Parameters.AddWithValue("@dateOfBirth", NpgsqlDbType.Date, dateOfBirth);
                        command.Parameters.AddWithValue("@gender", NpgsqlDbType.Char, gender);

                        if (photo != null && photo.Length > 0)
                        {
                            Console.WriteLine($"Размер фото: {photo.Length} байт");
                            command.Parameters.AddWithValue("@photo", NpgsqlDbType.Bytea, photo);
                        }
                        else
                        {
                            Console.WriteLine("Фото отсутствует");
                            command.Parameters.AddWithValue("@photo", DBNull.Value);
                        }

                        var result = command.ExecuteScalar();
                        Console.WriteLine($"Создан доктор с ID: {result}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при созд��нии докора: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public DataRow GetDoctorById(int doctorId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine($"Получение данных доктора с ID: {doctorId}");

                    string query = @"
                        SELECT 
                            doctor_id, first_name, last_name, patronymic,
                            specialty_id, experience_years, phone_number,
                            email, address_city, address_street, address_building,
                            date_of_birth, gender, photo
                        FROM doctors 
                        WHERE doctor_id = @doctorId";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@doctorId", doctorId);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            Console.WriteLine($"Найден доктор: {row["first_name"]} {row["last_name"]}");
                            if (row["photo"] != DBNull.Value)
                            {
                                var photoData = (byte[])row["photo"];
                                Console.WriteLine($"Размер загруженного фото: {photoData.Length} байт");
                            }
                            else
                            {
                                Console.WriteLine("Фото отсутствует в БД");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Доктор не найден");
                        }

                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных врача: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public DataTable GetAllSpecialties()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT specialty_id, name FROM specialties ORDER BY name";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public bool HasTimeOverlap(int doctorId, DateTime workDate, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM doctor_schedule 
                        WHERE doctor_id = @doctorId 
                        AND work_date = @workDate 
                        AND (
                            (@startTime BETWEEN start_time AND end_time)
                            OR (@endTime BETWEEN start_time AND end_time)
                            OR (start_time BETWEEN @startTime AND @endTime)
                            OR (end_time BETWEEN @startTime AND @endTime)
                        )";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@doctorId", doctorId);
                        command.Parameters.AddWithValue("@workDate", NpgsqlDbType.Date, workDate.Date);
                        command.Parameters.AddWithValue("@startTime", NpgsqlDbType.Time, startTime);
                        command.Parameters.AddWithValue("@endTime", NpgsqlDbType.Time, endTime);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке пересечения времени: {ex.Message}");
                return false;
            }
        }

        public bool AddDoctorWorkDay(int doctorId, DateTime workDate, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                // Проверяем пересечение времени
                if (HasTimeOverlap(doctorId, workDate, startTime, endTime))
                {
                    Console.WriteLine("Обнаружено пересечение времени");
                    return false;
                }

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO doctor_schedule (doctor_id, work_date, start_time, end_time)
                        VALUES (@doctorId, @workDate, @startTime, @endTime)";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@doctorId", doctorId);
                        command.Parameters.AddWithValue("@workDate", NpgsqlDbType.Date, workDate.Date);
                        command.Parameters.AddWithValue("@startTime", NpgsqlDbType.Time, startTime);
                        command.Parameters.AddWithValue("@endTime", NpgsqlDbType.Time, endTime);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении рабочего дня: {ex.Message}");
                return false;
            }
        }

        public bool DeleteWorkDay(int scheduleId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM doctor_schedule WHERE schedule_id = @scheduleId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@scheduleId", scheduleId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении рабочего дня: {ex.Message}");
                return false;
            }
        }

        public DataTable GetAllPatients()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    string query = @"SELECT * FROM patients ORDER BY last_name, first_name";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable patientsTable = new DataTable();
                        adapter.Fill(patientsTable);
                        return patientsTable;
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool AddPatient(string firstName, string lastName, string patronymic,
            DateTime birthDate, string addressCity, string addressStreet, string addressBuilding,
            string phoneNumber, string email, char gender, byte[] photo)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO patients (first_name, last_name, patronymic, birth_date, 
                            address_city, address_street, address_building, phone_number, email, gender, photo)
                        VALUES (@firstName, @lastName, @patronymic, @birthDate, 
                            @addressCity, @addressStreet, @addressBuilding, @phoneNumber, @email, @gender, @photo)", connection))
                    {
                        cmd.Parameters.AddWithValue("firstName", firstName);
                        cmd.Parameters.AddWithValue("lastName", lastName);
                        cmd.Parameters.AddWithValue("patronymic", (object)patronymic ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("birthDate", birthDate);
                        cmd.Parameters.AddWithValue("addressCity", (object)addressCity ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("addressStreet", (object)addressStreet ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("addressBuilding", (object)addressBuilding ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("phoneNumber", (object)phoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("email", (object)email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("gender", gender);
                        cmd.Parameters.AddWithValue("photo", (object)photo ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Создано записей: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
               
                throw;
            }
        }

        public bool UpdatePatient(int patientId, string firstName, string lastName, string patronymic,
            DateTime birthDate, string addressCity, string addressStreet, string addressBuilding,
            string phoneNumber, string email, char gender, byte[] photo)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE patients 
                        SET first_name = @firstName, 
                            last_name = @lastName,
                            patronymic = @patronymic,
                            birth_date = @birthDate,
                            address_city = @addressCity,
                            address_street = @addressStreet,
                            address_building = @addressBuilding,
                            phone_number = @phoneNumber,
                            email = @email,
                            gender = @gender,
                            photo = @photo
                        WHERE patient_id = @patientId", connection))
                    {
                        cmd.Parameters.AddWithValue("patientId", patientId);
                        cmd.Parameters.AddWithValue("firstName", firstName);
                        cmd.Parameters.AddWithValue("lastName", lastName);
                        cmd.Parameters.AddWithValue("patronymic", (object)patronymic ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("birthDate", birthDate);
                        cmd.Parameters.AddWithValue("addressCity", (object)addressCity ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("addressStreet", (object)addressStreet ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("addressBuilding", (object)addressBuilding ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("phoneNumber", (object)phoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("email", (object)email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("gender", gender);
                        cmd.Parameters.AddWithValue("photo", (object)photo ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Обновлено записей: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeletePatient(int patientId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand("DELETE FROM patients WHERE patient_id = @patientId", connection))
                    {
                        cmd.Parameters.AddWithValue("patientId", patientId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Удалено записей: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
              
                throw;
            }
        }

        public async Task<Dictionary<string, object>> GetPatientById(int patientId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM patients WHERE patient_id = @patientId", connection))
                    {
                        cmd.Parameters.AddWithValue("patientId", patientId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Dictionary<string, object>
                                {
                                    ["patient_id"] = reader["patient_id"],
                                    ["first_name"] = reader["first_name"],
                                    ["last_name"] = reader["last_name"],
                                    ["patronymic"] = reader.IsDBNull(reader.GetOrdinal("patronymic")) ? null : reader["patronymic"],
                                    ["birth_date"] = reader["birth_date"],
                                    ["address_city"] = reader.IsDBNull(reader.GetOrdinal("address_city")) ? null : reader["address_city"],
                                    ["address_street"] = reader.IsDBNull(reader.GetOrdinal("address_street")) ? null : reader["address_street"],
                                    ["address_building"] = reader.IsDBNull(reader.GetOrdinal("address_building")) ? null : reader["address_building"],
                                    ["phone_number"] = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? null : reader["phone_number"],
                                    ["email"] = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader["email"],
                                    ["gender"] = reader["gender"],
                                    ["photo"] = reader.IsDBNull(reader.GetOrdinal("photo")) ? null : reader["photo"]
                                };
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetPatientAppointments(int patientId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            a.appointment_id as ""ID"",
                            CONCAT(d.last_name, ' ', d.first_name) as ""Врач"",
                            a.appointment_date as ""Дата"",
                            a.appointment_time as ""Время"",
                            a.status as ""Статус""
                        FROM appointments a
                        JOIN doctors d ON a.doctor_id = d.doctor_id
                        WHERE a.patient_id = @patientId
                        ORDER BY a.appointment_date DESC, a.appointment_time DESC";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@patientId", patientId);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении приёмов пациента: {ex.Message}");
                return new DataTable();
            }
        }
    }
}