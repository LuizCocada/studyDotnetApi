namespace StudyAPI.Repositories.IRepositorys;

public interface IUnitOfWork
{
    IProdutoRepository ProdutoRepository { get; }
    ICategoriaRepository CategoriaRepository { get; }
    
     Task Commit();
}