using BankingSystem.DAL.Models;
using Bogus;
using System.Collections.Generic;

namespace BankingSystem.BogusFakers
{
    public static class Faker
    {
        public static List<Teller> GenerateFakeTellers(int count = 10)
        {
            var faker = new Faker<Teller>("en")
                .RuleFor(t => t.Id, f => Guid.NewGuid().ToString())
                .RuleFor(t => t.UserName, f => f.Internet.UserName())
                .RuleFor(t => t.NormalizedUserName, (f, t) => t.UserName.ToUpper())
                .RuleFor(t => t.Email, f => f.Internet.Email())
                .RuleFor(t => t.NormalizedEmail, (f, t) => t.Email.ToUpper())
                .RuleFor(t => t.EmailConfirmed, f => true)
                .RuleFor(t => t.SecurityStamp, f => Guid.NewGuid().ToString())
                .RuleFor(t => t.ConcurrencyStamp, f => Guid.NewGuid().ToString())
                .RuleFor(t => t.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(t => t.PhoneNumberConfirmed, f => true)

                .RuleFor(t => t.SSN, f => f.Random.Long(10000000000000, 99999999999999))
                .RuleFor(t => t.FirstName, f => f.Name.FirstName())
                .RuleFor(t => t.LastName, f => f.Name.LastName())
                .RuleFor(t => t.Address, f => f.Address.StreetAddress())
                .RuleFor(t => t.JoinDate, f => DateTime.Now)
                .RuleFor(t => t.BirthDate, f => f.Date.Past(30, DateTime.Now.AddYears(-20)))
                .RuleFor(t => t.Discriminator, f => "Teller")
                .RuleFor(t => t.IsDeleted, f => false)

                .RuleFor(t => t.Salary, f => f.Random.Double(5000, 10000))
                .RuleFor(t => t.BranchId, f => null /* f.Random.Int(1, 3) */ ) // adjust to your DB
                .RuleFor(t => t.DeptId, f => null /* f.Random.Int(1, 3) */ )   // adjust to your DB
                .RuleFor(t => t.ManagerId, f => null); // or set a manager if needed

            return faker.Generate(count);
        }


        public static List<Admin> GenerateFakeAdmins(int count = 10)
        {
            var faker = new Faker<Admin>("en")
                .RuleFor(a => a.Id, f => Guid.NewGuid().ToString())
                .RuleFor(a => a.UserName, f => f.Internet.UserName())
                .RuleFor(a => a.NormalizedUserName, (f, a) => a.UserName.ToUpper())
                .RuleFor(a => a.Email, f => f.Internet.Email())
                .RuleFor(a => a.NormalizedEmail, (f, a) => a.Email.ToUpper())
                .RuleFor(a => a.EmailConfirmed, f => true)
                .RuleFor(a => a.SecurityStamp, f => Guid.NewGuid().ToString())
                .RuleFor(a => a.ConcurrencyStamp, f => Guid.NewGuid().ToString())
                .RuleFor(a => a.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(a => a.PhoneNumberConfirmed, f => true)

                .RuleFor(a => a.SSN, f => f.Random.Long(10000000000000, 99999999999999))
                .RuleFor(a => a.FirstName, f => f.Name.FirstName())
                .RuleFor(a => a.LastName, f => f.Name.LastName())
                .RuleFor(a => a.Address, f => f.Address.StreetAddress())
                .RuleFor(a => a.JoinDate, f => DateTime.Now)
                .RuleFor(a => a.BirthDate, f => f.Date.Past(30, DateTime.Now.AddYears(-25)))
                .RuleFor(a => a.Discriminator, f => "Admin")
                .RuleFor(a => a.IsDeleted, f => false)

                .RuleFor(a => a.Salary, f => f.Random.Double(10000, 20000))
                .RuleFor(a => a.BankId, f => null /* f.Random.Int(1, 3) */ ); // Adjust based on your DB

            return faker.Generate(count);
        }


        public static List<Manager> GenerateFakeManagers(int count = 10)
        {
            var faker = new Faker<Manager>("en")
                .RuleFor(m => m.Id, f => Guid.NewGuid().ToString())
                .RuleFor(m => m.UserName, f => f.Internet.UserName())
                .RuleFor(m => m.NormalizedUserName, (f, m) => m.UserName.ToUpper())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.NormalizedEmail, (f, m) => m.Email.ToUpper())
                .RuleFor(m => m.EmailConfirmed, f => true)
                .RuleFor(m => m.SecurityStamp, f => Guid.NewGuid().ToString())
                .RuleFor(m => m.ConcurrencyStamp, f => Guid.NewGuid().ToString())
                .RuleFor(m => m.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(m => m.PhoneNumberConfirmed, f => true)

                .RuleFor(m => m.SSN, f => f.Random.Long(10000000000000, 99999999999999))
                .RuleFor(m => m.FirstName, f => f.Name.FirstName())
                .RuleFor(m => m.LastName, f => f.Name.LastName())
                .RuleFor(m => m.Address, f => f.Address.StreetAddress())
                .RuleFor(m => m.JoinDate, f => DateTime.Now)
                .RuleFor(m => m.BirthDate, f => f.Date.Past(30, DateTime.Now.AddYears(-25)))
                .RuleFor(m => m.Discriminator, f => "Manager")
                .RuleFor(m => m.IsDeleted, f => false)

                .RuleFor(m => m.Salary, f => f.Random.Double(15000, 30000))
                .RuleFor(m => m.BranchId, f => null /* f.Random.Int(1, 3) */ ); // Adjust based on your real branch IDs

            return faker.Generate(count);
        }


        public static List<Customer> GenerateFakeClients(int count = 10)
        {
            var faker = new Faker<Customer>("en")
                .RuleFor(c => c.Id, f => Guid.NewGuid().ToString())
                .RuleFor(c => c.UserName, f => f.Internet.UserName())
                .RuleFor(c => c.NormalizedUserName, (f, c) => c.UserName.ToUpper())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.NormalizedEmail, (f, c) => c.Email.ToUpper())
                .RuleFor(c => c.EmailConfirmed, f => true)
                .RuleFor(c => c.SecurityStamp, f => Guid.NewGuid().ToString())
                .RuleFor(c => c.ConcurrencyStamp, f => Guid.NewGuid().ToString())
                .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.PhoneNumberConfirmed, f => true)

                .RuleFor(c => c.SSN, f => f.Random.Long(10000000000000, 99999999999999))
                .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.Address, f => f.Address.StreetAddress())
                .RuleFor(c => c.JoinDate, f => DateTime.Now)
                .RuleFor(c => c.BirthDate, f => f.Date.Past(40, DateTime.Now.AddYears(-18)))
                .RuleFor(c => c.Discriminator, f => "Customer")
                .RuleFor(c => c.IsDeleted, f => false)

