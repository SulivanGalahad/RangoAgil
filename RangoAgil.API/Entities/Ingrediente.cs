namespace RangoAgil.API.Entities;

public class Ingrediente
{
    public int Id { get; set; }
    public string Nome { get; set; }

    public Ingrediente()
    {
        
    }

    public Ingrediente(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}
