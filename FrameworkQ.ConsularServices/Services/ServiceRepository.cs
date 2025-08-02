using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace FrameworkQ.ConsularServices.Services
{
    public interface IServiceRepository
    {
        ServiceInfo CreateServiceInfo(ServiceInfo service);
        ServiceInfo GetServiceInfo(long id);
        void UpdateServiceInfo(ServiceInfo service);
        void DeleteServiceInfo(long id);
        Token CreateToken(Token token);
        Token GetToken(string tokenId);
        void UpdateToken(Token token);
        void DeleteToken(string tokenId);
    }

    public class ServiceRepository : IServiceRepository
    {
        private readonly string _connectionString;
        private TokenGenerator _tokenGenerator;

        private const string _sequencePrefix = "FRANCE";

        public ServiceRepository(ConnectionProvider connProvider, TokenGenerator tokenGenerator)
        {
            _connectionString = connProvider.GetConnectionString();
            _tokenGenerator = tokenGenerator;
        }

        // ServiceInfo CRUD
        /// <summary>
        /// Creates a new service info record in the database.
        /// </summary>
        /// <param name="service">The service info object to create.</param>
        /// <returns>The new service_id assigned by the database.</returns>
        public ServiceInfo CreateServiceInfo(ServiceInfo service)
        {
            const string sql = @"
            INSERT INTO public.service_info (service_name, service_description, usual_service_days, service_fee) 
            VALUES (@ServiceName, @ServiceDescription, @UsualServiceDays, @ServiceFee)
            RETURNING service_id;";

            using var conn = new NpgsqlConnection(_connectionString);
            service.ServiceId = conn.ExecuteScalar<long>(sql, service);
            return service;
        }

        public ServiceInfo GetServiceInfo(long id)
        {
            const string sql = "SELECT * FROM public.service_info WHERE service_id = @id;";
            using var conn = new NpgsqlConnection(_connectionString);
            return conn.QueryFirstOrDefault<ServiceInfo>(sql, new { id });
        }

        public void UpdateServiceInfo(ServiceInfo service)
        {
            const string sql = @"
                UPDATE public.service_info 
                SET 
                    service_name = @ServiceName, 
                    service_description = @ServiceDescription, 
                    usual_service_days = @UsualServiceDays, 
                    service_fee = @ServiceFee 
                WHERE service_id = @ServiceId;";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Execute(sql, service);
        }

        public void DeleteServiceInfo(long id)
        {
            const string sql = "DELETE FROM public.service_info WHERE service_id = @id;";
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Execute(sql, new { id });
        }

        /// <summary>
        /// Creates a new token in the database.
        /// </summary>
        /// <param name="token">The token object to create.</param>
        /// <returns>The token object that was successfully saved.</returns>
        public Token CreateToken(Token token)
        {
            token.TokenId = _tokenGenerator.GenerateTokenWithSequence(_sequencePrefix, DateTime.Now);

            const string sql = @"
            INSERT INTO public.token (
                token_id, generated_at, appointment_at, completed_at, 
                description, mobile_no, email, service_type, 
                passport_no, nid_no
            ) 
            VALUES (
                @TokenId, @GeneratedAt, @AppointmentAt, @CompletedAt, 
                @Description, @MobileNo, @Email, @ServiceType, 
                @PassportNo, @NationalIdNo
            );";

            using var conn = new NpgsqlConnection(_connectionString);

            // Dapper automatically maps properties from the 'token' object to the SQL parameters.
            conn.Execute(sql, token);

            return token;
        }




        public Token GetToken(string tokenId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn.QueryFirstOrDefault<Token>(
                "SELECT token_id AS TokenId, generated_at AS GeneratedAt, appointment_at AS AppointmentAt, completed_at AS CompletedAt, description AS Description, comment AS Comment, attachments_received AS AttachmentsRecieved, mobile_no AS MobileNo, email AS Email, service_type AS ServiceType, passport_no AS PassportNo, nid_no AS NidNo FROM token WHERE token_id = @TokenId",
                new { TokenId = tokenId });
        }

        public void UpdateToken(Token token)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            conn.Execute(
                @"UPDATE token SET generated_at = @GeneratedAt, appointment_at = @AppointmentAt, completed_at = @CompletedAt, description = @Description, comment = @Comment, attachments_received = @AttachmentsReceived, mobile_no = @MobileNo, email = @Email, service_type = @ServiceType, passport_no = @PassportNo, nid_no = @NidNo WHERE token_id = @TokenId",
                new
                {
                    token.GeneratedAt,
                    token.AppointmentAt,
                    token.CompletedAt,
                    token.Description,
                    token.MobileNo,
                    token.Email,
                    token.ServiceType,
                    PassportNo = token.PassportNo ?? (object)DBNull.Value,
                    NidNo = token.NationalIdNo ?? (object)DBNull.Value,
                    token.TokenId
                });
        }

        public void DeleteToken(string tokenId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            conn.Execute("DELETE FROM token WHERE token_id = @TokenId", new { TokenId = tokenId });
        }

        
    }
}