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
using System.Windows.Forms;
using System.Collections;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using WindowsFormsApp1;

namespace WindowsFormsApp1
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
            _connectionString = "Host=db.zeit-dev.site;Port=15432;Username=admin;Password=admin_password;Database=db";
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

                    // Проверем, сущевует ли пользователь
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении списка врачей: {ex.Message}");
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
                Console.WriteLine($"Ошибка при создании докора: {ex.Message}");
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
                    connection.Open();
                    string query = @"
                        SELECT 
                            patient_id,
                            first_name,
                            last_name,
                            patronymic,
                            birth_date,
                            CONCAT(address_city, ', ', address_street, ', ', address_building) as ""address"",
                            phone_number,
                            email,
                            CASE 
                                WHEN gender = 'M' THEN 'Мужской'
                                WHEN gender = 'F' THEN 'Женский'
                            END as ""gender"",
                            photo
                        FROM patients
                        ORDER BY last_name, first_name";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении списка пациентов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
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

        public DataRow GetPatientById(int patientId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine($"Получение данных пациента с ID: {patientId}");

                    string query = @"
                                     SELECT 
                                         *
                                     FROM patients 
                                     WHERE patient_id = @patientId";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@patientId", patientId);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            Console.WriteLine($"Найден пациент: {row["first_name"]} {row["last_name"]}");
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
                            Console.WriteLine("Пациент не найден");
                        }

                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
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
                            CONCAT(d.last_name, ' ', d.first_name) as ""Врач"",
                            ds.work_date as ""Дата"",
                            vs.slot_time as ""Время"",
                            CASE 
                                WHEN v.diagnosis_id IS NOT NULL THEN 'Завершен'
                                ELSE 'Ожидается'
                            END as ""Статус""
                        FROM visits v
                        JOIN visit_slots vs ON v.slot_id = vs.slot_id
                        JOIN doctor_schedule ds ON vs.schedule_id = ds.schedule_id
                        JOIN doctors d ON ds.doctor_id = d.doctor_id
                        WHERE v.patient_id = @patientId
                        ORDER BY ds.work_date DESC, vs.slot_time DESC";

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

        public class UserAccount
        {
            public int UserId { get; set; }
            public string Username { get; set; }
        }

        public UserAccount GetLinkedAccount(int entityId, string entityType)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = $@"
                        SELECT user_id, username 
                        FROM users 
                        WHERE {entityType}_id = @entityId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@entityId", entityId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserAccount
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1)
                                };
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении связанного аккаунта: {ex.Message}");
                return null;
            }
        }

        public bool CreateAndLinkAccount(string username, string password, int entityId, string entityType)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string salt = GenerateSalt();
                            string passwordHash = HashPassword(password, salt);

                            string query = @"
                                INSERT INTO users (username, password_hash, salt, user_role)
                                VALUES (@username, @passwordHash, @salt, @role)
                                RETURNING user_id";

                            int userId;
                            using (var command = new NpgsqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@username", username);
                                command.Parameters.AddWithValue("@passwordHash", passwordHash);
                                command.Parameters.AddWithValue("@salt", salt);
                                command.Parameters.AddWithValue("@role", entityType);

                                userId = Convert.ToInt32(command.ExecuteScalar());
                            }

                            string updateQuery = $@"
                                UPDATE users 
                                SET {entityType}_id = @entityId 
                                WHERE user_id = @userId";

                            using (var command = new NpgsqlCommand(updateQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@entityId", entityId);
                                command.Parameters.AddWithValue("@userId", userId);
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании и привязке аккаунта: {ex.Message}");
                return false;
            }
        }

        public bool LinkAccount(int userId, int entityId, string entityType)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = $@"
                        UPDATE users 
                        SET {entityType}_id = @entityId 
                        WHERE user_id = @userId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@entityId", entityId);
                        command.Parameters.AddWithValue("@userId", userId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при привязке аккаунта: {ex.Message}");
                return false;
            }
        }

        public bool UnlinkAccount(int entityId, string entityType)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = $@"
                        UPDATE users 
                        SET {entityType}_id = NULL 
                        WHERE {entityType}_id = @entityId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@entityId", entityId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отвязке аккаунта: {ex.Message}");
                return false;
            }
        }

        public DataTable GetAvailableUsers()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT user_id, username 
                        FROM users 
                        WHERE doctor_id IS NULL 
                        AND patient_id IS NULL 
                        ORDER BY username";

                    using (var adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении доступных пользователей: {ex.Message}");
                return new DataTable();
            }
        }

        public Patient GetPatientById2(int patientId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT * FROM patients 
                        WHERE patient_id = @patientId";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@patientId", patientId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Patient
                                {
                                    PatientId = reader.GetInt32(reader.GetOrdinal("patient_id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                                    Patronymic = reader.IsDBNull(reader.GetOrdinal("patronymic")) ? null : reader.GetString(reader.GetOrdinal("patronymic")),
                                    BirthDate = reader.GetDateTime(reader.GetOrdinal("birth_date")),
                                    Gender = reader.GetString(reader.GetOrdinal("gender")),
                                    AddressCity = reader.IsDBNull(reader.GetOrdinal("address_city")) ? null : reader.GetString(reader.GetOrdinal("address_city")),
                                    AddressStreet = reader.IsDBNull(reader.GetOrdinal("address_street")) ? null : reader.GetString(reader.GetOrdinal("address_street")),
                                    AddressBuilding = reader.IsDBNull(reader.GetOrdinal("address_building")) ? null : reader.GetString(reader.GetOrdinal("address_building")),
                                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? null : reader.GetString(reader.GetOrdinal("phone_number")),
                                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                    Photo = reader.IsDBNull(reader.GetOrdinal("photo")) ? null : (byte[])reader["photo"]
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных пациента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public bool CreatePatient(Patient patient)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO patients (
                            first_name, last_name, patronymic, birth_date, 
                            gender, address_city, address_street, address_building, 
                            phone_number, email, photo)
                        VALUES (
                            @firstName, @lastName, @patronymic, @birthDate,
                            @gender, @addressCity, @addressStreet, @addressBuilding,
                            @phoneNumber, @email, @photo)";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        AddPatientParameters(cmd, patient);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании пациента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdatePatient(Patient patient)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE patients SET 
                            first_name = @firstName,
                            last_name = @lastName,
                            patronymic = @patronymic,
                            birth_date = @birthDate,
                            gender = @gender,
                            address_city = @addressCity,
                            address_street = @addressStreet,
                            address_building = @addressBuilding,
                            phone_number = @phoneNumber,
                            email = @email,
                            photo = @photo
                        WHERE patient_id = @patientId";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        AddPatientParameters(cmd, patient);
                        cmd.Parameters.AddWithValue("@patientId", patient.PatientId);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении пациента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void AddPatientParameters(NpgsqlCommand cmd, Patient patient)
        {
            cmd.Parameters.AddWithValue("@firstName", patient.FirstName);
            cmd.Parameters.AddWithValue("@lastName", patient.LastName);
            cmd.Parameters.AddWithValue("@patronymic", (object)patient.Patronymic ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@birthDate", patient.BirthDate);
            cmd.Parameters.AddWithValue("@gender", patient.Gender);
            cmd.Parameters.AddWithValue("@addressCity", (object)patient.AddressCity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@addressStreet", (object)patient.AddressStreet ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@addressBuilding", (object)patient.AddressBuilding ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@phoneNumber", (object)patient.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object)patient.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@photo", (object)patient.Photo ?? DBNull.Value);
        }

        public List<TimeSpan> GetAvailableTimeSlots(int doctorId, DateTime date)
        {
            var availableSlots = new List<TimeSpan>();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT ds.start_time, ds.end_time 
                        FROM doctor_schedule ds
                        WHERE ds.doctor_id = @doctorId 
                        AND ds.work_date = @date";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@doctorId", doctorId);
                        command.Parameters.AddWithValue("@date", date.Date);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TimeSpan startTime = reader.GetTimeSpan(0);
                                TimeSpan endTime = reader.GetTimeSpan(1);

                                // Генерируем слоты по 25 минут
                                var currentSlot = startTime;
                                while (currentSlot.Add(TimeSpan.FromMinutes(25)) <= endTime)
                                {
                                    availableSlots.Add(currentSlot);
                                    currentSlot = currentSlot.Add(TimeSpan.FromMinutes(25));
                                }
                            }
                        }
                    }

                    // Получаем уже занятые слоты
                    query = @"
                        SELECT vs.slot_time 
                        FROM visit_slots vs
                        JOIN doctor_schedule ds ON vs.schedule_id = ds.schedule_id
                        WHERE ds.doctor_id = @doctorId 
                        AND ds.work_date = @date
                        AND vs.is_booked = true";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@doctorId", doctorId);
                        command.Parameters.AddWithValue("@date", date.Date);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TimeSpan bookedTime = reader.GetTimeSpan(0);
                                availableSlots.Remove(bookedTime);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении доступных слотов: {ex.Message}");
            }
            return availableSlots;
        }

        public bool CreateAppointment(int doctorId, int patientId, DateTime date, TimeSpan time)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Находим schedule_id
                            string findScheduleQuery = @"
                                SELECT schedule_id 
                                FROM doctor_schedule 
                                WHERE doctor_id = @doctorId 
                                AND work_date = @date";

                            int scheduleId;
                            using (var command = new NpgsqlCommand(findScheduleQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@doctorId", doctorId);
                                command.Parameters.AddWithValue("@date", date.Date);
                                var result = command.ExecuteScalar();
                                if (result == null) return false;
                                scheduleId = Convert.ToInt32(result);
                            }

                            // Проверяем существование слота
                            string checkSlotQuery = @"
                                SELECT slot_id 
                                FROM visit_slots 
                                WHERE schedule_id = @scheduleId 
                                AND slot_time = @time";

                            int? existingSlotId = null;
                            using (var command = new NpgsqlCommand(checkSlotQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@scheduleId", scheduleId);
                                command.Parameters.AddWithValue("@time", time);
                                var result = command.ExecuteScalar();
                                if (result != null)
                                {
                                    existingSlotId = Convert.ToInt32(result);
                                }
                            }

                            int slotId;
                            if (existingSlotId.HasValue)
                            {
                                // Обновляем существующий слот
                                string updateSlotQuery = @"
                                    UPDATE visit_slots 
                                    SET is_booked = true 
                                    WHERE slot_id = @slotId 
                                    RETURNING slot_id";

                                using (var command = new NpgsqlCommand(updateSlotQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@slotId", existingSlotId.Value);
                                    slotId = Convert.ToInt32(command.ExecuteScalar());
                                }
                            }
                            else
                            {
                                // Создаем новый слот
                                string insertSlotQuery = @"
                                    INSERT INTO visit_slots (schedule_id, slot_time, is_booked)
                                    VALUES (@scheduleId, @time, true)
                                    RETURNING slot_id";

                                using (var command = new NpgsqlCommand(insertSlotQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@scheduleId", scheduleId);
                                    command.Parameters.AddWithValue("@time", time);
                                    slotId = Convert.ToInt32(command.ExecuteScalar());
                                }
                            }

                            // Создаем запись о визите
                            string createVisitQuery = @"
                                INSERT INTO visits (slot_id, patient_id)
                                VALUES (@slotId, @patientId)
                                RETURNING visit_id";

                            using (var command = new NpgsqlCommand(createVisitQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@slotId", slotId);
                                command.Parameters.AddWithValue("@patientId", patientId);
                                command.ExecuteScalar();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при создании записи: {ex.Message}");
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании записи: {ex.Message}");
                return false;
            }
        }

        public bool DeleteAppointment(int patientId, DateTime date, TimeSpan time)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Сначала получаем slot_id
                            string findSlotQuery = @"
                                SELECT v.slot_id
                                FROM visits v
                                JOIN visit_slots vs ON v.slot_id = vs.slot_id
                                JOIN doctor_schedule ds ON vs.schedule_id = ds.schedule_id
                                WHERE v.patient_id = @patientId
                                AND ds.work_date = @date
                                AND vs.slot_time = @time";

                            int slotId;
                            using (var cmd = new NpgsqlCommand(findSlotQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@patientId", patientId);
                                cmd.Parameters.AddWithValue("@date", date.Date);
                                cmd.Parameters.AddWithValue("@time", time);
                                var result = cmd.ExecuteScalar();
                                if (result == null) return false;
                                slotId = Convert.ToInt32(result);
                            }

                            // Удаляем запись
                            string deleteVisitQuery = "DELETE FROM visits WHERE slot_id = @slotId";
                            using (var cmd = new NpgsqlCommand(deleteVisitQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@slotId", slotId);
                                cmd.ExecuteNonQuery();
                            }

                            // Освобождаем слот
                            string updateSlotQuery = @"
                                UPDATE visit_slots 
                                SET is_booked = false 
                                WHERE slot_id = @slotId";

                            using (var cmd = new NpgsqlCommand(updateSlotQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@slotId", slotId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении записи: {ex.Message}");
                return false;
            }
        }

    }

}