                .RuleFor(c => c.BranchId, f => null /* f.Random.Int(1, 3) */ ) // Adjust to match actual Branch IDs
                //.RuleFor(c => c.Cards, f => new List<VisaCard>()) // Optional: you can populate them later
                .RuleFor(c => c.Loans, f => new List<Loan>())
                .RuleFor(c => c.Transactions, f => new List<Transaction>())
                .RuleFor(c => c.Accounts, f => new List<Account>())
                .RuleFor(c => c.SupportTickets, f => new List<SupportTicket>());

            return faker.Generate(count);
        }


        public static List<Account> GenerateFakeAccounts(List<Customer> customers, int accountsPerCustomer = 2)
        {
            var accounts = new List<Account>();
            var random = new Random();

            foreach (var customer in customers)
            {
                var faker = new Faker<Account>("en")
                    .RuleFor(a => a.Number, f => f.Random.Long(100000000000, 999999999999))
                    .RuleFor(a => a.Balance, f => f.Random.Double(10000, 10000000000))
                    .RuleFor(a => a.CreatedAt, f => f.Date.Past(5))
                    .RuleFor(a => a.AccountType, f => f.PickRandom<AccountType>())
                    .RuleFor(a => a.AccountStatus, f => f.PickRandom<AccountStatus>())
                    .RuleFor(a => a.CustomerId, f => customer.Id)
                    .RuleFor(a => a.BranchId, f => customer.BranchId)
                    .RuleFor(a => a.Certificates, f => new List<Certificate>())
                    .RuleFor(a => a.Loans, f => new List<Loan>())
                    .RuleFor(a => a.Card, f => null /*new VisaCard()*/)
                    .RuleFor(a => a.AccountTransactions, f => new List<Transaction>());

                accounts.AddRange(faker.Generate(accountsPerCustomer));
            }

            return accounts;
        }


