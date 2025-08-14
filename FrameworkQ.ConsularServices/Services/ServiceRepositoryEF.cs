using Microsoft.EntityFrameworkCore;
using FrameworkQ.ConsularServices.Data;

namespace FrameworkQ.ConsularServices.Services
{
    public class ServiceRepositoryEF : IServiceRepository
    {
        private readonly ConsularDbContext _context;
        private readonly TokenGenerator _tokenGenerator;
        private const string _sequencePrefix = "FRANCE";

        public ServiceRepositoryEF(ConsularDbContext context, TokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        // ServiceInfo CRUD
        public Service CreateServiceInfo(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            return service;
        }

        public Service GetServiceInfo(long id)
        {
            return _context.Services.Find(id)!;
        }

        public void UpdateServiceInfo(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
        }

        public void DeleteServiceInfo(long id)
        {
            var service = _context.Services.Find(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
            }
        }

        public Service[] ListServiceInfo()
        {
            return _context.Services.ToArray();
        }

        // Token CRUD
        public Token CreateToken(Token token)
        {
            token.TokenId = _tokenGenerator.GenerateTokenWithSequence(_sequencePrefix, DateTime.Now);
            _context.Tokens.Add(token);
            _context.SaveChanges();
            return token;
        }

        public Token GetToken(string tokenId)
        {
            return _context.Tokens.Find(tokenId)!;
        }

        public void UpdateToken(Token token)
        {
            _context.Tokens.Update(token);
            _context.SaveChanges();
        }

        public void DeleteToken(string tokenId)
        {
            var token = _context.Tokens.Find(tokenId);
            if (token != null)
            {
                _context.Tokens.Remove(token);
                _context.SaveChanges();
            }
        }

        // Station CRUD
        public Station[] GetStations()
        {
            return _context.Stations.ToArray();
        }

        public Station GetStation(long id)
        {
            return _context.Stations.Find(id)!;
        }

        public Station CreateStation(Station station)
        {
            _context.Stations.Add(station);
            _context.SaveChanges();
            return station;
        }
    }
}
