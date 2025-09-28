namespace RangoAgil.API.Entities;

public class Rango
{
    public int Id { get; set; }
    public string Nome { get; set; }

    public Rango()
    {
        
    }

    public Rango(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}