        public static List<Branch> GenerateFakeBranches(int count = 10)
        {
            var branchFaker = new Faker<Branch>()
                .RuleFor(b => b.Name, f => $"Branch {f.Address.City()}")
                .RuleFor(b => b.Location, f =>
                {
                    var address = f.Address.FullAddress();
                    return address.Length > 100 ? address.Substring(0, 100) : address;
                })
                .RuleFor(b => b.Opens, f => TimeSpan.FromHours(f.Random.Double(8, 9)))     // Opens between 8:00 and 9:00
                .RuleFor(b => b.Closes, f => TimeSpan.FromHours(f.Random.Double(17, 18)))  // Closes between 17:00 and 18:00
                .RuleFor(b => b.BankId,f=> null)
                .RuleFor(b => b.IsDeleted, f => false);

            return branchFaker.Generate(count);
        }


        public static List<Bank> GenerateFakeBanks(int count = 10)
        {
            var faker = new Faker<Bank>()
                .RuleFor(b => b.Name, f => $"Bank of {f.Address.City()}")
                .RuleFor(b => b.CentralAddress, f =>
                    f.Address.FullAddress().Length > 20
                    ? f.Address.FullAddress().Substring(0, 20)
                    : f.Address.FullAddress())
                .RuleFor(b => b.IsDeleted, f => false);

            return faker.Generate(count);
        }

        public static List<SupportTicket> GenerateFakeTickets(int count = 10, List<Customer> customers = null, List<Account> accounts = null)
        {
            var ticketFaker = new Faker<SupportTicket>("en")
                .RuleFor(t => t.Title, f => f.Lorem.Sentence(3, 6))
                .RuleFor(t => t.Description, f => f.Lorem.Paragraph(2))
                .RuleFor(t => t.Date, f => f.Date.Past(30))
                .RuleFor(t => t.Status, f => f.PickRandom<SupportTicketStatus>())
                .RuleFor(t => t.Type, f => f.PickRandom<SupportTicketType>())
                .RuleFor(t => t.Response, f => f.Random.Bool(0.7f) ? f.Lorem.Paragraphs(1, 3) : null)
                .RuleFor(t => t.CustomerId, f => customers != null ? f.PickRandom(customers)?.Id : null)
                .RuleFor(t => t.Customer, f => customers != null ? f.PickRandom(customers) : null)
                .RuleFor(t => t.TellerId, f => null) // Can be populated if you have tellers
                .RuleFor(t => t.Teller, f => null) // Can be populated if you have tellers
                .RuleFor(t => t.AccountId, f => accounts != null && f.Random.Bool(0.8f) ? f.PickRandom(accounts)?.Id : null)
                .RuleFor(t => t.Account, f => accounts != null && f.Random.Bool(0.8f) ? f.PickRandom(accounts) : null)
                .RuleFor(t => t.Date, f => DateTime.Now);

            return ticketFaker.Generate(count);
        }
    }
}
