using St.Services;
using Stumana.DataAcess.Models;

Province province = new()
{
    Id = "P1",
    Name = "Ca Mau",
};

District district = new()
{
    Id = "D1",
    Name = "TP Ca Mau"
};

School school = new()
{
    Id = "S1",
    Name = "THPT Ca Mau"
};


User user = new()
{
    Id = "U1",
    Username = "Principal",
    Password = "12345",
    Role = "Admin",
    SchoolId = school.Id,
};

