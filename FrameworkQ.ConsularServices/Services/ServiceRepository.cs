using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace FrameworkQ.ConsularServices.Services
{
    public class ServiceRepository
    {
        private readonly string _connectionString;

        public ServiceRepository()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ServiceInfo CRUD
        public void CreateServiceInfo(ServiceInfo service)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            conn.Execute("INSERT INTO ServiceInfo (ServiceName, ServiceDescription, UsualServiceDays, ServiceFee) VALUES (@ServiceName, @ServiceDescription, @UsualServiceDays, @ServiceFee)", service);
        }

        public ServiceInfo GetServiceInfo(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn.QueryFirstOrDefault<ServiceInfo>("SELECT ServiceId, ServiceName, ServiceDescription, UsualServiceDays, ServiceFee FROM ServiceInfo WHERE ServiceId = @Id", new { Id = id });
        }

        public void UpdateServiceInfo(ServiceInfo service)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            conn.Execute("UPDATE ServiceInfo SET ServiceName = @ServiceName, ServiceDescription = @ServiceDescription, UsualServiceDays = @UsualServiceDays, ServiceFee = @ServiceFee WHERE ServiceId = @ServiceId", service);
        }

        public void DeleteServiceInfo(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            conn.Execute("DELETE FROM ServiceInfo WHERE ServiceId = @Id", new { Id = id });
        }

        // Token CRUD
        public void CreateToken(Token token)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            conn.Execute(@"INSERT INTO Token (TokenId, GeneratedAt, AppointmentAt, CompletedAt, Description, Comment, AttachmentsRecieved, MobileNo, Email, ServiceType) 
                VALUES (@TokenId, @GeneratedAt, @AppointmentAt, @CompletedAt, @Description, @Comment, @AttachmentsRecieved, @MobileNo, @Email, @ServiceType)", 
                new {
                    token.TokenId,
                    token.GeneratedAt,
                    token.AppointmentAt,
                    token.CompletedAt,
                    token.Description,
                    token.Comment,
                    AttachmentsRecieved = token.AttachmentsRecieved, // Dapper will map string[]
                    token.MobileNo,
                    token.Email,
                    token.ServiceType
                });
        }

        public Token GetToken(string tokenId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn.QueryFirstOrDefault<Token>(
                "SELECT TokenId, GeneratedAt, AppointmentAt, CompletedAt, Description, Comment, AttachmentsRecieved, MobileNo, Email, ServiceType FROM Token WHERE TokenId = @TokenId",
                new { TokenId = tokenId });
        }

        public void UpdateToken(Token token)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            conn.Execute(@"UPDATE Token SET GeneratedAt = @GeneratedAt, AppointmentAt = @AppointmentAt, CompletedAt = @CompletedAt, Description = @Description, Comment = @Comment, AttachmentsRecieved = @AttachmentsRecieved, MobileNo = @MobileNo, Email = @Email, ServiceType = @ServiceType WHERE TokenId = @TokenId", 
                new {
                    token.GeneratedAt,
                    token.AppointmentAt,
                    token.CompletedAt,
                    token.Description,
                    token.Comment,
                    AttachmentsRecieved = token.AttachmentsRecieved,
                    token.MobileNo,
                    token.Email,
                    token.ServiceType,
                    token.TokenId
                });
        }

        public void DeleteToken(string tokenId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            conn.Execute("DELETE FROM Token WHERE TokenId = @TokenId", new { TokenId = tokenId });
        }
    }
}