namespace AppForSEII.API.Models;
public class Cliente: ApplicationUser
{
    public Cliente()
    {
    }

    public Cliente(string id, string name, string surname, string userName): base(id, name, surname, userName)
    {
    }
    
}