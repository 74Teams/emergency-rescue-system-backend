using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;
using RescueSystem.Infrastructure.Persistence;

namespace RescueSystem.Api.Seeders
{
    public static class ApplicationSeeder
    {
        private const int BatchSize = 10;

        public static async Task SeedAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            try
            {
                await dbContext.Database.MigrateAsync();
                await SeedRoles(roleManager);
                await SeedUsers(userManager);
                await SeedLocations(dbContext);
                await SeedRescueTeams(dbContext, userManager);
                await SeedRequests(dbContext, userManager);
                await SeedMissions(dbContext, userManager);
                await SeedMissionHistories(dbContext, userManager);
                await SeedReports(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Đã có lỗi xảy ra trong quá trình migrate hoặc seed dữ liệu");
            }
        }

        private static async Task SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            var roles = new[]
            {
                "Citizen",
                "Rescuer",
                "RescuerLeader",
                "Dispatcher",
                "Commander"
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{roleName} role"
                    });
                }
            }
        }

        private static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@rescuesystem.com";
            const string adminPassword = "Admin@123456";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "Nguyễn Minh Quân",
                    PhoneNumber = "0905123456",
                    Address = "Hải Châu 1, Hải Châu, Đà Nẵng",
                    DateOfBirth = new DateTime(1988, 3, 12),
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Commander");
            }

            var citizens = new (string Email, string UserName, string FullName, string Phone, string Address, DateTime DateOfBirth)[]
            {
                ("lan.nguyen@rescuesystem.com", "lan.nguyen", "Nguyễn Thị Lan", "0905123001", "Thạch Thang, Hải Châu, Đà Nẵng", new DateTime(1994, 5, 21)),
                ("phuc.tran@rescuesystem.com", "phuc.tran", "Trần Văn Phúc", "0905123002", "An Hải Bắc, Sơn Trà, Đà Nẵng", new DateTime(1992, 9, 14)),
                ("mai.le@rescuesystem.com", "mai.le", "Lê Thị Mai", "0905123003", "Mỹ An, Ngũ Hành Sơn, Đà Nẵng", new DateTime(1996, 1, 8)),
                ("huy.pham@rescuesystem.com", "huy.pham", "Phạm Quang Huy", "0905123004", "Hòa Khê, Thanh Khê, Đà Nẵng", new DateTime(1991, 12, 2)),
                ("anh.vo@rescuesystem.com", "anh.vo", "Võ Ngọc Ánh", "0905123005", "Hòa Minh, Liên Chiểu, Đà Nẵng", new DateTime(1995, 3, 30)),
                ("duc.bui@rescuesystem.com", "duc.bui", "Bùi Minh Đức", "0905123006", "Hòa Xuân, Cẩm Lệ, Đà Nẵng", new DateTime(1990, 7, 17)),
                ("duong.dang@rescuesystem.com", "duong.dang", "Đặng Thùy Dương", "0905123007", "Hòa Phước, Hòa Vang, Đà Nẵng", new DateTime(1997, 10, 11)),
                ("bao.ho@rescuesystem.com", "bao.ho", "Hồ Quốc Bảo", "0905123008", "Hải Châu 2, Hải Châu, Đà Nẵng", new DateTime(1993, 4, 5)),
                ("ha.ngo@rescuesystem.com", "ha.ngo", "Ngô Thanh Hà", "0905123009", "Mân Thái, Sơn Trà, Đà Nẵng", new DateTime(1998, 6, 23)),
                ("hung.phan@rescuesystem.com", "hung.phan", "Phan Gia Hưng", "0905123010", "Khuê Mỹ, Ngũ Hành Sơn, Đà Nẵng", new DateTime(1992, 11, 28))
            };

            var rescuers = new (string Email, string UserName, string FullName, string Phone, string Address, DateTime DateOfBirth)[]
            {
                ("tu.nguyen@rescuesystem.com", "tu.nguyen", "Nguyễn Thành Tự", "0905223001", "Hòa Cường Bắc, Hải Châu, Đà Nẵng", new DateTime(1989, 2, 9)),
                ("khanh.tran@rescuesystem.com", "khanh.tran", "Trần Quốc Khánh", "0905223002", "An Hải Đông, Sơn Trà, Đà Nẵng", new DateTime(1990, 8, 19)),
                ("linh.le@rescuesystem.com", "linh.le", "Lê Thanh Linh", "0905223003", "Mỹ Khê, Sơn Trà, Đà Nẵng", new DateTime(1991, 1, 27)),
                ("son.pham@rescuesystem.com", "son.pham", "Phạm Văn Sơn", "0905223004", "Hòa Minh, Liên Chiểu, Đà Nẵng", new DateTime(1988, 5, 4)),
                ("trang.vo@rescuesystem.com", "trang.vo", "Võ Thị Trang", "0905223005", "Hòa Khánh Nam, Liên Chiểu, Đà Nẵng", new DateTime(1993, 12, 12)),
                ("duc.dang@rescuesystem.com", "duc.dang", "Đặng Minh Đức", "0905223006", "Hòa Xuân, Cẩm Lệ, Đà Nẵng", new DateTime(1992, 7, 25)),
                ("an.ho@rescuesystem.com", "an.ho", "Hồ Văn An", "0905223007", "Hòa Phát, Cẩm Lệ, Đà Nẵng", new DateTime(1994, 9, 1)),
                ("nam.ngo@rescuesystem.com", "nam.ngo", "Ngô Trọng Nam", "0905223008", "Thanh Bình, Hải Châu, Đà Nẵng", new DateTime(1990, 3, 16)),
                ("phuong.phan@rescuesystem.com", "phuong.phan", "Phan Ngọc Phương", "0905223009", "An Hải Tây, Sơn Trà, Đà Nẵng", new DateTime(1987, 6, 7)),
                ("hieu.bui@rescuesystem.com", "hieu.bui", "Bùi Thanh Hiếu", "0905223010", "Hòa Quý, Ngũ Hành Sơn, Đà Nẵng", new DateTime(1991, 10, 22))
            };

            var leaders = new (string Email, string UserName, string FullName, string Phone, string Address, DateTime DateOfBirth)[]
            {
                ("vu.leader@rescuesystem.com", "vu.leader", "Lê Hoàng Vũ", "0905323001", "Hải Châu 1, Hải Châu, Đà Nẵng", new DateTime(1986, 4, 18)),
                ("hai.leader@rescuesystem.com", "hai.leader", "Nguyễn Đức Hải", "0905323002", "An Hải Bắc, Sơn Trà, Đà Nẵng", new DateTime(1985, 11, 6)),
                ("minh.leader@rescuesystem.com", "minh.leader", "Trần Quang Minh", "0905323003", "Hòa Minh, Liên Chiểu, Đà Nẵng", new DateTime(1987, 1, 29)),
                ("phat.leader@rescuesystem.com", "phat.leader", "Phạm Văn Phát", "0905323004", "Hòa Khánh Bắc, Liên Chiểu, Đà Nẵng", new DateTime(1984, 8, 13)),
                ("thao.leader@rescuesystem.com", "thao.leader", "Võ Thanh Thảo", "0905323005", "Hòa Cường Nam, Hải Châu, Đà Nẵng", new DateTime(1989, 2, 3)),
                ("hoa.leader@rescuesystem.com", "hoa.leader", "Đặng Thị Hoa", "0905323006", "Mỹ An, Ngũ Hành Sơn, Đà Nẵng", new DateTime(1988, 6, 20)),
                ("son.leader@rescuesystem.com", "son.leader", "Hồ Văn Sơn", "0905323007", "Hòa Xuân, Cẩm Lệ, Đà Nẵng", new DateTime(1985, 9, 9)),
                ("tien.leader@rescuesystem.com", "tien.leader", "Ngô Ngọc Tiến", "0905323008", "Thanh Khê Tây, Thanh Khê, Đà Nẵng", new DateTime(1986, 12, 1)),
                ("khang.leader@rescuesystem.com", "khang.leader", "Phan Quốc Khang", "0905323009", "Hòa Quý, Ngũ Hành Sơn, Đà Nẵng", new DateTime(1987, 7, 14)),
                ("hung.leader@rescuesystem.com", "hung.leader", "Bùi Hồng Hưng", "0905323010", "Mân Thái, Sơn Trà, Đà Nẵng", new DateTime(1984, 5, 26))
            };

            var dispatchers = new (string Email, string UserName, string FullName, string Phone, string Address, DateTime DateOfBirth)[]
            {
                ("thuy.dispatcher@rescuesystem.com", "thuy.dispatcher", "Nguyễn Thị Thúy", "0905423001", "Hòa Cường Bắc, Hải Châu, Đà Nẵng", new DateTime(1990, 10, 10)),
                ("quang.dispatcher@rescuesystem.com", "quang.dispatcher", "Trần Minh Quang", "0905423002", "Hòa Thuận Đông, Hải Châu, Đà Nẵng", new DateTime(1989, 12, 6)),
                ("lien.dispatcher@rescuesystem.com", "lien.dispatcher", "Lê Thị Liên", "0905423003", "Thanh Bình, Hải Châu, Đà Nẵng", new DateTime(1992, 2, 14)),
                ("long.dispatcher@rescuesystem.com", "long.dispatcher", "Phạm Duy Long", "0905423004", "Tân Chính, Thanh Khê, Đà Nẵng", new DateTime(1988, 6, 30)),
                ("nhu.dispatcher@rescuesystem.com", "nhu.dispatcher", "Võ Thị Như", "0905423005", "An Hải Tây, Sơn Trà, Đà Nẵng", new DateTime(1993, 4, 27)),
                ("tuan.dispatcher@rescuesystem.com", "tuan.dispatcher", "Đặng Anh Tuấn", "0905423006", "Hòa Minh, Liên Chiểu, Đà Nẵng", new DateTime(1987, 9, 5)),
                ("son.dispatcher@rescuesystem.com", "son.dispatcher", "Hồ Văn Sơn", "0905423007", "Hòa Xuân, Cẩm Lệ, Đà Nẵng", new DateTime(1991, 11, 19)),
                ("mai.dispatcher@rescuesystem.com", "mai.dispatcher", "Ngô Thị Mai", "0905423008", "Hòa Khê, Thanh Khê, Đà Nẵng", new DateTime(1994, 1, 23)),
                ("phuc.dispatcher@rescuesystem.com", "phuc.dispatcher", "Phan Văn Phúc", "0905423009", "Hòa Phát, Cẩm Lệ, Đà Nẵng", new DateTime(1989, 7, 2)),
                ("hien.dispatcher@rescuesystem.com", "hien.dispatcher", "Bùi Thị Hiền", "0905423010", "Mỹ An, Ngũ Hành Sơn, Đà Nẵng", new DateTime(1992, 8, 16))
            };

            var commanders = new (string Email, string UserName, string FullName, string Phone, string Address, DateTime DateOfBirth)[]
            {
                ("vu.commander@rescuesystem.com", "vu.commander", "Nguyễn Hoàng Vũ", "0905523001", "Thạch Thang, Hải Châu, Đà Nẵng", new DateTime(1982, 5, 11)),
                ("dung.commander@rescuesystem.com", "dung.commander", "Trần Thị Dung", "0905523002", "Hòa Cường Nam, Hải Châu, Đà Nẵng", new DateTime(1983, 3, 6)),
                ("phuong.commander@rescuesystem.com", "phuong.commander", "Lê Minh Phương", "0905523003", "An Hải Bắc, Sơn Trà, Đà Nẵng", new DateTime(1981, 10, 29)),
                ("thai.commander@rescuesystem.com", "thai.commander", "Phạm Hữu Thái", "0905523004", "Hòa Khánh Nam, Liên Chiểu, Đà Nẵng", new DateTime(1984, 12, 15)),
                ("hoa.commander@rescuesystem.com", "hoa.commander", "Võ Thị Hoa", "0905523005", "Hòa Xuân, Cẩm Lệ, Đà Nẵng", new DateTime(1985, 7, 7)),
                ("cuong.commander@rescuesystem.com", "cuong.commander", "Đặng Quốc Cường", "0905523006", "Hòa Phước, Hòa Vang, Đà Nẵng", new DateTime(1980, 9, 3)),
                ("binh.commander@rescuesystem.com", "binh.commander", "Hồ Thanh Bình", "0905523007", "Thanh Khê Tây, Thanh Khê, Đà Nẵng", new DateTime(1983, 1, 22)),
                ("nghia.commander@rescuesystem.com", "nghia.commander", "Ngô Quốc Nghĩa", "0905523008", "Mân Thái, Sơn Trà, Đà Nẵng", new DateTime(1982, 4, 18)),
                ("kiet.commander@rescuesystem.com", "kiet.commander", "Phan Quốc Kiệt", "0905523009", "Hòa Quý, Ngũ Hành Sơn, Đà Nẵng", new DateTime(1985, 11, 9)),
                ("loc.commander@rescuesystem.com", "loc.commander", "Bùi Văn Lộc", "0905523010", "Hòa Châu, Hòa Vang, Đà Nẵng", new DateTime(1981, 6, 25))
            };

            for (var i = 0; i < BatchSize; i++)
            {
                var citizen = citizens[i];
                await EnsureUserInRoleAsync(userManager, citizen.Email, citizen.UserName, citizen.FullName, citizen.Phone,
                    "Citizen@123", "Citizen", citizen.DateOfBirth, citizen.Address);

                var rescuer = rescuers[i];
                await EnsureUserInRoleAsync(userManager, rescuer.Email, rescuer.UserName, rescuer.FullName, rescuer.Phone,
                    "Rescuer@123", "Rescuer", rescuer.DateOfBirth, rescuer.Address);

                var leader = leaders[i];
                await EnsureUserInRoleAsync(userManager, leader.Email, leader.UserName, leader.FullName, leader.Phone,
                    "RescuerLeader@123", "RescuerLeader", leader.DateOfBirth, leader.Address);

                var dispatcher = dispatchers[i];
                await EnsureUserInRoleAsync(userManager, dispatcher.Email, dispatcher.UserName, dispatcher.FullName, dispatcher.Phone,
                    "Dispatcher@123", "Dispatcher", dispatcher.DateOfBirth, dispatcher.Address);

                var commander = commanders[i];
                await EnsureUserInRoleAsync(userManager, commander.Email, commander.UserName, commander.FullName, commander.Phone,
                    "Commander@123", "Commander", commander.DateOfBirth, commander.Address);
            }
        }

        private static async Task EnsureUserInRoleAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string userName,
            string fullName,
            string phone,
            string password,
            string role,
            DateTime dateOfBirth,
            string address)
        {
            if (await userManager.FindByEmailAsync(email) != null)
                return;

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                FullName = fullName,
                PhoneNumber = phone,
                Address = address,
                DateOfBirth = dateOfBirth,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }

        private static async Task SeedLocations(ApplicationDbContext context)
        {
            if (await context.Locations.AnyAsync())
                return;

            var locations = new List<Location>
            {
                new Location
                {
                    Latitude = 16.0720,
                    Longitude = 108.2240,
                    Address = "Bạch Đằng, Hải Châu 1, Hải Châu, Đà Nẵng",
                    Landmark = "Bờ sông Hàn"
                },
                new Location
                {
                    Latitude = 16.0618,
                    Longitude = 108.2198,
                    Address = "Cầu Rồng, An Hải Trung, Sơn Trà, Đà Nẵng",
                    Landmark = "Cầu Rồng"
                },
                new Location
                {
                    Latitude = 16.0594,
                    Longitude = 108.2463,
                    Address = "Bãi biển Mỹ Khê, Phước Mỹ, Sơn Trà, Đà Nẵng",
                    Landmark = "Mỹ Khê"
                },
                new Location
                {
                    Latitude = 16.0470,
                    Longitude = 108.2060,
                    Address = "Sân bay Quốc tế Đà Nẵng, Hòa Thuận Tây, Hải Châu, Đà Nẵng",
                    Landmark = "Sân bay Đà Nẵng"
                },
                new Location
                {
                    Latitude = 16.0040,
                    Longitude = 108.2640,
                    Address = "Ngũ Hành Sơn, Hòa Hải, Ngũ Hành Sơn, Đà Nẵng",
                    Landmark = "Ngũ Hành Sơn"
                },
                new Location
                {
                    Latitude = 16.0976,
                    Longitude = 108.2457,
                    Address = "Bán đảo Sơn Trà, Thọ Quang, Sơn Trà, Đà Nẵng",
                    Landmark = "Chùa Linh Ứng"
                },
                new Location
                {
                    Latitude = 16.0823,
                    Longitude = 108.1464,
                    Address = "Khu Công nghệ cao, Hòa Liên, Hòa Vang, Đà Nẵng",
                    Landmark = "Khu CNC Đà Nẵng"
                },
                new Location
                {
                    Latitude = 16.0610,
                    Longitude = 108.1542,
                    Address = "Bến xe Trung tâm, Hòa Minh, Liên Chiểu, Đà Nẵng",
                    Landmark = "Bến xe Trung tâm"
                },
                new Location
                {
                    Latitude = 16.0159,
                    Longitude = 108.2165,
                    Address = "Cầu Tiên Sơn, Hòa Cường Bắc, Hải Châu, Đà Nẵng",
                    Landmark = "Cầu Tiên Sơn"
                },
                new Location
                {
                    Latitude = 16.0230,
                    Longitude = 108.2115,
                    Address = "Helio Center, Hòa Cường Bắc, Hải Châu, Đà Nẵng",
                    Landmark = "Trung tâm Helio"
                }
            };

            await context.Locations.AddRangeAsync(locations);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRescueTeams(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.RescueTeams.AnyAsync())
                return;

            var leaders = (await userManager.GetUsersInRoleAsync("RescuerLeader"))
                .OrderBy(u => u.Email)
                .ToList();
            var rescuers = (await userManager.GetUsersInRoleAsync("Rescuer"))
                .OrderBy(u => u.Email)
                .ToList();
            var locations = await context.Locations.OrderBy(l => l.CreatedAt).ToListAsync();

            if (leaders.Count < BatchSize || rescuers.Count < BatchSize || locations.Count < BatchSize)
                return;

            var teams = new List<RescueTeam>();
            var teamNames = new[]
            {
                "Đội Cứu Hộ Hải Châu",
                "Đội Cứu Hộ Sơn Trà",
                "Đội Cứu Hộ Mỹ Khê",
                "Đội Cứu Hộ Liên Chiểu",
                "Đội Cứu Hộ Thanh Khê",
                "Đội Cứu Hộ Cẩm Lệ",
                "Đội Cứu Hộ Hòa Vang",
                "Đội Cứu Hộ Ngũ Hành Sơn",
                "Đội Cứu Hộ Hòa Cường",
                "Đội Cứu Hộ Bờ Đông"
            };
            for (var i = 0; i < BatchSize; i++)
            {
                teams.Add(new RescueTeam
                {
                    TeamName = teamNames[i],
                    TeamLeaderId = leaders[i].Id,
                    BaseLocationId = locations[i].Id,
                    Status = i % 3 == 0 ? TeamStatus.ON_MISSION : TeamStatus.AVAILABLE
                });
            }

            await context.RescueTeams.AddRangeAsync(teams);
            await context.SaveChangesAsync();

            var createdTeams = await context.RescueTeams
                .Include(t => t.Members)
                .OrderBy(t => t.TeamName)
                .ToListAsync();

            for (var i = 0; i < BatchSize; i++)
            {
                var member = await context.Users.FindAsync(rescuers[i].Id);
                if (member != null && createdTeams.Count > i)
                    createdTeams[i].Members.Add(member);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedRequests(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.Requests.AnyAsync())
                return;

            var locations = await context.Locations.OrderBy(l => l.CreatedAt).ToListAsync();
            var citizens = (await userManager.GetUsersInRoleAsync("Citizen"))
                .OrderBy(u => u.Email)
                .ToList();

            if (citizens.Count < BatchSize || locations.Count < BatchSize)
                return;

            var statuses = new[]
            {
                RequestStatus.PENDING,
                RequestStatus.ACCEPTED,
                RequestStatus.IN_PROGRESS,
                RequestStatus.COMPLETED,
                RequestStatus.PENDING,
                RequestStatus.ACCEPTED,
                RequestStatus.IN_PROGRESS,
                RequestStatus.COMPLETED,
                RequestStatus.PENDING,
                RequestStatus.ACCEPTED
            };

            var requests = new List<RescueRequest>();
            var descriptions = new[]
            {
                "Ngã xe máy gần Cầu Rồng, cần hỗ trợ sơ cứu và đưa đi bệnh viện.",
                "Người bị kẹt thang máy tại Helio Center, cần cứu hộ khẩn cấp.",
                "Tai nạn lao động nhẹ tại Khu CNC, cần hỗ trợ y tế tại chỗ.",
                "Người đuối nước gần bãi biển Mỹ Khê, cần đội cứu hộ biển.",
                "Cháy nhỏ tại khu bếp nhà dân ở Hòa Cường Bắc, cần kiểm tra an toàn.",
                "Sập mái tôn sau mưa lớn tại Liên Chiểu, cần hỗ trợ di dời.",
                "Người già bị ngất tại chợ Hàn, cần sơ cứu và chuyển viện.",
                "Va chạm giao thông trên đường 2/9, cần điều tiết và hỗ trợ y tế.",
                "Lạc trẻ em tại khu vực Cầu Tiên Sơn, cần hỗ trợ tìm kiếm.",
                "Người bị thương khi leo núi tại Ngũ Hành Sơn, cần vận chuyển y tế."
            };
            for (var i = 0; i < BatchSize; i++)
            {
                requests.Add(new RescueRequest
                {
                    UserId = citizens[i].Id,
                    EmergencyType = (EmergencyType)((i % 8) + 1),
                    Priority = (Priority)((i % 4) + 1),
                    Status = statuses[i],
                    LocationId = locations[i].Id,
                    Description = descriptions[i]
                });
            }

            await context.Requests.AddRangeAsync(requests);
            await context.SaveChangesAsync();
        }

        private static async Task SeedMissions(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.Missions.AnyAsync())
                return;

            var requests = await context.Requests.OrderBy(r => r.CreatedAt).ToListAsync();
            var teams = await context.RescueTeams.OrderBy(t => t.TeamName).ToListAsync();
            var dispatchers = (await userManager.GetUsersInRoleAsync("Dispatcher"))
                .OrderBy(u => u.Email)
                .ToList();

            if (requests.Count < BatchSize || teams.Count < BatchSize || dispatchers.Count < BatchSize)
                return;

            // Request[0] giữ PENDING không mission; các request 1..6 có mission để nối dispatcher + team
            const int missionCount = 6;
            var missions = new List<Mission>();
            var missionStatuses = new[]
            {
                MissionStatus.ASSIGNED,
                MissionStatus.EN_ROUTE,
                MissionStatus.ON_SITE,
                MissionStatus.IN_PROGRESS,
                MissionStatus.COMPLETED,
                MissionStatus.COMPLETED
            };

            for (var i = 0; i < missionCount; i++)
            {
                var req = requests[i + 1];
                var start = DateTime.UtcNow.AddHours(-(i + 1) * 3);
                var end = missionStatuses[i] == MissionStatus.COMPLETED
                    ? start.AddHours(2)
                    : (DateTime?)null;

                missions.Add(new Mission
                {
                    RequestId = req.Id,
                    DispatcherId = dispatchers[i].Id,
                    RescueTeamId = teams[i].Id,
                    StartTime = start,
                    EndTime = end,
                    Status = missionStatuses[i]
                });
            }

            await context.Missions.AddRangeAsync(missions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedMissionHistories(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.MissionHistories.AnyAsync())
                return;

            var missions = await context.Missions
                .Include(m => m.RescueTeam)
                .OrderBy(m => m.StartTime)
                .ToListAsync();

            var commanders = (await userManager.GetUsersInRoleAsync("Commander"))
                .OrderBy(u => u.Email)
                .ToList();

            if (missions.Count == 0 || commanders.Count == 0)
                return;

            var histories = new List<MissionHistory>();
            var commanderIdx = 0;

            foreach (var mission in missions)
            {
                var changedBy = commanders[commanderIdx % commanders.Count];
                commanderIdx++;

                histories.Add(new MissionHistory
                {
                    MissionId = mission.Id,
                    FromStatus = null,
                    ToStatus = MissionStatus.ASSIGNED,
                    ChangedById = changedBy.Id,
                    Note = "Seed: tạo nhiệm vụ"
                });

                if (mission.Status != MissionStatus.ASSIGNED)
                {
                    histories.Add(new MissionHistory
                    {
                        MissionId = mission.Id,
                        FromStatus = MissionStatus.ASSIGNED,
                        ToStatus = mission.Status,
                        ChangedById = changedBy.Id,
                        Note = "Seed: cập nhật trạng thái hiện tại"
                    });
                }
            }

            await context.MissionHistories.AddRangeAsync(histories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedReports(ApplicationDbContext context)
        {
            if (await context.Reports.AnyAsync())
                return;

            var completedMissions = await context.Missions
                .Where(m => m.Status == MissionStatus.COMPLETED)
                .Include(m => m.RescueTeam!)
                    .ThenInclude(t => t.Members)
                .OrderBy(m => m.StartTime)
                .ToListAsync();

            if (completedMissions.Count == 0)
                return;

            var reports = new List<Report>();

            foreach (var mission in completedMissions)
            {
                var authorId = mission.RescueTeam?.Members.FirstOrDefault()?.Id;
                if (authorId == null)
                    continue;

                reports.Add(new Report
                {
                    MissionId = mission.Id,
                    CreatedById = authorId.Value,
                    Content = $"Báo cáo hoàn thành seed — mission {mission.Id}",
                    AttachmentUrl = string.Empty,
                    Type = ReportType.COMPLETION
                });
            }

            if (reports.Count > 0)
            {
                await context.Reports.AddRangeAsync(reports);
                await context.SaveChangesAsync();
            }
        }
    }
}
