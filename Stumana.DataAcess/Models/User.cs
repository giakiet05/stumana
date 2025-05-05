namespace Stumana.DataAcess.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public User() { }

        public User(string username, string password, string email)
        {
            Id = Guid.NewGuid().ToString();
            Username = username;
            Password = HashPassword(password);
            Email = email;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool VerifyPassword(string enteredPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, Password);
        }

        public void ChangePassword(string newPassword)
        {
            Password = HashPassword(newPassword);
        }
    }
}
